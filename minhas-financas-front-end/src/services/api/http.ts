import { ApiError, RetornoGenerico } from "@/types/api";
import { API_BASE_URL } from "./config";

interface RequestOptions extends RequestInit {
  token?: string;
}

export async function apiRequest<T>(
  path: string,
  { token, headers, ...init }: RequestOptions = {}
): Promise<RetornoGenerico<T>> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
  });

  const rawBody = await response.text();
  let data: RetornoGenerico<T> | null = null;

  if (rawBody) {
    try {
      data = JSON.parse(rawBody) as RetornoGenerico<T>;
    } catch {
      data = null;
    }
  }

  if (!response.ok || !data?.sucesso) {
    throw new ApiError(
      data?.mensagemUsuario ||
        data?.mensagemSistema ||
        (rawBody && !data ? rawBody : "Erro ao processar a requisicao."),
      response.status,
      data ?? undefined
    );
  }

  return data;
}
