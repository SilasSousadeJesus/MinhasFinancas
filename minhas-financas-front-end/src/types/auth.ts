export interface LoginPayload {
  email: string;
  senha: string;
}

export interface CadastroPayload {
  nome: string;
  email: string;
  senha: string;
  confirmacaoSenha: string;
}

export interface TokenResponse {
  token: string;
  refrenshToken: string;
}

export interface TokenPayload {
  sub: string;
  email?: string;
  name?: string;
  exp?: number;
  iat?: number;
  [key: string]: string | number | undefined;
}

export interface AuthUser {
  id: string;
  nome: string;
  email: string;
}

export interface AuthSession {
  token: string;
  refreshToken: string;
  usuario: AuthUser;
}
