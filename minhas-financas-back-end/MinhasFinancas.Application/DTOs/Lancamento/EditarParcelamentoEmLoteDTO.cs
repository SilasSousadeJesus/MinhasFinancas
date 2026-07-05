namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class EditarParcelamentoEmLoteDTO
    {
        public string DescricaoBase { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public Guid? ContaId { get; set; }
        public Guid? CartaoId { get; set; }
        public Guid? CategoriaId { get; set; }
        public Guid? SubCategoriaId { get; set; }
        public DateTime DataInicialParcelamento { get; set; }
        public bool AlterarParcelasEfetivadas { get; set; }
    }
}
