namespace MinhasFinancas.Infra.IA.Modelos
{
    public class RespostaIA
    {
        public bool Sucesso { get; set; }
        public string Provedor { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Conteudo { get; set; } = string.Empty;
        public bool FoiSimulada { get; set; }
        public string ObservacaoInfraestrutura { get; set; } = string.Empty;
        public string MensagemTecnica { get; set; } = string.Empty;
        public string MensagemAmigavel { get; set; } = string.Empty;
        public string OrigemErro { get; set; } = string.Empty;
        public int? StatusHttpProvedor { get; set; }
        public int TentativasRealizadas { get; set; }
        public int CaracteresEntrada { get; set; }
        public int TokensEntradaEstimados { get; set; }
        public bool EntradaFoiTruncada { get; set; }
    }
}
