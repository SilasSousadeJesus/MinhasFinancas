export interface RetornoGenerico<T = unknown> {
  sucesso: boolean;
  mensagemSistema: string;
  mensagemUsuario: string;
  httpStatusCode: number | string;
  dados: T | null;
}

export class ApiError extends Error {
  status: number;
  response?: RetornoGenerico;

  constructor(message: string, status = 500, response?: RetornoGenerico) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.response = response;
  }
}
