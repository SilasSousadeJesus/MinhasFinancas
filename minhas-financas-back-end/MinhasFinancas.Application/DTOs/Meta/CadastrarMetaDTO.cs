using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Application.DTOs.Meta
{
    public class CadastrarMetaDTO
    {
        public string NomeMeta { get; set; } = string.Empty;
        public decimal ValorFinal { get; set; } = decimal.Zero;
        public decimal ValorAtual { get; set; } = decimal.Zero;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
    }
}
