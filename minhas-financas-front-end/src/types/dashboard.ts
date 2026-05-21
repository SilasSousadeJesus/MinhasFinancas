export interface DashboardReceita {
  receitaAnoCorrente: string;
  receitaMesCorrente: string;
  receitaMesPassado: string;
}

export interface DashboardDespesa {
  despesasAnoCorrente: string;
  despesasMesCorrente: string;
  despesasMesPassado: string;
}

export interface DashboardInvestimento {
  investimentoAnoCorrente: string;
  investimentoMesCorrente: string;
  investimentoMesPassado: string;
}

export interface DashboardResultado {
  resultadoAnoCorrente: string;
  resultadoMesCorrente: string;
  resultadoMesPassado: string;
}

export interface DashboardReceitaDespesaMensal {
  mesAno: string;
  receita: string;
  despesa: string;
}

export interface DashboardLancamentoCategoriaItem {
  valor: number;
}

export interface DashboardCategoriaDespesa {
  id: string;
  nome: string;
  icone: string;
  lancamentos: DashboardLancamentoCategoriaItem[];
}

export interface DashboardData {
  receita: DashboardReceita;
  despesa: DashboardDespesa;
  investimento: DashboardInvestimento;
  resultado: DashboardResultado;
  receitasDespesasMensais: DashboardReceitaDespesaMensal[];
  acumuloInvestimentoMensal: unknown[];
  lancamentosPorCategoriaDeDespesaDashboard: DashboardCategoriaDespesa[];
  contasApagarDashboard: unknown;
}

export type DashboardPeriodo = "ano" | "mesAtual" | "mesPassado";
