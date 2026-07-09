using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class MfScoreFinanceiro
    {
        public int PontuacaoBase { get; set; }
        public int PontuacaoFinal { get; set; }
        public string Classificacao { get; set; } = string.Empty;
        public string Risco { get; set; } = string.Empty;
        public TendenciaMfScoreFinanceiro Tendencia { get; set; } = new();
        public List<PilarMfScoreFinanceiro> Pilares { get; set; } = [];
        public List<IndicadorCriticoMfScoreFinanceiro> IndicadoresCriticos { get; set; } = [];
        public List<string> ResumoExecutivoDosPilares { get; set; } = [];
        public List<string> RegrasCriticasAplicadas { get; set; } = [];
        public string Descricao { get; set; } = string.Empty;
        public decimal PenalidadeTotal { get; set; }
    }
}
