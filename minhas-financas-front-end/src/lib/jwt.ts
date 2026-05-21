import { AuthSession, TokenPayload } from "@/types/auth";

const STORAGE_KEY = "minhas-financas.auth";

function normalizeBase64(value: string) {
  const remainder = value.length % 4;
  const padded = remainder === 0 ? value : `${value}${"=".repeat(4 - remainder)}`;

  return padded.replace(/-/g, "+").replace(/_/g, "/");
}

export function decodeJwt(token: string): TokenPayload | null {
  try {
    const [, payload] = token.split(".");

    if (!payload) {
      return null;
    }

    const normalized = normalizeBase64(payload);
    const decoded = atob(normalized);

    return JSON.parse(decoded) as TokenPayload;
  } catch {
    return null;
  }
}

export function buildSession(token: string, refreshToken: string): AuthSession | null {
  const payload = decodeJwt(token);

  if (!payload?.sub) {
    return null;
  }

  return {
    token,
    refreshToken,
    usuario: {
      id: payload.sub,
      nome: payload.name ?? "",
      email: payload.email ?? "",
    },
  };
}

export function isTokenExpired(token: string) {
  const payload = decodeJwt(token);

  if (!payload?.exp) {
    return false;
  }

  return payload.exp * 1000 <= Date.now();
}

export function getAuthStorageKey() {
  return STORAGE_KEY;
}
