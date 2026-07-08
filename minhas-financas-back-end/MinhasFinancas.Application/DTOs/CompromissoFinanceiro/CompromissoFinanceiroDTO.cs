using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Application.DTOs.CompromissoFinanceiro
{
    public class CompromissoFinanceiroDTO
    {
        public Guid Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public EnumOrigemCompromissoFinanceiro Origem { get; set; }
        public EnumStatusCompromissoFinanceiro Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; }
    }
}
