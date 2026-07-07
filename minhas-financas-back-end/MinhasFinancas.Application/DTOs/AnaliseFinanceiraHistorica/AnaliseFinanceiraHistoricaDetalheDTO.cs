namespace MinhasFinancas.Application.DTOs.AnaliseFinanceiraHistorica
{
    public class AnaliseFinanceiraHistoricaDetalheDTO
    {
        public Guid Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataGeracao { get; set; }
        public DateTime PeriodoReferencia { get; set; }
        public int PontuacaoSaudeFinanceira { get; set; }
        public string ClassificacaoSaudeFinanceira { get; set; } = string.Empty;
        public string ResumoExecutivoSistema { get; set; } = string.Empty;
        public string ContextoResumoFinanceiroIAJson { get; set; } = string.Empty;
        public string IndicadoresResumidosJson { get; set; } = string.Empty;
        public string InsightsResumidosJson { get; set; } = string.Empty;
        public string PerfilFinanceiroVigenteJson { get; set; } = string.Empty;
        public string PrincipaisRiscosJson { get; set; } = string.Empty;
        public string PrincipaisPontosPositivosJson { get; set; } = string.Empty;
        public string PrincipaisRecomendacoesJson { get; set; } = string.Empty;
        public string PrioridadesJson { get; set; } = string.Empty;
        public string PerguntaUsuario { get; set; } = string.Empty;
        public string RespostaIA { get; set; } = string.Empty;
        public string ProvedorIA { get; set; } = string.Empty;
        public string ModeloIA { get; set; } = string.Empty;
        public string VersaoPrompt { get; set; } = string.Empty;
        public string VersaoSistema { get; set; } = string.Empty;
        public int TokensEntrada { get; set; }
        public int TokensSaida { get; set; }
        public int TokensTotais { get; set; }
        public decimal CustoEstimadoUsd { get; set; }
        public long TempoTotalMs { get; set; }
        public bool Sucesso { get; set; }
        public string MensagemErro { get; set; } = string.Empty;
        public bool Ativa { get; set; }
    }
}
