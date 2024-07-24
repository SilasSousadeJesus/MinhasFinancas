

using Microsoft.AspNetCore.Identity;

namespace MinhasFinancas.Domain.Entities
{
    public class Usuario : IdentityUser
    {
        public string? Nome  { get; set; }

        public virtual List<Banco>? Bancos { get; set; }
        public virtual List<Cartao>? Cartoes { get; set; }
        public virtual List<Lancamento>? Lancamentos { get; set; }
        public virtual List<Categoria>? Categorias { get; set; }
        public virtual List<BemPatrimonial>? BensPatrimoniais { get; set; }
    }
}
