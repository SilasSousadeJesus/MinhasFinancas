namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class ContextoComplementarMfScoreFinanceiro
    {
        public bool PossuiFluxoMensalNegativoAtual { get; set; }
        public int MesesConsecutivosFluxoNegativo { get; set; }
        public bool PossuiInadimplencia { get; set; }
        public bool PossuiDadosEssenciaisInsuficientes { get; set; }
        public List<int> HistoricoPontuacoesFinais { get; set; } = [];
    }
}
