using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Domain.Entities
{
    public class Categoria
    {
        public Categoria() { }

        [Key]
        public Guid Id { get; set; }
        public string NomeCategoria { get; set; } = string.Empty;
        public string Icone { get; set; } = string.Empty;
        public EnumTipoCategoria Tipo { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
        public virtual List<SubCategoria>? SubCategorias { get; set; }

    }
}
