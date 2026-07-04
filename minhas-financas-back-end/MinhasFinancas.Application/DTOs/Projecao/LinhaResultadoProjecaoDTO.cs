namespace MinhasFinancas.Application.DTOs.Projecao
{
    public class LinhaResultadoProjecaoDTO
    {
        public string MesReferencia { get; set; } = string.Empty;
        public decimal DividasTotais { get; set; } = decimal.Zero;
        public decimal RendaExtraMensal { get; set; } = decimal.Zero;
        public decimal RendaManualTotal { get; set; } = decimal.Zero;
        public decimal ReceitaTotalMes { get; set; } = decimal.Zero;
        public decimal SobraDoMes { get; set; } = decimal.Zero;
        public decimal AcumuladoProjetado { get; set; } = decimal.Zero;
        public bool ObjetivoAtingidoNoMes { get; set; }
    }
}
