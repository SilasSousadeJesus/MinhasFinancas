namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class DadosReferenciaAnaliseFinanceira
    {
        public decimal ReceitaMensalAtual { get; set; }
        public decimal DespesaMensalAtual { get; set; }
        public decimal EconomiaMensalAtual { get; set; }
        public decimal PercentualEconomiaAtual { get; set; }
        public decimal TotalAtivos { get; set; }
        public decimal TotalPassivos { get; set; }
        public decimal PatrimonioLiquidoAtual { get; set; }
        public decimal ReservaEmergenciaAtual { get; set; }
        public decimal BaseReservaEmergenciaIntegral { get; set; }
        public decimal ReservaEmergenciaIdealConfigurada { get; set; }
        public decimal CoberturaReservaEmMeses { get; set; }
        public decimal ComprometimentoRendaAtual { get; set; }
        public decimal ObrigacoesFinanceirasFuturas30Dias { get; set; }
        public decimal ObrigacoesFinanceirasFuturas90Dias { get; set; }
        public decimal ObrigacoesFinanceirasFuturas180Dias { get; set; }
        public decimal ObrigacoesFinanceirasFuturas365Dias { get; set; }
        public decimal ComprometimentoFinanceiroFuturoAtual { get; set; }
        public decimal ComprometimentoFinanceiroFuturo90DiasAtual { get; set; }
        public decimal ComprometimentoFinanceiroFuturo180DiasAtual { get; set; }
        public decimal ComprometimentoFinanceiroFuturo365DiasAtual { get; set; }
        public decimal ReceitaPrevista30Dias { get; set; }
        public decimal ReceitaPrevista90Dias { get; set; }
        public decimal ReceitaPrevista180Dias { get; set; }
        public decimal ReceitaPrevista365Dias { get; set; }
        public decimal EndividamentoAtual { get; set; }
        public decimal PatrimonioAlvo { get; set; }
        public decimal PercentualPatrimonioAlvoAtual { get; set; }
    }
}
