"use client";

import { useEffect, useMemo, useState } from "react";

import { Sidebar } from "@/components/Sidebar/Sidebar";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAuth } from "@/providers/auth-provider";
import { buscarResumoFinanceiroIA } from "@/services/api/resumo-financeiro-ia";
import { buscarSaudeFinanceira } from "@/services/api/saude-financeira";
import { ApiError } from "@/types/api";
import { ResumoFinanceiroIAData } from "@/types/resumo-financeiro-ia";
import { IndicadorFinanceiroSaude, SaudeFinanceiraData } from "@/types/saude-financeira";

const FORMATO_MOEDA = 0;
const FORMATO_PERCENTUAL = 1;
const FORMATO_MESES = 2;

const STATUS_EXCELENTE = 0;
const STATUS_BOM = 1;
const STATUS_ATENCAO = 2;
const STATUS_CRITICO = 3;

const TIPO_ALERTA = 0;
const TIPO_OPORTUNIDADE = 1;
const TIPO_DESTAQUE_POSITIVO = 2;
const TIPO_CONFIGURACAO = 3;

const PRIORIDADE_ALTA = 0;
const PRIORIDADE_MEDIA = 1;

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
    case "CrÃ­tica":
      return "destructive";
    default:
      return "default";
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
      return "Configuracao";
    default:
      return "Informacao";
  }
}

function obterTextoPrioridade(prioridade: number) {
  switch (prioridade) {
    case PRIORIDADE_ALTA:
      return "Critico";
    case PRIORIDADE_MEDIA:
      return "Atencao";
    default:
      return "Informacao";
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

export function SaudeFinanceiraManager() {
  const { session } = useAuth();
  const [saudeFinanceira, setSaudeFinanceira] = useState<SaudeFinanceiraData | null>(null);
  const [resumoFinanceiroIA, setResumoFinanceiroIA] = useState<ResumoFinanceiroIAData | null>(null);
  const [mensagemErro, setMensagemErro] = useState("");

  useEffect(() => {
    async function carregarSaudeFinanceira() {
      if (!session?.usuario.id || !session.token) {
        return;
      }

      try {
        setMensagemErro("");
        const [responseSaude, responseResumo] = await Promise.all([
          buscarSaudeFinanceira(session.usuario.id, session.token),
          buscarResumoFinanceiroIA(session.usuario.id, session.token),
        ]);

        setSaudeFinanceira(responseSaude.dados);
        setResumoFinanceiroIA(responseResumo.dados);
      } catch (error) {
        if (error instanceof ApiError) {
          setMensagemErro(error.message);
        } else {
          setMensagemErro("Nao foi possivel carregar a saude financeira.");
        }
      }
    }

    carregarSaudeFinanceira();
  }, [session?.token, session?.usuario.id]);

  const indicadores = useMemo(() => {
    return (
      saudeFinanceira?.indicadores.todos ?? []
    ).filter((indicador): indicador is IndicadorFinanceiroSaude => Boolean(indicador?.nome));
  }, [saudeFinanceira]);

  const insights = useMemo(() => {
    return resumoFinanceiroIA?.insights.todos ?? [];
  }, [resumoFinanceiroIA]);

  return (
    <div className="flex flex-row">
      <Sidebar />
      <main className="flex-1 bg-gray-50 p-6 dark:bg-[#020817]">
        <div className="space-y-2">
          <h1 className="text-2xl font-bold">SaÃºde Financeira</h1>
          <p className="text-sm text-muted-foreground">
            Leitura analÃ­tica detalhada dos indicadores, insights e sinais de evoluÃ§Ã£o financeira.
          </p>
        </div>

        {mensagemErro ? (
          <div className="mt-4 rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
            {mensagemErro}
          </div>
        ) : null}

        <div className="mt-6">
          <Card>
            <CardHeader>
              <CardTitle>Resumo geral</CardTitle>
              <CardDescription>Pontuação consolidada, classificação atual e principais pontos de atenção da sua saúde financeira.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="grid gap-4 md:grid-cols-2">
                <div className="rounded-2xl border border-border/70 bg-background/70 p-6">
                  <p className="text-sm text-muted-foreground">Pontuação geral</p>
                  <p className="mt-2 text-5xl font-bold tracking-tight">
                    {saudeFinanceira?.resumo.pontuacaoGeral ?? 0}
                  </p>
                  <p className="mt-2 text-sm text-muted-foreground">Escala de 0 a 100.</p>
                </div>
                <div className="rounded-2xl border border-border/70 bg-background/70 p-6">
                  <p className="text-sm text-muted-foreground">Classificação</p>
                  <div className="mt-3">
                    <Badge variant={obterVariantClassificacao(saudeFinanceira?.resumo.classificacao ?? "Atenção")}>
                      {saudeFinanceira?.resumo.classificacao ?? "Atenção"}
                    </Badge>
                  </div>
                  <p className="mt-4 text-sm text-muted-foreground">
                    A classificação resume o equilíbrio atual entre renda, dívidas, patrimônio e reserva.
                  </p>
                </div>
              </div>

              <div className="rounded-2xl border border-border/70 bg-background/70 p-4">
                <div className="flex items-center justify-between gap-3">
                  <div>
                    <p className="text-sm font-medium">Pontos de atenção</p>
                    <p className="text-xs text-muted-foreground">Os principais itens que mais pesam na leitura atual.</p>
                  </div>
                  <Badge variant="outline">{saudeFinanceira?.resumo.pontosAtencao.length ?? 0}</Badge>
                </div>

                <div className="mt-4 space-y-3">
                  {saudeFinanceira?.resumo.pontosAtencao.length ? (
                    saudeFinanceira.resumo.pontosAtencao.map((ponto) => (
                      <div key={ponto.nome} className="rounded-xl border border-border/70 bg-background/70 p-4">
                        <div className="flex items-center justify-between gap-3">
                          <p className="font-medium">{ponto.nome}</p>
                          <Badge variant={obterVariantBadge(ponto.status)}>{obterTextoStatus(ponto.status)}</Badge>
                        </div>
                        <p className="mt-2 text-sm text-muted-foreground">{ponto.descricao}</p>
                        <p className="mt-2 text-xs text-muted-foreground">{ponto.observacao}</p>
                      </div>
                    ))
                  ) : (
                    <div className="rounded-xl border border-dashed border-border/70 bg-background/70 p-4 text-sm text-muted-foreground">
                      Nenhum ponto de atenção relevante no momento.
                    </div>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        <section className="mt-8">
          <div className="mb-4 space-y-1">
            <h2 className="text-xl font-semibold">Indicadores</h2>
            <p className="text-sm text-muted-foreground">
              Cada card mostra o valor atual, a rÃ©gua ideal quando existir, o status e uma observaÃ§Ã£o curta.
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

        <section className="mt-8">
          <div className="mb-4 space-y-1">
            <h2 className="text-xl font-semibold">Insights financeiros</h2>
            <p className="text-sm text-muted-foreground">
              Leituras priorizadas pelo sistema para explicar oportunidades, riscos e ajustes de configuraÃ§Ã£o.
            </p>
          </div>

          <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
            {insights.length ? (
              insights.map((insight) => (
                <Card key={`${insight.titulo}-${insight.prioridade}-${insight.tipo}`}>
                  <CardHeader className="space-y-3">
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <CardTitle className="text-base">{insight.titulo}</CardTitle>
                        <CardDescription className="mt-1">{obterTextoTipoInsight(insight.tipo)}</CardDescription>
                      </div>
                      <Badge variant={obterVariantPrioridade(insight.prioridade)}>
                        {obterTextoPrioridade(insight.prioridade)}
                      </Badge>
                    </div>
                  </CardHeader>
                  <CardContent className="space-y-3">
                    <p className="text-sm text-muted-foreground">{insight.descricao}</p>
                    {insight.acaoSugerida ? (
                      <div className="rounded-xl border border-border/60 bg-background/70 p-3 text-sm">
                        <span className="font-medium">AÃ§Ã£o sugerida:</span> {insight.acaoSugerida}
                      </div>
                    ) : null}
                  </CardContent>
                </Card>
              ))
            ) : (
              <Card className="xl:col-span-2">
                <CardContent className="p-6 text-sm text-muted-foreground">
                  O backend ainda nÃ£o retornou insights financeiros para o contexto atual.
                </CardContent>
              </Card>
            )}
          </div>
        </section>

        <section className="mt-8">
          <Card>
            <CardHeader>
              <CardTitle>GrÃ¡ficos</CardTitle>
              <CardDescription>
                EspaÃ§o centralizado da saÃºde financeira para evoluÃ§Ã£o patrimonial, economia mensal e reserva de emergÃªncia.
              </CardDescription>
            </CardHeader>
            <CardContent className="grid gap-4 md:grid-cols-3">
              <div className="rounded-2xl border border-dashed border-border/70 bg-background/70 p-5">
                <p className="text-sm font-medium">EvoluÃ§Ã£o patrimonial</p>
                <p className="mt-2 text-sm text-muted-foreground">
                  Aguardando sÃ©rie histÃ³rica consolidada para exibiÃ§Ã£o grÃ¡fica.
                </p>
              </div>
              <div className="rounded-2xl border border-dashed border-border/70 bg-background/70 p-5">
                <p className="text-sm font-medium">Economia mensal</p>
                <p className="mt-2 text-sm text-muted-foreground">
                  Ãrea reservada para leitura visual sem criar cÃ¡lculo novo no frontend.
                </p>
              </div>
              <div className="rounded-2xl border border-dashed border-border/70 bg-background/70 p-5">
                <p className="text-sm font-medium">Reserva de emergÃªncia</p>
                <p className="mt-2 text-sm text-muted-foreground">
                  SerÃ¡ exibida aqui quando o backend expuser histÃ³rico suficiente.
                </p>
              </div>
            </CardContent>
          </Card>
        </section>
      </main>
    </div>
  );
}
