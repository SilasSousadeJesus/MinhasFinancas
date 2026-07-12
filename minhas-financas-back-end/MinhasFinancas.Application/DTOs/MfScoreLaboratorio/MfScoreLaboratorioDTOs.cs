namespace MinhasFinancas.Application.DTOs.MfScoreLaboratorio
{
    public class UsuarioMfScoreLaboratorioDTO
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? DataCadastro { get; set; }
        public bool EhUsuarioSintetico { get; set; }
        public string OrigemUsuario { get; set; } = string.Empty;
        public string CodigoCenario { get; set; } = string.Empty;
        public string VersaoBase { get; set; } = string.Empty;
        public DateTime? DataGeracaoBase { get; set; }
        public string DescricaoCenario { get; set; } = string.Empty;
        public string ObjetivoCenario { get; set; } = string.Empty;
    }

    public class ResultadoGeracaoBaseSimulacaoMfScoreDTO
    {
        public string VersaoBase { get; set; } = string.Empty;
        public int QuantidadeCenarios { get; set; }
        public int QuantidadeUsuariosGerados { get; set; }
        public DateTime DataGeracao { get; set; }
        public List<UsuarioMfScoreLaboratorioDTO> UsuariosGerados { get; set; } = [];
    }

    public class ResultadoLimpezaBaseSimulacaoMfScoreDTO
    {
        public int QuantidadeUsuariosRemovidos { get; set; }
        public List<string> CodigosCenariosRemovidos { get; set; } = [];
    }

    public class TendenciaMfScoreLaboratorioDTO
    {
        public string Direcao { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public List<int> HistoricoNotas { get; set; } = [];
    }

    public class PilarMfScoreLaboratorioDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public decimal Peso { get; set; }
        public int Nota { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public List<string> Indicadores { get; set; } = [];
    }

    public class IndicadorMfScoreLaboratorioDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public decimal ValorAtual { get; set; }
        public decimal ValorIdeal { get; set; }
        public decimal Percentual { get; set; }
        public decimal? ValorObrigacoesPrevistas { get; set; }
        public decimal? ValorReceitaPrevista { get; set; }
        public decimal? PercentualComprometimento { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;
        public string Formato { get; set; } = string.Empty;
    }

    public class IndicadorCriticoMfScoreLaboratorioDTO
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public decimal Penalidade { get; set; }
        public string PilarRelacionado { get; set; } = string.Empty;
    }

    public class PenalizacaoMfScoreLaboratorioDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public decimal Penalidade { get; set; }
        public string PilarRelacionado { get; set; } = string.Empty;
    }

    public class DadosEntradaMfScoreLaboratorioDTO
    {
        public DateTime DataReferencia { get; set; }
        public int QuantidadeLancamentos { get; set; }
        public int QuantidadeReceitas { get; set; }
        public int QuantidadeDespesas { get; set; }
        public decimal ReceitaMensalConsiderada { get; set; }
        public decimal DespesaMensalConsiderada { get; set; }
        public int QuantidadeAtivos { get; set; }
        public int QuantidadePassivos { get; set; }
        public decimal ValorAtivosConsiderados { get; set; }
        public decimal ValorPassivosConsiderados { get; set; }
        public int QuantidadeMetas { get; set; }
        public bool PossuiPerfilFinanceiroConfigurado { get; set; }
        public bool PossuiPlanoEstrategicoVigente { get; set; }
        public int QuantidadeObjetivosPlanoAtivos { get; set; }
        public int QuantidadeObjetivosPlanoAltaPrioridade { get; set; }
        public int QuantidadeObjetivosPlanoConcluidos { get; set; }
        public bool PossuiCompromissosFinanceiros { get; set; }
        public int QuantidadeCompromissosEmAndamento { get; set; }
        public int QuantidadeCompromissosConcluidos { get; set; }
        public int QuantidadeCompromissosCancelados { get; set; }
        public bool PossuiFluxoMensalNegativoAtual { get; set; }
        public int MesesConsecutivosFluxoNegativo { get; set; }
        public bool PossuiInadimplencia { get; set; }
        public int NivelInadimplencia { get; set; }
        public int DiasMaximosAtraso { get; set; }
        public decimal ValorTotalEmAtraso { get; set; }
        public decimal PercentualValorEmAtrasoSobreRenda { get; set; }
        public bool PossuiCuraRecenteInadimplencia { get; set; }
        public int QuantidadeOcorrenciasAtrasoRecente { get; set; }
        public int QuantidadeMesesComOcorrenciaAtrasoRecente { get; set; }
        public bool PossuiDadosEssenciaisInsuficientes { get; set; }
        public int QuantidadeParametrosPlanejamentoConfigurados { get; set; }
        public int TotalParametrosPlanejamentoEsperados { get; set; }
        public bool PerfilFinanceiroBasicoCompleto { get; set; }
        public int NotaConfiguracaoPlanejamento { get; set; }
        public int? NotaPlanoEstrategico { get; set; }
        public int? NotaCompromissosFinanceiros { get; set; }
    }

    public class MfScoreLaboratorioDetalheDTO
    {
        public UsuarioMfScoreLaboratorioDTO Usuario { get; set; } = new();
        public string VersaoModelo { get; set; } = string.Empty;
        public int MfScoreBase { get; set; }
        public int MfScoreFinal { get; set; }
        public string Classificacao { get; set; } = string.Empty;
        public string Risco { get; set; } = string.Empty;
        public decimal PenalidadeTotal { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public TendenciaMfScoreLaboratorioDTO Tendencia { get; set; } = new();
        public List<string> ResumoExecutivoDosPilares { get; set; } = [];
        public List<PilarMfScoreLaboratorioDTO> Pilares { get; set; } = [];
        public List<IndicadorMfScoreLaboratorioDTO> Indicadores { get; set; } = [];
        public List<IndicadorCriticoMfScoreLaboratorioDTO> IndicadoresCriticos { get; set; } = [];
        public List<PenalizacaoMfScoreLaboratorioDTO> Penalizacoes { get; set; } = [];
        public List<string> RegrasCriticasAplicadas { get; set; } = [];
        public DadosEntradaMfScoreLaboratorioDTO DadosEntrada { get; set; } = new();
        public List<string> ObservacoesLimitacoes { get; set; } = [];
        public AnaliseCalibracaoMfScoreLaboratorioDTO AnaliseCalibracao { get; set; } = new();
    }

    public class BenchmarkCenarioMfScoreLaboratorioDTO
    {
        public string CodigoCenario { get; set; } = string.Empty;
        public string NomeCenario { get; set; } = string.Empty;
        public int NotaHumanaReferencia { get; set; }
        public int FaixaAceitavelMinima { get; set; }
        public int FaixaAceitavelMaxima { get; set; }
        public string FaixaAceitavelTexto { get; set; } = string.Empty;
        public int DiferencaRegistrada { get; set; }
        public string Status { get; set; } = string.Empty;
        public string JustificativaHumana { get; set; } = string.Empty;
        public List<string> IndicadoresResponsaveis { get; set; } = [];
        public string DecisaoAuditoria { get; set; } = string.Empty;
    }

    public class AnalisePilarCalibracaoMfScoreLaboratorioDTO
    {
        public string CodigoPilar { get; set; } = string.Empty;
        public string NomePilar { get; set; } = string.Empty;
        public int NotaPilar { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
    }

    public class AnaliseCalibracaoMfScoreLaboratorioDTO
    {
        public bool Disponivel { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public BenchmarkCenarioMfScoreLaboratorioDTO? Benchmark { get; set; }
        public int? DiferencaAtual { get; set; }
        public bool DentroDaFaixaEsperada { get; set; }
        public string SituacaoFaixa { get; set; } = string.Empty;
        public List<AnalisePilarCalibracaoMfScoreLaboratorioDTO> AnalisesPilares { get; set; } = [];
        public List<string> IndicadoresQuePuxaramParaBaixo { get; set; } = [];
        public List<string> PrincipaisPontosPositivos { get; set; } = [];
        public string DiagnosticoFinal { get; set; } = string.Empty;
        public string RecomendacaoProximaCalibracao { get; set; } = string.Empty;
    }
}
