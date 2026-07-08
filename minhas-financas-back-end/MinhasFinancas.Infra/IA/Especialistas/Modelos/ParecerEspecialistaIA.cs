namespace MinhasFinancas.Infra.IA.Especialistas.Modelos
{
    public class ParecerEspecialistaIA
    {
        public string NomeEspecialista { get; set; } = string.Empty;
        public string SituacaoAtual { get; set; } = string.Empty;
        public string Conclusao { get; set; } = string.Empty;
        public List<string> Riscos { get; set; } = [];
        public List<string> PontosPositivos { get; set; } = [];
        public List<string> Recomendacoes { get; set; } = [];
        public string Prioridade { get; set; } = string.Empty;
        public List<string> Observacoes { get; set; } = [];
    }
}
