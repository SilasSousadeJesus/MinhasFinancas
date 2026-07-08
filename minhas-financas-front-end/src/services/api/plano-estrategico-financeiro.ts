import { apiRequest } from "./http";
import {
  PlanoEstrategicoFinanceiroDetalhe,
  PlanoEstrategicoFinanceiroResumo,
  SalvarPlanoEstrategicoFinanceiroPayload,
} from "@/types/plano-estrategico-financeiro";

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

export function listarPlanosEstrategicos(usuarioId: string, token: string) {
  return withDedupe(`listar-planos-estrategicos:${usuarioId}`, () =>
    apiRequest<PlanoEstrategicoFinanceiroResumo[]>(
      `/PlanoEstrategicoFinanceiro/BuscarTodos/${usuarioId}`,
      {
        method: "GET",
        token,
      }
    )
  );
}

export function buscarPlanoEstrategicoVigente(usuarioId: string, token: string) {
  return withDedupe(`buscar-plano-estrategico-vigente:${usuarioId}`, () =>
    apiRequest<PlanoEstrategicoFinanceiroDetalhe>(
      `/PlanoEstrategicoFinanceiro/BuscarVigente/${usuarioId}`,
      {
        method: "GET",
        token,
      }
    )
  );
}

export function buscarPlanoEstrategico(
  usuarioId: string,
  planoId: string,
  token: string
) {
  return withDedupe(`buscar-plano-estrategico:${usuarioId}:${planoId}`, () =>
    apiRequest<PlanoEstrategicoFinanceiroDetalhe>(
      `/PlanoEstrategicoFinanceiro/BuscarUm/${usuarioId}/${planoId}`,
      {
        method: "GET",
        token,
      }
    )
  );
}

export function criarPlanoEstrategico(
  usuarioId: string,
  payload: SalvarPlanoEstrategicoFinanceiroPayload,
  token: string
) {
  return apiRequest<PlanoEstrategicoFinanceiroDetalhe>(
    `/PlanoEstrategicoFinanceiro/Cadastrar/${usuarioId}`,
    {
      method: "POST",
      token,
      body: JSON.stringify(payload),
    }
  );
}

export function atualizarVersaoPlanoEstrategico(
  usuarioId: string,
  planoId: string,
  payload: SalvarPlanoEstrategicoFinanceiroPayload,
  token: string
) {
  return apiRequest<PlanoEstrategicoFinanceiroDetalhe>(
    `/PlanoEstrategicoFinanceiro/AtualizarVersao/${usuarioId}/${planoId}`,
    {
      method: "PUT",
      token,
      body: JSON.stringify(payload),
    }
  );
}

export function inativarPlanoEstrategico(
  usuarioId: string,
  planoId: string,
  token: string
) {
  return apiRequest<void>(`/PlanoEstrategicoFinanceiro/Inativar/${usuarioId}/${planoId}`, {
    method: "DELETE",
    token,
  });
}
