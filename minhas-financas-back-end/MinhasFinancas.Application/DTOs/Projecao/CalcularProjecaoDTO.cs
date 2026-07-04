namespace MinhasFinancas.Application.DTOs.Projecao
{
    public class CalcularProjecaoDTO
    {
        public List<RendaProjecaoDTO> Rendas { get; set; } = new();
        public decimal ValorAcumuladoInicial { get; set; } = decimal.Zero;
        public decimal ValorObjetivo { get; set; } = decimal.Zero;
        public DateTime? DataInicial { get; set; }
        public int MesesLimite { get; set; } = 60;
        public List<RendaExtraMensalProjecaoDTO> RendasExtrasMensais { get; set; } = new();
    }
}
