"use client";

import { buildSession, getAuthStorageKey, isTokenExpired } from "@/lib/jwt";
import { cadastrar, login } from "@/services/api/auth";
import { AuthSession, CadastroPayload, LoginPayload } from "@/types/auth";
import { ApiError } from "@/types/api";
import {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useState,
} from "react";

interface AuthContextValue {
  session: AuthSession | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  loginWithCredentials: (payload: LoginPayload) => Promise<void>;
  registerUser: (payload: CadastroPayload) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function getStoredSession() {
  if (typeof window === "undefined") {
    return null;
  }

  const raw = window.localStorage.getItem(getAuthStorageKey());

  if (!raw) {
    return null;
  }

  try {
    const parsed = JSON.parse(raw) as AuthSession;

    if (!parsed?.token || isTokenExpired(parsed.token)) {
      window.localStorage.removeItem(getAuthStorageKey());
      return null;
    }

    return parsed;
  } catch {
    window.localStorage.removeItem(getAuthStorageKey());
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    setSession(getStoredSession());
    setIsLoading(false);
  }, []);

  const persistSession = (nextSession: AuthSession | null) => {
    setSession(nextSession);

    if (typeof window === "undefined") {
      return;
    }

    if (!nextSession) {
      window.localStorage.removeItem(getAuthStorageKey());
      return;
    }

    window.localStorage.setItem(getAuthStorageKey(), JSON.stringify(nextSession));
  };

  const logout = () => {
    persistSession(null);
  };

  const loginWithCredentials = async (payload: LoginPayload) => {
    const response = await login(payload);
    const tokenData = response.dados;

    if (!tokenData?.token) {
      throw new ApiError("A autenticação foi concluída, mas o token não foi retornado.");
    }

    const nextSession = buildSession(tokenData.token, tokenData.refrenshToken);

    if (!nextSession) {
      throw new ApiError("Não foi possível interpretar o token retornado pela API.");
    }

    persistSession(nextSession);
  };

  const registerUser = async (payload: CadastroPayload) => {
    await cadastrar(payload);
  };

  const value: AuthContextValue = {
    session,
    isAuthenticated: Boolean(session),
    isLoading,
    loginWithCredentials,
    registerUser,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth deve ser usado dentro de AuthProvider.");
  }

  return context;
}
