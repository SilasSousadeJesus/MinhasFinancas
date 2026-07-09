using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore
{
    public class CenarioMfScore
    {
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int ScoreEsperadoMin { get; set; }
        public int ScoreEsperadoMax { get; set; }
        public string Justificativa { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public ContextoAnaliseFinanceira Contexto { get; set; } = new();
        public DadosEntradaPersonaMfScore DadosEntrada { get; set; } = new();
    }
}
