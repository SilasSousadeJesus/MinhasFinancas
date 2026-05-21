export type TipoCategoria = 0 | 1 | 2 | 3;

export interface SubCategoriaResumo {
  id: string;
  nomeSubCategoria: string;
  categoriaId: string;
}

export interface CategoriaResumo {
  id: string;
  nomeCategoria: string;
  icone: string;
  tipo: TipoCategoria;
  usuarioId?: string | null;
  subCategorias?: SubCategoriaResumo[] | null;
}

export interface CadastrarCategoriaPayload {
  nomeCategoria: string;
  icone: string;
  tipo: TipoCategoria;
  usuarioId: string;
}

export interface EditarCategoriaPayload extends CadastrarCategoriaPayload {}

export interface CadastrarSubCategoriaPayload {
  nomeSubCategoria: string;
  categoriaId: string;
}

export interface EditarSubCategoriaPayload extends CadastrarSubCategoriaPayload {
  id: string;
}
