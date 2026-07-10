import { apiRequest } from "./http";
import {
  MfScoreLaboratorioDetalhe,
  UsuarioMfScoreLaboratorio,
} from "@/types/mf-score-laboratorio";

export function listarUsuariosMfScoreLaboratorio(token: string) {
  return apiRequest<UsuarioMfScoreLaboratorio[]>("/MfScoreLaboratorio/Usuarios", {
    method: "GET",
    token,
  });
}

export function buscarScoreUsuarioMfScoreLaboratorio(
  usuarioId: string,
  token: string
) {
  return apiRequest<MfScoreLaboratorioDetalhe>(
    `/MfScoreLaboratorio/Usuarios/${usuarioId}/Score`,
    {
      method: "GET",
      token,
    }
  );
}
