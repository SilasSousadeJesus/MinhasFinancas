using MinhasFinancas.CrossCutting.Util.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhasFinancas.Application.DTOs.Cartao
{
    public class CadastrarCartaoDTO
    {

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        [StringLength(20, ErrorMessage = "O Campo {0} deve ter até 20 caracteres", MinimumLength = 2)]
        public string NomeCartao { get; set; } = string.Empty;
        public decimal Saldo { get; set; } = decimal.Zero;
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public string Bandeira { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public string Ultimos4Digitos { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public string DiaFechamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public string DiaVencimento { get; set; } = string.Empty;
        public string ContaPadraoPagamento { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public string Instituicao { get; set; } = string.Empty;

        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio")]
        public EnumTipoCartao Tipo { get; set; }

   
        [Required(ErrorMessage = "0 Campo {0} é Obrigatorio, o cartão precisa esta associado a um usuario")]
        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
    }
}
