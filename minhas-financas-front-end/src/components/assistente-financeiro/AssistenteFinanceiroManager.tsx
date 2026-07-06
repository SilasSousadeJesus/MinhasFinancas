"use client";

import { useEffect, useMemo, useState } from "react";
import { BrainCircuit, CheckCircle2, ChevronRight, HeartPulse, Info, Sparkles, TriangleAlert } from "lucide-react";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
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

const PRIORIDADE_ALTA = 0;
const PRIORIDADE_MEDIA = 1;
const PRIORIDADE_BAIXA = 2;

const TIPO_ALERTA = 0;
const TIPO_OPORTUNIDADE = 1;
const TIPO_DESTAQUE_POSITIVO = 2;
const TIPO_CONFIGURACAO = 3;

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
      return "Bom";
    case STATUS_CRITICO:
      return "Crítico";
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
      return "destructive";
    default:
      return "default";
  }
}

function obterTituloPrioridade(prioridade: number) {
  switch (prioridade) {
    case PRIORIDADE_ALTA:
      return "Crítico";
    case PRIORIDADE_MEDIA:
      return "Atenção";
    default:
      return "Informação";
  }
}

function obterVariantPrioridade(prioridade: number): "default" | "secondary" | "destructive" | "outline" {
  switch (prioridade) {
    case PRIORIDADE_ALTA:
      return "destructive";
    case PRIORIDADE_MEDIA:
      return "default";
    default:
      return "outline";
  }
}

function obterTextoTipoInsight(tipo: number) {
  switch (tipo) {
    case TIPO_ALERTA:
      return "Alerta";
    case TIPO_OPORTUNIDADE:
      return "Oportunidade";
    case TIPO_DESTAQUE_POSITIVO:
      return "Positivo";
    case TIPO_CONFIGURACAO:
      return "Configuração";
    default:
      return "Informação";
  }
}

function obterIconeTipoInsight(tipo: number) {
  switch (tipo) {
    case TIPO_ALERTA:
      return TriangleAlert;
    case TIPO_OPORTUNIDADE:
      return Sparkles;
    case TIPO_DESTAQUE_POSITIVO:
      return CheckCircle2;
    case TIPO_CONFIGURACAO:
      return Info;
    default:
      return BrainCircuit;
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
          setMensagemErro("Não foi possível carregar o Assistente Financeiro.");
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

  const insights = useMemo(() => {
    return resumo?.insights.todos ?? [];
  }, [resumo]);

  const prioridades = useMemo(() => {
    return resumo?.prioridadesImediatas.slice(0, 3) ?? [];
  }, [resumo]);

  return (
    <div className="flex flex-row">
      <Sidebar />
      <main className="flex-1 bg-gray-50 p-6 dark:bg-[#020817]">
        <div className="space-y-2">
          <h1 className="text-2xl font-bold">Assistente Financeiro</h1>
          <p className="text-sm text-muted-foreground">Resumo da sua situação financeira.</p>
        </div>

        {mensagemErro ? (
          <div className="mt-4 rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
            {mensagemErro}
          </div>
        ) : null}

        <div className="mt-6 grid grid-cols-1 gap-4 xl:grid-cols-[1.35fr_0.65fr]">
          <Card>
            <CardHeader>
              <CardTitle>Resumo executivo</CardTitle>
              <CardDescription>
                Leitura consolidada de {resumo ? formatarDataReferencia(resumo.dataReferencia) : "sua base atual"}.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="rounded-2xl border border-border/70 bg-background/70 p-5">
                <p className="text-base leading-7 text-foreground/90">
                  {resumo?.resumoExecutivo ?? "Seu resumo executivo aparecerá aqui assim que o backend retornar o ResumoFinanceiroIA."}
                </p>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Saúde Financeira</CardTitle>
              <CardDescription>Pontuação geral e classificação atual.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="rounded-2xl border border-border/70 bg-background/70 p-5 text-center">
                <p className="text-sm text-muted-foreground">Pontuação</p>
                <p className="mt-2 text-5xl font-bold tracking-tight">
                  {resumo?.saudeFinanceira.pontuacaoGeral ?? 0}
                  <span className="text-xl text-muted-foreground"> / 100</span>
                </p>
              </div>
              <div className="rounded-2xl border border-border/70 bg-background/70 p-5">
                <p className="text-sm text-muted-foreground">Classificação</p>
                <div className="mt-3">
                  <Badge variant={obterVariantClassificacao(resumo?.saudeFinanceira.classificacao ?? "Atenção")}>
                    {resumo?.saudeFinanceira.classificacao ?? "Atenção"}
                  </Badge>
                </div>
                <p className="mt-4 text-sm text-muted-foreground">
                  A leitura executiva abaixo sempre reutiliza a mesma base analítica do sistema.
                </p>
              </div>
            </CardContent>
          </Card>
        </div>

        <section className="mt-8">
          <div className="mb-4 space-y-1">
            <h2 className="text-xl font-semibold">Indicadores Financeiros</h2>
            <p className="text-sm text-muted-foreground">
              Cards executivos montados a partir do ResumoFinanceiroIA, sem recalcular nenhum indicador no frontend.
            </p>
          </div>

          <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-4">
            {indicadores.map((indicador) => (
              <Card key={indicador.codigo}>
                <CardHeader className="space-y-3">
                  <div className="flex items-start justify-between gap-3">
                    <CardTitle className="text-base leading-snug">{indicador.nome}</CardTitle>
                    <Badge variant={obterVariantBadge(indicador.status)}>{obterTextoStatus(indicador.status)}</Badge>
                  </div>
                  <CardDescription>{indicador.descricao}</CardDescription>
                </CardHeader>
                <CardContent className="space-y-3">
                  <div>
                    <p className="text-xs uppercase tracking-wide text-muted-foreground">Valor atual</p>
                    <p className="text-2xl font-semibold">
                      {formatarValorIndicador(indicador.valorAtual, indicador.formato)}
                    </p>
                  </div>

                  <div className="grid grid-cols-2 gap-3 text-sm">
                    <div className="rounded-lg border border-border/60 bg-background/70 p-3">
                      <p className="text-xs uppercase tracking-wide text-muted-foreground">Valor ideal</p>
                      <p className="mt-1 font-medium">
                        {formatarValorIndicador(indicador.valorIdeal, indicador.formato)}
                      </p>
                    </div>
                    <div className="rounded-lg border border-border/60 bg-background/70 p-3">
                      <p className="text-xs uppercase tracking-wide text-muted-foreground">Progresso</p>
                      <p className="mt-1 font-medium">{indicador.percentual.toFixed(1)}%</p>
                    </div>
                  </div>

                  <div className="rounded-lg border border-border/60 bg-background/70 p-3 text-sm text-muted-foreground">
                    {indicador.observacao}
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </section>

        <div className="mt-8 grid grid-cols-1 gap-4 xl:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle>Pontos fortes</CardTitle>
              <CardDescription>Somente os destaques positivos destacados pelo backend.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              {pontosFortes.length ? (
                pontosFortes.map((ponto) => (
                  <div key={ponto} className="flex items-start gap-3 rounded-xl border border-emerald-500/20 bg-emerald-500/5 p-4">
                    <CheckCircle2 className="mt-0.5 h-5 w-5 text-emerald-500" />
                    <p className="text-sm font-medium text-foreground">{ponto}</p>
                  </div>
                ))
              ) : (
                <div className="rounded-xl border border-dashed border-border/70 bg-background/70 p-4 text-sm text-muted-foreground">
                  Ainda não há destaques positivos suficientes no resumo atual.
                </div>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Pontos de atenção</CardTitle>
              <CardDescription>Indicadores classificados como Atenção ou Crítico.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              {pontosAtencao.length ? (
                pontosAtencao.map((indicador) => (
                  <div key={indicador.codigo} className="rounded-xl border border-border/70 bg-background/70 p-4">
                    <div className="flex items-center justify-between gap-3">
                      <p className="font-medium">{indicador.nome}</p>
                      <Badge variant={obterVariantBadge(indicador.status)}>{obterTextoStatus(indicador.status)}</Badge>
                    </div>
                    <p className="mt-2 text-sm text-muted-foreground">{indicador.descricao}</p>
                    <p className="mt-2 text-xs text-muted-foreground">{indicador.observacao}</p>
                  </div>
                ))
              ) : (
                <div className="rounded-xl border border-dashed border-border/70 bg-background/70 p-4 text-sm text-muted-foreground">
                  Nenhum indicador crítico ou de atenção no momento.
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        <section className="mt-8">
          <div className="mb-4 space-y-1">
            <h2 className="text-xl font-semibold">Insights Financeiros</h2>
            <p className="text-sm text-muted-foreground">
              Lista priorizada das leituras produzidas pelo backend para orientar sua próxima decisão.
            </p>
          </div>

          <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
            {insights.length ? (
              insights.map((insight) => {
                const IconeInsight = obterIconeTipoInsight(insight.tipo);

                return (
                  <Card key={`${insight.titulo}-${insight.prioridade}-${insight.tipo}`}>
                    <CardHeader className="space-y-3">
                      <div className="flex items-start justify-between gap-3">
                        <div className="flex items-start gap-3">
                          <div className="rounded-xl border border-border/60 bg-background/70 p-2">
                            <IconeInsight className="h-5 w-5" />
                          </div>
                          <div>
                            <CardTitle className="text-base">{insight.titulo}</CardTitle>
                            <CardDescription className="mt-1">{obterTextoTipoInsight(insight.tipo)}</CardDescription>
                          </div>
                        </div>
                        <Badge variant={obterVariantPrioridade(insight.prioridade)}>
                          {obterTituloPrioridade(insight.prioridade)}
                        </Badge>
                      </div>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <p className="text-sm text-muted-foreground">{insight.descricao}</p>
                      {insight.acaoSugerida ? (
                        <div className="rounded-xl border border-border/60 bg-background/70 p-3 text-sm">
                          <span className="font-medium">Ação sugerida:</span> {insight.acaoSugerida}
                        </div>
                      ) : null}
                    </CardContent>
                  </Card>
                );
              })
            ) : (
              <Card className="xl:col-span-2">
                <CardContent className="p-6 text-sm text-muted-foreground">
                  O backend ainda não retornou insights financeiros para o contexto atual.
                </CardContent>
              </Card>
            )}
          </div>
        </section>

        <div className="mt-8 grid grid-cols-1 gap-4 xl:grid-cols-[0.9fr_1.1fr]">
          <Card>
            <CardHeader>
              <CardTitle>Próximas prioridades</CardTitle>
              <CardDescription>As três prioridades imediatas enviadas pelo ResumoFinanceiroIA.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              {prioridades.length ? (
                prioridades.map((prioridade) => (
                  <div key={prioridade} className="flex items-start gap-3 rounded-xl border border-border/70 bg-background/70 p-4">
                    <ChevronRight className="mt-0.5 h-5 w-5 text-sky-500" />
                    <p className="text-sm font-medium">{prioridade}</p>
                  </div>
                ))
              ) : (
                <div className="rounded-xl border border-dashed border-border/70 bg-background/70 p-4 text-sm text-muted-foreground">
                  Nenhuma prioridade imediata foi sinalizada no momento.
                </div>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Gráficos</CardTitle>
              <CardDescription>
                Espaço preparado para evolução patrimonial, economia mensal e reserva de emergência assim que o ResumoFinanceiroIA incluir séries históricas suficientes.
              </CardDescription>
            </CardHeader>
            <CardContent className="grid gap-4 md:grid-cols-3">
              <div className="rounded-2xl border border-dashed border-border/70 bg-background/70 p-5">
                <p className="text-sm font-medium">Evolução patrimonial</p>
                <p className="mt-2 text-sm text-muted-foreground">
                  Aguardando série histórica consolidada no resumo.
                </p>
              </div>
              <div className="rounded-2xl border border-dashed border-border/70 bg-background/70 p-5">
                <p className="text-sm font-medium">Economia mensal</p>
                <p className="mt-2 text-sm text-muted-foreground">
                  Área reservada para leitura visual sem criar cálculo novo no frontend.
                </p>
              </div>
              <div className="rounded-2xl border border-dashed border-border/70 bg-background/70 p-5">
                <p className="text-sm font-medium">Reserva de emergência</p>
                <p className="mt-2 text-sm text-muted-foreground">
                  Será exibida aqui quando o backend expuser histórico suficiente.
                </p>
              </div>
            </CardContent>
          </Card>
        </div>

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
