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
  observacoes?: string | null;
}
