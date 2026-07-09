using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public class InsightsFinanceirosService : IInsightsFinanceirosService
    {
        public PainelInsightsFinanceiros GerarPainel(PainelSaudeFinanceira painelSaudeFinanceira)
        {
            var insights = painelSaudeFinanceira.Indicadores.Todos
                .Select(CriarInsightPrincipal)
                .Where(insight => insight is not null)
                .Cast<InsightFinanceiro>()
                .ToList();

            var destaquesPositivos = painelSaudeFinanceira.Indicadores.Todos
                .Where(indicador => indicador.Status == StatusIndicadorFinanceiro.Excelente)
                .Select(CriarDestaquePositivo)
                .Where(insight => insight is not null)
                .Cast<InsightFinanceiro>()
                .Take(3)
                .ToList();

            insights.AddRange(destaquesPositivos);

            var ordenados = insights
                .OrderBy(insight => insight.Prioridade)
                .ThenBy(insight => insight.Tipo)
                .ThenBy(insight => insight.Titulo)
                .ToList();

            return new PainelInsightsFinanceiros
            {
                Todos = ordenados,
                Prioritarios = ordenados
                    .Where(insight => insight.Tipo != TipoInsightFinanceiro.DestaquePositivo)
                    .Take(5)
                    .ToList(),
                DestaquesPositivos = destaquesPositivos
            };
        }

        private static InsightFinanceiro? CriarInsightPrincipal(IndicadorFinanceiro indicador)
        {
            return indicador.Codigo switch
            {
                CodigoIndicadorFinanceiro.EconomiaMensal when indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "O mês está fechando sem folga financeira.",
                        "No ritmo atual, as saídas do período já pressionam o caixa e reduzem a margem para decisões de curto prazo.",
                        "Reorganize despesas imediatas e busque recuperar sobra antes do fechamento do próximo ciclo."),

                CodigoIndicadorFinanceiro.PercentualEconomia when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Oportunidade, PrioridadeInsightFinanceiro.Media,
                        "A capacidade de poupança ainda pode evoluir.",
                        "A renda ainda não está se convertendo em economia no ritmo necessário para acelerar sua construção financeira.",
                        "Identifique gastos ajustáveis e direcione uma parcela maior da renda para formação de reserva ou patrimônio."),

                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "A proteção contra imprevistos ainda é limitada.",
                        "A reserva disponível ainda não oferece a segurança ideal para atravessar oscilações ou despesas inesperadas com tranquilidade.",
                        "Priorize liquidez e proteção antes de assumir compromissos mais longos ou aumentar o padrão de gasto."),

                CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Media,
                        "Ainda falta definir a régua da sua reserva.",
                        "Sem uma meta clara de proteção, a leitura do nível de segurança financeira perde precisão.",
                        "Configure no perfil financeiro a meta de reserva para orientar melhor as próximas decisões."),

                CodigoIndicadorFinanceiro.ComprometimentoRenda when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Media,
                        "Ainda falta definir o limite de comprometimento da renda.",
                        "Sem essa régua no perfil financeiro, o sistema não consegue diferenciar com precisão o que é um nível confortável e o que já representa pressão excessiva.",
                        "Defina no perfil financeiro o percentual máximo desejado para o comprometimento da renda."),

                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Media,
                        "Ainda falta definir a régua dos compromissos futuros.",
                        "Sem um limite máximo de comprometimento da renda, a leitura dos próximos 30 dias fica menos precisa.",
                        "Use o perfil financeiro para definir o percentual máximo desejado para compromissos futuros."),

                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Media,
                        "Ainda falta definir a régua da pressão financeira acumulada de 90 dias.",
                        "Sem essa referência, a leitura do médio prazo perde precisão e deixa o planejamento menos confiável.",
                        "Use o perfil financeiro para definir a referência desejada para os próximos 90 dias."),

                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Media,
                        "Ainda falta definir a régua da pressão financeira acumulada de 180 dias.",
                        "Sem essa referência, o sistema enxerga menos claramente a pressão financeira de médio prazo.",
                        "Use o perfil financeiro para definir a referência desejada para os próximos 180 dias."),

                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Media,
                        "Ainda falta definir a régua da pressão financeira acumulada de 12 meses.",
                        "Sem essa referência, a visão de longo prazo fica menos precisa para orientar o planejamento.",
                        "Use o perfil financeiro para definir a referência desejada para os próximos 12 meses."),

                CodigoIndicadorFinanceiro.Endividamento when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Media,
                        "Ainda falta definir o limite de endividamento patrimonial.",
                        "Sem essa referência no perfil financeiro, o sistema não consegue calibrar com precisão a leitura dos passivos patrimoniais.",
                        "Defina no perfil financeiro o percentual máximo desejado para o endividamento patrimonial."),

                CodigoIndicadorFinanceiro.ComprometimentoRenda when indicador.Status == StatusIndicadorFinanceiro.Atencao
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "O orçamento mensal está pressionado.",
                        "Uma parcela relevante da renda já está comprometida, o que reduz flexibilidade para reagir a imprevistos ou aproveitar oportunidades.",
                        "Revise despesas recorrentes e contratos parcelados para recuperar margem de decisão no mês."),

                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "Os próximos 30 dias já trazem compromissos relevantes.",
                        "O volume de despesas futuras em relação à renda prevista começa a reduzir a flexibilidade de caixa do curto prazo.",
                        "Reorganize os compromissos mais próximos para evitar pressão excessiva no próximo ciclo."),

                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "Os próximos 90 dias já trazem pressão financeira acumulada relevante.",
                        "A soma das obrigações pendentes nesse horizonte começa a reduzir a flexibilidade do planejamento de médio prazo.",
                        "Reorganize os compromissos do trimestre para evitar acúmulo de pressão financeira."),

                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Media,
                        "O horizonte de 180 dias já pede organização.",
                        "A pressão financeira acumulada no médio prazo indica necessidade de planejamento mais estruturado para evitar aperto futuro.",
                        "Revise o calendário de despesas e distribua melhor os compromissos ao longo dos próximos meses."),

                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Oportunidade, PrioridadeInsightFinanceiro.Media,
                        "O longo prazo já está sendo pressionado por compromissos futuros.",
                        "Mesmo em um horizonte maior, a pressão financeira acumulada pede disciplina para não comprometer flexibilidade estratégica.",
                        "Reveja compromissos de longo prazo antes que eles limitem objetivos maiores."),

                CodigoIndicadorFinanceiro.Endividamento when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "O endividamento patrimonial está limitando sua evolução.",
                        "O peso atual dos passivos já interfere na capacidade de formar patrimônio e aumenta a pressão sobre os próximos ciclos.",
                        "Considere priorizar amortizações e conter novas dívidas até restabelecer uma faixa mais saudável."),

                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual when indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "A base patrimonial ainda está fragilizada.",
                        "No cenário atual, os passivos ainda superam os ativos, o que reduz a solidez financeira e aumenta a vulnerabilidade estrutural.",
                        "Use fluxo de caixa, patrimônio e projeções para montar um plano realista de reversão desse quadro."),

                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Baixa,
                        "Ainda falta uma referência clara de patrimônio.",
                        "Sem um patrimônio-alvo definido, fica mais difícil medir avanço real e calibrar expectativas de longo prazo.",
                        "Defina um objetivo patrimonial no perfil financeiro para acompanhar evolução com mais clareza."),

                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Oportunidade, PrioridadeInsightFinanceiro.Media,
                        "Há espaço para acelerar a construção patrimonial.",
                        "O patrimônio já avança, mas ainda abaixo da velocidade necessária para atingir o objetivo definido com mais conforto.",
                        "Ajuste a combinação entre poupança, patrimônio e planejamento para aproximar a trajetória do objetivo esperado."),

                _ => null
            };
        }

        private static InsightFinanceiro? CriarDestaquePositivo(IndicadorFinanceiro indicador)
        {
            return indicador.Codigo switch
            {
                CodigoIndicadorFinanceiro.EconomiaMensal or
                CodigoIndicadorFinanceiro.PercentualEconomia or
                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual or
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo or
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias or
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias or
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias or
                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual or
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo
                    => CriarInsight(indicador, TipoInsightFinanceiro.DestaquePositivo, PrioridadeInsightFinanceiro.Baixa,
                        $"Há um avanço consistente em {indicador.Nome.ToLowerInvariant()}.",
                        "Esse indicador contribui positivamente para a estabilidade financeira atual e ajuda a sustentar os próximos passos com mais segurança.",
                        "Mantenha o padrão atual e concentre o esforço de ajuste apenas nos pontos que ainda pressionam sua estrutura financeira."),
                _ => null
            };
        }

        private static InsightFinanceiro CriarInsight(
            IndicadorFinanceiro indicador,
            TipoInsightFinanceiro tipo,
            PrioridadeInsightFinanceiro prioridade,
            string titulo,
            string descricao,
            string acaoSugerida)
        {
            return new InsightFinanceiro
            {
                CodigoIndicadorRelacionado = indicador.Codigo,
                Tipo = tipo,
                Prioridade = prioridade,
                Titulo = titulo,
                Descricao = descricao,
                AcaoSugerida = acaoSugerida
            };
        }
    }
}
