import { CategoriaResumo } from "./categories";

export interface ContaResumo {
  id: string;
  nomeConta: string;
  instituicao: string;
  tipo?: number;
  saldo?: number;
  saldoInvestimento?: number;
}

export interface CartaoResumo {
  id: string;
  nomeCartao: string;
  instituicao: string;
  tipo?: number;
  bandeira?: string;
  ultimos4Digitos?: string;
}

export interface CadastrarLancamentoPayload {
  valor: number;
  descricao: string;
  observacao: string;
  dataVencimento: string;
  dataLancamento: string;
  dataEfetivacao: string | null;
  statusLancamento: number;
  frequenciaLancamento: number;
  quantidadeParcelas?: number | null;
  numeroDiaUtil?: number | null;
  tipo: number;
  vinculo: number;
  contaId: string | null;
  cartaoId: string | null;
  usuarioId: string;
  categoriaId: string | null;
  subCategoriaId: string | null;
}
