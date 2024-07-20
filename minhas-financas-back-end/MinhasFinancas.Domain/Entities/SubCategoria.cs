using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class SubCategoria
    {
        public SubCategoria() { }

        [Key]
        public Guid Id { get; set; }
        public string NomeSubCategoria { get; set; } = string.Empty;

        [ForeignKey("CategoriaId")]
        public Guid CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}
