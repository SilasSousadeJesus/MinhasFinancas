using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class Banco
    {
        public Banco() { }

        [Key]
        public Guid Id { get; set; }
        public string NomeConta { get; set; } = string.Empty;
        public string Saldo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Instituicao { get; set; } = string.Empty;
        public EnumTipoConta Tipo { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

    }
}
