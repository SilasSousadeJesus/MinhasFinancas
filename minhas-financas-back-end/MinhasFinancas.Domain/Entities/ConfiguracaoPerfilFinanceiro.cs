using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Entities
{
    public class ConfiguracaoPerfilFinanceiro
    {
        public Guid Id { get; set; }
        public Guid PerfilFinanceiroId { get; set; }
        public DateTime DataInicioVigencia { get; set; }
        public DateTime? DataFimVigencia { get; set; }
        public decimal PercentualEconomiaMensalDesejado { get; set; }
        public decimal PercentualReservaEmergenciaDesejado { get; set; }
        public int MesesReservaEmergenciaDesejados { get; set; }
        public decimal PercentualMaximoComprometimentoRenda { get; set; }
        public decimal PercentualMaximoEndividamento { get; set; }
        public decimal PercentualMinimoInvestimento { get; set; }
        public decimal? PatrimonioLiquidoAlvo { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public EnumOrigemPerfilFinanceiro OrigemPerfilFinanceiro { get; set; }

        public virtual PerfilFinanceiro? PerfilFinanceiro { get; set; }
    }
}
