"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { ArrowRight, HeartPulse } from "lucide-react";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  obterTextoExecutivoIndicador,
  obterTextoPontoAtencao,
} from "@/lib/assistente-financeiro-textos";
import { ConclusaoFinanceiraBuilder } from "@/lib/conclusao-financeira-builder";
import { useAuth } from "@/providers/auth-provider";
import { buscarResumoFinanceiroIA } from "@/services/api/resumo-financeiro-ia";
import { ApiError } from "@/types/api";
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

export function AssistenteFinanceiroManager() {
  const { session } = useAuth();
  const [resumo, setResumo] = useState<ResumoFinanceiroIAData | null>(null);
  const [mensagemErro, setMensagemErro] = useState("");

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
                  {resumo?.resumoExecutivo ?? "Seu resumo executivo aparecerá aqui assim que o backend retornar o ResumoFinanceiroIA."}
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
                  {conclusao || "A conclusão dinâmica será exibida aqui assim que o resumo financeiro estiver disponível."}
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
                Futuramente será possível receber uma análise personalizada baseada no seu histórico financeiro consolidado.
              </CardDescription>
            </CardHeader>
            <CardContent className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div className="flex items-start gap-3">
                <div className="rounded-2xl border border-primary/20 bg-background/80 p-3">
                  <HeartPulse className="h-6 w-6 text-primary" />
                </div>
                <p className="max-w-2xl text-sm text-muted-foreground">
                  Esta área já está preparada na arquitetura, mas a chamada real para IA só entrará na próxima fase do roadmap.
                </p>
              </div>
              <div className="flex flex-col items-start gap-2 sm:items-end">
                <Button disabled>Gerar análise aprofundada</Button>
                <p className="text-xs text-muted-foreground">Disponível nas próximas versões.</p>
              </div>
            </CardContent>
          </Card>
        </section>
      </main>
    </div>
  );
}
