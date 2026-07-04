import { CategoriaResumo, SubCategoriaResumo } from "./categories";

export interface LancamentoResumo {
  id: string;
  valor: number;
  descricao: string;
  observacao: string;
  dataVencimento: string;
  dataLancamento: string;
  dataEfetivacao: string | null;
  grupoParcelamentoId: string | null;
  numeroParcela: number | null;
  totalParcelas: number | null;
  grupoLancamentoProgramadoId: string | null;
  tipoProgramacao: number | null;
  numeroDiaUtil: number | null;
  statusLancamento: number;
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

export interface FiltroLancamentosParams {
  buscaDescricao?: string;
  tipo?: string;
  categoriaId?: string;
  contaId?: string;
  cartaoId?: string;
  statusLancamento?: string;
  dataInicialLancamento?: string;
  dataFinalLancamento?: string;
  dataInicialVencimento?: string;
  dataFinalVencimento?: string;
  dataInicialEfetivacao?: string;
  dataFinalEfetivacao?: string;
  ordenarPor?: "data" | "valor";
  direcao?: "asc" | "desc";
  pagina?: number;
  tamanhoPagina?: number;
}

export interface ResultadoPaginado<T> {
  itens: T[];
  paginaAtual: number;
  tamanhoPagina: number;
  totalItens: number;
  totalPaginas: number;
}

export type RespostaLancamentos = ResultadoPaginado<LancamentoResumo> | LancamentoResumo[];

export interface EditarLancamentoPayload {
  id: string;
  valor: number;
  descricao: string;
  observacao: string;
  dataVencimento: string;
  dataLancamento: string;
  dataEfetivacao: string | null;
  grupoParcelamentoId: string | null;
  numeroParcela: number | null;
  totalParcelas: number | null;
  grupoLancamentoProgramadoId: string | null;
  tipoProgramacao: number | null;
  numeroDiaUtil: number | null;
  statusLancamento: number;
  frequenciaLancamento: number;
  tipo: number;
  vinculo: number;
  contaId: string | null;
  cartaoId: string | null;
  usuarioId: string;
  categoriaId: string | null;
  subCategoriaId: string | null;
}
