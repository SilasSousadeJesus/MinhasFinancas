import { apiRequest } from "./http";
import {
  AnaliseFinanceiraHistoricaDetalhe,
  AnaliseFinanceiraHistoricaLista,
  GerarAnaliseAssistenteFinanceiroPayload,
  RespostaAssistenteFinanceiroIA,
  ResultadoPaginadoAnaliseFinanceiraHistorica,
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

export function buscarAnalisesFinanceirasHistoricas(
  usuarioId: string,
  token: string,
  pagina = 1,
  tamanhoPagina = 5
) {
  return apiRequest<ResultadoPaginadoAnaliseFinanceiraHistorica>(
    `/AnalisesFinanceirasHistoricas/${usuarioId}?pagina=${pagina}&tamanhoPagina=${tamanhoPagina}`,
    {
      method: "GET",
      token,
    }
  );
}

export function buscarAnaliseFinanceiraHistoricaDetalhe(
  usuarioId: string,
  analiseId: string,
  token: string
) {
  return apiRequest<AnaliseFinanceiraHistoricaDetalhe>(
    `/AnalisesFinanceirasHistoricas/${usuarioId}/${analiseId}`,
    {
      method: "GET",
      token,
    }
  );
}

export function excluirAnaliseFinanceiraHistorica(
  usuarioId: string,
  analiseId: string,
  token: string
) {
  return apiRequest(`/AnalisesFinanceirasHistoricas/${usuarioId}/${analiseId}`, {
    method: "DELETE",
    token,
  });
}
