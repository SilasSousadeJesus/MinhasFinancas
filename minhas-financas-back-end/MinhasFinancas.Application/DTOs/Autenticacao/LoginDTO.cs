using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhasFinancas.Application.DTOs.Autenticacao
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [EmailAddress(ErrorMessage = "O Campo {0} é inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [StringLength(20, ErrorMessage = "O Campo {0} deve ter até 20 caracteres", MinimumLength = 4)]
        public string Senha { get; set; } = string.Empty;
    }
}
