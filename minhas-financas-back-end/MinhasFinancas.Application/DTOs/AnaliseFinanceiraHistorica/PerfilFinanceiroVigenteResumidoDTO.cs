namespace MinhasFinancas.Application.DTOs.AnaliseFinanceiraHistorica
{
    public class PerfilFinanceiroVigenteResumidoDTO
    {
        public decimal PercentualEconomiaMensalDesejado { get; set; }
        public decimal PercentualReservaEmergenciaDesejado { get; set; }
        public int MesesReservaEmergenciaDesejados { get; set; }
        public decimal PercentualMaximoComprometimentoRenda { get; set; }
        public decimal PercentualMaximoEndividamento { get; set; }
        public decimal PercentualMinimoInvestimento { get; set; }
        public decimal? PatrimonioLiquidoAlvo { get; set; }
    }
}
