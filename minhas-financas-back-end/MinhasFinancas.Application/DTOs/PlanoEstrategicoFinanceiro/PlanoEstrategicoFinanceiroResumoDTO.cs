namespace MinhasFinancas.Application.DTOs.PlanoEstrategicoFinanceiro
{
    public class PlanoEstrategicoFinanceiroResumoDTO
    {
        public Guid Id { get; set; }
        public Guid PlanoRaizId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int NumeroVersao { get; set; }
        public DateTime DataInicioVigencia { get; set; }
        public DateTime? DataFimVigencia { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
        public int QuantidadeObjetivos { get; set; }
    }
}
