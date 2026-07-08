namespace MinhasFinancas.Infra.IA.Modelos
{
    public class ContextoAssistenteFinanceiro
    {
        public DateTime DataReferencia { get; set; }
        public int PontuacaoSaudeFinanceira { get; set; }
        public string ClassificacaoSaudeFinanceira { get; set; } = string.Empty;
        public InterpretacaoPlanoEstrategicoIA InterpretacaoPlanoEstrategico { get; set; } = new();
        public List<string> PrioridadesImediatas { get; set; } = [];
        public List<string> DestaquesPositivos { get; set; } = [];
        public List<string> InsightsPrioritarios { get; set; } = [];
        public List<string> MemoriaFinanceiraResumida { get; set; } = [];
        public string ResumoEvolucaoFinanceira { get; set; } = string.Empty;
        public List<string> EvolucaoFinanceira { get; set; } = [];
        public string ResumoExecutivo { get; set; } = string.Empty;
        public string ContextoTextual { get; set; } = string.Empty;
        public string PerguntaUsuario { get; set; } = string.Empty;
    }
}
