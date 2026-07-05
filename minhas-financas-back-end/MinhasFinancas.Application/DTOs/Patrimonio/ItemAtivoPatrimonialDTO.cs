namespace MinhasFinancas.Application.DTOs.Patrimonio
{
    public class ItemAtivoPatrimonialDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime? DataReferenciaValor { get; set; }
        public DateTime? DataAquisicao { get; set; }
        public bool Ativo { get; set; }
    }
}
