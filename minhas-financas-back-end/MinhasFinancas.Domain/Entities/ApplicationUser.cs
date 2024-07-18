

using Microsoft.AspNetCore.Identity;

namespace MinhasFinancas.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string NomeProprio { get; set; } = string.Empty;
    }
}
