using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class SnapshotPatrimonial
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime DataReferencia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAtivos { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPassivos { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PatrimonioLiquido { get; set; }

        public string Observacao { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
    }
}
