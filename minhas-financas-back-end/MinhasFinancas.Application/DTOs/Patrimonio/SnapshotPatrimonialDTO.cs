namespace MinhasFinancas.Application.DTOs.Patrimonio
{
    public class SnapshotPatrimonialDTO
    {
        public Guid Id { get; set; }
        public DateTime DataReferencia { get; set; }
        public decimal TotalAtivos { get; set; }
        public decimal TotalPassivos { get; set; }
        public decimal PatrimonioLiquido { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
    }
}
