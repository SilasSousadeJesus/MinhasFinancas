import { apiRequest } from "./http";
import {
  SalvarPerfilFinanceiroPayload,
  VisaoGeralPerfilFinanceiro,
} from "@/types/perfil-financeiro";

export function buscarPerfilFinanceiro(usuarioId: string, token: string) {
  return apiRequest<VisaoGeralPerfilFinanceiro>(
    `/PerfilFinanceiro/BuscarPerfilFinanceiro/${usuarioId}`,
    {
      method: "GET",
      token,
    }
  );
}

export function salvarPerfilFinanceiro(
  usuarioId: string,
  payload: SalvarPerfilFinanceiroPayload,
  token: string
) {
  return apiRequest<VisaoGeralPerfilFinanceiro>(
    `/PerfilFinanceiro/SalvarPerfilFinanceiro/${usuarioId}`,
    {
      method: "POST",
      token,
      body: JSON.stringify(payload),
    }
  );
}
