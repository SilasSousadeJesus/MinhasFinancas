namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class ResumoSaudeFinanceira
    {
        public int PontuacaoGeral { get; set; }
        public string Classificacao { get; set; } = string.Empty;
        public List<PontoAtencaoSaudeFinanceira> PontosAtencao { get; set; } = [];
    }
}
