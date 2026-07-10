namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class AvaliacaoInadimplenciaMfScoreFinanceiro
    {
        public bool PossuiInadimplencia { get; set; }
        public int Nivel { get; set; }
        public int DiasMaximosAtraso { get; set; }
        public decimal ValorTotalEmAtraso { get; set; }
        public decimal PercentualValorEmAtrasoSobreRenda { get; set; }
    }
}
