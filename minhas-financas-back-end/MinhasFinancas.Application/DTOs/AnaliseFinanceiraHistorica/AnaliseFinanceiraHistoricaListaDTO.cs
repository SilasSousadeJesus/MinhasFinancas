namespace MinhasFinancas.Application.DTOs.AnaliseFinanceiraHistorica
{
    public class AnaliseFinanceiraHistoricaListaDTO
    {
        public Guid Id { get; set; }
        public DateTime DataGeracao { get; set; }
        public DateTime PeriodoReferencia { get; set; }
        public int PontuacaoSaudeFinanceira { get; set; }
        public string ClassificacaoSaudeFinanceira { get; set; } = string.Empty;
        public string ResumoExecutivoSistema { get; set; } = string.Empty;
        public string PerguntaUsuario { get; set; } = string.Empty;
        public string ProvedorIA { get; set; } = string.Empty;
        public string ModeloIA { get; set; } = string.Empty;
        public bool Sucesso { get; set; }
        public long TempoTotalMs { get; set; }
        public decimal CustoEstimadoUsd { get; set; }
    }
}
