using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.Passivo
{
    public class CadastrarPassivoDTO
    {
        public string NomeBemPatrimonial { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public EnumPassivo Tipo { get; set; }
        public decimal ValorAtual { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? UsuarioId { get; set; }
    }
}
