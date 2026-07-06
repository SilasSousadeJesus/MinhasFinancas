import { apiRequest } from "./http";
import { ResumoFinanceiroIAData } from "@/types/resumo-financeiro-ia";

export function buscarResumoFinanceiroIA(usuarioId: string, token: string) {
  return apiRequest<ResumoFinanceiroIAData>(`/ResumoFinanceiroIA/${usuarioId}`, {
    method: "GET",
    token,
  });
}
