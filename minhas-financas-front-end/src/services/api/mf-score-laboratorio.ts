import { apiRequest } from "./http";
import {
  MfScoreLaboratorioDetalhe,
  ResultadoGeracaoBaseSimulacaoMfScore,
  ResultadoLimpezaBaseSimulacaoMfScore,
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

export function gerarBaseSimulacaoMfScoreLaboratorio(token: string) {
  return apiRequest<ResultadoGeracaoBaseSimulacaoMfScore>(
    "/MfScoreLaboratorio/GerarBaseSimulacao",
    {
      method: "POST",
      token,
    }
  );
}

export function limparBaseSimulacaoMfScoreLaboratorio(token: string) {
  return apiRequest<ResultadoLimpezaBaseSimulacaoMfScore>(
    "/MfScoreLaboratorio/LimparBaseSimulacao",
    {
      method: "DELETE",
      token,
    }
  );
}
