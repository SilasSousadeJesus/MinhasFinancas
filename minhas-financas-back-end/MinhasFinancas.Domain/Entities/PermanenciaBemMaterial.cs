using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Domain.Entities
{
    public class PermanenciaBemMaterial
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DataPermanencia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [ForeignKey("BemPatrimonialId")]
        public Guid BemPatrimonialId { get; set; }

    }
}
