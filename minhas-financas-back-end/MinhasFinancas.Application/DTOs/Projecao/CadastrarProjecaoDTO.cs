namespace MinhasFinancas.Application.DTOs.Projecao
{
    public class CadastrarProjecaoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public DateTime? DataInicial { get; set; }
        public decimal ValorAcumuladoInicial { get; set; } = decimal.Zero;
        public decimal ValorObjetivo { get; set; } = decimal.Zero;
        public int MesesLimite { get; set; } = 60;
        public bool AtreladaADespesas { get; set; } = true;
        public string UsuarioId { get; set; } = string.Empty;
        public List<RendaProjecaoDTO> Rendas { get; set; } = new();
        public List<RendaExtraMensalProjecaoDTO> RendasExtrasMensais { get; set; } = new();
        public List<DividaManualMensalProjecaoDTO> DividasManuaisMensais { get; set; } = new();
    }
}
