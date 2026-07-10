namespace MinhasFinancas.Application.DTOs.MfScore
{
    public class ResultadoGeracaoHistoricoMfScoreDTO
    {
        public int CompetenciaAno { get; set; }
        public int CompetenciaMes { get; set; }
        public string VersaoModelo { get; set; } = string.Empty;
        public int UsuariosProcessados { get; set; }
        public int HistoricosCriados { get; set; }
        public int HistoricosJaExistentes { get; set; }
        public List<string> Falhas { get; set; } = [];
    }
}
