import { apiRequest } from "./http";
import { RetornoGenerico } from "@/types/api";
import {
  AtivoPatrimonialPayload,
  PassivoPatrimonialPayload,
  SnapshotPatrimonialPayload,
  VisaoGeralPatrimonio,
} from "@/types/patrimonio";

export function buscarVisaoGeralPatrimonio(usuarioId: string, token: string) {
  return apiRequest<VisaoGeralPatrimonio>(`/Patrimonio/BuscarVisaoGeral/${usuarioId}`, {
    method: "GET",
    token,
  });
}

export function cadastrarAtivoPatrimonial(
  payload: AtivoPatrimonialPayload,
  token: string
) {
  return apiRequest<null>(`/BemMaterial/CadastrarBemMaterial`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarAtivoPatrimonial(
  usuarioId: string,
  ativoId: string,
  payload: AtivoPatrimonialPayload,
  token: string
) {
  return apiRequest<null>(`/BemMaterial/EditarBemMaterial/${usuarioId}/${ativoId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function inativarAtivoPatrimonial(
  usuarioId: string,
  ativoId: string,
  token: string
) {
  return apiRequest<null>(`/BemMaterial/DeletarBemMaterial/${usuarioId}/${ativoId}`, {
    method: "DELETE",
    token,
  });
}

export function cadastrarPassivoPatrimonial(
  payload: PassivoPatrimonialPayload,
  token: string
) {
  return apiRequest<null>(`/Passivo/CadastrarPassivo`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarPassivoPatrimonial(
  usuarioId: string,
  passivoId: string,
  payload: PassivoPatrimonialPayload,
  token: string
) {
  return apiRequest<null>(`/Passivo/EditarPassivo/${usuarioId}/${passivoId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function inativarPassivoPatrimonial(
  usuarioId: string,
  passivoId: string,
  token: string
) {
  return apiRequest<null>(`/Passivo/DeletarPassivo/${usuarioId}/${passivoId}`, {
    method: "DELETE",
    token,
  });
}

export function gerarSnapshotPatrimonial(
  usuarioId: string,
  payload: SnapshotPatrimonialPayload,
  token: string
) {
  return apiRequest<null>(`/Patrimonio/GerarSnapshot/${usuarioId}`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export type PatrimonioResponse = RetornoGenerico<VisaoGeralPatrimonio>;
