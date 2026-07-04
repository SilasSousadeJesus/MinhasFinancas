using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class CadastrarLancamentoDTO
    {
        public decimal Valor { get; set; } = decimal.Zero;
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public DateTime DataPagamento { get; set; }
        public DateTime DataLancamento { get; set; }
        public bool Realizado { get; set; }
        public EnumTipoFrequenciaLancamento FrequenciaLancamento { get; set; }
        public int? QuantidadeParcelas { get; set; }
        public int? NumeroDiaUtil { get; set; }
        public EnumTipoLancamento Tipo { get; set; }
        public EnumVinculoLancamento Vinculo { get; set; }
        public Guid? ContaId { get; set; } = Guid.Empty;
        public Guid? CartaoId { get; set; } = Guid.Empty;
        public string? UsuarioId { get; set; }
        public Guid? CategoriaId { get; set; } = Guid.Empty;
        public Guid? SubCategoriaId { get; set; } = Guid.Empty;
    }
}
