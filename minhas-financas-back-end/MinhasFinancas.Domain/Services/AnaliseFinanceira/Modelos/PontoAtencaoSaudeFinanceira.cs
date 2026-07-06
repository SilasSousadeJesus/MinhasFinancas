using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class PontoAtencaoSaudeFinanceira
    {
        public string Nome { get; set; } = string.Empty;
        public StatusIndicadorFinanceiro Status { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
    }
}
