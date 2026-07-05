using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class DetalheParcelamentoDTO
    {
        public Guid GrupoParcelamentoId { get; set; }
        public string DescricaoBase { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public Guid? ContaId { get; set; }
        public Guid? CartaoId { get; set; }
        public Guid? CategoriaId { get; set; }
        public Guid? SubCategoriaId { get; set; }
        public DateTime DataInicialParcelamento { get; set; }
        public int TotalParcelas { get; set; }
        public bool PossuiParcelasEfetivadas { get; set; }
        public int QuantidadeParcelasEfetivadas { get; set; }
        public EnumTipoLancamento Tipo { get; set; }
        public List<ParcelaDetalheDTO> Parcelas { get; set; } = [];
    }

    public class ParcelaDetalheDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public int NumeroParcela { get; set; }
        public int TotalParcelas { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public EnumStatusLancamento StatusLancamento { get; set; }
        public DateTime? DataEfetivacao { get; set; }
    }
}
