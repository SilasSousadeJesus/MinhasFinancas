export interface FluxoCaixaSimplesItem {
  id: string;
  descricao: string;
  categoria?: string | null;
  valor: number;
  dataVencimento: string;
}

export interface FluxoCaixaSimplesResumo {
  ano: number;
  mes: number;
  receitasTotal: number;
  despesasTotal: number;
  saldoMes: number;
  receitas: FluxoCaixaSimplesItem[];
  despesas: FluxoCaixaSimplesItem[];
}
