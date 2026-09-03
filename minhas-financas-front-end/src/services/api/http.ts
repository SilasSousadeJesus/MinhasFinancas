import { ApiError, RetornoGenerico } from "@/types/api";
import { API_BASE_URL } from "./config";
import { LoadingRequestOptions, startGlobalLoading } from "./loading-manager";

interface RequestOptions extends RequestInit {
  token?: string;
  loading?: LoadingRequestOptions;
}

export async function apiRequest<T>(
  path: string,
  { token, headers, loading, ...init }: RequestOptions = {}
): Promise<RetornoGenerico<T>> {
  const finalizarLoading = startGlobalLoading(loading);

  try {
    const isFormData = init.body instanceof FormData;

    const response = await fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers: {
        ...(isFormData ? {} : { "Content-Type": "application/json" }),
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
  } finally {
    finalizarLoading();
  }
}

export async function downloadRequest(
  path: string,
  { token, headers, loading, ...init }: RequestOptions = {}
): Promise<Blob> {
  const finalizarLoading = startGlobalLoading(loading);

  try {
    const response = await fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers: {
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...headers,
      },
    });

    if (!response.ok) {
      const rawBody = await response.text();
      let data: RetornoGenerico | null = null;

      if (rawBody) {
        try {
          data = JSON.parse(rawBody) as RetornoGenerico;
        } catch {
          data = null;
        }
      }

      throw new ApiError(
        data?.mensagemUsuario ||
          data?.mensagemSistema ||
          rawBody ||
          "Erro ao baixar o arquivo.",
        response.status,
        data ?? undefined
      );
    }

    return await response.blob();
  } finally {
    finalizarLoading();
  }
}
