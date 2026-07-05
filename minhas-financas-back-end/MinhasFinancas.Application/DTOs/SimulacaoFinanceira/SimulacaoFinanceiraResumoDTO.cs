namespace MinhasFinancas.Application.DTOs.SimulacaoFinanceira
{
    public class SimulacaoFinanceiraResumoDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataInicial { get; set; }
        public int QuantidadeMeses { get; set; }
        public bool Ativa { get; set; }
        public int QuantidadeAcoes { get; set; }
        public ResultadoSimulacaoFinanceiraDTO? ResultadoAtual { get; set; }
    }
}
