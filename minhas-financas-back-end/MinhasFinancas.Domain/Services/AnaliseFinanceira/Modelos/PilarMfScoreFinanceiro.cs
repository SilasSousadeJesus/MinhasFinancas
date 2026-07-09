using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class PilarMfScoreFinanceiro
    {
        public CodigoPilarMfScoreFinanceiro Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Peso { get; set; }
        public int Nota { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public List<string> Indicadores { get; set; } = [];
    }
}
