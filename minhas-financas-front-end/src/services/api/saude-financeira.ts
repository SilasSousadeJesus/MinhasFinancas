import { apiRequest } from "./http";
import { SaudeFinanceiraData } from "@/types/saude-financeira";

export function buscarSaudeFinanceira(usuarioId: string, token: string) {
  return apiRequest<SaudeFinanceiraData>(`/SaudeFinanceira/${usuarioId}`, {
    method: "GET",
    token,
  });
}
