import { apiRequest } from "./http";
import {
  MfScorePersonaItem,
  ResultadoRodarMfScorePersona,
  SalvarMfScorePersonaPayload,
} from "@/types/mf-score-personas";

export function listarMfScorePersonas(token: string) {
  return apiRequest<MfScorePersonaItem[]>("/MfScorePersonas", {
    method: "GET",
    token,
  });
}

export function buscarMfScorePersona(personaId: string, token: string) {
  return apiRequest<MfScorePersonaItem>(`/MfScorePersonas/${personaId}`, {
    method: "GET",
    token,
  });
}

export function cadastrarMfScorePersona(
  payload: SalvarMfScorePersonaPayload,
  token: string
) {
  return apiRequest<MfScorePersonaItem>("/MfScorePersonas", {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarMfScorePersona(
  personaId: string,
  payload: SalvarMfScorePersonaPayload,
  token: string
) {
  return apiRequest<MfScorePersonaItem>(`/MfScorePersonas/${personaId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function inativarMfScorePersona(personaId: string, token: string) {
  return apiRequest<void>(`/MfScorePersonas/${personaId}`, {
    method: "DELETE",
    token,
  });
}

export function rodarMfScorePersona(personaId: string, token: string) {
  return apiRequest<ResultadoRodarMfScorePersona>(
    `/MfScorePersonas/${personaId}/RodarScore`,
    {
      method: "POST",
      token,
    }
  );
}

export function marcarPersonaAuditada(personaId: string, token: string) {
  return apiRequest<MfScorePersonaItem>(
    `/MfScorePersonas/${personaId}/MarcarAuditada`,
    {
      method: "POST",
      token,
    }
  );
}

export function marcarPersonaCasoCanonico(personaId: string, token: string) {
  return apiRequest<MfScorePersonaItem>(
    `/MfScorePersonas/${personaId}/MarcarCasoCanonico`,
    {
      method: "POST",
      token,
    }
  );
}
