namespace MinhasFinancas.Application.DTOs.SimulacaoFinanceira
{
    public class LinhaResultadoSimulacaoFinanceiraDTO
    {
        public string MesReferencia { get; set; } = string.Empty;
        public decimal ReceitasReais { get; set; }
        public decimal DespesasReais { get; set; }
        public decimal SaldoReal { get; set; }
        public decimal ReceitasSimuladas { get; set; }
        public decimal DespesasSimuladas { get; set; }
        public decimal SaldoSimulado { get; set; }
        public decimal Diferenca { get; set; }
    }
}
