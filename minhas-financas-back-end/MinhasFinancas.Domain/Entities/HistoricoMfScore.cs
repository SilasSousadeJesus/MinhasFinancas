using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Domain.Entities
{
    public class HistoricoMfScore
    {
        public Guid Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public Usuario? Usuario { get; set; }
        public int CompetenciaAno { get; set; }
        public int CompetenciaMes { get; set; }
        public int MfScoreBase { get; set; }
        public int MfScoreFinal { get; set; }
        public string Classificacao { get; set; } = string.Empty;
        public string Risco { get; set; } = string.Empty;
        public decimal PenalidadeTotal { get; set; }
        public DateTime DataCalculo { get; set; }
        public string VersaoModelo { get; set; } = string.Empty;
        public string JsonPilares { get; set; } = "[]";
        public string JsonIndicadoresCriticos { get; set; } = "[]";
        public string JsonResumo { get; set; } = "{}";
        public DateTime CriadoEm { get; set; }
    }
}
