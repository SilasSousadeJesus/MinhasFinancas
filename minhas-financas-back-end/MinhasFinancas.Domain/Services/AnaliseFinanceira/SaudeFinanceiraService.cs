using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public class SaudeFinanceiraService : ISaudeFinanceiraService
    {
        private static readonly IReadOnlyDictionary<CodigoIndicadorFinanceiro, decimal> PesosIndicadores = new Dictionary<CodigoIndicadorFinanceiro, decimal>
        {
            { CodigoIndicadorFinanceiro.EconomiaMensal, 1.0m },
            { CodigoIndicadorFinanceiro.PercentualEconomia, 1.0m },
            { CodigoIndicadorFinanceiro.ReservaEmergenciaAtual, 1.5m },
            { CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal, 0.5m },
            { CodigoIndicadorFinanceiro.ComprometimentoRenda, 1.5m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo, 1.5m },
            { CodigoIndicadorFinanceiro.Endividamento, 1.5m },
            { CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual, 1.25m },
            { CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo, 0.75m },
        };

        public PainelSaudeFinanceira GerarPainel(PainelIndicadoresFinanceiros indicadores)
        {
            var lista = indicadores.Todos;
            var indicadoresPontuados = lista
                .Where(indicador => PesosIndicadores.ContainsKey(indicador.Codigo))
                .ToList();

            var somaPesos = indicadoresPontuados.Sum(indicador => PesosIndicadores[indicador.Codigo]);
            var pontuacao = indicadoresPontuados.Count == 0 || somaPesos <= 0
                ? 0
                : (int)Math.Round(indicadoresPontuados.Sum(indicador => ObterPontuacao(indicador.Status) * PesosIndicadores[indicador.Codigo]) / somaPesos);

            return new PainelSaudeFinanceira
            {
                Resumo = new ResumoSaudeFinanceira
                {
                    PontuacaoGeral = pontuacao,
                    Classificacao = ObterClassificacao(pontuacao),
                    PontosAtencao = lista
                        .Where(indicador => indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico)
                        .OrderByDescending(indicador => (int)indicador.Status)
                        .Take(3)
                        .Select(indicador => new PontoAtencaoSaudeFinanceira
                        {
                            Nome = indicador.Nome,
                            Status = indicador.Status,
                            Descricao = indicador.Descricao,
                            Observacao = indicador.Observacao
                        })
                        .ToList()
                },
                Indicadores = indicadores
            };
        }

        private static decimal ObterPontuacao(StatusIndicadorFinanceiro status)
        {
            return status switch
            {
                StatusIndicadorFinanceiro.Excelente => 100m,
                StatusIndicadorFinanceiro.Bom => 80m,
                StatusIndicadorFinanceiro.Atencao => 55m,
                _ => 25m,
            };
        }

        private static string ObterClassificacao(int pontuacao)
        {
            if (pontuacao >= 85)
            {
                return "Excelente";
            }

            if (pontuacao >= 70)
            {
                return "Boa";
            }

            if (pontuacao >= 50)
            {
                return "Atenção";
            }

            return "Crítica";
        }
    }
}
