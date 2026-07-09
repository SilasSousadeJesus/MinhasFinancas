namespace MinhasFinancas.CrossCutting.Reports
{
    public class MfScoreAuditoriaHumanaExcelReportData
    {
        public string NomeArquivo { get; set; } = string.Empty;
        public DateTime DataGeracao { get; set; }
        public string VersaoMfScore { get; set; } = string.Empty;
        public List<MfScoreAuditoriaHumanaPersonaExcelReportData> Personas { get; set; } = [];
    }

    public class MfScoreAuditoriaHumanaPersonaExcelReportData
    {
        public string Persona { get; set; } = string.Empty;
        public string Objetivo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int ScoreCalculado { get; set; }
        public string ClassificacaoCalculada { get; set; } = string.Empty;
        public string RiscoCalculado { get; set; } = string.Empty;
        public int FluxoDeCaixa { get; set; }
        public int LiquidezEReserva { get; set; }
        public int EndividamentoEObrigacoes { get; set; }
        public int Patrimonio { get; set; }
        public int PlanejamentoEDisciplina { get; set; }
        public decimal PenalidadeTotal { get; set; }
        public string IndicadoresCriticosResumo { get; set; } = string.Empty;
        public string PenalizacoesAplicadasResumo { get; set; } = string.Empty;
        public int ScoreEsperadoMinAtual { get; set; }
        public int ScoreEsperadoMaxAtual { get; set; }
        public MfScoreAuditoriaHumanaDadosEntradaExcelReportData DadosEntrada { get; set; } = new();
        public List<MfScoreAuditoriaHumanaIndicadorExcelReportData> Indicadores { get; set; } = [];
        public List<MfScoreAuditoriaHumanaPilarExcelReportData> Pilares { get; set; } = [];
        public List<MfScoreAuditoriaHumanaPenalizacaoExcelReportData> Penalizacoes { get; set; } = [];
    }

    public class MfScoreAuditoriaHumanaDadosEntradaExcelReportData
    {
        public decimal RendaMensal { get; set; }
        public decimal ReceitasNoPeriodo { get; set; }
        public decimal DespesasMensais { get; set; }
        public decimal DespesasFuturas30Dias { get; set; }
        public decimal DespesasFuturas90Dias { get; set; }
        public decimal DespesasFuturas180Dias { get; set; }
        public decimal DespesasFuturas12Meses { get; set; }
        public decimal Reserva { get; set; }
        public decimal PatrimonioBruto { get; set; }
        public decimal Passivos { get; set; }
        public decimal PatrimonioLiquido { get; set; }
        public string PerfilFinanceiroConfigurado { get; set; } = string.Empty;
        public string PlanoEstrategico { get; set; } = string.Empty;
        public string Compromissos { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
    }

    public class MfScoreAuditoriaHumanaIndicadorExcelReportData
    {
        public string Persona { get; set; } = string.Empty;
        public string Indicador { get; set; } = string.Empty;
        public decimal ValorAtual { get; set; }
        public decimal ValorIdeal { get; set; }
        public decimal Percentual { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public string PilarRelacionado { get; set; } = string.Empty;
    }

    public class MfScoreAuditoriaHumanaPilarExcelReportData
    {
        public string Persona { get; set; } = string.Empty;
        public string Pilar { get; set; } = string.Empty;
        public int NotaPilar { get; set; }
        public decimal PesoPilar { get; set; }
        public decimal ContribuicaoScoreBase { get; set; }
        public string Observacao { get; set; } = string.Empty;
    }

    public class MfScoreAuditoriaHumanaPenalizacaoExcelReportData
    {
        public string Persona { get; set; } = string.Empty;
        public string RegraCritica { get; set; } = string.Empty;
        public string IndicadorRelacionado { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Penalizacao { get; set; }
        public string Justificativa { get; set; } = string.Empty;
    }
}
