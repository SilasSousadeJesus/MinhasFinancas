import { apiRequest, downloadRequest } from "./http";
import { FluxoCaixaSimplesResumo } from "@/types/fluxo-caixa-simples";

export function buscarFluxoCaixaSimples(
  usuarioId: string,
  ano: number,
  mes: number,
  token: string
) {
  return apiRequest<FluxoCaixaSimplesResumo>(
    `/Lancamento/FluxoCaixaSimples/${usuarioId}?ano=${ano}&mes=${mes}`,
    {
      method: "GET",
      token,
    }
  );
}

export interface ExportarFluxoCaixaSimplesParams {
  tipoPeriodo: "mes-atual" | "intervalo" | "ano";
  ano?: number;
  mes?: number;
  anoInicial?: number;
  mesInicial?: number;
  anoFinal?: number;
  mesFinal?: number;
}

export function exportarFluxoCaixaSimplesExcel(
  usuarioId: string,
  params: ExportarFluxoCaixaSimplesParams,
  token: string
) {
  const searchParams = new URLSearchParams();

  Object.entries(params).forEach(([key, value]) => {
    if (value === undefined || value === null || value === "") {
      return;
    }

    const queryKey = key.charAt(0).toUpperCase() + key.slice(1);
    searchParams.set(queryKey, String(value));
  });

  const queryString = searchParams.toString();

  return downloadRequest(
    `/Lancamento/ExportarFluxoCaixaSimplesExcel/${usuarioId}${queryString ? `?${queryString}` : ""}`,
    {
      method: "GET",
      token,
    }
  );
}
