using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public class SaudeFinanceiraService : ISaudeFinanceiraService
    {
        public PainelSaudeFinanceira GerarPainel(PainelIndicadoresFinanceiros indicadores)
        {
            var lista = indicadores.Todos;
            var pontuacao = lista.Count == 0
                ? 0
                : (int)Math.Round(lista.Average(indicador => ObterPontuacao(indicador.Status)));

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
