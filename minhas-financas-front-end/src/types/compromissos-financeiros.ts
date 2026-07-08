export enum OrigemCompromissoFinanceiro {
  Manual = 0,
  IA = 1,
}

export enum StatusCompromissoFinanceiro {
  EmAndamento = 0,
  Concluido = 1,
  Cancelado = 2,
}

export interface CompromissoFinanceiroItem {
  id: string;
  usuarioId: string;
  descricao: string;
  origem: OrigemCompromissoFinanceiro;
  status: StatusCompromissoFinanceiro;
  analiseFinanceiraHistoricaId?: string | null;
  dataCriacao: string;
  dataConclusao?: string | null;
  dataCancelamento?: string | null;
  observacoes?: string | null;
  ativo: boolean;
}

export interface SalvarCompromissoFinanceiroPayload {
  usuarioId?: string;
  descricao: string;
  origem: OrigemCompromissoFinanceiro;
  analiseFinanceiraHistoricaId?: string | null;
  observacoes?: string | null;
}
