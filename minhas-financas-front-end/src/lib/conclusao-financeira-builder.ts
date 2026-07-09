import { IndicadorResumoFinanceiroIA, ResumoFinanceiroIAData } from "@/types/resumo-financeiro-ia";

const STATUS_EXCELENTE = 0;
const STATUS_BOM = 1;
const STATUS_ATENCAO = 2;
const STATUS_CRITICO = 3;

const INDICADOR_ECONOMIA_MENSAL = 0;
const INDICADOR_PERCENTUAL_ECONOMIA = 1;
const INDICADOR_RESERVA_EMERGENCIA_ATUAL = 2;
const INDICADOR_RESERVA_EMERGENCIA_IDEAL = 3;
const INDICADOR_COMPROMETIMENTO_RENDA = 4;
const INDICADOR_ENDIVIDAMENTO = 5;
const INDICADOR_PATRIMONIO_LIQUIDO_ATUAL = 6;
const INDICADOR_PERCENTUAL_PATRIMONIO_ALVO = 7;
const INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO = 8;
const INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_90 = 9;
const INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_180 = 10;
const INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_365 = 11;

const ORDEM_PRIORIDADE_ATENCAO = [
  INDICADOR_RESERVA_EMERGENCIA_ATUAL,
  INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO,
  INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_90,
  INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_180,
  INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_365,
  INDICADOR_ENDIVIDAMENTO,
  INDICADOR_COMPROMETIMENTO_RENDA,
  INDICADOR_PERCENTUAL_PATRIMONIO_ALVO,
  INDICADOR_PATRIMONIO_LIQUIDO_ATUAL,
  INDICADOR_PERCENTUAL_ECONOMIA,
  INDICADOR_ECONOMIA_MENSAL,
  INDICADOR_RESERVA_EMERGENCIA_IDEAL,
];

const ORDEM_PRIORIDADE_FORCA = [
  INDICADOR_PATRIMONIO_LIQUIDO_ATUAL,
  INDICADOR_PERCENTUAL_ECONOMIA,
  INDICADOR_ECONOMIA_MENSAL,
  INDICADOR_RESERVA_EMERGENCIA_ATUAL,
  INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO,
  INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_90,
  INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_180,
  INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_365,
  INDICADOR_ENDIVIDAMENTO,
  INDICADOR_COMPROMETIMENTO_RENDA,
  INDICADOR_PERCENTUAL_PATRIMONIO_ALVO,
  INDICADOR_RESERVA_EMERGENCIA_IDEAL,
];

function buscarIndicadorPorOrdem(
  indicadores: IndicadorResumoFinanceiroIA[],
  ordem: number[],
  statusPermitidos: number[]
) {
  for (const codigo of ordem) {
    const indicador = indicadores.find(
      (item) => item.codigo === codigo && statusPermitidos.includes(item.status)
    );

    if (indicador) {
      return indicador;
    }
  }

  return indicadores.find((item) => statusPermitidos.includes(item.status)) ?? null;
}

function obterAbertura(classificacao?: string) {
  switch (classificacao) {
    case "Excelente":
      return "Sua vida financeira apresenta um cenário bastante sólido neste momento.";
    case "Muito Bom":
      return "Sua situação financeira mostra uma base muito consistente e com boa margem de proteção.";
    case "Boa":
      return "Sua situação financeira mostra sinais consistentes de equilíbrio.";
    case "Crítica":
      return "O momento financeiro exige cautela e reorganização das prioridades.";
    case "Muito Crítico":
      return "O momento financeiro exige intervenção imediata e reorganização profunda das prioridades.";
    default:
      return "Sua situação financeira ainda exige alguns ajustes importantes.";
  }
}

function interpretarPontoForte(indicador: IndicadorResumoFinanceiroIA | null) {
  if (!indicador) {
    return "uma base financeira estável, ainda com espaço para evoluir";
  }

  switch (indicador.codigo) {
    case INDICADOR_PATRIMONIO_LIQUIDO_ATUAL:
      return "o crescimento consistente do patrimônio, sinalizando avanço na construção de longo prazo";
    case INDICADOR_PERCENTUAL_ECONOMIA:
      return "a disciplina de economia, que mostra boa capacidade de transformar renda em progresso";
    case INDICADOR_ECONOMIA_MENSAL:
      return "a geração de sobra mensal, que sustenta o planejamento com mais previsibilidade";
    case INDICADOR_RESERVA_EMERGENCIA_ATUAL:
    case INDICADOR_RESERVA_EMERGENCIA_IDEAL:
      return "a reserva de emergência, que já oferece uma proteção mais confiável contra imprevistos";
    case INDICADOR_ENDIVIDAMENTO:
      return "o endividamento patrimonial sob controle, que preserva margem para decisões futuras";
    case INDICADOR_COMPROMETIMENTO_RENDA:
      return "a boa folga no orçamento, que reduz a pressão sobre a renda mensal";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO:
      return "o comprometimento financeiro futuro dos próximos 30 dias, que ainda cabe com conforto dentro da renda prevista";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_90:
      return "a pressão financeira acumulada dos próximos 90 dias, que ainda permanece administrável";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_180:
      return "a pressão financeira acumulada dos próximos 180 dias, que ainda se mantém dentro de uma faixa sustentável";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_365:
      return "a pressão financeira acumulada de longo prazo, que continua compatível com a renda projetada";
    case INDICADOR_PERCENTUAL_PATRIMONIO_ALVO:
      return "a proximidade gradual do patrimônio em relação ao objetivo planejado";
    default:
      return "um desempenho equilibrado nos principais indicadores";
  }
}

function interpretarPontoAtencao(indicador: IndicadorResumoFinanceiroIA | null) {
  if (!indicador) {
    return "não há um risco dominante pressionando a leitura atual";
  }

  switch (indicador.codigo) {
    case INDICADOR_RESERVA_EMERGENCIA_ATUAL:
    case INDICADOR_RESERVA_EMERGENCIA_IDEAL:
      return "a reserva de emergência ainda oferece pouca proteção contra imprevistos";
    case INDICADOR_PERCENTUAL_PATRIMONIO_ALVO:
      return "o patrimônio ainda está abaixo do objetivo planejado";
    case INDICADOR_ENDIVIDAMENTO:
      return "o endividamento patrimonial ainda reduz sua capacidade de crescimento financeiro";
    case INDICADOR_COMPROMETIMENTO_RENDA:
      return "o comprometimento atual da renda limita a folga necessária para avançar com mais segurança";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO:
      return "o comprometimento financeiro futuro dos próximos 30 dias já começa a reduzir a folga disponível no curto prazo";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_90:
      return "a pressão financeira acumulada dos próximos 90 dias já pede atenção para não reduzir a flexibilidade do trimestre";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_180:
      return "a pressão financeira acumulada dos próximos 180 dias já exige organização para não comprometer o ritmo financeiro";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_365:
      return "a pressão financeira acumulada de longo prazo ainda precisa de acompanhamento para não limitar os objetivos futuros";
    case INDICADOR_PERCENTUAL_ECONOMIA:
      return "a taxa de economia ainda está abaixo do ritmo desejado para sustentar sua evolução";
    case INDICADOR_ECONOMIA_MENSAL:
      return "a sobra mensal segue frágil e pressiona a estabilidade do curto prazo";
    case INDICADOR_PATRIMONIO_LIQUIDO_ATUAL:
      return "a base patrimonial ainda está em fase inicial de consolidação";
    default:
      return "há um ponto de atenção relevante que ainda pede correção";
  }
}

function interpretarDirecao(indicador: IndicadorResumoFinanceiroIA | null, classificacao?: string) {
  if (!indicador) {
    if (classificacao === "Excelente" || classificacao === "Muito Bom" || classificacao === "Boa") {
      return "preservar a disciplina atual e transformar estabilidade em crescimento consistente";
    }

    return "reorganizar os principais indicadores para retomar uma trajetória mais equilibrada";
  }

  switch (indicador.codigo) {
    case INDICADOR_RESERVA_EMERGENCIA_ATUAL:
    case INDICADOR_RESERVA_EMERGENCIA_IDEAL:
      return "reforçar a reserva de emergência até alcançar um nível mais confortável de proteção";
    case INDICADOR_ENDIVIDAMENTO:
      return "reduzir o peso dos passivos patrimoniais e recuperar margem para evolução de longo prazo";
    case INDICADOR_COMPROMETIMENTO_RENDA:
      return "diminuir o comprometimento da renda para restabelecer folga no orçamento";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO:
      return "rever o comprometimento financeiro futuro dos próximos 30 dias para aliviar a pressão do caixa futuro";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_90:
      return "organizar a pressão financeira acumulada dos próximos 90 dias para manter equilíbrio entre previsibilidade e flexibilidade";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_180:
      return "planejar a pressão financeira acumulada dos próximos 180 dias para evitar acúmulo de peso";
    case INDICADOR_COMPROMETIMENTO_FINANCEIRO_FUTURO_365:
      return "rever a pressão financeira acumulada de longo prazo para não comprometer decisões estratégicas";
    case INDICADOR_PERCENTUAL_PATRIMONIO_ALVO:
      return "aproximar o patrimônio do objetivo definido com constância nos aportes";
    case INDICADOR_PATRIMONIO_LIQUIDO_ATUAL:
      return "fortalecer a formação patrimonial com visão mais consistente de longo prazo";
    case INDICADOR_PERCENTUAL_ECONOMIA:
    case INDICADOR_ECONOMIA_MENSAL:
      return "ampliar a capacidade de poupança para sustentar crescimento financeiro ao longo dos próximos meses";
    default:
      return "corrigir os principais desvios antes de ampliar novos compromissos";
  }
}

export class ConclusaoFinanceiraBuilder {
  static construir(resumo: ResumoFinanceiroIAData, indicadores: IndicadorResumoFinanceiroIA[]) {
    const mfScore = resumo.saudeFinanceira.mfScore;
    const pontuacao = mfScore?.pontuacaoFinal ?? resumo.saudeFinanceira.pontuacaoGeral ?? 0;
    const classificacao = mfScore?.classificacao ?? resumo.saudeFinanceira.classificacao ?? "Atenção";
    const risco = mfScore?.risco ?? "Risco não informado";

    const principalPontoForte = buscarIndicadorPorOrdem(
      indicadores,
      ORDEM_PRIORIDADE_FORCA,
      [STATUS_EXCELENTE, STATUS_BOM]
    );

    const principalPontoAtencao = buscarIndicadorPorOrdem(
      indicadores,
      ORDEM_PRIORIDADE_ATENCAO,
      [STATUS_CRITICO, STATUS_ATENCAO]
    );

    const abertura = obterAbertura(classificacao);
    const pontoForte = interpretarPontoForte(principalPontoForte);
    const pontoAtencao = interpretarPontoAtencao(principalPontoAtencao);
    const direcao = interpretarDirecao(principalPontoAtencao, classificacao);

    return `${abertura} Com MF Score de ${pontuacao}/100, classificação ${classificacao} e ${risco.toLowerCase()}, o quadro atual combina ${pontoForte}. O principal fator de pressão hoje é ${pontoAtencao}. Para o próximo ciclo, a direção mais prudente é ${direcao}.`;
  }
}
