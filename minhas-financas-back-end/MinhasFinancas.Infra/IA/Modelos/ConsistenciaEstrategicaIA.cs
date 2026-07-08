using MinhasFinancas.Infra.IA.Enums;

namespace MinhasFinancas.Infra.IA.Modelos
{
    public class ConsistenciaEstrategicaIA
    {
        public bool PossuiPlano { get; set; }
        public NivelConsistenciaEstrategica NivelConsistencia { get; set; } = NivelConsistenciaEstrategica.Indeterminada;
        public string Resumo { get; set; } = string.Empty;
        public List<string> MotivosFavoraveis { get; set; } = [];
        public List<string> MotivosDesfavoraveis { get; set; } = [];
        public List<string> ObjetivosImpactados { get; set; } = [];
        public string TextoParaIA { get; set; } = string.Empty;
    }
}
