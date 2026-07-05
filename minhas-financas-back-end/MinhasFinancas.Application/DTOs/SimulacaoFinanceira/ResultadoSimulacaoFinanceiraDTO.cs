namespace MinhasFinancas.Application.DTOs.SimulacaoFinanceira
{
    public class ResultadoSimulacaoFinanceiraDTO
    {
        public List<LinhaResultadoSimulacaoFinanceiraDTO> Linhas { get; set; } = new();
        public decimal TotalReceitasReais { get; set; }
        public decimal TotalDespesasReais { get; set; }
        public decimal SaldoRealAcumulado { get; set; }
        public decimal TotalReceitasSimuladas { get; set; }
        public decimal TotalDespesasSimuladas { get; set; }
        public decimal SaldoSimuladoAcumulado { get; set; }
        public decimal DiferencaAcumulada { get; set; }
    }
}
