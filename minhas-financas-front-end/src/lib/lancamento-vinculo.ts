import { CartaoResumo, ContaResumo } from "@/types/finance";

export const SELECT_NONE = "none";
export const VINCULO_CARTAO_CREDITO = 0;
export const VINCULO_CARTAO_DEBITO = 1;
export const VINCULO_CONTA_INVESTIMENTO = 2;
export const VINCULO_CONTA = 3;
export const VINCULO_AVULSO = 4;

export function normalizarSelecaoOpcional(value: string) {
  const valorNormalizado = value?.trim();

  if (!valorNormalizado || valorNormalizado === SELECT_NONE) {
    return null;
  }

  return valorNormalizado;
}

export function resolverVinculoLancamento(
  contaId: string | null,
  cartaoId: string | null,
  contas: ContaResumo[],
  cartoes: CartaoResumo[]
) {
  if (cartaoId) {
    const cartao = cartoes.find((item) => item.id === cartaoId);
    return cartao?.tipo === 1 ? VINCULO_CARTAO_DEBITO : VINCULO_CARTAO_CREDITO;
  }

  if (contaId) {
    const conta = contas.find((item) => item.id === contaId);
    return conta?.tipo === 2 ? VINCULO_CONTA_INVESTIMENTO : VINCULO_CONTA;
  }

  return VINCULO_AVULSO;
}
