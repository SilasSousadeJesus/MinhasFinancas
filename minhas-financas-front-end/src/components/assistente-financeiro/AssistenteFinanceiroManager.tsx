"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import {
  ArrowRight,
  Clipboard,
  CopyCheck,
  HeartPulse,
  Loader2,
  Sparkles,
} from "lucide-react";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Textarea } from "@/components/ui/textarea";
import {
  obterTextoExecutivoIndicador,
  obterTextoPontoAtencao,
} from "@/lib/assistente-financeiro-textos";
import { ConclusaoFinanceiraBuilder } from "@/lib/conclusao-financeira-builder";
import { useAuth } from "@/providers/auth-provider";
import { gerarAnaliseAssistenteFinanceiro } from "@/services/api/assistente-financeiro";
import { buscarResumoFinanceiroIA } from "@/services/api/resumo-financeiro-ia";
import { ApiError } from "@/types/api";
import { RespostaAssistenteFinanceiroIA } from "@/types/assistente-financeiro";
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

const PERGUNTA_PADRAO_ANALISE =
  "Quais sao os principais riscos e prioridades da minha situacao financeira atual?";

function formatarValorIndicador(valor: number, formato: number) {
  if (formato === FORMATO_PERCENTUAL) {
    return `${valor.toFixed(1)}%`;
  }

  if (formato === FORMATO_MESES) {
    return `${valor.toFixed(1)} mes(es)`;
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
      return "Bom";
    case STATUS_CRITICO:
      return "Critico";
    default:
      return "Atencao";
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

function formatarDataHora(data: Date) {
  return new Intl.DateTimeFormat("pt-BR", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(data);
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

export function AssistenteFinanceiroManager() {
  const { session } = useAuth();
  const [resumo, setResumo] = useState<ResumoFinanceiroIAData | null>(null);
  const [mensagemErro, setMensagemErro] = useState("");
  const [perguntaAnalise, setPerguntaAnalise] = useState("");
  const [analiseIa, setAnaliseIa] = useState<RespostaAssistenteFinanceiroIA | null>(null);
  const [erroAnaliseIa, setErroAnaliseIa] = useState("");
  const [gerandoAnaliseIa, setGerandoAnaliseIa] = useState(false);
  const [analiseCopiada, setAnaliseCopiada] = useState(false);
  const [dataGeracaoAnalise, setDataGeracaoAnalise] = useState<Date | null>(null);

  useEffect(() => {
    async function carregarResumo() {
      if (!session?.usuario.id || !session.token) {
        return;
      }

      try {
        setMensagemErro("");
        const response = await buscarResumoFinanceiroIA(session.usuario.id, session.token);
        setResumo(response.dados);
      } catch (error) {
        if (error instanceof ApiError) {
          setMensagemErro(error.message);
        } else {
          setMensagemErro("Nao foi possivel carregar o Assistente Financeiro.");
        }
      }
    }

    carregarResumo();
  }, [session?.token, session?.usuario.id]);

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

      const jaExiste = selecionados.some((item) => item.codigo === indicador.codigo);

      if (!jaExiste && selecionados.length < 4) {
        selecionados.push(indicador);
      }
    });

    if (selecionados.length < 4) {
      indicadores.forEach((indicador) => {
        const jaExiste = selecionados.some((item) => item.codigo === indicador.codigo);

        if (!jaExiste && selecionados.length < 4) {
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

  const conclusao = useMemo(() => {
    if (!resumo) {
      return "";
    }

    return ConclusaoFinanceiraBuilder.construir(resumo, indicadores);
  }, [indicadores, resumo]);

  async function handleGerarAnalise() {
    if (!session?.usuario.id || !session.token) {
      return;
    }

    const perguntaSanitizada = perguntaAnalise.trim();
    const perguntaFinal = perguntaSanitizada || PERGUNTA_PADRAO_ANALISE;

    try {
      setGerandoAnaliseIa(true);
      setErroAnaliseIa("");
      setAnaliseCopiada(false);

      const response = await gerarAnaliseAssistenteFinanceiro(session.usuario.id, session.token, {
        perguntaUsuario: perguntaFinal,
      });

      setAnaliseIa(response.dados);
      setDataGeracaoAnalise(new Date());
    } catch (error) {
      if (error instanceof ApiError) {
        setErroAnaliseIa(error.message);
      } else {
        setErroAnaliseIa("Nao foi possivel gerar a analise aprofundada neste momento.");
      }
    } finally {
      setGerandoAnaliseIa(false);
    }
  }

  async function handleCopiarAnalise() {
    if (!analiseIa?.conteudo) {
      return;
    }

    await navigator.clipboard.writeText(analiseIa.conteudo);
    setAnaliseCopiada(true);

    window.setTimeout(() => {
      setAnaliseCopiada(false);
    }, 2500);
  }

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
              <div className="space-y-1">
                <CardTitle>Resumo executivo</CardTitle>
                <CardDescription>
                  Leitura consolidada de {resumo ? formatarDataReferencia(resumo.dataReferencia) : "sua base atual"}.
                </CardDescription>
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
                          <Badge variant={obterVariantBadge(indicador.status)}>{obterTextoStatus(indicador.status)}</Badge>
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
          </Card>
        </section>

        <section className="mt-8">
          <Card className="border-primary/20 bg-primary/5">
            <CardHeader>
              <CardTitle>Análise aprofundada com IA</CardTitle>
              <CardDescription>
                O sistema mostra primeiro sua situação consolidada e depois a IA aprofunda a interpretação.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
                <div className="flex items-start gap-3">
                  <div className="rounded-2xl border border-primary/20 bg-background/80 p-3">
                    <HeartPulse className="h-6 w-6 text-primary" />
                  </div>
                  <div className="space-y-2">
                    <p className="max-w-2xl text-sm text-muted-foreground">
                      A análise usa os dados financeiros consolidados do sistema, incluindo o resumo executivo, os
                      indicadores, os insights e a Memória Financeira já registrada no backend.
                    </p>
                    <p className="text-xs text-muted-foreground">
                      Você pode escrever uma pergunta opcional. Se deixar em branco, o Assistente usará a pergunta
                      padrão do sistema.
                    </p>
                  </div>
                </div>

                <div className="flex flex-col items-start gap-2 lg:items-end">
                  <Button onClick={handleGerarAnalise} disabled={gerandoAnaliseIa || !session?.token}>
                    {gerandoAnaliseIa ? (
                      <>
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        Gerando análise...
                      </>
                    ) : analiseIa ? (
                      "Gerar novamente"
                    ) : (
                      "Gerar análise aprofundada"
                    )}
                  </Button>
                  <p className="text-xs text-muted-foreground">
                    A Memória Financeira continua sendo salva apenas pelo backend.
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

              {gerandoAnaliseIa ? (
                <div className="rounded-xl border border-primary/20 bg-background/80 px-4 py-4 text-sm text-muted-foreground">
                  <div className="flex items-center gap-2 font-medium text-foreground">
                    <Sparkles className="h-4 w-4 text-primary" />
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

              {analiseIa ? (
                <Card className="border-border/60 bg-background/90 shadow-sm">
                  <CardHeader className="gap-4">
                    <div className="flex flex-col gap-3 lg:flex-row lg:items-start lg:justify-between">
                      <div className="space-y-2">
                        <CardTitle className="text-xl">Parecer executivo da IA</CardTitle>
                        <CardDescription>
                          Relatório aprofundado construído a partir do contexto financeiro consolidado do sistema.
                        </CardDescription>
                      </div>

                      <div className="flex flex-wrap gap-2">
                        <Button variant="outline" onClick={handleCopiarAnalise}>
                          {analiseCopiada ? (
                            <>
                              <CopyCheck className="mr-2 h-4 w-4" />
                              Análise copiada
                            </>
                          ) : (
                            <>
                              <Clipboard className="mr-2 h-4 w-4" />
                              Copiar análise
                            </>
                          )}
                        </Button>
                        <Button onClick={handleGerarAnalise} disabled={gerandoAnaliseIa}>
                          {gerandoAnaliseIa ? (
                            <>
                              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                              Gerando...
                            </>
                          ) : (
                            "Gerar novamente"
                          )}
                        </Button>
                      </div>
                    </div>

                    <div className="flex flex-wrap gap-2">
                      {analiseIa.analiseFinanceiraHistoricaId ? (
                        <Badge variant="outline">Histórico #{analiseIa.analiseFinanceiraHistoricaId.slice(0, 8)}</Badge>
                      ) : null}
                      {analiseIa.foiSimulada ? <Badge variant="secondary">Resposta simulada</Badge> : null}
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
                            strong: ({ children }) => <strong className="font-semibold text-foreground">{children}</strong>,
                            hr: () => <hr className="border-border/70" />,
                            blockquote: ({ children }) => (
                              <blockquote className="border-l-2 border-primary/30 pl-4 italic text-muted-foreground">
                                {children}
                              </blockquote>
                            ),
                          }}
                        >
                          {analiseIa.conteudo}
                        </ReactMarkdown>
                      </div>
                    </div>

                    <div className="rounded-xl border border-border/50 bg-muted/30 px-4 py-3">
                      <p className="text-xs font-semibold uppercase tracking-wide text-muted-foreground">
                        Dados técnicos da geração
                      </p>
                      <div className="mt-3 grid gap-2 text-xs text-muted-foreground sm:grid-cols-2 xl:grid-cols-5">
                        <p>
                          <span className="font-medium text-foreground">Modelo:</span> {analiseIa.modelo || "Não informado"}
                        </p>
                        <p>
                          <span className="font-medium text-foreground">Tempo total:</span> {formatarTempo(analiseIa.tempoTotalMs)}
                        </p>
                        <p>
                          <span className="font-medium text-foreground">Tokens totais:</span> {analiseIa.tokensTotaisUtilizados || 0}
                        </p>
                        <p>
                          <span className="font-medium text-foreground">Custo estimado:</span> {formatarCusto(analiseIa.custoEstimadoUsd)}
                        </p>
                        <p>
                          <span className="font-medium text-foreground">Gerado em:</span>{" "}
                          {dataGeracaoAnalise ? formatarDataHora(dataGeracaoAnalise) : "Agora"}
                        </p>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ) : (
                <div className="rounded-xl border border-dashed border-border/70 bg-background/70 px-4 py-5 text-sm text-muted-foreground">
                  Gere a primeira análise aprofundada para visualizar o parecer executivo em Markdown diretamente nesta
                  tela.
                </div>
              )}
            </CardContent>
          </Card>
        </section>
      </main>
    </div>
  );
}
