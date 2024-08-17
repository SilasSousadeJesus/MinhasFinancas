using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Application.DTOs.Meta
{
    public class EditarMetalDTO
    {
        public Guid Id { get; set; }
        public string NomeMeta { get; set; } = string.Empty;
        public decimal ValorFinal { get; set; } = decimal.Zero;
        public decimal ValorAtual { get; set; } = decimal.Zero;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
