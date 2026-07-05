import { apiRequest, downloadRequest } from "./http";
import {
  EditarLancamentoPayload,
  FiltroLancamentosParams,
  LancamentoResumo,
  RespostaLancamentos,
} from "@/types/lancamentos";
import { RetornoGenerico } from "@/types/api";

const requisicoesLancamentosEmAndamento = new Map<
  string,
  Promise<RetornoGenerico<RespostaLancamentos>>
>();

const keyMap: Record<string, string> = {
  buscaDescricao: "BuscaDescricao",
  tipo: "Tipo",
  categoriaId: "CategoriaId",
  contaId: "ContaId",
  cartaoId: "CartaoId",
  statusLancamento: "StatusLancamento",
  dataInicialLancamento: "DataInicialLancamento",
  dataFinalLancamento: "DataFinalLancamento",
  dataInicialVencimento: "DataInicialVencimento",
  dataFinalVencimento: "DataFinalVencimento",
  dataInicialEfetivacao: "DataInicialEfetivacao",
  dataFinalEfetivacao: "DataFinalEfetivacao",
  ordenarPor: "OrdenarPor",
  direcao: "Direcao",
  pagina: "Pagina",
  tamanhoPagina: "TamanhoPagina",
};

export function buildLancamentosQueryString(filtros: FiltroLancamentosParams = {}) {
  const searchParams = new URLSearchParams();

  Object.entries(filtros).forEach(([key, value]) => {
    if (value === undefined || value === null || value === "") {
      return;
    }

    const queryKey = keyMap[key] ?? key;
    searchParams.set(queryKey, String(value));
  });

  return searchParams.toString();
}

export function buscarLancamentos(
  usuarioId: string,
  token: string,
  filtros: FiltroLancamentosParams = {}
) {
  const queryString = buildLancamentosQueryString(filtros);
  const path = `/Lancamento/BuscarTodosOsLancamento/${usuarioId}${
    queryString ? `?${queryString}` : ""
  }`;
  const requestKey = `${usuarioId}:${queryString || "sem-filtros"}`;

  const requisicaoEmAndamento = requisicoesLancamentosEmAndamento.get(requestKey);

  if (requisicaoEmAndamento) {
    return requisicaoEmAndamento;
  }

  const request = apiRequest<RespostaLancamentos>(path, {
    method: "GET",
    token,
  }).finally(() => {
    const requisicaoAtual = requisicoesLancamentosEmAndamento.get(requestKey);

    if (requisicaoAtual === request) {
      requisicoesLancamentosEmAndamento.delete(requestKey);
    }
  });

  requisicoesLancamentosEmAndamento.set(requestKey, request);

  return request;
}

export function buscarLancamento(usuarioId: string, lancamentoId: string, token: string) {
  return apiRequest<LancamentoResumo>(
    `/Lancamento/BuscarUmLancamento/${usuarioId}/${lancamentoId}`,
    {
      method: "GET",
      token,
    }
  );
}

export function editarLancamento(
  usuarioId: string,
  lancamentoId: string,
  payload: EditarLancamentoPayload,
  token: string
) {
  return apiRequest<null>(`/Lancamento/EditarLancamento/${usuarioId}/${lancamentoId}`, {
    method: "PUT",
    token,
    body: JSON.stringify(payload),
  });
}

export function efetivarLancamento(usuarioId: string, lancamentoId: string, token: string) {
  return apiRequest<null>(`/Lancamento/EfetivarLancamento/${usuarioId}/${lancamentoId}`, {
    method: "POST",
    token,
  });
}

export function deletarLancamento(usuarioId: string, lancamentoId: string, token: string) {
  return apiRequest<null>(`/Lancamento/DeletarLancamento/${usuarioId}/${lancamentoId}`, {
    method: "DELETE",
    token,
  });
}

export function exportarLancamentosExcel(
  usuarioId: string,
  token: string,
  filtros: FiltroLancamentosParams = {}
) {
  const queryString = buildLancamentosQueryString({
    ...filtros,
    pagina: undefined,
    tamanhoPagina: undefined,
  });

  return downloadRequest(
    `/Lancamento/ExportarExcel/${usuarioId}${queryString ? `?${queryString}` : ""}`,
    {
      method: "GET",
      token,
    }
  );
}
