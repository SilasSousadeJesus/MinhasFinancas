import { apiRequest } from "./http";
import {
  GerarAnaliseAssistenteFinanceiroPayload,
  RespostaAssistenteFinanceiroIA,
} from "@/types/assistente-financeiro";

export function gerarAnaliseAssistenteFinanceiro(
  usuarioId: string,
  token: string,
  payload: GerarAnaliseAssistenteFinanceiroPayload
) {
  return apiRequest<RespostaAssistenteFinanceiroIA>(`/AssistenteFinanceiro/GerarAnalise/${usuarioId}`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}
