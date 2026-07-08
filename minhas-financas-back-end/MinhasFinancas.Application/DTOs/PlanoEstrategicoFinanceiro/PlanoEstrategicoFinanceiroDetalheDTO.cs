namespace MinhasFinancas.Application.DTOs.PlanoEstrategicoFinanceiro
{
    public class PlanoEstrategicoFinanceiroDetalheDTO
    {
        public Guid Id { get; set; }
        public Guid PlanoRaizId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? Observacao { get; set; }
        public int NumeroVersao { get; set; }
        public DateTime DataInicioVigencia { get; set; }
        public DateTime? DataFimVigencia { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
        public List<ObjetivoPlanoEstrategicoDTO> Objetivos { get; set; } = [];
    }
}
