import {
  CadastrarCategoriaPayload,
  CadastrarSubCategoriaPayload,
  CategoriaResumo,
  EditarCategoriaPayload,
  EditarSubCategoriaPayload,
  SubCategoriaResumo,
} from "@/types/categories";
import { apiRequest } from "./http";

export function buscarCategorias(usuarioId: string, token: string) {
  return apiRequest<CategoriaResumo[]>(`/Categoria/BuscarTodosAsCategorias/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function buscarSubCategorias(usuarioId: string, categoriaId: string, token: string) {
  return apiRequest<SubCategoriaResumo[]>(
    `/Categoria/BuscarTodosAsSubCategorias/${usuarioId}/${categoriaId}`,
    {
      method: "GET",
      token,
    }
  );
}

export function cadastrarCategoria(payload: CadastrarCategoriaPayload, token: string) {
  return apiRequest<null>("/Categoria/CadastrarCategoria", {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarCategoria(
  usuarioId: string,
  categoriaId: string,
  payload: EditarCategoriaPayload,
  token: string
) {
  return apiRequest<null>(`/Categoria/EditarCategoria/${usuarioId}/${categoriaId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function deletarCategoria(usuarioId: string, categoriaId: string, token: string) {
  return apiRequest<null>(`/Categoria/DeletarCategoria/${usuarioId}/${categoriaId}`, {
    method: "DELETE",
    token,
  });
}

export function cadastrarSubCategoria(
  usuarioId: string,
  categoriaId: string,
  payload: CadastrarSubCategoriaPayload,
  token: string
) {
  return apiRequest<null>(`/Categoria/CadastrarSubCategoria/${usuarioId}/${categoriaId}`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarSubCategoria(
  usuarioId: string,
  categoriaId: string,
  subCategoriaId: string,
  payload: EditarSubCategoriaPayload,
  token: string
) {
  return apiRequest<null>(
    `/Categoria/EditarSubCategoria/${usuarioId}/${categoriaId}/${subCategoriaId}`,
    {
      method: "PUT",
      token,
      body: JSON.stringify(payload),
    }
  );
}

export function deletarSubCategoria(
  usuarioId: string,
  categoriaId: string,
  subCategoriaId: string,
  token: string
) {
  return apiRequest<null>(
    `/Categoria/DeletarSubCategoria/${usuarioId}/${categoriaId}/${subCategoriaId}`,
    {
      method: "DELETE",
      token,
    }
  );
}
