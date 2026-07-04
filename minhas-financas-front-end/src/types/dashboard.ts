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

export interface DashboardProximoVencimento {
  descricao: string;
  categoria: string;
  valor: string;
  dataVencimento: string;
  situacao: string;
}

export interface DashboardContaAtrasada {
  descricao: string;
  diasEmAtraso: number;
  valor: string;
}

export interface DashboardAlertaFinanceiro {
  codigo: string;
  titulo: string;
  descricao: string;
  severidade: string;
}

export interface DashboardFluxoCaixaTimelineItem {
  data: string;
  receita: string;
  despesa: string;
  saldo: string;
}

export interface DashboardFluxoCaixaProximos30Dias {
  receitasPrevistas: string;
  despesasPrevistas: string;
  saldoPrevisto: string;
  linhaDoTempo: DashboardFluxoCaixaTimelineItem[];
}

export interface DashboardRadarFinanceiro {
  proximosVencimentos: DashboardProximoVencimento[];
  contasAtrasadas: DashboardContaAtrasada[];
  alertasFinanceiros: DashboardAlertaFinanceiro[];
  fluxoCaixaProximos30Dias: DashboardFluxoCaixaProximos30Dias;
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
  radarFinanceiro: DashboardRadarFinanceiro;
}

export type DashboardPeriodo = "ano" | "mesAtual" | "mesPassado";
