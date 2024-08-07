using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Domain.Entities
{
    public class Passivo
    {
        [Key]
        public Guid Id { get; set; }
        public string NomePassivo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Permanencia { get; set; }
        public DateTime DataCadastro { get; set; }
        public EnumPassivo Tipo { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
        public virtual List<PermanenciaPassivo>? DataPermanencia { get; set; }
    }
}
