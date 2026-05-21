import { CadastroPayload, LoginPayload, TokenResponse } from "@/types/auth";
import { apiRequest } from "./http";

export function login(payload: LoginPayload) {
  return apiRequest<TokenResponse>("/Autenticacao/Login", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export function cadastrar(payload: CadastroPayload) {
  return apiRequest<null>("/Usuario/Cadastrar", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}
