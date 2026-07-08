namespace MinhasFinancas.Infra.IA.Modelos
{
    public class InterpretacaoPlanoEstrategicoIA
    {
        public bool PossuiPlanoVigente { get; set; }
        public int? NumeroVersaoPlanoVigente { get; set; }
        public string NomePlano { get; set; } = string.Empty;
        public string ResumoEstrategico { get; set; } = string.Empty;
        public List<string> PrioridadesEstrategicas { get; set; } = [];
        public List<string> ObjetivosEmAndamento { get; set; } = [];
        public List<string> ObjetivosConcluidos { get; set; } = [];
        public List<string> ObjetivosCriticosOuAltaPrioridade { get; set; } = [];
        public List<string> AlertasEstrategicos { get; set; } = [];
        public List<string> ObservacoesRelevantes { get; set; } = [];
        public string TextoParaIA { get; set; } = string.Empty;
    }
}
