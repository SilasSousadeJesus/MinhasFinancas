namespace MinhasFinancas.Application.DTOs.MfScorePersona
{
    public class ResultadoRodarMfScorePersonaDTO
    {
        public Guid PersonaId { get; set; }
        public string Persona { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int MfScoreBase { get; set; }
        public int MfScoreFinal { get; set; }
        public string Classificacao { get; set; } = string.Empty;
        public string Risco { get; set; } = string.Empty;
        public decimal PenalidadeTotal { get; set; }
        public int? ScoreHumanoSugerido { get; set; }
        public int? FaixaEsperadaMin { get; set; }
        public int? FaixaEsperadaMax { get; set; }
        public int? DiferencaScoreHumano { get; set; }
        public bool? DentroDaFaixaEsperada { get; set; }
        public string? ObservacaoComparativa { get; set; }
        public List<ResultadoPilarMfScorePersonaDTO> Pilares { get; set; } = [];
        public List<ResultadoIndicadorCriticoMfScorePersonaDTO> IndicadoresCriticos { get; set; } = [];
        public List<string> PenalizacoesAplicadas { get; set; } = [];
    }

    public class ResultadoPilarMfScorePersonaDTO
    {
        public string Pilar { get; set; } = string.Empty;
        public int Nota { get; set; }
        public decimal Peso { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }

    public class ResultadoIndicadorCriticoMfScorePersonaDTO
    {
        public string Indicador { get; set; } = string.Empty;
        public string PilarRelacionado { get; set; } = string.Empty;
        public decimal Penalidade { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }
}
