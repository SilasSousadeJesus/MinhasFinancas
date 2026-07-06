import { useEffect, useMemo, useState } from "react";
import { PiechartcustomChart, LinechartChart } from "../Icons/Icons";
import { AlertTriangle, BellRing, CalendarClock, Clock3 } from "lucide-react";
import { Button } from "../ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "../ui/card";
import { Badge } from "../ui/badge";
import { useAuth } from "@/providers/auth-provider";
import { buscarDashboard } from "@/services/api/dashboard";
import { buscarSaudeFinanceira } from "@/services/api/saude-financeira";
import { ApiError } from "@/types/api";
import { DashboardData, DashboardIndicadorFinanceiro, DashboardPeriodo } from "@/types/dashboard";
import { SaudeFinanceiraData } from "@/types/saude-financeira";
import { NovoLancamentoModal } from "@/components/lancamentos/NovoLancamentoModal";
import { GerenciarContasCartoesModal } from "@/components/contas-cartoes/GerenciarContasCartoesModal";

const INDICATOR_FORMAT_CURRENCY = 0;
const INDICATOR_FORMAT_PERCENTAGE = 1;
const INDICATOR_FORMAT_MONTHS = 2;

const INDICATOR_STATUS_EXCELENTE = 0;
const INDICATOR_STATUS_BOM = 1;
const INDICATOR_STATUS_ATENCAO = 2;
const INDICATOR_STATUS_CRITICO = 3;

function parseCurrencyString(value: string) {
  const normalized = value
    .replace(/[^\d,-]/g, "")
    .replace(/\./g, "")
    .replace(",", ".");

  const parsed = Number(normalized);
  return Number.isNaN(parsed) ? 0 : parsed;
}

function formatMonthLabel(value: string) {
  const [year, month] = value.split("-");

  if (!year || !month) {
    return value;
  }

  return new Date(Number(year), Number(month) - 1, 1).toLocaleDateString("pt-BR", {
    month: "short",
    year: "2-digit",
  });
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("pt-BR").format(new Date(value));
}

function formatIndicatorValue(value: number, format: number) {
  if (format === INDICATOR_FORMAT_PERCENTAGE) {
    return `${value.toFixed(1)}%`;
  }

  if (format === INDICATOR_FORMAT_MONTHS) {
    return `${value.toFixed(1)} mês(es)`;
  }

  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value ?? 0);
}

function getIndicatorStatusLabel(status: number) {
  switch (status) {
    case INDICATOR_STATUS_EXCELENTE:
      return "Excelente";
    case INDICATOR_STATUS_BOM:
      return "Bom";
    case INDICATOR_STATUS_CRITICO:
      return "Crítico";
    default:
      return "Atenção";
  }
}

function getIndicatorStatusVariant(
  status: number
): "default" | "secondary" | "destructive" | "outline" {
  switch (status) {
    case INDICATOR_STATUS_EXCELENTE:
      return "secondary";
    case INDICATOR_STATUS_BOM:
      return "outline";
    case INDICATOR_STATUS_CRITICO:
      return "destructive";
    default:
      return "default";
  }
}

function getHealthClassificationVariant(
  classificacao: string
): "default" | "secondary" | "destructive" | "outline" {
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

function getAlertBadgeVariant(severidade: string): "default" | "secondary" | "destructive" | "outline" {
  switch (severidade) {
    case "alta":
      return "destructive";
    case "media":
      return "default";
    default:
      return "secondary";
  }
}

function getEmptyLineChartData() {
  const today = new Date();

  return Array.from({ length: 3 }, (_, index) => {
    const date = new Date(today.getFullYear(), today.getMonth() - (2 - index), 1);

    return {
      month: date.toLocaleDateString("pt-BR", {
        month: "short",
        year: "2-digit",
      }),
      receita: 0,
      despesa: 0,
    };
  });
}

export function PainelDashboard() {
  const { session } = useAuth();
  const [periodo, setPeriodo] = useState<DashboardPeriodo>("ano");
  const [dashboard, setDashboard] = useState<DashboardData | null>(null);
  const [saudeFinanceira, setSaudeFinanceira] = useState<SaudeFinanceiraData | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState("");
  const [reloadToken, setReloadToken] = useState(0);

  useEffect(() => {
    async function carregarDashboard() {
      if (!session?.usuario.id || !session.token) {
        return;
      }

      try {
        setIsLoading(true);
        setErrorMessage("");
        const [responseDashboard, responseSaudeFinanceira] = await Promise.all([
          buscarDashboard(session.usuario.id, session.token),
          buscarSaudeFinanceira(session.usuario.id, session.token),
        ]);

        setDashboard(responseDashboard.dados);
        setSaudeFinanceira(responseSaudeFinanceira.dados);
      } catch (error) {
        if (error instanceof ApiError) {
          setErrorMessage(error.message);
        } else {
          setErrorMessage("Não foi possível carregar o dashboard.");
        }
      } finally {
        setIsLoading(false);
      }
    }

    carregarDashboard();
  }, [reloadToken, session?.token, session?.usuario.id]);

  const resumo = useMemo(() => {
    if (!dashboard) {
      return {
        receita: "R$ 0,00",
        investimento: "R$ 0,00",
        despesa: "R$ 0,00",
        resultado: "0%",
      };
    }

    if (periodo === "mesAtual") {
      return {
        receita: dashboard.receita.receitaMesCorrente,
        investimento: dashboard.investimento.investimentoMesCorrente,
        despesa: dashboard.despesa.despesasMesCorrente,
        resultado: `${dashboard.resultado.resultadoMesCorrente}%`,
      };
    }

    if (periodo === "mesPassado") {
      return {
        receita: dashboard.receita.receitaMesPassado,
        investimento: dashboard.investimento.investimentoMesPassado,
        despesa: dashboard.despesa.despesasMesPassado,
        resultado: `${dashboard.resultado.resultadoMesPassado}%`,
      };
    }

    return {
      receita: dashboard.receita.receitaAnoCorrente,
      investimento: dashboard.investimento.investimentoAnoCorrente,
      despesa: dashboard.despesa.despesasAnoCorrente,
      resultado: `${dashboard.resultado.resultadoAnoCorrente}%`,
    };
  }, [dashboard, periodo]);

  const lineChartData = useMemo(() => {
    const data =
      dashboard?.receitasDespesasMensais.map((item) => ({
        month: formatMonthLabel(item.mesAno),
        receita: parseCurrencyString(item.receita),
        despesa: parseCurrencyString(item.despesa),
      })) ?? [];

    return data.length > 0 ? data : getEmptyLineChartData();
  }, [dashboard]);

  const pieChartData = useMemo(() => {
    const totalsByCategory = new Map<string, number>();

    dashboard?.lancamentosPorCategoriaDeDespesaDashboard.forEach((categoria) => {
      const totalCategoria = categoria.lancamentos.reduce(
        (sum, lancamento) => sum + lancamento.valor,
        0
      );

      if (totalCategoria <= 0) {
        return;
      }

      const nomeCategoria = categoria.nome?.trim() || "Sem categoria";
      totalsByCategory.set(
        nomeCategoria,
        (totalsByCategory.get(nomeCategoria) ?? 0) + totalCategoria
      );
    });

    const data = Array.from(totalsByCategory.entries())
      .map(([category, total]) => ({
        category,
        total,
      }))
      .sort((a, b) => b.total - a.total);

    if (data.length > 0) {
      return data;
    }

    return [
      {
        category: "Sem dados",
        total: 0,
      },
    ];
  }, [dashboard]);

  const indicadoresFinanceiros = useMemo(() => {
    return (
      dashboard?.indicadoresFinanceiros?.todos ?? []
    ).filter((indicador): indicador is DashboardIndicadorFinanceiro => Boolean(indicador?.nome));
  }, [dashboard]);

  return (
    <main className="flex-1 bg-gray-50 p-6 dark:bg-[#020817]">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Dashboard</h1>
          <p className="text-sm text-gray-500">
            Bem-vindo de volta, {session?.usuario.nome || "usuário"}!
          </p>
        </div>
        <div className="flex space-x-2">
          <GerenciarContasCartoesModal />
          <NovoLancamentoModal onCreated={() => setReloadToken((current) => current + 1)} />
        </div>
      </div>

      <div className="mt-6 flex items-center space-x-2">
        <Button
          variant={periodo === "ano" ? "default" : "outline"}
          onClick={() => setPeriodo("ano")}
        >
          Este Ano
        </Button>
        <Button
          variant={periodo === "mesAtual" ? "default" : "outline"}
          onClick={() => setPeriodo("mesAtual")}
        >
          Este mês
        </Button>
        <Button
          variant={periodo === "mesPassado" ? "default" : "outline"}
          onClick={() => setPeriodo("mesPassado")}
        >
          Mês passado
        </Button>
      </div>

      {errorMessage ? (
        <div className="mt-4 rounded-md border border-destructive/20 bg-destructive/5 px-4 py-3 text-sm text-destructive">
          {errorMessage}
        </div>
      ) : null}

      <div className="mt-6 grid grid-cols-1 justify-center gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardContent className="flex flex-col items-center">
            <div className="mt-4 flex h-16 w-16 items-center justify-center rounded-full bg-green-100">
              <span className="text-2xl font-bold text-green-500">0%</span>
            </div>
            <p className="mt-2 text-lg font-medium">Receitas</p>
            <p className="text-2xl font-bold">{isLoading ? "..." : resumo.receita}</p>
            <p className="text-sm text-gray-600">Orçado R$ 0,00</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="flex flex-col items-center">
            <div className="mt-4 flex h-16 w-16 items-center justify-center rounded-full bg-yellow-100">
              <span className="text-2xl font-bold text-yellow-500">0%</span>
            </div>
            <p className="mt-2 text-lg font-medium">Investimentos</p>
            <p className="text-2xl font-bold">{isLoading ? "..." : resumo.investimento}</p>
            <p className="text-sm text-gray-600">Orcado R$ 0,00</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="flex flex-col items-center">
            <div className="mt-4 flex h-16 w-16 items-center justify-center rounded-full bg-red-100">
              <span className="text-2xl font-bold text-red-500">0%</span>
            </div>
            <p className="mt-2 text-lg font-medium">Despesas</p>
            <p className="text-2xl font-bold">{isLoading ? "..." : resumo.despesa}</p>
            <p className="text-sm text-gray-600">Orcado R$ 0,00</p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="flex flex-col items-center">
            <div className="mt-4 flex h-16 w-16 items-center justify-center rounded-full bg-gray-100">
              <span className="text-2xl font-bold text-gray-500">0%</span>
            </div>
            <p className="mt-2 text-lg font-medium">Resultado</p>
            <p className="text-2xl font-bold">{isLoading ? "..." : resumo.resultado}</p>
            <p className="text-sm text-gray-600">Orcado R$ 0,00</p>
          </CardContent>
        </Card>
      </div>

      <section className="mt-6">
        <Card>
          <CardHeader>
            <CardTitle>Saúde Financeira</CardTitle>
            <CardDescription>
              Resumo rápido consumindo a mesma leitura usada na tela completa de Saúde Financeira.
            </CardDescription>
          </CardHeader>
          <CardContent className="grid gap-4 lg:grid-cols-[280px_1fr]">
            <div className="rounded-2xl border border-border/70 bg-background/70 p-5">
              <p className="text-sm text-muted-foreground">Pontuação geral</p>
              <p className="mt-2 text-4xl font-bold">{saudeFinanceira?.resumo.pontuacaoGeral ?? 0}</p>
              <div className="mt-3">
                <Badge variant={getHealthClassificationVariant(saudeFinanceira?.resumo.classificacao ?? "Atenção")}>
                  {saudeFinanceira?.resumo.classificacao ?? "Atenção"}
                </Badge>
              </div>
            </div>

            <div className="space-y-3">
              <p className="text-sm font-medium">Principais pontos de atenção</p>
              {saudeFinanceira?.resumo.pontosAtencao.length ? (
                saudeFinanceira.resumo.pontosAtencao.slice(0, 3).map((ponto) => (
                  <div key={ponto.nome} className="rounded-xl border border-border/70 bg-background/70 p-4">
                    <div className="flex items-center justify-between gap-3">
                      <p className="font-medium">{ponto.nome}</p>
                      <Badge variant={getIndicatorStatusVariant(ponto.status)}>
                        {getIndicatorStatusLabel(ponto.status)}
                      </Badge>
                    </div>
                    <p className="mt-2 text-sm text-muted-foreground">{ponto.descricao}</p>
                  </div>
                ))
              ) : (
                <div className="rounded-xl border border-dashed border-border/70 bg-background/70 p-4 text-sm text-muted-foreground">
                  Nenhum ponto de atenção relevante no momento.
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      </section>

      <section className="mt-6 space-y-4">
        <div>
          <h2 className="text-xl font-semibold">Indicadores Financeiros</h2>
          <p className="text-sm text-muted-foreground">
            Leitura analítica centralizada da saúde financeira com base em lançamentos, patrimônio e perfil financeiro.
          </p>
        </div>

        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-4">
          {indicadoresFinanceiros.map((indicador) => (
            <Card key={indicador.codigo}>
              <CardHeader className="space-y-3">
                <div className="flex items-start justify-between gap-3">
                  <CardTitle className="text-base">{indicador.nome}</CardTitle>
                  <Badge variant={getIndicatorStatusVariant(indicador.status)}>
                    {getIndicatorStatusLabel(indicador.status)}
                  </Badge>
                </div>
                <CardDescription>{indicador.descricao}</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                <div>
                  <p className="text-2xl font-bold">
                    {formatIndicatorValue(indicador.valorAtual, indicador.formato)}
                  </p>
                  <p className="text-sm text-muted-foreground">
                    Ideal: {formatIndicatorValue(indicador.valorIdeal, indicador.formato)}
                  </p>
                </div>
                <div className="rounded-xl border border-border/60 px-3 py-2 text-sm text-muted-foreground">
                  Progresso analítico: {indicador.percentual.toFixed(1)}%
                </div>
                {indicador.observacao ? (
                  <p className="text-sm text-muted-foreground">{indicador.observacao}</p>
                ) : null}
              </CardContent>
            </Card>
          ))}
        </div>
      </section>

      <div className="mt-6 grid grid-cols-1 gap-4 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Despesas por categoria</CardTitle>
            <CardDescription>Distribuição das despesas por categoria</CardDescription>
          </CardHeader>
          <CardContent>
            <PiechartcustomChart className="aspect-[4/3] w-full" data={pieChartData} />
            {dashboard?.lancamentosPorCategoriaDeDespesaDashboard?.length === 0 ? (
              <p className="mt-4 text-center text-sm text-muted-foreground">
                Sem despesas cadastradas ainda.
              </p>
            ) : null}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>Receitas e Despesas</CardTitle>
            <CardDescription>Evolução mensal de receitas e despesas</CardDescription>
          </CardHeader>
          <CardContent>
            <LinechartChart className="aspect-[4/3] w-full" data={lineChartData} />
            {dashboard?.receitasDespesasMensais?.length === 0 ? (
              <p className="mt-4 text-center text-sm text-muted-foreground">
                Sem movimentações mensais ainda.
              </p>
            ) : null}
          </CardContent>
        </Card>
      </div>

      <section className="mt-6 space-y-4">
        <div>
          <h2 className="text-xl font-semibold">Radar Financeiro</h2>
          <p className="text-sm text-muted-foreground">
            Itens que exigem atenção imediata e ajudam a antecipar o caixa dos próximos dias.
          </p>
        </div>

        <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
          <Card>
            <CardHeader>
              <div className="flex items-center gap-2">
                <CalendarClock className="h-5 w-5 text-amber-500" />
                <CardTitle>Próximos vencimentos</CardTitle>
              </div>
              <CardDescription>Lançamentos previstos para os próximos 7 dias.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              {dashboard?.radarFinanceiro.proximosVencimentos.length ? (
                dashboard.radarFinanceiro.proximosVencimentos.map((item, index) => (
                  <div
                    key={`${item.descricao}-${item.dataVencimento}-${index}`}
                    className="rounded-xl border border-border/60 p-4"
                  >
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <p className="font-medium">{item.descricao}</p>
                        <p className="text-sm text-muted-foreground">{item.categoria}</p>
                      </div>
                      <p className="font-semibold">{item.valor}</p>
                    </div>
                    <div className="mt-3 flex flex-wrap items-center gap-2 text-sm text-muted-foreground">
                      <span>Vencimento {formatDate(item.dataVencimento)}</span>
                      <Badge variant="outline">{item.situacao}</Badge>
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-sm text-muted-foreground">
                  Nenhum vencimento previsto para os próximos dias.
                </p>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <div className="flex items-center gap-2">
                <Clock3 className="h-5 w-5 text-rose-500" />
                <CardTitle>Contas atrasadas</CardTitle>
              </div>
              <CardDescription>Lançamentos vencidos e ainda não pagos.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              {dashboard?.radarFinanceiro.contasAtrasadas.length ? (
                dashboard.radarFinanceiro.contasAtrasadas.map((item, index) => (
                  <div
                    key={`${item.descricao}-${item.diasEmAtraso}-${index}`}
                    className="rounded-xl border border-border/60 p-4"
                  >
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <p className="font-medium">{item.descricao}</p>
                        <p className="text-sm text-muted-foreground">
                          {item.diasEmAtraso} dia(s) em atraso
                        </p>
                      </div>
                      <p className="font-semibold">{item.valor}</p>
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-sm text-muted-foreground">
                  Nenhuma conta atrasada encontrada.
                </p>
              )}
            </CardContent>
          </Card>
        </div>

        <div className="grid grid-cols-1 gap-4 xl:grid-cols-[1.1fr_0.9fr]">
          <Card>
            <CardHeader>
              <div className="flex items-center gap-2">
                <BellRing className="h-5 w-5 text-sky-500" />
                <CardTitle>Alertas financeiros</CardTitle>
              </div>
              <CardDescription>
                Alertas objetivos prontos para expansão futura sem alterar a tela.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              {dashboard?.radarFinanceiro.alertasFinanceiros.length ? (
                dashboard.radarFinanceiro.alertasFinanceiros.map((alerta) => (
                  <div
                    key={alerta.codigo}
                    className="rounded-xl border border-border/60 p-4"
                  >
                    <div className="flex items-start justify-between gap-3">
                      <div>
                        <p className="font-medium">{alerta.titulo}</p>
                        <p className="mt-1 text-sm text-muted-foreground">
                          {alerta.descricao}
                        </p>
                      </div>
                      <Badge variant={getAlertBadgeVariant(alerta.severidade)}>
                        {alerta.severidade}
                      </Badge>
                    </div>
                  </div>
                ))
              ) : (
                <div className="rounded-xl border border-dashed border-emerald-500/30 bg-emerald-500/5 p-4">
                  <p className="text-sm text-emerald-700 dark:text-emerald-300">
                    Nenhum alerta financeiro importante foi identificado agora.
                  </p>
                </div>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <div className="flex items-center gap-2">
                <AlertTriangle className="h-5 w-5 text-violet-500" />
                <CardTitle>Fluxo de caixa dos próximos 30 dias</CardTitle>
              </div>
              <CardDescription>
                Resumo das receitas e despesas previstas ainda não realizadas.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
                <div className="rounded-xl border border-border/60 p-4">
                  <p className="text-sm text-muted-foreground">Receitas previstas</p>
                  <p className="mt-2 text-lg font-semibold">
                    {dashboard?.radarFinanceiro.fluxoCaixaProximos30Dias.receitasPrevistas ?? "R$ 0,00"}
                  </p>
                </div>
                <div className="rounded-xl border border-border/60 p-4">
                  <p className="text-sm text-muted-foreground">Despesas previstas</p>
                  <p className="mt-2 text-lg font-semibold">
                    {dashboard?.radarFinanceiro.fluxoCaixaProximos30Dias.despesasPrevistas ?? "R$ 0,00"}
                  </p>
                </div>
                <div className="rounded-xl border border-border/60 p-4">
                  <p className="text-sm text-muted-foreground">Saldo previsto</p>
                  <p className="mt-2 text-lg font-semibold">
                    {dashboard?.radarFinanceiro.fluxoCaixaProximos30Dias.saldoPrevisto ?? "R$ 0,00"}
                  </p>
                </div>
              </div>

              <div className="space-y-3">
                <p className="text-sm font-medium">Linha do tempo</p>
                {dashboard?.radarFinanceiro.fluxoCaixaProximos30Dias.linhaDoTempo.length ? (
                  dashboard.radarFinanceiro.fluxoCaixaProximos30Dias.linhaDoTempo.map((item) => (
                    <div
                      key={item.data}
                      className="flex flex-col gap-2 rounded-xl border border-border/60 p-4 sm:flex-row sm:items-center sm:justify-between"
                    >
                      <div>
                        <p className="font-medium">{formatDate(item.data)}</p>
                        <p className="text-sm text-muted-foreground">
                          Receita {item.receita} | Despesa {item.despesa}
                        </p>
                      </div>
                      <p className="font-semibold">{item.saldo}</p>
                    </div>
                  ))
                ) : (
                  <p className="text-sm text-muted-foreground">
                    Nenhuma movimentação prevista nos próximos 30 dias.
                  </p>
                )}
              </div>
            </CardContent>
          </Card>
        </div>
      </section>
    </main>
  );
}
