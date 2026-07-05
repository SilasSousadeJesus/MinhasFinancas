import { apiRequest } from "./http";
import {
  CadastrarSimulacaoFinanceiraPayload,
  EditarSimulacaoFinanceiraPayload,
  ResultadoSimulacaoFinanceira,
  SimulacaoFinanceiraDetalhe,
  SimulacaoFinanceiraResumo,
} from "@/types/simulacao-financeira";

export function listarSimulacoesFinanceiras(usuarioId: string, token: string) {
  return apiRequest<SimulacaoFinanceiraResumo[]>(
    `/SimulacaoFinanceira/BuscarTodas/${usuarioId}`,
    {
      method: "GET",
      token,
    }
  );
}

export function buscarSimulacaoFinanceira(
  usuarioId: string,
  simulacaoId: string,
  token: string
) {
  return apiRequest<SimulacaoFinanceiraDetalhe>(
    `/SimulacaoFinanceira/BuscarUma/${usuarioId}/${simulacaoId}`,
    {
      method: "GET",
      token,
    }
  );
}

export function criarSimulacaoFinanceira(
  payload: CadastrarSimulacaoFinanceiraPayload,
  token: string
) {
  return apiRequest<string>(`/SimulacaoFinanceira/Cadastrar`, {
    method: "POST",
    token,
    body: JSON.stringify(payload),
  });
}

export function editarSimulacaoFinanceira(
  usuarioId: string,
  simulacaoId: string,
  payload: EditarSimulacaoFinanceiraPayload,
  token: string
) {
  return apiRequest<null>(`/SimulacaoFinanceira/Editar/${usuarioId}/${simulacaoId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function inativarSimulacaoFinanceira(
  usuarioId: string,
  simulacaoId: string,
  token: string
) {
  return apiRequest<null>(`/SimulacaoFinanceira/Inativar/${usuarioId}/${simulacaoId}`, {
    method: "DELETE",
    token,
  });
}

export function calcularSimulacaoFinanceira(
  usuarioId: string,
  simulacaoId: string,
  token: string
) {
  return apiRequest<ResultadoSimulacaoFinanceira>(
    `/SimulacaoFinanceira/Calcular/${usuarioId}/${simulacaoId}`,
    {
      method: "GET",
      token,
    }
  );
}
