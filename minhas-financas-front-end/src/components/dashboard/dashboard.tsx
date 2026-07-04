import { useEffect, useMemo, useState } from "react";
import { PiechartcustomChart, LinechartChart } from "../Icons/Icons";
import { Button } from "../ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "../ui/card";
import { useAuth } from "@/providers/auth-provider";
import { buscarDashboard } from "@/services/api/dashboard";
import { ApiError } from "@/types/api";
import { DashboardData, DashboardPeriodo } from "@/types/dashboard";
import { NovoLancamentoModal } from "@/components/lancamentos/NovoLancamentoModal";
import { GerenciarContasCartoesModal } from "@/components/contas-cartoes/GerenciarContasCartoesModal";

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
        const response = await buscarDashboard(session.usuario.id, session.token);
        setDashboard(response.dados);
      } catch (error) {
        if (error instanceof ApiError) {
          setErrorMessage(error.message);
        } else {
          setErrorMessage("Nao foi possivel carregar o dashboard.");
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
    const data =
      dashboard?.lancamentosPorCategoriaDeDespesaDashboard
        .map((categoria) => ({
          category: categoria.nome,
          total: categoria.lancamentos.reduce((sum, lancamento) => sum + lancamento.valor, 0),
        }))
        .filter((categoria) => categoria.total > 0) ?? [];

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

  return (
    <main className="flex-1 bg-gray-50 p-6 dark:bg-[#020817]">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Dashboard</h1>
          <p className="text-sm text-gray-500">
            Bem vindo de volta, {session?.usuario.nome || "usuario"}!
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
          Este Mes
        </Button>
        <Button
          variant={periodo === "mesPassado" ? "default" : "outline"}
          onClick={() => setPeriodo("mesPassado")}
        >
          Mes Passado
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
            <p className="text-sm text-gray-600">Orcado R$ 0,00</p>
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

      <div className="mt-6 grid grid-cols-1 gap-4 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Despesas por categoria</CardTitle>
            <CardDescription>Distribuicao das despesas por categoria</CardDescription>
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
            <CardDescription>Evolucao mensal de receitas e despesas</CardDescription>
          </CardHeader>
          <CardContent>
            <LinechartChart className="aspect-[4/3] w-full" data={lineChartData} />
            {dashboard?.receitasDespesasMensais?.length === 0 ? (
              <p className="mt-4 text-center text-sm text-muted-foreground">
                Sem movimentacoes mensais ainda.
              </p>
            ) : null}
          </CardContent>
        </Card>
      </div>
    </main>
  );
}
