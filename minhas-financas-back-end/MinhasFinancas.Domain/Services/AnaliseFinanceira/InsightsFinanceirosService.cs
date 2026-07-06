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
                        "O mês atual está consumindo mais do que gera.",
                        "A economia mensal ficou negativa, o que indica fechamento do mês no vermelho.",
                        "Revise as despesas previstas do mês e identifique cortes ou receitas adicionais para reequilibrar o caixa."),

                CodigoIndicadorFinanceiro.PercentualEconomia when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Oportunidade, PrioridadeInsightFinanceiro.Media,
                        "A taxa de economia está abaixo do desejado.",
                        "A parcela da renda que realmente sobra ainda não alcançou o objetivo definido no perfil financeiro.",
                        "Use o fluxo de caixa do mês para identificar despesas ajustáveis e aproximar a sobra do percentual desejado."),

                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "A reserva de emergência ainda está abaixo do nível ideal.",
                        "Os recursos líquidos disponíveis ainda não cobrem a meta de segurança definida para o usuário.",
                        "Priorize a formação de caixa antes de assumir novos compromissos de longo prazo."),

                CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Media,
                        "A meta de reserva de emergência ainda não foi configurada.",
                        "Sem uma referência configurada, a leitura da proteção financeira fica menos precisa.",
                        "Defina no perfil financeiro a quantidade de meses e o percentual desejado de reserva."),

                CodigoIndicadorFinanceiro.ComprometimentoRenda when indicador.Status == StatusIndicadorFinanceiro.Atencao
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "Uma parcela elevada da renda já está comprometida.",
                        "As despesas previstas do mês estão consumindo mais renda do que o limite saudável definido no perfil.",
                        "Reavalie despesas recorrentes e parcelamentos para recuperar margem de decisão no mês."),

                CodigoIndicadorFinanceiro.Endividamento when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "O endividamento está exigindo atenção.",
                        "O peso dos passivos sobre a base patrimonial atual está acima da faixa ideal.",
                        "Avalie priorização de amortizações e contenção de novas dívidas até o indicador voltar ao nível desejado."),

                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual when indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Alerta, PrioridadeInsightFinanceiro.Alta,
                        "O patrimônio líquido atual ainda não é positivo.",
                        "O conjunto de passivos supera os ativos disponíveis, reduzindo a solidez patrimonial.",
                        "Use patrimônio, projeções e fluxo de caixa para estruturar uma trajetória de reversão desse quadro."),

                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo when indicador.ValorIdeal <= 0
                    => CriarInsight(indicador, TipoInsightFinanceiro.Configuracao, PrioridadeInsightFinanceiro.Baixa,
                        "Ainda não existe um patrimônio-alvo definido.",
                        "Sem uma meta patrimonial, fica mais difícil medir avanço de longo prazo.",
                        "Configure um patrimônio líquido alvo no perfil financeiro para acompanhar evolução real."),

                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo when indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico
                    => CriarInsight(indicador, TipoInsightFinanceiro.Oportunidade, PrioridadeInsightFinanceiro.Media,
                        "O patrimônio ainda está distante do objetivo definido.",
                        "O avanço rumo ao patrimônio-alvo existe, mas ainda não atingiu a faixa ideal.",
                        "Cruze patrimônio, capacidade de poupança e projeções para definir uma rota de aceleração."),

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
                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual or
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo
                    => CriarInsight(indicador, TipoInsightFinanceiro.DestaquePositivo, PrioridadeInsightFinanceiro.Baixa,
                        $"Ponto forte atual: {indicador.Nome}.",
                        "Este indicador está em nível excelente e pode ser usado como base para consolidar a saúde financeira.",
                        "Mantenha a disciplina atual e concentre esforço nos demais indicadores que ainda pedem atenção."),
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
