using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Entities
{
    public class CompromissoFinanceiro
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public string UsuarioId { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Descricao { get; set; } = string.Empty;

        public EnumOrigemCompromissoFinanceiro Origem { get; set; }

        public EnumStatusCompromissoFinanceiro Status { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataConclusao { get; set; }

        public DateTime? DataCancelamento { get; set; }

        [MaxLength(4000)]
        public string? Observacoes { get; set; }

        public bool Ativo { get; set; } = true;

        public Usuario? Usuario { get; set; }
    }
}
