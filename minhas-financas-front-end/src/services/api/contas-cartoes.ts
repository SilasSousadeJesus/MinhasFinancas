import { apiRequest } from "./http";
import { CartaoItem, CartaoPayload, ContaItem, ContaPayload } from "@/types/contas-cartoes";

export function buscarContas(usuarioId: string, token: string) {
  return apiRequest<ContaItem[]>(`/Conta/BuscarTodosAsContas/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function cadastrarConta(payload: ContaPayload, token: string) {
  return apiRequest<null>("/Conta/Cadastrar", {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarConta(usuarioId: string, contaId: string, payload: ContaPayload, token: string) {
  return apiRequest<null>(`/Conta/EditarConta/${usuarioId}/${contaId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function deletarConta(usuarioId: string, contaId: string, token: string) {
  return apiRequest<null>(`/Conta/DeletarConta/${usuarioId}/${contaId}`, {
    method: "DELETE",
    token,
  });
}

export function buscarCartoes(usuarioId: string, token: string) {
  return apiRequest<CartaoItem[]>(`/Cartao/BuscarTodosOsCartoes/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function cadastrarCartao(payload: CartaoPayload, token: string) {
  return apiRequest<null>("/Cartao/CadastrarCartao", {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarCartao(usuarioId: string, cartaoId: string, payload: CartaoPayload, token: string) {
  return apiRequest<null>(`/Cartao/EditarCartao/${usuarioId}/${cartaoId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function deletarCartao(usuarioId: string, cartaoId: string, token: string) {
  return apiRequest<null>(`/Cartao/DeletarCartao/${usuarioId}/${cartaoId}`, {
    method: "DELETE",
    token,
  });
}
