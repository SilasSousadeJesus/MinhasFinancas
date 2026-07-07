export interface GerarAnaliseAssistenteFinanceiroPayload {
  perguntaUsuario?: string;
}

export interface RespostaAssistenteFinanceiroIA {
  sucesso: boolean;
  provedor: string;
  modelo: string;
  analiseFinanceiraHistoricaId?: string | null;
  conteudo: string;
  foiSimulada: boolean;
  observacaoInfraestrutura: string;
  mensagemTecnica: string;
  mensagemAmigavel: string;
  origemErro: string;
  categoriaErro: number;
  statusHttpProvedor?: number | null;
  tentativasRealizadas: number;
  caracteresEntrada: number;
  tokensEntradaEstimados: number;
  tokensEntradaUtilizados: number;
  tokensSaidaUtilizados: number;
  tokensRaciocinioUtilizados: number;
  tokensTotaisUtilizados: number;
  tokensReaisDisponiveis: boolean;
  entradaFoiTruncada: boolean;
  tempoTotalMs: number;
  custoEstimadoUsd: number;
  precoEntradaPorMilhaoTokensUsd: number;
  precoSaidaPorMilhaoTokensUsd: number;
}
