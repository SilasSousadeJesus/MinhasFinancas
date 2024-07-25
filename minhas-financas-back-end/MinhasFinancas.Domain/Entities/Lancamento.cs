using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class Lancamento
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; } = decimal.Zero;
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public DateTime DataPagamento { get; set; }
        public DateTime DataLancamento { get; set; }

        // Boleanos
        public bool Realizado { get; set; }
 
        // ENUM
        public EnumTipoFrequenciaLancamento FrequenciaLancamento { get; set; }
        public EnumTipoLancamento Tipo { get; set; }
        public EnumOrigemLancamento Origem { get; set; }

        // Identificação
        public Guid IdentificaoOrigem { get; set; }


        // RELACIONAMENTO
        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }

        [ForeignKey("CategoriaId")]
        public Guid? CategoriaId { get; set; }
        public virtual Categoria? Categoria { get; set; }

        [ForeignKey("SubCategoriaId")]
        public Guid? SubCategoriaId { get; set; }
        public virtual SubCategoria? SubCategoria { get; set; }

        [ForeignKey("ContaId")]
        public Guid? ContaId { get; set; }
        public virtual Conta? Conta { get; set; }

        public virtual List<LancamentoFixo>? LancamentosFixo { get; set; }
        public virtual List<LancamentoParcelado>? LancamentoParcelado { get; set; }

    }
}