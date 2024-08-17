using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class AporteMeta
    {
        public Guid Id { get; set; }
        public DateTime DataAporte { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [ForeignKey("MetaId")]
        public Guid MetaId { get; set; }
    }
}
