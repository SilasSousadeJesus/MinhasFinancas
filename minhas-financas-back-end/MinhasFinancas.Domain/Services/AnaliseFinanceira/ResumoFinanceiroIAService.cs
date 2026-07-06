using System.Globalization;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public class ResumoFinanceiroIAService : IResumoFinanceiroIAService
    {
        public ResumoFinanceiroIA GerarResumo(
            DateTime dataReferencia,
            PainelSaudeFinanceira painelSaudeFinanceira,
            PainelInsightsFinanceiros painelInsightsFinanceiros)
        {
            var prioridades = painelInsightsFinanceiros.Prioritarios
                .Take(3)
                .Select(insight => insight.Titulo)
                .ToList();

            var destaques = painelInsightsFinanceiros.DestaquesPositivos
                .Take(2)
                .Select(insight => insight.Titulo)
                .ToList();

            return new ResumoFinanceiroIA
            {
                DataReferencia = dataReferencia,
                SaudeFinanceira = painelSaudeFinanceira.Resumo,
                Indicadores = painelSaudeFinanceira.Indicadores,
                Insights = painelInsightsFinanceiros,
                ResumoExecutivo = MontarResumoExecutivo(dataReferencia, painelSaudeFinanceira, prioridades, destaques),
                PrioridadesImediatas = prioridades,
                DestaquesPositivos = destaques
            };
        }

        private static string MontarResumoExecutivo(
            DateTime dataReferencia,
            PainelSaudeFinanceira painelSaudeFinanceira,
            IReadOnlyCollection<string> prioridades,
            IReadOnlyCollection<string> destaques)
        {
            var mesReferencia = dataReferencia.ToString("MMMM 'de' yyyy", new CultureInfo("pt-BR"));
            var resumo = $"Em {mesReferencia}, a saúde financeira está classificada como {painelSaudeFinanceira.Resumo.Classificacao}, com pontuação {painelSaudeFinanceira.Resumo.PontuacaoGeral}/100.";

            if (prioridades.Count > 0)
            {
                resumo += $" As prioridades mais imediatas são: {string.Join("; ", prioridades)}.";
            }

            if (destaques.Count > 0)
            {
                resumo += $" Entre os pontos fortes atuais, destacam-se: {string.Join("; ", destaques)}.";
            }

            return resumo;
        }
    }
}
