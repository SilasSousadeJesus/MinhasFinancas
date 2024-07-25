using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace MinhasFinancas.Domain.Entities
{
    public class Conta
    {
        public Conta() { }

        [Key]
        public Guid Id { get; set; }
        public string NomeConta { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(18,2)")]
        public decimal SaldoInvestimento { get; set; } = decimal.Zero;
        public string Descricao { get; set; } = string.Empty;
        public string Instituicao { get; set; } = string.Empty;
        public EnumTipoConta Tipo { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual List<Lancamento>? BensPatrimoniais { get; set; }

    }
}