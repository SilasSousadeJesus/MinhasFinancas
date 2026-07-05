using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.BemPatrimonial
{
    public class EditarBemPatrimonialDTO
    {
        public Guid Id { get; set; }
        public string NomeBemPatrimonial { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public EnumBemPatrimonial Tipo { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime? DataAquisicao { get; set; }
        public string? UsuarioId { get; set; }
    }
}
