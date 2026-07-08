import { apiRequest } from "./http";
import {
  CompromissoFinanceiroItem,
  SalvarCompromissoFinanceiroPayload,
} from "@/types/compromissos-financeiros";

const inflightRequests = new Map<string, Promise<unknown>>();

function withDedupe<T>(key: string, factory: () => Promise<T>) {
  const existing = inflightRequests.get(key);
  if (existing) {
    return existing as Promise<T>;
  }

  const request = factory().finally(() => {
    inflightRequests.delete(key);
  });

  inflightRequests.set(key, request);
  return request;
}

export function listarCompromissosFinanceiros(usuarioId: string, token: string) {
  return withDedupe(`listar-compromissos-financeiros:${usuarioId}`, () =>
    apiRequest<CompromissoFinanceiroItem[]>(`/CompromissosFinanceiros/${usuarioId}`, {
      method: "GET",
      token,
    })
  );
}

export function buscarCompromissoFinanceiro(
  usuarioId: string,
  compromissoId: string,
  token: string
) {
  return withDedupe(`buscar-compromisso-financeiro:${usuarioId}:${compromissoId}`, () =>
    apiRequest<CompromissoFinanceiroItem>(
      `/CompromissosFinanceiros/${usuarioId}/${compromissoId}`,
      {
        method: "GET",
        token,
      }
    )
  );
}

export function cadastrarCompromissoFinanceiro(
  usuarioId: string,
  payload: SalvarCompromissoFinanceiroPayload,
  token: string
) {
  return apiRequest<CompromissoFinanceiroItem>(`/CompromissosFinanceiros/Cadastrar/${usuarioId}`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarCompromissoFinanceiro(
  usuarioId: string,
  compromissoId: string,
  payload: SalvarCompromissoFinanceiroPayload,
  token: string
) {
  return apiRequest<CompromissoFinanceiroItem>(
    `/CompromissosFinanceiros/Editar/${usuarioId}/${compromissoId}`,
    {
      method: "PUT",
      token,
      body: JSON.stringify(payload),
    }
  );
}

export function concluirCompromissoFinanceiro(
  usuarioId: string,
  compromissoId: string,
  token: string
) {
  return apiRequest<CompromissoFinanceiroItem>(
    `/CompromissosFinanceiros/Concluir/${usuarioId}/${compromissoId}`,
    {
      method: "PUT",
      token,
    }
  );
}

export function cancelarCompromissoFinanceiro(
  usuarioId: string,
  compromissoId: string,
  token: string
) {
  return apiRequest<CompromissoFinanceiroItem>(
    `/CompromissosFinanceiros/Cancelar/${usuarioId}/${compromissoId}`,
    {
      method: "PUT",
      token,
    }
  );
}

export function excluirCompromissoFinanceiro(
  usuarioId: string,
  compromissoId: string,
  token: string
) {
  return apiRequest<void>(`/CompromissosFinanceiros/Excluir/${usuarioId}/${compromissoId}`, {
    method: "DELETE",
    token,
  });
}
