import { IndicadorResumoFinanceiroIA } from "@/types/resumo-financeiro-ia";

const STATUS_EXCELENTE = 0;
const STATUS_BOM = 1;
const STATUS_ATENCAO = 2;

const INDICADOR_ECONOMIA_MENSAL = 0;
const INDICADOR_PERCENTUAL_ECONOMIA = 1;
const INDICADOR_RESERVA_EMERGENCIA_ATUAL = 2;
const INDICADOR_RESERVA_EMERGENCIA_IDEAL = 3;
const INDICADOR_COMPROMETIMENTO_RENDA = 4;
const INDICADOR_ENDIVIDAMENTO = 5;
const INDICADOR_PATRIMONIO_LIQUIDO_ATUAL = 6;
const INDICADOR_PERCENTUAL_PATRIMONIO_ALVO = 7;
const INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO = 8;

function estaPositivo(status: number) {
  return status === STATUS_EXCELENTE || status === STATUS_BOM;
}

function estaEmAtencao(status: number) {
  return status === STATUS_ATENCAO;
}

export function obterTextoExecutivoIndicador(indicador: IndicadorResumoFinanceiroIA) {
  switch (indicador.codigo) {
    case INDICADOR_ECONOMIA_MENSAL:
      if (estaPositivo(indicador.status)) {
        return "O fluxo do mês ainda preserva sobra financeira e ajuda a sustentar o planejamento de curto prazo.";
      }

      return "A folga do mês está apertada e reduz a margem para absorver imprevistos com tranquilidade.";

    case INDICADOR_PERCENTUAL_ECONOMIA:
      if (estaPositivo(indicador.status)) {
        return "Uma parcela saudável da renda está sendo convertida em avanço financeiro concreto.";
      }

      return "A capacidade de transformar renda em economia ainda está abaixo do ritmo desejado.";

    case INDICADOR_RESERVA_EMERGENCIA_ATUAL:
      if (estaPositivo(indicador.status)) {
        return "A reserva atual já oferece uma base mais segura para lidar com oscilações e imprevistos.";
      }

      return "A proteção disponível ainda é limitada para atravessar períodos de pressão sem comprometer o restante do plano.";

    case INDICADOR_RESERVA_EMERGENCIA_IDEAL:
      if (indicador.valorIdeal <= 0) {
        return "Ainda não existe uma meta configurada para medir com clareza o nível ideal de proteção financeira.";
      }

      if (estaPositivo(indicador.status)) {
        return "A meta de proteção financeira está coerente com o perfil atual e serve bem como referência.";
      }

      return "A distância em relação à reserva ideal ainda indica necessidade de reforço de liquidez.";

    case INDICADOR_COMPROMETIMENTO_RENDA:
      if (estaPositivo(indicador.status)) {
        return "O orçamento mensal mantém boa folga e preserva espaço para escolhas futuras.";
      }

      if (estaEmAtencao(indicador.status)) {
        return "Uma parcela relevante da renda já está comprometida e reduz a flexibilidade do mês.";
      }

      return "O orçamento está bastante pressionado e exige reorganização para recuperar margem de decisão.";

    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO:
      if (estaPositivo(indicador.status)) {
        return "Os compromissos dos próximos 30 dias ainda cabem com conforto dentro da renda prevista.";
      }

      if (estaEmAtencao(indicador.status)) {
        return "Os compromissos dos próximos 30 dias já começam a reduzir a folga disponível no curto prazo.";
      }

      return "Os compromissos dos próximos 30 dias estão pressionando o caixa futuro e pedem revisão.";

    case INDICADOR_ENDIVIDAMENTO:
      if (estaPositivo(indicador.status)) {
        return "O endividamento permanece controlado e não interfere de forma relevante na evolução patrimonial.";
      }

      if (estaEmAtencao(indicador.status)) {
        return "As dívidas já começam a limitar a capacidade de crescimento e pedem acompanhamento mais próximo.";
      }

      return "O nível atual de endividamento pesa sobre a estrutura financeira e reduz a liberdade para avançar.";

    case INDICADOR_PATRIMONIO_LIQUIDO_ATUAL:
      if (estaPositivo(indicador.status)) {
        return "A base patrimonial já demonstra consistência e fortalece a estabilidade de longo prazo.";
      }

      return "A construção patrimonial ainda está em fase inicial e precisa de constância para ganhar solidez.";

    case INDICADOR_PERCENTUAL_PATRIMONIO_ALVO:
      if (indicador.valorIdeal <= 0) {
        return "Ainda falta definir uma referência patrimonial que permita medir avanço de longo prazo com mais clareza.";
      }

      if (estaPositivo(indicador.status)) {
        return "A trajetória patrimonial segue compatível com o objetivo definido para o longo prazo.";
      }

      return "O patrimônio ainda está distante do objetivo planejado e exige continuidade nos aportes.";

    default:
      return indicador.observacao;
  }
}

export function obterTextoPontoAtencao(indicador: IndicadorResumoFinanceiroIA) {
  switch (indicador.codigo) {
    case INDICADOR_ECONOMIA_MENSAL:
      return "A sobra mensal ainda não oferece o conforto necessário para sustentar o planejamento com segurança.";
    case INDICADOR_PERCENTUAL_ECONOMIA:
      return "A taxa de economia ainda está abaixo do ritmo necessário para acelerar a evolução financeira.";
    case INDICADOR_RESERVA_EMERGENCIA_ATUAL:
    case INDICADOR_RESERVA_EMERGENCIA_IDEAL:
      return "A reserva de emergência ainda oferece pouca proteção diante de imprevistos ou oscilações de renda.";
    case INDICADOR_COMPROMETIMENTO_RENDA:
      return "O comprometimento da renda está reduzindo a margem disponível para ajustes e novas decisões.";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO:
      return "Os compromissos futuros já começam a pressionar a flexibilidade do caixa.";
    case INDICADOR_ENDIVIDAMENTO:
      return "O peso atual das dívidas limita a capacidade de crescimento financeiro e aumenta a pressão futura.";
    case INDICADOR_PATRIMONIO_LIQUIDO_ATUAL:
      return "A base patrimonial ainda precisa amadurecer para oferecer mais solidez ao plano financeiro.";
    case INDICADOR_PERCENTUAL_PATRIMONIO_ALVO:
      return "O patrimônio permanece abaixo do objetivo definido e ainda exige construção consistente.";
    default:
      return indicador.descricao;
  }
}
