import {
  IndicadorFinanceiroSaude,
  PainelIndicadoresFinanceirosSaude,
  ResumoSaudeFinanceira,
} from "@/types/saude-financeira";

export interface InsightFinanceiroData {
  codigoIndicadorRelacionado?: number | null;
  tipo: number;
  prioridade: number;
  titulo: string;
  descricao: string;
  acaoSugerida: string;
}

export interface PainelInsightsFinanceirosData {
  todos: InsightFinanceiroData[];
  prioritarios: InsightFinanceiroData[];
  destaquesPositivos: InsightFinanceiroData[];
}

export interface ResumoFinanceiroIAData {
  dataReferencia: string;
  saudeFinanceira: ResumoSaudeFinanceira;
  indicadores: PainelIndicadoresFinanceirosSaude;
  insights: PainelInsightsFinanceirosData;
  resumoExecutivo: string;
  prioridadesImediatas: string[];
  destaquesPositivos: string[];
}

export type IndicadorResumoFinanceiroIA = IndicadorFinanceiroSaude;
