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

export interface PainelIndicadoresFinanceirosSaude {
  economiaMensal: IndicadorFinanceiroSaude;
  percentualEconomia: IndicadorFinanceiroSaude;
  reservaEmergenciaAtual: IndicadorFinanceiroSaude;
  reservaEmergenciaIdeal: IndicadorFinanceiroSaude;
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
  pontosAtencao: PontoAtencaoSaudeFinanceira[];
}

export interface SaudeFinanceiraData {
  resumo: ResumoSaudeFinanceira;
  indicadores: PainelIndicadoresFinanceirosSaude;
}
