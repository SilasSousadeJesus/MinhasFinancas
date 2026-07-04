export interface RendaProjecaoInput {
  nome: string;
  valorMensal: number;
}

export interface CalcularProjecaoPayload {
  rendas: RendaProjecaoInput[];
  valorAcumuladoInicial: number;
  valorObjetivo: number;
  dataInicial?: string | null;
  mesesLimite?: number;
}

export interface CriarProjecaoPayload {
  nome: string;
  usuarioId: string;
  dataInicial?: string | null;
  valorAcumuladoInicial: number;
  valorObjetivo: number;
  mesesLimite?: number;
  rendas: RendaProjecaoInput[];
}

export interface EditarProjecaoPayload {
  nome: string;
  dataInicial?: string | null;
  valorAcumuladoInicial: number;
  valorObjetivo: number;
  mesesLimite?: number;
  rendas: RendaProjecaoInput[];
}

export interface LinhaResultadoProjecao {
  mesReferencia: string;
  dividasTotais: number;
  receitasDosLancamentos: number;
  rendaManualTotal: number;
  receitaTotalMes: number;
  sobraDoMes: number;
  acumuladoProjetado: number;
  objetivoAtingidoNoMes: boolean;
}

export interface ResultadoProjecao {
  rendaManualTotal: number;
  valorAcumuladoInicial: number;
  valorObjetivo: number;
  valorRestanteParaObjetivo: number;
  mesObjetivo?: string | null;
  quantidadeMesesParaObjetivo?: number | null;
  objetivoAlcancado: boolean;
  linhas: LinhaResultadoProjecao[];
}

export interface ProjecaoResumo {
  id: string;
  nome: string;
  dataInicial: string;
  valorAcumuladoInicial: number;
  valorObjetivo: number;
  mesesLimite: number;
  quantidadeRendas: number;
  rendaManualTotal: number;
  resultadoAtual?: ResultadoProjecao | null;
}

export interface ProjecaoDetalhe {
  id: string;
  nome: string;
  dataInicial: string;
  valorAcumuladoInicial: number;
  valorObjetivo: number;
  mesesLimite: number;
  rendas: RendaProjecaoInput[];
  resultadoAtual?: ResultadoProjecao | null;
}
