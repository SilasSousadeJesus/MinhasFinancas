using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Application.DTOs.AnaliseFinanceiraHistorica
{
    public class RegistrarAnaliseFinanceiraHistoricaDTO
    {
        public string UsuarioId { get; set; } = string.Empty;
        public ResumoFinanceiroIA ResumoFinanceiroIA { get; set; } = new();
        public ContextoAssistenteFinanceiro ContextoAssistenteFinanceiro { get; set; } = new();
        public RequisicaoIA RequisicaoIA { get; set; } = new();
        public RespostaIA RespostaIA { get; set; } = new();
    }
}
