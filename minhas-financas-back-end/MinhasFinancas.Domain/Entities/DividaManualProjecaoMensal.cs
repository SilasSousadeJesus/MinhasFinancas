using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class DividaManualProjecaoMensal
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime MesReferencia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; } = decimal.Zero;

        [ForeignKey("ProjecaoId")]
        public Guid ProjecaoId { get; set; }
        public Projecao? Projecao { get; set; }
    }
}
