namespace MinhasFinancas.Infra.IA.Modelos
{
    public class InterpretacaoMemoriaFinanceiraIA
    {
        public bool PossuiHistorico { get; set; }
        public bool PossuiEvolucaoComparavel { get; set; }
        public string ResumoEvolucao { get; set; } = string.Empty;
        public List<string> SinaisContinuidade { get; set; } = [];
        public List<string> Narrativas { get; set; } = [];
        public List<string> MemoriaFinanceiraCompacta { get; set; } = [];
    }
}
