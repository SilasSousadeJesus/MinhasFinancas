using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Application.DTOs.Passivo
{
    public class EditarPassivoDTO
    {
        public Guid Id { get; set; }
        public string NomeBemPatrimonial { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public EnumBemPatrimonial Tipo { get; set; }
        public string? UsuarioId { get; set; }
    }
}
