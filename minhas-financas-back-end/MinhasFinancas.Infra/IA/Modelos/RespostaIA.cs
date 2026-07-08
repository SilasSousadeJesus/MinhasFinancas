namespace MinhasFinancas.Infra.IA.Modelos
{
    public class RespostaIA
    {
        public bool Sucesso { get; set; }
        public string Provedor { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public Guid? AnaliseFinanceiraHistoricaId { get; set; }
        public string Conteudo { get; set; } = string.Empty;
        public string? SugestaoCompromissoFinanceiro { get; set; }
        public bool FoiSimulada { get; set; }
        public string ObservacaoInfraestrutura { get; set; } = string.Empty;
        public string MensagemTecnica { get; set; } = string.Empty;
        public string MensagemAmigavel { get; set; } = string.Empty;
        public string OrigemErro { get; set; } = string.Empty;
        public CategoriaErroIA CategoriaErro { get; set; }
        public int? StatusHttpProvedor { get; set; }
        public int TentativasRealizadas { get; set; }
        public int CaracteresEntrada { get; set; }
        public int TokensEntradaEstimados { get; set; }
        public int TokensEntradaUtilizados { get; set; }
        public int TokensSaidaUtilizados { get; set; }
        public int TokensRaciocinioUtilizados { get; set; }
        public int TokensTotaisUtilizados { get; set; }
        public bool TokensReaisDisponiveis { get; set; }
        public bool EntradaFoiTruncada { get; set; }
        public long TempoTotalMs { get; set; }
        public decimal CustoEstimadoUsd { get; set; }
        public decimal PrecoEntradaPorMilhaoTokensUsd { get; set; }
        public decimal PrecoSaidaPorMilhaoTokensUsd { get; set; }
    }
}
