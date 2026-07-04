export interface ContaItem {
  id: string;
  nomeConta: string;
  saldo: number;
  saldoInvestimento: number;
  descricao: string;
  instituicao: string;
  tipo: number;
  usuarioId?: string | null;
}

export interface CartaoItem {
  id: string;
  nomeCartao: string;
  saldo: number;
  bandeira: string;
  ultimos4Digitos: string;
  diaFechamento: string;
  diaVencimento: string;
  contaPadraoPagamento: string;
  descricao: string;
  instituicao: string;
  tipo: number;
  usuarioId?: string | null;
}

export interface ContaPayload {
  nomeConta: string;
  saldo: number;
  saldoInvestimento: number;
  descricao: string;
  instituicao: string;
  tipo: number;
  usuarioId?: string;
}

export interface CartaoPayload {
  nomeCartao: string;
  saldo: number;
  bandeira: string;
  ultimos4Digitos: string;
  diaFechamento: string;
  diaVencimento: string;
  contaPadraoPagamento: string;
  descricao: string;
  instituicao: string;
  tipo: number;
  usuarioId?: string;
}
