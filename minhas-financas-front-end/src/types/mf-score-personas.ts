export enum StatusPersonaMfScore {
  Rascunho = 0,
  EmAuditoria = 1,
  Auditada = 2,
  CasoCanonico = 3,
  Inativa = 4,
}

export interface MfScorePersonaItem {
  id: string;
  nome: string;
  descricao: string;
  objetivoDaPersona: string;
  rendaMensal: number;
  receitasPrevistas30Dias: number;
  receitasPrevistas90Dias: number;
  receitasPrevistas180Dias: number;
  receitasPrevistas12Meses: number;
  despesasMensais: number;
  obrigacoes30Dias: number;
  obrigacoes90Dias: number;
  obrigacoes180Dias: number;
  obrigacoes12Meses: number;
  reservaEmergencia: number;
  patrimonioBruto: number;
  passivos: number;
  patrimonioLiquido: number;
  possuiPerfilFinanceiroConfigurado: boolean;
  possuiPlanoEstrategico: boolean;
  possuiMetas: boolean;
  possuiCompromissos: boolean;
  compromissosCumpridos: number;
  possuiInadimplencia: boolean;
  scoreHumanoSugerido?: number | null;
  faixaEsperadaMin?: number | null;
  faixaEsperadaMax?: number | null;
  justificativaNotaHumana?: string | null;
  status: StatusPersonaMfScore;
  ehCasoCanonico: boolean;
  observacoes?: string | null;
  dataCriacao: string;
  dataAtualizacao: string;
}

export interface SalvarMfScorePersonaPayload {
  nome: string;
  descricao: string;
  objetivoDaPersona: string;
  rendaMensal: number;
  receitasPrevistas30Dias: number;
  receitasPrevistas90Dias: number;
  receitasPrevistas180Dias: number;
  receitasPrevistas12Meses: number;
  despesasMensais: number;
  obrigacoes30Dias: number;
  obrigacoes90Dias: number;
  obrigacoes180Dias: number;
  obrigacoes12Meses: number;
  reservaEmergencia: number;
  patrimonioBruto: number;
  passivos: number;
  patrimonioLiquido: number;
  possuiPerfilFinanceiroConfigurado: boolean;
  possuiPlanoEstrategico: boolean;
  possuiMetas: boolean;
  possuiCompromissos: boolean;
  compromissosCumpridos: number;
  possuiInadimplencia: boolean;
  scoreHumanoSugerido?: number | null;
  faixaEsperadaMin?: number | null;
  faixaEsperadaMax?: number | null;
  justificativaNotaHumana?: string | null;
  observacoes?: string | null;
}

export interface ResultadoPilarMfScorePersona {
  pilar: string;
  nota: number;
  peso: number;
  descricao: string;
}

export interface ResultadoIndicadorCriticoMfScorePersona {
  indicador: string;
  pilarRelacionado: string;
  penalidade: number;
  motivo: string;
}

export interface ResultadoRodarMfScorePersona {
  personaId: string;
  persona: string;
  descricao: string;
  mfScoreBase: number;
  mfScoreFinal: number;
  classificacao: string;
  risco: string;
  penalidadeTotal: number;
  scoreHumanoSugerido?: number | null;
  faixaEsperadaMin?: number | null;
  faixaEsperadaMax?: number | null;
  diferencaScoreHumano?: number | null;
  dentroDaFaixaEsperada?: boolean | null;
  observacaoComparativa?: string | null;
  pilares: ResultadoPilarMfScorePersona[];
  indicadoresCriticos: ResultadoIndicadorCriticoMfScorePersona[];
  penalizacoesAplicadas: string[];
}
