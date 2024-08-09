using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Application.DTOs.Usuario
{
    public class CadastrarUsuarioDTO
    {
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [StringLength(20, ErrorMessage = "O Campo {0} deve ter até 20 caracteres", MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [EmailAddress(ErrorMessage = "O Campo {0} é inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [StringLength(20, ErrorMessage = "O Campo {0} deve ter até 20 caracteres", MinimumLength = 4)]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [Compare(nameof(Senha), ErrorMessage = "As senhas devems ser iguais")]
        public string ConfirmacaoSenha { get; set; } = string.Empty;
    }
}
