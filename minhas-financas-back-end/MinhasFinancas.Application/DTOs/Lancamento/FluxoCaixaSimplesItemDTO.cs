namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class FluxoCaixaSimplesItemDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
    }
}
