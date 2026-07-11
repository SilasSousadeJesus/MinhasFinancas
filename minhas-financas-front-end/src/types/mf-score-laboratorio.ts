export interface UsuarioMfScoreLaboratorio {
  usuarioId: string;
  nome: string;
  email: string;
  dataCadastro?: string | null;
  ehUsuarioSintetico: boolean;
  origemUsuario: string;
  codigoCenario: string;
  versaoBase: string;
  dataGeracaoBase?: string | null;
  descricaoCenario: string;
  objetivoCenario: string;
}

export interface ResultadoGeracaoBaseSimulacaoMfScore {
  versaoBase: string;
  quantidadeCenarios: number;
  quantidadeUsuariosGerados: number;
  dataGeracao: string;
  usuariosGerados: UsuarioMfScoreLaboratorio[];
}

export interface ResultadoLimpezaBaseSimulacaoMfScore {
  quantidadeUsuariosRemovidos: number;
  codigosCenariosRemovidos: string[];
}

export interface TendenciaMfScoreLaboratorio {
  direcao: string;
  descricao: string;
  historicoNotas: number[];
}

export interface PilarMfScoreLaboratorio {
  codigo: string;
  nome: string;
  peso: number;
  nota: number;
  descricao: string;
  indicadores: string[];
}

export interface IndicadorMfScoreLaboratorio {
  codigo: string;
  nome: string;
  valorAtual: number;
  valorIdeal: number;
  percentual: number;
  valorObrigacoesPrevistas?: number | null;
  valorReceitaPrevista?: number | null;
  percentualComprometimento?: number | null;
  status: string;
  descricao: string;
  observacao: string;
  formato: string;
}

export interface IndicadorCriticoMfScoreLaboratorio {
  codigo: string;
  nome: string;
  motivo: string;
  penalidade: number;
  pilarRelacionado: string;
}

export interface PenalizacaoMfScoreLaboratorio {
  nome: string;
  motivo: string;
  penalidade: number;
  pilarRelacionado: string;
}

export interface DadosEntradaMfScoreLaboratorio {
  dataReferencia: string;
  quantidadeLancamentos: number;
  quantidadeReceitas: number;
  quantidadeDespesas: number;
  receitaMensalConsiderada: number;
  despesaMensalConsiderada: number;
  quantidadeAtivos: number;
  quantidadePassivos: number;
  valorAtivosConsiderados: number;
  valorPassivosConsiderados: number;
  quantidadeMetas: number;
  possuiPerfilFinanceiroConfigurado: boolean;
  possuiPlanoEstrategicoVigente: boolean;
  quantidadeObjetivosPlanoAtivos: number;
  quantidadeObjetivosPlanoAltaPrioridade: number;
  quantidadeObjetivosPlanoConcluidos: number;
  possuiCompromissosFinanceiros: boolean;
  quantidadeCompromissosEmAndamento: number;
  quantidadeCompromissosConcluidos: number;
  quantidadeCompromissosCancelados: number;
  possuiFluxoMensalNegativoAtual: boolean;
  mesesConsecutivosFluxoNegativo: number;
  possuiInadimplencia: boolean;
  nivelInadimplencia: number;
  diasMaximosAtraso: number;
  valorTotalEmAtraso: number;
  percentualValorEmAtrasoSobreRenda: number;
  possuiCuraRecenteInadimplencia: boolean;
  quantidadeOcorrenciasAtrasoRecente: number;
  quantidadeMesesComOcorrenciaAtrasoRecente: number;
  possuiDadosEssenciaisInsuficientes: boolean;
  quantidadeParametrosPlanejamentoConfigurados: number;
  totalParametrosPlanejamentoEsperados: number;
  perfilFinanceiroBasicoCompleto: boolean;
  notaConfiguracaoPlanejamento: number;
  notaPlanoEstrategico?: number | null;
  notaCompromissosFinanceiros?: number | null;
}

export interface MfScoreLaboratorioDetalhe {
  usuario: UsuarioMfScoreLaboratorio;
  versaoModelo: string;
  mfScoreBase: number;
  mfScoreFinal: number;
  classificacao: string;
  risco: string;
  penalidadeTotal: number;
  descricao: string;
  tendencia: TendenciaMfScoreLaboratorio;
  resumoExecutivoDosPilares: string[];
  pilares: PilarMfScoreLaboratorio[];
  indicadores: IndicadorMfScoreLaboratorio[];
  indicadoresCriticos: IndicadorCriticoMfScoreLaboratorio[];
  penalizacoes: PenalizacaoMfScoreLaboratorio[];
  regrasCriticasAplicadas: string[];
  dadosEntrada: DadosEntradaMfScoreLaboratorio;
  observacoesLimitacoes: string[];
}
