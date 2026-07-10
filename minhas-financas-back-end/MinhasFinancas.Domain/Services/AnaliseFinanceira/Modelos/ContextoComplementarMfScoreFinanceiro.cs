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
        public bool PossuiDadosEssenciaisInsuficientes { get; set; }
        public List<int> HistoricoPontuacoesFinais { get; set; } = [];
    }
}
