using MinhasFinancas.CrossCutting.Util.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class BemPatrimonial
    {

        [Key]
        public Guid Id { get; set; }
        public string NomeBemPatrimonial { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public bool Permanencia { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAquisicao { get; set; }
        public EnumBemPatrimonial Tipo { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }

        public virtual List<PermanenciaBemMaterial> DataPermanencia { get; set; } = new();
    }
}
