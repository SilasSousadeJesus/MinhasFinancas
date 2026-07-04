using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class EditarLancamentoDTO
    {
        public Guid Id { get; set; }
        public decimal Valor { get; set; } = decimal.Zero;
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public DateTime DataPagamento { get; set; }
        public DateTime DataLancamento { get; set; }
        public Guid? GrupoParcelamentoId { get; set; }
        public int? NumeroParcela { get; set; }
        public int? TotalParcelas { get; set; }
        public Guid? GrupoLancamentoProgramadoId { get; set; }
        public EnumTipoProgramacaoLancamento? TipoProgramacao { get; set; }
        public int? NumeroDiaUtil { get; set; }
        public bool Realizado { get; set; }
        public EnumTipoFrequenciaLancamento FrequenciaLancamento { get; set; }
        public EnumTipoLancamento Tipo { get; set; }
        public EnumVinculoLancamento Vinculo { get; set; }
        public Guid? ContaId { get; set; }
        public virtual Conta? Conta { get; set; }
        public Guid? CartaoId { get; set; }
        public string? UsuarioId { get; set; }
        public Guid? CategoriaId { get; set; }
        public Guid? SubCategoriaId { get; set; }
    }
}
