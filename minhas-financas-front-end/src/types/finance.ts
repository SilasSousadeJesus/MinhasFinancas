import { CategoriaResumo } from "./categories";

export interface ContaResumo {
  id: string;
  nomeConta: string;
  instituicao: string;
}

export interface CartaoResumo {
  id: string;
  nomeCartao: string;
  instituicao: string;
}

export interface CadastrarLancamentoPayload {
  valor: number;
  descricao: string;
  observacao: string;
  dataPagamento: string;
  dataLancamento: string;
  realizado: boolean;
  frequenciaLancamento: number;
  tipo: number;
  vinculo: number;
  contaId: string | null;
  cartaoId: string | null;
  usuarioId: string;
  categoriaId: string | null;
  subCategoriaId: string | null;
}
