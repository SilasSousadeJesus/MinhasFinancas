using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Application.DTOs.Categoria
{
    public class CadastrarCategoria
    {
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [StringLength(20, ErrorMessage = "O Campo {0} deve ter até 20 caracteres", MinimumLength = 2)]
        public string NomeCategoria { get; set; } = string.Empty;
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public string Icone { get; set; } = string.Empty;
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public EnumTipoCategoria Tipo { get; set; }
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public string? UsuarioId { get; set; }
    }
}
