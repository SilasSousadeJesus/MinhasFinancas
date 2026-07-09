namespace MinhasFinancas.CrossCutting.Reports
{
    public class MfScoreAuditoriaExcelReportData
    {
        public string NomeArquivo { get; set; } = string.Empty;
        public DateTime DataGeracao { get; set; }
        public string VersaoMfScore { get; set; } = string.Empty;
        public List<MfScoreAuditoriaCenarioExcelReportData> Cenarios { get; set; } = [];
    }

    public class MfScoreAuditoriaCenarioExcelReportData
    {
        public string Persona { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int ScoreEsperadoMin { get; set; }
        public int ScoreEsperadoMax { get; set; }
        public int ScoreObtido { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Classificacao { get; set; } = string.Empty;
        public string Risco { get; set; } = string.Empty;
        public string Justificativa { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public int FluxoDeCaixa { get; set; }
        public int LiquidezEReserva { get; set; }
        public int EndividamentoEObrigacoes { get; set; }
        public int Patrimonio { get; set; }
        public int PlanejamentoEDisciplina { get; set; }
        public List<MfScoreAuditoriaIndicadorCriticoExcelReportData> IndicadoresCriticos { get; set; } = [];
        public MfScoreAuditoriaDadosEntradaExcelReportData DadosEntrada { get; set; } = new();
    }

    public class MfScoreAuditoriaIndicadorCriticoExcelReportData
    {
        public string Persona { get; set; } = string.Empty;
        public string Indicador { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Penalidade { get; set; }
        public string Observacao { get; set; } = string.Empty;
    }

    public class MfScoreAuditoriaDadosEntradaExcelReportData
    {
        public decimal Renda { get; set; }
        public decimal Despesas { get; set; }
        public decimal Reserva { get; set; }
        public decimal Patrimonio { get; set; }
        public decimal Passivos { get; set; }
        public decimal ObrigacoesFuturas30Dias { get; set; }
        public decimal ObrigacoesFuturas90Dias { get; set; }
        public decimal ObrigacoesFuturas180Dias { get; set; }
        public decimal ObrigacoesFuturas12Meses { get; set; }
    }
}
