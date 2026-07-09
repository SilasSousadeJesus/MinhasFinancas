"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import Link from "next/link";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import {
  ArrowRight,
  ChevronDown,
  ChevronUp,
  Clipboard,
  CopyCheck,
  HeartPulse,
  Loader2,
  Sparkles,
  Trash2,
} from "lucide-react";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Textarea } from "@/components/ui/textarea";
import { CompromissoFinanceiroModal } from "@/components/compromissos-financeiros/CompromissoFinanceiroModal";
import { obterTextoExecutivoIndicador, obterTextoPontoAtencao } from "@/lib/assistente-financeiro-textos";
import { ConclusaoFinanceiraBuilder } from "@/lib/conclusao-financeira-builder";
import { useAuth } from "@/providers/auth-provider";
import {
  buscarAnaliseFinanceiraHistoricaDetalhe,
  buscarAnalisesFinanceirasHistoricas,
  excluirAnaliseFinanceiraHistorica,
  gerarAnaliseAssistenteFinanceiro,
} from "@/services/api/assistente-financeiro";
import { cadastrarCompromissoFinanceiro } from "@/services/api/compromissos-financeiros";
import {
  OrigemCompromissoFinanceiro,
  SalvarCompromissoFinanceiroPayload,
} from "@/types/compromissos-financeiros";
import { buscarResumoFinanceiroIA } from "@/services/api/resumo-financeiro-ia";
import { ApiError } from "@/types/api";
import {
  AnaliseAssistenteExibida,
  AnaliseFinanceiraHistoricaDetalhe,
  AnaliseFinanceiraHistoricaLista,
  RespostaAssistenteFinanceiroIA,
  ResultadoPaginadoAnaliseFinanceiraHistorica,
} from "@/types/assistente-financeiro";
import { IndicadorResumoFinanceiroIA, ResumoFinanceiroIAData } from "@/types/resumo-financeiro-ia";

const FORMATO_MOEDA = 0;
const FORMATO_PERCENTUAL = 1;
const FORMATO_MESES = 2;

const STATUS_EXCELENTE = 0;
const STATUS_BOM = 1;
const STATUS_ATENCAO = 2;
const STATUS_CRITICO = 3;

const INDICADOR_ECONOMIA_MENSAL = 0;
const INDICADOR_PERCENTUAL_ECONOMIA = 1;
const INDICADOR_RESERVA_EMERGENCIA_ATUAL = 2;
const INDICADOR_ENDIVIDAMENTO = 5;
const INDICADOR_PATRIMONIO_LIQUIDO_ATUAL = 6;
const INDICADOR_PERCENTUAL_PATRIMONIO_ALVO = 7;
const INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO = 8;

const ITENS_POR_PAGINA_PADRAO = 5;
const PERGUNTA_PADRAO_ANALISE =
  "Quais são os principais riscos e prioridades da minha situação financeira atual?";

function formatarValorIndicador(valor: number, formato: number) {
  if (formato === FORMATO_PERCENTUAL) {
    return `${valor.toFixed(1)}%`;
  }

  if (formato === FORMATO_MESES) {
    return `${valor.toFixed(1)} mês(es)`;
  }

  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(valor ?? 0);
}

function obterTextoStatus(status: number) {
  switch (status) {
    case STATUS_EXCELENTE:
      return "Excelente";
    case STATUS_BOM:
      return "Boa";
    case STATUS_CRITICO:
      return "Crítica";
    default:
      return "Atenção";
  }
}

function obterVariantBadge(status: number): "default" | "secondary" | "destructive" | "outline" {
  switch (status) {
    case STATUS_EXCELENTE:
      return "secondary";
    case STATUS_BOM:
      return "outline";
    case STATUS_CRITICO:
      return "destructive";
    default:
      return "default";
  }
}

function obterVariantClassificacao(classificacao: string): "default" | "secondary" | "destructive" | "outline" {
  switch (classificacao) {
    case "Excelente":
      return "secondary";
    case "Boa":
      return "outline";
    case "Crítica":
    case "Critica":
      return "destructive";
    default:
      return "default";
  }
}

function formatarDataReferencia(dataReferencia: string) {
  return new Intl.DateTimeFormat("pt-BR", {
    month: "long",
    year: "numeric",
  }).format(new Date(dataReferencia));
}

function formatarData(data: string | Date) {
  return new Intl.DateTimeFormat("pt-BR", { dateStyle: "short" }).format(new Date(data));
}

function formatarHora(data: string | Date) {
  return new Intl.DateTimeFormat("pt-BR", {
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(data));
}

function formatarCusto(valor: number) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
    minimumFractionDigits: 4,
    maximumFractionDigits: 4,
  }).format(valor ?? 0);
}

function formatarTempo(ms: number) {
  if (!ms) {
    return "0 ms";
  }

  if (ms < 1000) {
    return `${ms} ms`;
  }

  return `${(ms / 1000).toFixed(1)} s`;
}

function truncarTexto(texto: string, limite: number) {
  const valor = texto?.trim() || "Análise financeira geral";
  if (valor.length <= limite) {
    return valor;
  }

  return `${valor.slice(0, limite - 3).trimEnd()}...`;
}

function obterFraseResumoCompacto(resumo: ResumoFinanceiroIAData | null) {
  if (!resumo) {
    return "Resumo executivo indisponível no momento.";
  }

  return resumo.resumoExecutivo || "Sua leitura executiva completa continua disponível para consulta.";
}

function mapearNovaAnalise(
  resposta: RespostaAssistenteFinanceiroIA,
  dataGeracao: Date,
  perguntaUsuario: string
): AnaliseAssistenteExibida {
  const conteudoProcessado = processarConteudoAnalise(resposta.conteudo, resposta.sugestaoCompromissoFinanceiro);

  return {
    id: resposta.analiseFinanceiraHistoricaId ?? null,
    perguntaUsuario,
    conteudo: conteudoProcessado.conteudo,
    sugestaoCompromisso: conteudoProcessado.sugestaoCompromisso,
    compromissoFinanceiroId: null,
    modelo: resposta.modelo,
    provedor: resposta.provedor,
    tempoTotalMs: resposta.tempoTotalMs,
    custoEstimadoUsd: resposta.custoEstimadoUsd,
    tokensTotaisUtilizados: resposta.tokensTotaisUtilizados,
    dataGeracao: dataGeracao.toISOString(),
    foiSimulada: resposta.foiSimulada,
    observacaoInfraestrutura: resposta.observacaoInfraestrutura,
    origem: "nova",
  };
}

function mapearDetalheHistorico(
  detalhe: AnaliseFinanceiraHistoricaDetalhe
): AnaliseAssistenteExibida {
  const conteudoProcessado = processarConteudoAnalise(detalhe.respostaIA);

  return {
    id: detalhe.id,
    perguntaUsuario: detalhe.perguntaUsuario,
    conteudo: conteudoProcessado.conteudo,
    sugestaoCompromisso: conteudoProcessado.sugestaoCompromisso,
    compromissoFinanceiroId: detalhe.compromissoFinanceiroId ?? null,
    modelo: detalhe.modeloIA,
    provedor: detalhe.provedorIA,
    tempoTotalMs: detalhe.tempoTotalMs,
    custoEstimadoUsd: detalhe.custoEstimadoUsd,
    tokensTotaisUtilizados: detalhe.tokensTotais,
    dataGeracao: detalhe.dataGeracao,
    origem: "historico",
  };
}

function processarConteudoAnalise(conteudo: string, sugestaoCompromissoBackend?: string | null) {
  const regex = /(?:^|\n)\s*(?:#{2,3}\s*)?Sugest[aã]o de compromisso\s*:?\s*([\s\S]*?)(?=\n(?:#{2,3}\s*|Sugest[aã]o de compromisso\s*:)|\n---|\s*$)/i;
  const correspondencia = conteudo.match(regex);

  if (sugestaoCompromissoBackend?.trim()) {
    return {
      conteudo: correspondencia ? conteudo.replace(correspondencia[0], "\n").trim() : removerSecaoSugestaoCompromisso(conteudo),
      sugestaoCompromisso: sugestaoCompromissoBackend.trim(),
    };
  }

  if (!correspondencia) {
    return {
      conteudo,
      sugestaoCompromisso: null,
    };
  }

  const sugestao = correspondencia[1].trim();
  const conteudoLimpo = conteudo.replace(correspondencia[0], "\n").trim();

  return {
    conteudo: conteudoLimpo,
    sugestaoCompromisso: sugestao || null,
  };
}

function removerSecaoSugestaoCompromisso(conteudo: string) {
  return conteudo
    .replace(/(?:^|\n)\s*(?:#{2,3}\s*)?Sugest[aã]o de compromisso\s*:?\s*([\s\S]*?)(?=\n(?:#{2,3}\s*|Sugest[aã]o de compromisso\s*:)|\n---|\s*$)/i, "\n")
    .trim();
}

export function AssistenteFinanceiroManager() {
  const { session } = useAuth();
  const analiseRef = useRef<HTMLDivElement | null>(null);

  const [resumo, setResumo] = useState<ResumoFinanceiroIAData | null>(null);
  const [mensagemErro, setMensagemErro] = useState("");
  const [resumoMinimizado, setResumoMinimizado] = useState(true);

  const [perguntaAnalise, setPerguntaAnalise] = useState("");
  const [analiseExibida, setAnaliseExibida] = useState<AnaliseAssistenteExibida | null>(null);
  const [erroAnaliseIa, setErroAnaliseIa] = useState("");
  const [gerandoAnaliseIa, setGerandoAnaliseIa] = useState(false);
  const [analiseCopiada, setAnaliseCopiada] = useState(false);
  const [analiseMinimizada, setAnaliseMinimizada] = useState(true);
  const [compromissoModalOpen, setCompromissoModalOpen] = useState(false);
  const [mensagemCompromisso, setMensagemCompromisso] = useState("");
  const [compromissoCriadoAnaliseId, setCompromissoCriadoAnaliseId] = useState<string | null>(null);

  const [historicoAnalises, setHistoricoAnalises] = useState<AnaliseFinanceiraHistoricaLista[]>([]);
  const [historicoErro, setHistoricoErro] = useState("");
  const [carregandoHistorico, setCarregandoHistorico] = useState(false);
  const [historicoMinimizado, setHistoricoMinimizado] = useState(true);
  const [historicoPaginaAtual, setHistoricoPaginaAtual] = useState(1);
  const [historicoItensPorPagina, setHistoricoItensPorPagina] = useState(ITENS_POR_PAGINA_PADRAO);
  const [historicoTotalPaginas, setHistoricoTotalPaginas] = useState(1);
  const [historicoTotalItens, setHistoricoTotalItens] = useState(0);
  const [carregandoAnaliseHistoricaId, setCarregandoAnaliseHistoricaId] = useState<string | null>(null);
  const [excluindoAnaliseId, setExcluindoAnaliseId] = useState<string | null>(null);
  const [analiseSelecionadaId, setAnaliseSelecionadaId] = useState<string | null>(null);

  const indicadores = useMemo(() => {
    return (resumo?.indicadores.todos ?? []).filter(
      (indicador): indicador is IndicadorResumoFinanceiroIA => Boolean(indicador?.nome)
    );
  }, [resumo]);

  const principaisIndicadores = useMemo(() => {
    const preferencia = [
      INDICADOR_PERCENTUAL_ECONOMIA,
      INDICADOR_ECONOMIA_MENSAL,
      INDICADOR_RESERVA_EMERGENCIA_ATUAL,
      INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO,
      INDICADOR_ENDIVIDAMENTO,
      INDICADOR_PATRIMONIO_LIQUIDO_ATUAL,
      INDICADOR_PERCENTUAL_PATRIMONIO_ALVO,
    ];

    const selecionados: IndicadorResumoFinanceiroIA[] = [];

    preferencia.forEach((codigo) => {
      const indicador = indicadores.find((item) => item.codigo === codigo);
      if (!indicador) {
        return;
      }

      if (selecionados.every((item) => item.codigo !== indicador.codigo) && selecionados.length < 4) {
        selecionados.push(indicador);
      }
    });

    if (selecionados.length < 4) {
      indicadores.forEach((indicador) => {
        if (selecionados.every((item) => item.codigo !== indicador.codigo) && selecionados.length < 4) {
          selecionados.push(indicador);
        }
      });
    }

    return selecionados;
  }, [indicadores]);

  const pontosAtencao = useMemo(() => {
    return indicadores.filter(
      (indicador) => indicador.status === STATUS_ATENCAO || indicador.status === STATUS_CRITICO
    );
  }, [indicadores]);

  const pontosFortes = useMemo(() => {
    const destaques = resumo?.destaquesPositivos ?? [];

    if (destaques.length > 0) {
      return destaques.slice(0, 3);
    }

    return resumo?.insights.destaquesPositivos.map((insight) => insight.titulo).slice(0, 3) ?? [];
  }, [resumo]);

  const prioridades = useMemo(() => {
    return resumo?.prioridadesImediatas.slice(0, 3) ?? [];
  }, [resumo]);

  const sugestaoCompromisso = analiseExibida?.sugestaoCompromisso?.trim() ?? "";
  const chaveAnaliseAtual = analiseExibida?.id ?? analiseExibida?.dataGeracao ?? null;
  const compromissoJaCriadoNestaAnalise = Boolean(
    chaveAnaliseAtual &&
      (compromissoCriadoAnaliseId === chaveAnaliseAtual || analiseExibida?.compromissoFinanceiroId)
  );

  const conclusao = useMemo(() => {
    if (!resumo) {
      return "";
    }

    return ConclusaoFinanceiraBuilder.construir(resumo, indicadores);
  }, [indicadores, resumo]);

  const carregarAnaliseHistorica = useCallback(
    async (analiseId: string) => {
      if (!session?.usuario.id || !session.token) {
        return;
      }

      try {
        setCarregandoAnaliseHistoricaId(analiseId);
        setErroAnaliseIa("");

        const response = await buscarAnaliseFinanceiraHistoricaDetalhe(
          session.usuario.id,
          analiseId,
          session.token
        );

        if (!response.dados) {
          return;
        }

        const analise = mapearDetalheHistorico(response.dados);
        setAnaliseExibida(analise);
        setPerguntaAnalise(analise.perguntaUsuario || "");
        setAnaliseSelecionadaId(analiseId);
        setAnaliseMinimizada(false);

        window.setTimeout(() => {
          analiseRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
        }, 50);
      } catch (error) {
        if (error instanceof ApiError) {
          setErroAnaliseIa(error.message);
        } else {
          setErroAnaliseIa("Não foi possível carregar a análise selecionada.");
        }
      } finally {
        setCarregandoAnaliseHistoricaId(null);
      }
    },
    [session?.token, session?.usuario.id]
  );

  const carregarResumo = useCallback(async () => {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    setMensagemErro("");

    try {
      const response = await buscarResumoFinanceiroIA(session.usuario.id, session.token);
      setResumo(response.dados);
    } catch (error) {
      if (error instanceof ApiError) {
        setMensagemErro(error.message);
      } else {
        setMensagemErro("Não foi possível carregar o Assistente Financeiro.");
      }
    }
  }, [session?.token, session?.usuario.id]);

  const carregarHistorico = useCallback(
    async (pagina = 1, tamanhoPagina = ITENS_POR_PAGINA_PADRAO) => {
      if (!session?.usuario.id || !session.token) {
        return;
      }

      setCarregandoHistorico(true);
      setHistoricoErro("");

      try {
        const response = await buscarAnalisesFinanceirasHistoricas(
          session.usuario.id,
          session.token,
          pagina,
          tamanhoPagina
        );

        const dados = response.dados as ResultadoPaginadoAnaliseFinanceiraHistorica | null;
        if (!dados) {
          setHistoricoAnalises([]);
          setHistoricoPaginaAtual(1);
          setHistoricoTotalPaginas(1);
          setHistoricoTotalItens(0);
          return;
        }

        if (dados.itens.length === 0 && dados.paginaAtual > 1 && dados.totalPaginas > 0) {
          setHistoricoPaginaAtual(dados.totalPaginas);
          return;
        }

        setHistoricoAnalises(dados.itens);
        setHistoricoPaginaAtual(dados.paginaAtual);
        setHistoricoItensPorPagina(dados.tamanhoPagina);
        setHistoricoTotalItens(dados.totalItens);
        setHistoricoTotalPaginas(dados.totalPaginas);
      } catch (error) {
        if (error instanceof ApiError) {
          setHistoricoErro(error.message);
        } else {
          setHistoricoErro("Não foi possível carregar o histórico de análises.");
        }
      } finally {
        setCarregandoHistorico(false);
      }
    },
    [session?.token, session?.usuario.id]
  );

  useEffect(() => {
    void carregarResumo();
  }, [carregarResumo]);

  useEffect(() => {
    void carregarHistorico(historicoPaginaAtual, historicoItensPorPagina);
  }, [carregarHistorico, historicoPaginaAtual, historicoItensPorPagina]);

  async function handleGerarAnalise() {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    const perguntaSanitizada = perguntaAnalise.trim();
    const perguntaFinal = perguntaSanitizada || analiseExibida?.perguntaUsuario || PERGUNTA_PADRAO_ANALISE;

    try {
      setGerandoAnaliseIa(true);
      setErroAnaliseIa("");
      setAnaliseCopiada(false);
      setMensagemCompromisso("");
      setCompromissoModalOpen(false);
      setCompromissoCriadoAnaliseId(null);

      const response = await gerarAnaliseAssistenteFinanceiro(session.usuario.id, session.token, {
        perguntaUsuario: perguntaFinal,
      });

      if (!response.dados) {
        return;
      }

      const agora = new Date();
      const analise = mapearNovaAnalise(response.dados, agora, perguntaFinal);
      setAnaliseExibida(analise);
      setPerguntaAnalise(perguntaFinal);
      setAnaliseSelecionadaId(response.dados.analiseFinanceiraHistoricaId ?? null);
      setAnaliseMinimizada(false);

      window.setTimeout(() => {
        analiseRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
      }, 50);

      await carregarHistorico(1, historicoItensPorPagina);
    } catch (error) {
      if (error instanceof ApiError) {
        setErroAnaliseIa(error.message);
      } else {
        setErroAnaliseIa("Não foi possível gerar a análise aprofundada neste momento.");
      }
    } finally {
      setGerandoAnaliseIa(false);
    }
  }

  async function handleCopiarAnalise() {
    if (!analiseExibida?.conteudo) {
      return;
    }

    await navigator.clipboard.writeText(analiseExibida.conteudo);
    setAnaliseCopiada(true);

    window.setTimeout(() => {
      setAnaliseCopiada(false);
    }, 2500);
  }

  function abrirModalCompromisso() {
    if (!analiseExibida?.sugestaoCompromisso || compromissoJaCriadoNestaAnalise) {
      return;
    }

    setMensagemCompromisso("");
    setCompromissoModalOpen(true);
  }

  async function handleSalvarCompromisso(payload: SalvarCompromissoFinanceiroPayload) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    try {
      setMensagemCompromisso("");

      await cadastrarCompromissoFinanceiro(
        session.usuario.id,
        {
          ...payload,
          usuarioId: session.usuario.id,
          origem: OrigemCompromissoFinanceiro.IA,
          analiseFinanceiraHistoricaId: chaveAnaliseAtual,
        },
        session.token
      );

      setMensagemCompromisso("Compromisso criado com sucesso.");
      if (chaveAnaliseAtual) {
        setCompromissoCriadoAnaliseId(chaveAnaliseAtual);
      }
      setAnaliseExibida((analiseAtual) =>
        analiseAtual
          ? {
              ...analiseAtual,
              compromissoFinanceiroId: chaveAnaliseAtual,
            }
          : analiseAtual
      );
      setCompromissoModalOpen(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setMensagemCompromisso(error.message);
      } else {
        setMensagemCompromisso("Não foi possível transformar a sugestão em compromisso.");
      }
    }
  }

  async function handleExcluirAnalise(analiseId: string) {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    const confirmar = window.confirm("Deseja excluir esta análise do histórico?");
    if (!confirmar) {
      return;
    }

    try {
      setExcluindoAnaliseId(analiseId);
      setHistoricoErro("");

      await excluirAnaliseFinanceiraHistorica(session.usuario.id, analiseId, session.token);

      if (analiseSelecionadaId === analiseId) {
        setAnaliseExibida(null);
        setAnaliseSelecionadaId(null);
      }

      await carregarHistorico(historicoPaginaAtual, historicoItensPorPagina);
    } catch (error) {
      if (error instanceof ApiError) {
        setHistoricoErro(error.message);
      } else {
        setHistoricoErro("Não foi possível excluir a análise selecionada.");
      }
    } finally {
      setExcluindoAnaliseId(null);
    }
  }

  function abrirAnaliseHistorica(analiseId: string) {
    setAnaliseMinimizada(false);
    void carregarAnaliseHistorica(analiseId);
  }

  const totalExibidoInicio = historicoTotalItens === 0 ? 0 : (historicoPaginaAtual - 1) * historicoItensPorPagina + 1;
  const totalExibidoFim = Math.min(historicoTotalItens, historicoPaginaAtual * historicoItensPorPagina);

  return (
    <div className="flex flex-row">
      <Sidebar />
      <main className="flex-1 bg-gray-50 p-6 dark:bg-[#020817]">
        <div className="space-y-2">
          <h1 className="text-2xl font-bold">Assistente Financeiro</h1>
          <p className="text-sm text-muted-foreground">Resumo executivo da sua situação financeira.</p>
        </div>

        {mensagemErro ? (
          <div className="mt-4 rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
            {mensagemErro}
          </div>
        ) : null}

        <section className="mt-6">
          <Card>
            <CardHeader className="space-y-4">
              <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                <div className="space-y-1">
                  <CardTitle>Resumo executivo</CardTitle>
                  <CardDescription>
                    Leitura consolidada de {resumo ? formatarDataReferencia(resumo.dataReferencia) : "sua base atual"}.
                  </CardDescription>
                </div>

                <Button variant="outline" onClick={() => setResumoMinimizado((valorAnterior) => !valorAnterior)}>
                  {resumoMinimizado ? (
                    <>
                      <ChevronDown className="mr-2 h-4 w-4" />
                      Mostrar resumo
                    </>
                  ) : (
                    <>
                      <ChevronUp className="mr-2 h-4 w-4" />
                      Minimizar resumo
                    </>
                  )}
                </Button>
              </div>

              <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
                <div className="flex items-end gap-3">
                  <p className="text-5xl font-bold tracking-tight">
                    {resumo?.saudeFinanceira.pontuacaoGeral ?? 0}
                    <span className="text-xl text-muted-foreground">/100</span>
                  </p>
                  <Badge
                    variant={obterVariantClassificacao(resumo?.saudeFinanceira.classificacao ?? "Atenção")}
                    className="mb-1"
                  >
                    {resumo?.saudeFinanceira.classificacao ?? "Atenção"}
                  </Badge>
                </div>

                <p className="max-w-md text-sm text-muted-foreground">
                  O detalhamento completo dos indicadores e gráficos fica centralizado em Saúde Financeira.
                </p>
              </div>
            </CardHeader>

            {resumoMinimizado ? (
              <CardContent className="pt-0">
                <div className="rounded-xl border border-border/60 bg-muted/20 px-4 py-4">
                  <p className="text-sm leading-6 text-muted-foreground">{obterFraseResumoCompacto(resumo)}</p>
                </div>
              </CardContent>
            ) : (
              <CardContent className="space-y-8">
                <section className="space-y-3">
                  <h3 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Resumo</h3>
                  <p className="text-base leading-7 text-foreground/90">
                    {resumo?.resumoExecutivo ??
                      "Seu resumo executivo aparecerá aqui assim que o backend retornar o ResumoFinanceiroIA."}
                  </p>
                </section>

                <section className="space-y-4">
                  <h3 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Prioridades</h3>
                  <div className="space-y-3">
                    {prioridades.length ? (
                      prioridades.map((prioridade, index) => (
                        <div key={prioridade} className="flex items-start gap-3 text-sm">
                          <span className="flex h-6 w-6 items-center justify-center rounded-full border border-border/70 text-xs font-semibold">
                            {index + 1}
                          </span>
                          <p className="pt-0.5 font-medium">{prioridade}</p>
                        </div>
                      ))
                    ) : (
                      <p className="text-sm text-muted-foreground">
                        Nenhuma prioridade imediata foi sinalizada no momento.
                      </p>
                    )}
                  </div>
                </section>

                <section className="space-y-4">
                  <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                    <div>
                      <h3 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">
                        Principais indicadores
                      </h3>
                      <p className="mt-1 text-sm text-muted-foreground">
                        Síntese em texto dos indicadores mais relevantes para decisão rápida.
                      </p>
                    </div>
                    <Button asChild variant="outline" className="sm:w-auto">
                      <Link href="/saude-financeira">
                        Ver análise completa
                        <ArrowRight className="ml-2 h-4 w-4" />
                      </Link>
                    </Button>
                  </div>

                  <div className="space-y-4">
                    {principaisIndicadores.length ? (
                      principaisIndicadores.map((indicador) => (
                        <div key={indicador.codigo} className="space-y-2 border-l-2 border-border pl-4">
                          <div className="flex items-center justify-between gap-3">
                            <p className="font-medium">{indicador.nome}</p>
                            <Badge variant={obterVariantBadge(indicador.status)}>
                              {obterTextoStatus(indicador.status)}
                            </Badge>
                          </div>
                          <p className="text-sm font-medium text-foreground">
                            {formatarValorIndicador(indicador.valorAtual, indicador.formato)}
                          </p>
                          <p className="text-sm text-muted-foreground">{obterTextoExecutivoIndicador(indicador)}</p>
                        </div>
                      ))
                    ) : (
                      <p className="text-sm text-muted-foreground">
                        Nenhum indicador foi retornado para o resumo executivo.
                      </p>
                    )}
                  </div>
                </section>

                <section className="space-y-4">
                  <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                    <div>
                      <h3 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">
                        Leitura estratégica
                      </h3>
                      <p className="mt-1 text-sm text-muted-foreground">
                        Pontos fortes e pontos de atenção resumidos, sem repetir a análise detalhada.
                      </p>
                    </div>
                    <Button asChild variant="ghost" className="px-0 text-primary hover:text-primary">
                      <Link href="/saude-financeira">
                        Ver análise completa em Saúde Financeira
                        <ArrowRight className="ml-2 h-4 w-4" />
                      </Link>
                    </Button>
                  </div>

                  <div className="space-y-2">
                    <p className="text-sm font-medium text-foreground">Pontos fortes</p>
                    {pontosFortes.length ? (
                      pontosFortes.map((ponto) => (
                        <p key={ponto} className="text-sm text-muted-foreground">
                          - {ponto}
                        </p>
                      ))
                    ) : (
                      <p className="text-sm text-muted-foreground">Nenhum destaque positivo relevante no momento.</p>
                    )}
                  </div>

                  <div className="space-y-2">
                    <p className="text-sm font-medium text-foreground">Pontos de atenção</p>
                    {pontosAtencao.length ? (
                      pontosAtencao.slice(0, 4).map((indicador) => (
                        <p key={indicador.codigo} className="text-sm text-muted-foreground">
                          - {indicador.nome}: {obterTextoPontoAtencao(indicador)}
                        </p>
                      ))
                    ) : (
                      <p className="text-sm text-muted-foreground">Nenhum ponto de atenção relevante no momento.</p>
                    )}
                  </div>
                </section>

                <section className="space-y-3">
                  <h3 className="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Conclusão</h3>
                  <p className="text-base leading-7 text-foreground/90">
                    {conclusao ||
                      "A conclusão dinâmica será exibida aqui assim que o resumo financeiro estiver disponível."}
                  </p>
                </section>
              </CardContent>
            )}
          </Card>
        </section>

        <section className="mt-8">
          <Card className="border-primary/20 bg-primary/5" ref={analiseRef}>
            <CardHeader className="space-y-3">
              <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                <div>
                  <CardTitle className="flex items-center gap-2">
                    <Sparkles className="h-5 w-5 text-primary" />
                    Análise aprofundada com IA
                  </CardTitle>
                  <CardDescription>
                    O sistema mostra primeiro sua situação consolidada e depois a IA aprofunda a interpretação.
                  </CardDescription>
                </div>

                <div className="flex flex-wrap gap-2">
                  <Button variant="outline" onClick={() => setAnaliseMinimizada((valorAnterior) => !valorAnterior)}>
                    {analiseMinimizada ? (
                      <>
                        <ChevronDown className="mr-2 h-4 w-4" />
                        Mostrar análise
                      </>
                    ) : (
                      <>
                        <ChevronUp className="mr-2 h-4 w-4" />
                        Minimizar análise
                      </>
                    )}
                  </Button>
                  <Button onClick={handleGerarAnalise} disabled={gerandoAnaliseIa || !session?.token}>
                    {gerandoAnaliseIa ? (
                      <>
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        Gerando análise...
                      </>
                    ) : analiseExibida ? (
                      "Gerar novamente"
                    ) : (
                      "Gerar análise aprofundada"
                    )}
                  </Button>
                </div>
              </div>

              {analiseMinimizada ? (
                <div className="rounded-xl border border-border/60 bg-muted/20 px-4 py-3 text-sm text-muted-foreground">
                  {analiseExibida
                    ? `Análise aberta: ${truncarTexto(analiseExibida.perguntaUsuario, 90)}. Pontuação ${
                        resumo?.saudeFinanceira.pontuacaoGeral ?? 0
                      }/100.`
                    : "Nenhuma análise carregada no momento. Gere uma análise aprofundada para começar a construir sua Memória Financeira."}
                </div>
              ) : null}
            </CardHeader>

            {!analiseMinimizada ? (
              <CardContent className="space-y-6 pt-0">
                <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
                  <div className="flex items-start gap-3">
                    <div className="rounded-2xl border border-primary/20 bg-background/80 p-3">
                      <HeartPulse className="h-6 w-6 text-primary" />
                    </div>
                    <div className="space-y-2">
                      <p className="max-w-2xl text-sm text-muted-foreground">
                        A análise usa os dados financeiros consolidados do sistema, incluindo o resumo executivo, os
                        indicadores, os insights e a Memória Financeira já registrada.
                      </p>
                      <p className="text-xs text-muted-foreground">
                        Você pode escrever uma pergunta opcional. Se deixar em branco, o Assistente usará a pergunta
                        padrão do sistema.
                      </p>
                    </div>
                  </div>

                  <div className="space-y-2">
                    <label htmlFor="pergunta-analise-ia" className="text-sm font-medium">
                      Pergunta opcional para a IA
                    </label>
                    <Textarea
                      id="pergunta-analise-ia"
                      placeholder="Ex.: Quais ajustes fariam mais diferença no meu próximo ciclo financeiro?"
                      value={perguntaAnalise}
                      onChange={(event) => setPerguntaAnalise(event.target.value)}
                      className="min-h-[96px] bg-background"
                    />
                  </div>
                </div>

                {gerandoAnaliseIa ? (
                  <div className="rounded-xl border border-primary/20 bg-background/80 px-4 py-4 text-sm text-muted-foreground">
                    <div className="flex items-center gap-2 font-medium text-foreground">
                      <Loader2 className="h-4 w-4 animate-spin text-primary" />
                      O Assistente Financeiro está analisando sua situação...
                    </div>
                    <p className="mt-2">
                      Aguarde um instante enquanto a IA aprofunda a leitura dos seus dados financeiros consolidados.
                    </p>
                  </div>
                ) : null}

                {erroAnaliseIa ? (
                  <div className="rounded-xl border border-destructive/20 bg-destructive/5 px-4 py-4 text-sm text-destructive">
                    {erroAnaliseIa}
                  </div>
                ) : null}

                <div className="space-y-4">
                  {analiseExibida ? (
                    <Card
                      key={`${analiseExibida.id ?? analiseExibida.dataGeracao}-${analiseExibida.conteudo.length}`}
                      className="border-border/60 bg-background/90 shadow-sm"
                    >
                      <CardHeader className="gap-4">
                        <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                          <div className="space-y-2">
                            <CardTitle className="text-xl">
                              {truncarTexto(analiseExibida.perguntaUsuario, 90)}
                            </CardTitle>
                            <CardDescription>
                              {analiseExibida.origem === "historico"
                                ? "Análise carregada do histórico visual do Assistente Financeiro."
                                : "Relatório aprofundado construído a partir do contexto financeiro consolidado do sistema."}
                            </CardDescription>
                          </div>
                        </div>

                        <div className="flex flex-wrap gap-2">
                          {analiseExibida.id ? (
                            <Badge variant="outline">Histórico #{analiseExibida.id.slice(0, 8)}</Badge>
                          ) : null}
                          {analiseExibida.foiSimulada ? <Badge variant="secondary">Resposta simulada</Badge> : null}
                        </div>
                      </CardHeader>

                      <CardContent className="space-y-6">
                        <div className="rounded-2xl border border-border/60 bg-card px-5 py-5">
                          <div className="space-y-4 text-sm leading-7 text-foreground/90">
                            <ReactMarkdown
                              remarkPlugins={[remarkGfm]}
                              components={{
                                h1: ({ children }) => <h1 className="text-2xl font-bold tracking-tight">{children}</h1>,
                                h2: ({ children }) => <h2 className="pt-2 text-xl font-semibold">{children}</h2>,
                                h3: ({ children }) => <h3 className="pt-1 text-lg font-semibold">{children}</h3>,
                                p: ({ children }) => <p className="leading-7">{children}</p>,
                                ul: ({ children }) => <ul className="list-disc space-y-2 pl-5">{children}</ul>,
                                ol: ({ children }) => <ol className="list-decimal space-y-2 pl-5">{children}</ol>,
                                li: ({ children }) => <li>{children}</li>,
                                strong: ({ children }) => (
                                  <strong className="font-semibold text-foreground">{children}</strong>
                                ),
                                hr: () => <hr className="border-border/70" />,
                                blockquote: ({ children }) => (
                                  <blockquote className="border-l-2 border-primary/30 pl-4 italic text-muted-foreground">
                                    {children}
                                  </blockquote>
                                ),
                              }}
                            >
                              {analiseExibida.conteudo}
                            </ReactMarkdown>
                          </div>
                        </div>

                        {sugestaoCompromisso ? (
                          <div className="rounded-2xl border border-primary/20 bg-primary/5 px-5 py-4">
                            <div className="space-y-3">
                              <div className="space-y-2">
                                <p className="text-sm font-semibold uppercase tracking-wide text-primary">
                                  Sugestão de compromisso
                                </p>
                                <p className="text-sm leading-6 text-foreground/90">{sugestaoCompromisso}</p>
                                {compromissoJaCriadoNestaAnalise ? (
                                  <p className="text-sm font-medium text-primary">Compromisso criado com sucesso.</p>
                                ) : null}
                              </div>

                              <div className="flex flex-wrap items-center gap-3">
                                <Button
                                  type="button"
                                  onClick={abrirModalCompromisso}
                                  disabled={compromissoJaCriadoNestaAnalise}
                                >
                                  {compromissoJaCriadoNestaAnalise
                                    ? "Compromisso criado"
                                    : "Transformar em compromisso"}
                                </Button>
                                <span className="text-xs text-muted-foreground">
                                  Revise o texto antes de salvar o compromisso.
                                </span>
                              </div>
                            </div>

                            {mensagemCompromisso ? (
                              <p className="mt-3 text-sm text-muted-foreground">{mensagemCompromisso}</p>
                            ) : null}
                          </div>
                        ) : null}
                      </CardContent>
                    </Card>
                  ) : (
                    <div className="rounded-xl border border-dashed border-border/70 bg-background/70 px-4 py-5 text-sm text-muted-foreground">
                      Nenhuma análise carregada no momento. Selecione um item do histórico ou gere uma nova análise
                      aprofundada.
                    </div>
                  )}
                </div>
              </CardContent>
            ) : null}
          </Card>
        </section>

        <section className="mt-8">
          <Card className="border-border/60 bg-background/90">
            <CardHeader className="space-y-3">
              <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                <div>
                  <CardTitle className="flex items-center gap-2">
                    <HeartPulse className="h-5 w-5 text-primary" />
                    Histórico de análises
                  </CardTitle>
                  <CardDescription>
                    Acesso rápido às últimas Memórias Financeiras geradas para este usuário.
                  </CardDescription>
                </div>

                <Button variant="outline" onClick={() => setHistoricoMinimizado((valorAnterior) => !valorAnterior)}>
                  {historicoMinimizado ? (
                    <>
                      <ChevronDown className="mr-2 h-4 w-4" />
                      Mostrar histórico
                    </>
                  ) : (
                    <>
                      <ChevronUp className="mr-2 h-4 w-4" />
                      Minimizar histórico
                    </>
                  )}
                </Button>
              </div>

              {historicoMinimizado ? (
                <div className="rounded-xl border border-border/60 bg-muted/20 px-4 py-3 text-sm text-muted-foreground">
                  {historicoTotalItens
                    ? `${historicoTotalItens} análise(s) disponível(is). Mostrando ${totalExibidoInicio}-${totalExibidoFim}.`
                    : "Nenhuma análise gerada ainda. Gere uma análise aprofundada para começar a construir sua Memória Financeira."}
                </div>
              ) : null}
            </CardHeader>

            {!historicoMinimizado ? (
              <CardContent className="space-y-4 pt-0">
                {historicoErro ? (
                  <div className="rounded-xl border border-destructive/20 bg-destructive/5 px-4 py-4 text-sm text-destructive">
                    {historicoErro}
                  </div>
                ) : null}

                <div className="flex flex-col gap-3 rounded-xl border border-border/60 bg-background/70 p-3 sm:flex-row sm:items-end sm:justify-between">
                  <div className="grid gap-3 sm:grid-cols-2">
                    <div className="space-y-1">
                      <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
                        Itens por página
                      </p>
                      <Select
                        value={String(historicoItensPorPagina)}
                        onValueChange={(valor) => {
                          const novoTamanho = Number(valor);
                          setHistoricoItensPorPagina(novoTamanho);
                          setHistoricoPaginaAtual(1);
                        }}
                      >
                        <SelectTrigger className="w-[140px]">
                          <SelectValue placeholder="Itens" />
                        </SelectTrigger>
                        <SelectContent>
                          {[5, 10, 20, 50].map((valor) => (
                            <SelectItem key={valor} value={String(valor)}>
                              {valor}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>

                    <div className="space-y-1">
                      <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">Página atual</p>
                      <p className="text-sm font-semibold">
                        {historicoPaginaAtual} de {historicoTotalPaginas}
                      </p>
                    </div>
                  </div>

                  <div className="flex items-center gap-2">
                    <Button
                      variant="outline"
                      onClick={() => setHistoricoPaginaAtual((valor) => Math.max(1, valor - 1))}
                      disabled={historicoPaginaAtual <= 1 || carregandoHistorico}
                    >
                      Página anterior
                    </Button>
                    <Button
                      variant="outline"
                      onClick={() =>
                        setHistoricoPaginaAtual((valor) => Math.min(historicoTotalPaginas, valor + 1))
                      }
                      disabled={historicoPaginaAtual >= historicoTotalPaginas || carregandoHistorico}
                    >
                      Próxima página
                    </Button>
                  </div>
                </div>

                {carregandoHistorico ? (
                  <div className="flex items-center gap-2 px-4 py-5 text-sm text-muted-foreground">
                    <Loader2 className="h-4 w-4 animate-spin" />
                    Carregando histórico...
                  </div>
                ) : historicoAnalises.length ? (
                  <div className="rounded-xl border border-border/60 bg-background/70">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead className="w-[110px]">Data</TableHead>
                          <TableHead className="w-[90px]">Hora</TableHead>
                          <TableHead>Pergunta do usuário</TableHead>
                          <TableHead className="w-[90px]">Nota</TableHead>
                          <TableHead className="w-[130px]">Classificação</TableHead>
                          <TableHead className="w-[180px] text-right">Ações</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {historicoAnalises.map((analise) => {
                          const estaSelecionada = analiseSelecionadaId === analise.id;
                          const estaCarregando = carregandoAnaliseHistoricaId === analise.id;
                          const estaExcluindo = excluindoAnaliseId === analise.id;

                          return (
                            <TableRow
                              key={analise.id}
                              className={estaSelecionada ? "bg-primary/5" : "cursor-pointer"}
                              onClick={() => abrirAnaliseHistorica(analise.id)}
                            >
                              <TableCell className="text-xs">{formatarData(analise.dataGeracao)}</TableCell>
                              <TableCell className="text-xs">{formatarHora(analise.dataGeracao)}</TableCell>
                              <TableCell>
                                <div className="space-y-2">
                                  <button
                                    type="button"
                                    className="block max-w-[360px] truncate text-left text-sm font-medium text-foreground hover:underline"
                                    title={analise.perguntaUsuario || "Análise financeira geral"}
                                    onClick={(event) => {
                                      event.stopPropagation();
                                      abrirAnaliseHistorica(analise.id);
                                    }}
                                    disabled={estaCarregando || estaExcluindo}
                                  >
                                    {analise.perguntaUsuario || "Análise financeira geral"}
                                  </button>
                                  {analise.compromissoFinanceiroId ? (
                                    <Badge variant="secondary" className="text-[11px]">
                                      Compromisso gerado
                                    </Badge>
                                  ) : null}
                                </div>
                              </TableCell>
                              <TableCell className="font-semibold">{analise.pontuacaoSaudeFinanceira}/100</TableCell>
                              <TableCell>
                                <Badge variant={obterVariantClassificacao(analise.classificacaoSaudeFinanceira)}>
                                  {obterTextoStatus(
                                    analise.classificacaoSaudeFinanceira === "Excelente"
                                      ? STATUS_EXCELENTE
                                      : analise.classificacaoSaudeFinanceira === "Boa"
                                        ? STATUS_BOM
                                        : analise.classificacaoSaudeFinanceira === "Crítica" ||
                                            analise.classificacaoSaudeFinanceira === "Critica"
                                          ? STATUS_CRITICO
                                          : STATUS_ATENCAO
                                  )}
                                </Badge>
                              </TableCell>
                              <TableCell>
                                <div className="flex items-center justify-end gap-2">
                                  <Button
                                    variant="outline"
                                    size="sm"
                                    onClick={(event) => {
                                      event.stopPropagation();
                                      abrirAnaliseHistorica(analise.id);
                                    }}
                                    disabled={estaCarregando || estaExcluindo}
                                  >
                                    Ver
                                  </Button>
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    onClick={(event) => {
                                      event.stopPropagation();
                                      void handleExcluirAnalise(analise.id);
                                    }}
                                    disabled={estaExcluindo || estaCarregando}
                                  >
                                    {estaExcluindo ? (
                                      <Loader2 className="h-4 w-4 animate-spin" />
                                    ) : (
                                      <Trash2 className="h-4 w-4" />
                                    )}
                                  </Button>
                                </div>
                              </TableCell>
                            </TableRow>
                          );
                        })}
                      </TableBody>
                    </Table>
                  </div>
                ) : (
                  <div className="rounded-xl border border-dashed border-border/70 bg-background/70 px-4 py-5 text-sm text-muted-foreground">
                    Nenhuma análise gerada ainda. Gere uma análise aprofundada para começar a construir sua Memória
                    Financeira.
                  </div>
                )}
              </CardContent>
            ) : null}
          </Card>
        </section>

        <CompromissoFinanceiroModal
          open={compromissoModalOpen}
          onOpenChange={setCompromissoModalOpen}
          mode="create"
          initialDescricao={sugestaoCompromisso}
          defaultOrigin={OrigemCompromissoFinanceiro.IA}
          travarOrigem
          title="Transformar em compromisso"
          description="Edite a sugestão antes de confirmar. O compromisso será salvo para acompanhar suas próximas análises."
          submitLabel="Confirmar compromisso"
          onSubmit={handleSalvarCompromisso}
        />
      </main>
    </div>
  );
}




