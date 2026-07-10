export interface IndicadorFinanceiroSaude {
  codigo: number;
  nome: string;
  valorAtual: number;
  valorIdeal: number;
  percentual: number;
  valorObrigacoesPrevistas?: number | null;
  valorReceitaPrevista?: number | null;
  percentualComprometimento?: number | null;
  status: number;
  descricao: string;
  observacao: string;
  formato: number;
}

export interface PilarMfScoreFinanceiro {
  codigo: number;
  nome: string;
  peso: number;
  nota: number;
  descricao: string;
  indicadores: string[];
}

export interface IndicadorCriticoMfScoreFinanceiro {
  codigoIndicador: number;
  nome: string;
  motivo: string;
  penalidade: number;
  pilarRelacionado: string;
}

export interface TendenciaMfScoreFinanceiro {
  direcao: number;
  descricao: string;
  historicoNotas: number[];
}

export interface MfScoreFinanceiro {
  pontuacaoBase: number;
  pontuacaoFinal: number;
  classificacao: string;
  risco: string;
  tendencia: TendenciaMfScoreFinanceiro;
  pilares: PilarMfScoreFinanceiro[];
  indicadoresCriticos: IndicadorCriticoMfScoreFinanceiro[];
  resumoExecutivoDosPilares: string[];
  regrasCriticasAplicadas: string[];
  descricao: string;
  penalidadeTotal: number;
}

export interface PainelIndicadoresFinanceirosSaude {
  economiaMensal: IndicadorFinanceiroSaude;
  percentualEconomia: IndicadorFinanceiroSaude;
  reservaEmergenciaAtual: IndicadorFinanceiroSaude;
  reservaEmergenciaIdeal: IndicadorFinanceiroSaude;
  capacidadeFormacaoReserva: IndicadorFinanceiroSaude;
  comprometimentoRenda: IndicadorFinanceiroSaude;
  comprometimentoFinanceiroFuturo: IndicadorFinanceiroSaude;
  comprometimentoFinanceiroFuturo90Dias: IndicadorFinanceiroSaude;
  comprometimentoFinanceiroFuturo180Dias: IndicadorFinanceiroSaude;
  comprometimentoFinanceiroFuturo365Dias: IndicadorFinanceiroSaude;
  endividamento: IndicadorFinanceiroSaude;
  patrimonioLiquidoAtual: IndicadorFinanceiroSaude;
  percentualPatrimonioAlvo: IndicadorFinanceiroSaude;
  todos: IndicadorFinanceiroSaude[];
}

export interface PontoAtencaoSaudeFinanceira {
  nome: string;
  status: number;
  descricao: string;
  observacao: string;
}

export interface ResumoSaudeFinanceira {
  pontuacaoGeral: number;
  classificacao: string;
  mfScore: MfScoreFinanceiro;
  pontosAtencao: PontoAtencaoSaudeFinanceira[];
}

export interface SaudeFinanceiraData {
  resumo: ResumoSaudeFinanceira;
  indicadores: PainelIndicadoresFinanceirosSaude;
}
