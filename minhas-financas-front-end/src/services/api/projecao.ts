import { apiRequest } from "./http";
import {
  CalcularProjecaoPayload,
  CriarProjecaoPayload,
  EditarProjecaoPayload,
  ProjecaoDetalhe,
  ProjecaoResumo,
  ResultadoProjecao,
} from "@/types/projecao";

export function listarProjecoes(usuarioId: string, token: string) {
  return apiRequest<ProjecaoResumo[]>(`/Projecao/BuscarTodas/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function buscarProjecao(usuarioId: string, projecaoId: string, token: string) {
  return apiRequest<ProjecaoDetalhe>(`/Projecao/BuscarUma/${usuarioId}/${projecaoId}`, {
    method: "GET",
    token,
  });
}

export function criarProjecao(payload: CriarProjecaoPayload, token: string) {
  return apiRequest<string>(`/Projecao/Cadastrar`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarProjecao(
  usuarioId: string,
  projecaoId: string,
  payload: EditarProjecaoPayload,
  token: string
) {
  return apiRequest<void>(`/Projecao/Editar/${usuarioId}/${projecaoId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function excluirProjecao(usuarioId: string, projecaoId: string, token: string) {
  return apiRequest<void>(`/Projecao/Deletar/${usuarioId}/${projecaoId}`, {
    method: "DELETE",
    token,
  });
}

export function calcularProjecao(
  usuarioId: string,
  payload: CalcularProjecaoPayload,
  token: string
) {
  return apiRequest<ResultadoProjecao>(`/Projecao/Calcular/${usuarioId}`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function calcularProjecaoSalva(usuarioId: string, projecaoId: string, token: string) {
  return apiRequest<ResultadoProjecao>(`/Projecao/CalcularSalva/${usuarioId}/${projecaoId}`, {
    method: "POST",
    token,
  });
}
