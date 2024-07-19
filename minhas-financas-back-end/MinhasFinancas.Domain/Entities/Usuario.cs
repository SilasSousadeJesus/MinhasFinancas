

using Microsoft.AspNetCore.Identity;

namespace MinhasFinancas.Domain.Entities
{
    public class Usuario : IdentityUser
    {
        public string? Nome  { get; set; }
    }
}
