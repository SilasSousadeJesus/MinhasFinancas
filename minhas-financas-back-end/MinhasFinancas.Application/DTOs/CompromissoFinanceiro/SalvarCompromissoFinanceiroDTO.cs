using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.CompromissoFinanceiro
{
    public class SalvarCompromissoFinanceiroDTO
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public EnumOrigemCompromissoFinanceiro Origem { get; set; } = EnumOrigemCompromissoFinanceiro.Manual;
        public string? Observacoes { get; set; }
    }
}
