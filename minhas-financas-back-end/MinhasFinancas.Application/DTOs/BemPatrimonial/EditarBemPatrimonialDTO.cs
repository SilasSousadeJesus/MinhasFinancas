using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Application.DTOs.BemPatrimonial
{
    public class EditarBemPatrimonialDTO
    {
        [Key]
        public Guid Id { get; set; }
        public string NomeBemPatrimonial { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public EnumBemPatrimonial Tipo { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
    }
}
