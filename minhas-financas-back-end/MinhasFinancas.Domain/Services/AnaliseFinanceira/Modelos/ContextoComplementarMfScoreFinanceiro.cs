namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class ContextoComplementarMfScoreFinanceiro
    {
        public bool PossuiFluxoMensalNegativoAtual { get; set; }
        public int MesesConsecutivosFluxoNegativo { get; set; }
        public bool PossuiInadimplencia { get; set; }
        public int NivelInadimplencia { get; set; }
        public int DiasMaximosAtraso { get; set; }
        public decimal ValorTotalEmAtraso { get; set; }
        public decimal PercentualValorEmAtrasoSobreRenda { get; set; }
        public int QuantidadeParametrosPlanejamentoConfigurados { get; set; }
        public int TotalParametrosPlanejamentoEsperados { get; set; }
        public bool PerfilFinanceiroBasicoCompleto { get; set; }
        public int NotaConfiguracaoPlanejamento { get; set; }
        public bool PossuiPlanoEstrategicoVigente { get; set; }
        public int QuantidadeObjetivosPlanoAtivos { get; set; }
        public int QuantidadeObjetivosPlanoAltaPrioridade { get; set; }
        public int QuantidadeObjetivosPlanoConcluidos { get; set; }
        public int? NotaPlanoEstrategico { get; set; }
        public bool PossuiCompromissosFinanceiros { get; set; }
        public int QuantidadeCompromissosEmAndamento { get; set; }
        public int QuantidadeCompromissosConcluidos { get; set; }
        public int QuantidadeCompromissosCancelados { get; set; }
        public int? NotaCompromissosFinanceiros { get; set; }
        public bool PossuiCuraRecenteInadimplencia { get; set; }
        public int QuantidadeOcorrenciasAtrasoRecente { get; set; }
        public int QuantidadeMesesComOcorrenciaAtrasoRecente { get; set; }
        public bool PossuiDadosEssenciaisInsuficientes { get; set; }
        public List<int> HistoricoPontuacoesFinais { get; set; } = [];
    }
}
