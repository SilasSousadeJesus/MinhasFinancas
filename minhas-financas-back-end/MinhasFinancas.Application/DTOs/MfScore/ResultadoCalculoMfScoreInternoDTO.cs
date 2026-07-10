using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Application.DTOs.MfScore
{
    public class ResultadoCalculoMfScoreInternoDTO
    {
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataReferencia { get; set; }
        public ContextoAnaliseFinanceira ContextoAnalise { get; set; } = new();
        public PainelIndicadoresFinanceiros PainelIndicadores { get; set; } = new();
        public PainelSaudeFinanceira PainelSaude { get; set; } = new();
        public ContextoComplementarMfScoreFinanceiro ContextoComplementar { get; set; } = new();
    }
}
