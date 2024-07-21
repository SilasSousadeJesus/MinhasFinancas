using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class EditarLancamento
    {
        public Guid Id { get; set; }
        public decimal Valor { get; set; } = decimal.Zero;
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public DateTime DataPagamento { get; set; }
        public bool Realizado { get; set; }
        public EnumTipoFrequenciaLancamento FrequenciaLancamento { get; set; }
        public EnumTipoLancamento Tipo { get; set; }
        public EnumOrigemLancamento Origem { get; set; }
        public Guid IdentificaoOrigem { get; set; }
        public string? UsuarioId { get; set; }
        public Guid CategoriaId { get; set; }
        public Guid SubCategoriaId { get; set; }
    }
}
