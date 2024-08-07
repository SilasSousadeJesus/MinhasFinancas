using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Domain.Entities
{
    public class PermanenciaPassivo
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DataPermanencia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        [ForeignKey("PassivoId")]
        public Guid PassivoId { get; set; }
    }
}
