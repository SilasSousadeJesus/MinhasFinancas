namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class PainelIndicadoresFinanceiros
    {
        public IndicadorFinanceiro EconomiaMensal { get; set; } = new();
        public IndicadorFinanceiro PercentualEconomia { get; set; } = new();
        public IndicadorFinanceiro ReservaEmergenciaAtual { get; set; } = new();
        public IndicadorFinanceiro ReservaEmergenciaIdeal { get; set; } = new();
        public IndicadorFinanceiro ComprometimentoRenda { get; set; } = new();
        public IndicadorFinanceiro ComprometimentoFinanceiroFuturo { get; set; } = new();
        public IndicadorFinanceiro ComprometimentoFinanceiroFuturo90Dias { get; set; } = new();
        public IndicadorFinanceiro ComprometimentoFinanceiroFuturo180Dias { get; set; } = new();
        public IndicadorFinanceiro ComprometimentoFinanceiroFuturo365Dias { get; set; } = new();
        public IndicadorFinanceiro Endividamento { get; set; } = new();
        public IndicadorFinanceiro PatrimonioLiquidoAtual { get; set; } = new();
        public IndicadorFinanceiro PercentualPatrimonioAlvo { get; set; } = new();
        public List<IndicadorFinanceiro> Todos { get; set; } = [];
    }
}
