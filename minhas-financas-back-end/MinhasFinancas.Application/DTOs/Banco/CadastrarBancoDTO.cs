using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Application.DTOs.Banco
{
    public class CadastrarBancoDTO
    {
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [StringLength(20, ErrorMessage = "O Campo {0} deve ter até 20 caracteres", MinimumLength = 2)]
        public string NomeConta { get; set; } = string.Empty;
        public decimal Saldo { get; set; } = decimal.Zero;
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [StringLength(20, ErrorMessage = "O Campo {0} deve ter até 20 caracteres", MinimumLength = 2)]
        public string Instituicao { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public EnumTipoConta Tipo { get; set; }

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio, o banco precisa esta associado a um usuario")]
        public string? UsuarioId { get; set; }
    }
}
