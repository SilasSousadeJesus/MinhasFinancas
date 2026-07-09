using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class TendenciaMfScoreFinanceiro
    {
        public DirecaoTendenciaMfScoreFinanceiro Direcao { get; set; } = DirecaoTendenciaMfScoreFinanceiro.Indeterminada;
        public string Descricao { get; set; } = string.Empty;
        public List<int> HistoricoNotas { get; set; } = [];
    }
}
