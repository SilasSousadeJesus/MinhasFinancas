using MinhasFinancas.CrossCutting.Util.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
