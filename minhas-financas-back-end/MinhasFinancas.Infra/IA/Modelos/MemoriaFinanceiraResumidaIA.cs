namespace MinhasFinancas.Infra.IA.Modelos
{
    public class MemoriaFinanceiraResumidaIA
    {
        public DateTime DataGeracao { get; set; }
        public DateTime PeriodoReferencia { get; set; }
        public int PontuacaoSaudeFinanceira { get; set; }
        public string ClassificacaoSaudeFinanceira { get; set; } = string.Empty;
        public string ResumoExecutivoSistema { get; set; } = string.Empty;
        public List<string> PrincipaisRiscos { get; set; } = [];
        public List<string> PrincipaisPontosPositivos { get; set; } = [];
        public List<string> PrincipaisRecomendacoes { get; set; } = [];
        public List<string> Prioridades { get; set; } = [];
    }
}
