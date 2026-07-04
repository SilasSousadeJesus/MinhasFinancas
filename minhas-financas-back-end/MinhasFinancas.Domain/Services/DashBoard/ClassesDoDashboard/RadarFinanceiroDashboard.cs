namespace MinhasFinancas.Domain.Services.DashBoard.ClassesDoDashboard
{
    public class RadarFinanceiroDashboard
    {
        public List<ProximoVencimentoDashboard> ProximosVencimentos { get; set; } = new();
        public List<ContaAtrasadaDashboard> ContasAtrasadas { get; set; } = new();
        public List<AlertaFinanceiroDashboard> AlertasFinanceiros { get; set; } = new();
        public FluxoCaixaProximos30DiasDashboard FluxoCaixaProximos30Dias { get; set; } = new();
    }
}
