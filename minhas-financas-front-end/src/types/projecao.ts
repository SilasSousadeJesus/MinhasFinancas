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
  atreladaADespesas: boolean;
  rendasExtrasMensais: RendaExtraMensalProjecaoInput[];
  dividasManuaisMensais: DividaManualMensalProjecaoInput[];
}

export interface CriarProjecaoPayload {
  nome: string;
  usuarioId: string;
  dataInicial?: string | null;
  valorAcumuladoInicial: number;
  valorObjetivo: number;
  mesesLimite?: number;
  atreladaADespesas: boolean;
  rendas: RendaProjecaoInput[];
  rendasExtrasMensais: RendaExtraMensalProjecaoInput[];
  dividasManuaisMensais: DividaManualMensalProjecaoInput[];
}

export interface EditarProjecaoPayload {
  nome: string;
  dataInicial?: string | null;
  valorAcumuladoInicial: number;
  valorObjetivo: number;
  mesesLimite?: number;
  atreladaADespesas: boolean;
  rendas: RendaProjecaoInput[];
  rendasExtrasMensais: RendaExtraMensalProjecaoInput[];
  dividasManuaisMensais: DividaManualMensalProjecaoInput[];
}

export interface RendaExtraMensalProjecaoInput {
  mesReferencia: string;
  valor: number;
}

export interface DividaManualMensalProjecaoInput {
  mesReferencia: string;
  valor: number;
}

export interface LinhaResultadoProjecao {
  mesReferencia: string;
  dividasTotais: number;
  dividasEditaveis: boolean;
  rendaExtraMensal: number;
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
  percentualConcluido: number;
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
  atreladaADespesas: boolean;
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
  atreladaADespesas: boolean;
  rendas: RendaProjecaoInput[];
  rendasExtrasMensais: RendaExtraMensalProjecaoInput[];
  dividasManuaisMensais: DividaManualMensalProjecaoInput[];
  resultadoAtual?: ResultadoProjecao | null;
}
