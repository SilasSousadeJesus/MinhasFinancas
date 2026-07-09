using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class IndicadorFinanceiro
    {
        public CodigoIndicadorFinanceiro Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal ValorAtual { get; set; }
        public decimal ValorIdeal { get; set; }
        public decimal Percentual { get; set; }
        public decimal? ValorObrigacoesPrevistas { get; set; }
        public decimal? ValorReceitaPrevista { get; set; }
        public decimal? PercentualComprometimento { get; set; }
        public StatusIndicadorFinanceiro Status { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public FormatoValorIndicadorFinanceiro Formato { get; set; }
    }
}
