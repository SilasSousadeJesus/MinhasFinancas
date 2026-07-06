namespace MinhasFinancas.Infra.IA.Modelos
{
    public class RespostaIA
    {
        public string Provedor { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public bool FoiSimulada { get; set; }
        public string ObservacaoInfraestrutura { get; set; } = string.Empty;
    }
}
