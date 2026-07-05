export type TipoAcaoSimulacao =
  | 0
  | 1
  | 2
  | 3
  | 4;

export interface AcaoSimulacaoFinanceiraInput {
  tipoAcao: TipoAcaoSimulacao;
  descricao: string;
  valor: number;
  dataInicial: string;
  dataFinal: string | null;
  quantidadeParcelas: number | null;
  observacao: string;
}

export interface LinhaResultadoSimulacaoFinanceira {
  mesReferencia: string;
  receitasReais: number;
  despesasReais: number;
  saldoReal: number;
  receitasSimuladas: number;
  despesasSimuladas: number;
  saldoSimulado: number;
  diferenca: number;
}

export interface ResultadoSimulacaoFinanceira {
  linhas: LinhaResultadoSimulacaoFinanceira[];
  totalReceitasReais: number;
  totalDespesasReais: number;
  saldoRealAcumulado: number;
  totalReceitasSimuladas: number;
  totalDespesasSimuladas: number;
  saldoSimuladoAcumulado: number;
  diferencaAcumulada: number;
}

export interface SimulacaoFinanceiraResumo {
  id: string;
  nome: string;
  descricao: string;
  dataInicial: string;
  quantidadeMeses: number;
  ativa: boolean;
  quantidadeAcoes: number;
  resultadoAtual?: ResultadoSimulacaoFinanceira | null;
}

export interface SimulacaoFinanceiraDetalhe {
  id: string;
  nome: string;
  descricao: string;
  dataInicial: string;
  quantidadeMeses: number;
  ativa: boolean;
  acoes: AcaoSimulacaoFinanceiraInput[];
  resultadoAtual?: ResultadoSimulacaoFinanceira | null;
}

export interface CadastrarSimulacaoFinanceiraPayload {
  usuarioId: string;
  nome: string;
  descricao: string;
  dataInicial: string;
  quantidadeMeses: number;
  acoes: AcaoSimulacaoFinanceiraInput[];
}

export interface EditarSimulacaoFinanceiraPayload {
  nome: string;
  descricao: string;
  dataInicial: string;
  quantidadeMeses: number;
  acoes: AcaoSimulacaoFinanceiraInput[];
}
