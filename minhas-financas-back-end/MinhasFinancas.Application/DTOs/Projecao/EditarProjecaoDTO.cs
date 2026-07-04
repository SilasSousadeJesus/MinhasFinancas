namespace MinhasFinancas.Application.DTOs.Projecao
{
    public class EditarProjecaoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime? DataInicial { get; set; }
        public decimal ValorAcumuladoInicial { get; set; } = decimal.Zero;
        public decimal ValorObjetivo { get; set; } = decimal.Zero;
        public int MesesLimite { get; set; } = 60;
        public List<RendaProjecaoDTO> Rendas { get; set; } = new();
        public List<RendaExtraMensalProjecaoDTO> RendasExtrasMensais { get; set; } = new();
    }
}
