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

export interface AnaliseFinanceiraHistoricaLista {
  id: string;
  dataGeracao: string;
  periodoReferencia: string;
  pontuacaoSaudeFinanceira: number;
  classificacaoSaudeFinanceira: string;
  resumoExecutivoSistema: string;
  perguntaUsuario: string;
  provedorIA: string;
  modeloIA: string;
  sucesso: boolean;
  tempoTotalMs: number;
  custoEstimadoUsd: number;
}

export interface AnaliseFinanceiraHistoricaDetalhe {
  id: string;
  usuarioId: string;
  dataGeracao: string;
  periodoReferencia: string;
  pontuacaoSaudeFinanceira: number;
  classificacaoSaudeFinanceira: string;
  resumoExecutivoSistema: string;
  contextoResumoFinanceiroIAJson: string;
  indicadoresResumidosJson: string;
  insightsResumidosJson: string;
  perfilFinanceiroVigenteJson: string;
  principaisRiscosJson: string;
  principaisPontosPositivosJson: string;
  principaisRecomendacoesJson: string;
  prioridadesJson: string;
  perguntaUsuario: string;
  respostaIA: string;
  provedorIA: string;
  modeloIA: string;
  versaoPrompt: string;
  versaoSistema: string;
  tokensEntrada: number;
  tokensSaida: number;
  tokensTotais: number;
  custoEstimadoUsd: number;
  tempoTotalMs: number;
  sucesso: boolean;
  mensagemErro: string;
  ativa: boolean;
}

export interface AnaliseAssistenteExibida {
  id?: string | null;
  perguntaUsuario: string;
  conteudo: string;
  sugestaoCompromisso?: string | null;
  modelo: string;
  provedor: string;
  tempoTotalMs: number;
  custoEstimadoUsd: number;
  tokensTotaisUtilizados: number;
  dataGeracao: string;
  foiSimulada?: boolean;
  observacaoInfraestrutura?: string;
  origem: "nova" | "historico";
}

export interface ResultadoPaginadoAnaliseFinanceiraHistorica {
  itens: AnaliseFinanceiraHistoricaLista[];
  paginaAtual: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
}
