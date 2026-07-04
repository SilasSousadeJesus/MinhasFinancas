namespace MinhasFinancas.Application.DTOs.Projecao
{
    public class DetalheProjecaoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataInicial { get; set; }
        public decimal ValorAcumuladoInicial { get; set; }
        public decimal ValorObjetivo { get; set; }
        public int MesesLimite { get; set; }
        public bool AtreladaADespesas { get; set; }
        public List<RendaProjecaoDTO> Rendas { get; set; } = new();
        public List<RendaExtraMensalProjecaoDTO> RendasExtrasMensais { get; set; } = new();
        public List<DividaManualMensalProjecaoDTO> DividasManuaisMensais { get; set; } = new();
        public ResultadoProjecaoDTO? ResultadoAtual { get; set; }
    }
}
