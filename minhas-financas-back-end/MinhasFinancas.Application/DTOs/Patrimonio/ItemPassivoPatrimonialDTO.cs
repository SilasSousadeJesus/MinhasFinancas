namespace MinhasFinancas.Application.DTOs.Patrimonio
{
    public class ItemPassivoPatrimonialDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime? DataReferenciaValor { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }
    }
}
