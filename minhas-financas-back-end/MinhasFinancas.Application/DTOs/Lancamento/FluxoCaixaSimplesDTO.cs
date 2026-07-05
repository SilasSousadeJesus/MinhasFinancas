namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class FluxoCaixaSimplesDTO
    {
        public int Ano { get; set; }
        public int Mes { get; set; }
        public decimal ReceitasTotal { get; set; }
        public decimal DespesasTotal { get; set; }
        public decimal SaldoMes { get; set; }
        public List<FluxoCaixaSimplesItemDTO> Receitas { get; set; } = new();
        public List<FluxoCaixaSimplesItemDTO> Despesas { get; set; } = new();
    }
}
