import { apiRequest } from "./http";
import { EditarLancamentoPayload, LancamentoResumo } from "@/types/lancamentos";

export function buscarLancamentos(usuarioId: string, token: string) {
  return apiRequest<LancamentoResumo[]>(`/Lancamento/BuscarTodosOsLancamento/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function buscarLancamento(usuarioId: string, lancamentoId: string, token: string) {
  return apiRequest<LancamentoResumo>(
    `/Lancamento/BuscarUmLancamento/${usuarioId}/${lancamentoId}`,
    {
      method: "GET",
      token,
    }
  );
}

export function editarLancamento(
  usuarioId: string,
  lancamentoId: string,
  payload: EditarLancamentoPayload,
  token: string
) {
  return apiRequest<null>(`/Lancamento/EditarLancamento/${usuarioId}/${lancamentoId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function deletarLancamento(usuarioId: string, lancamentoId: string, token: string) {
  return apiRequest<null>(`/Lancamento/DeletarLancamento/${usuarioId}/${lancamentoId}`, {
    method: "DELETE",
    token,
  });
}
