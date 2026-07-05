import { apiRequest } from "./http";
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
