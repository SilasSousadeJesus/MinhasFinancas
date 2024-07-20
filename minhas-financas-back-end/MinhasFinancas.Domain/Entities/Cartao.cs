using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Domain.Entities
{
    public class Cartao
    {
        public Cartao() { }

        [Key]
        public Guid Id { get; set; }
        public string NomeCartao { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = decimal.Zero;
        public string Bandeira { get; set; } = string.Empty;
        public string Ultimos4Digitos { get; set; } = string.Empty;
        public string DiaFechamento { get; set; } = string.Empty;
        public string DiaVencimento { get; set; } = string.Empty;
        public string ContaPadraoPagamento { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Instituicao { get; set; } = string.Empty;
        public EnumTipoCartao Tipo { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
