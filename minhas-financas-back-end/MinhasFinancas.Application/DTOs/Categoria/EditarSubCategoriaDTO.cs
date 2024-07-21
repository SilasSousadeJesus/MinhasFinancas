using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Application.DTOs.Categoria
{
    public class EditarSubCategoriaDTO
    {
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [StringLength(20, ErrorMessage = "O Campo {0} deve ter até 20 caracteres", MinimumLength = 2)]
        public string NomeSubCategoria { get; set; } = string.Empty;
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]

        [ForeignKey("CategoriaId")]
        public Guid CategoriaId { get; set; }
    }
}
