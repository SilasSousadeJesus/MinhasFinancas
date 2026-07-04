import { CategoriaResumo, SubCategoriaResumo } from "./categories";

export interface LancamentoResumo {
  id: string;
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
  usuarioId: string | null;
  categoriaId: string | null;
  subCategoriaId: string | null;
  categoria?: CategoriaResumo | null;
  subCategoria?: SubCategoriaResumo | null;
}

export interface EditarLancamentoPayload {
  id: string;
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
