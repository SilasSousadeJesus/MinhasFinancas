

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Domain.Entities
{
    public class Usuario : IdentityUser<Guid>
    {
        public string? Nome  { get; set; }

        public virtual List<Banco>? Bancos { get; set; }
        public virtual List<Cartao>? Cartoes { get; set; }
        public virtual List<Lancamento>? Lancamentos { get; set; }
        public virtual List<Categoria>? Categorias { get; set; }
    }
}
