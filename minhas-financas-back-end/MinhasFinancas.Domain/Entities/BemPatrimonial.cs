using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class BemPatrimonial
    {

        [Key]
        public Guid Id { get; set; }
        public string NomeBemPatrimonial { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public EnumBemPatrimonial Tipo { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
