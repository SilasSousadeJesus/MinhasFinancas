import {
  CadastrarLancamentoPayload,
  CartaoResumo,
  ContaResumo,
} from "@/types/finance";
import { CategoriaResumo } from "@/types/categories";
import { apiRequest } from "./http";

export function buscarContas(usuarioId: string, token: string) {
  return apiRequest<ContaResumo[]>(`/Conta/BuscarTodosAsContas/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function buscarCartoes(usuarioId: string, token: string) {
  return apiRequest<CartaoResumo[]>(`/Cartao/BuscarTodosOsCartoes/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function buscarCategorias(usuarioId: string, token: string) {
  return apiRequest<CategoriaResumo[]>(`/Categoria/BuscarTodosAsCategorias/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function cadastrarLancamento(payload: CadastrarLancamentoPayload, token: string) {
  return apiRequest<null>("/Lancamento/CadastrarLancamento", {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}
