namespace MinhasFinancas.Application.DTOs.SimulacaoFinanceira
{
    public class CadastrarSimulacaoFinanceiraDTO
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataInicial { get; set; }
        public int QuantidadeMeses { get; set; } = 12;
        public List<AcaoSimulacaoFinanceiraDTO> Acoes { get; set; } = new();
    }
}
