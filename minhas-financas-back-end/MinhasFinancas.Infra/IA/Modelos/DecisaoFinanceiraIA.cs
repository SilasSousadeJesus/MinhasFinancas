using MinhasFinancas.Infra.IA.Enums;

namespace MinhasFinancas.Infra.IA.Modelos
{
    public class DecisaoFinanceiraIA
    {
        public TipoDecisaoFinanceira TipoDecisao { get; set; } = TipoDecisaoFinanceira.Indefinida;
        public string Categoria { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal? ValorEstimado { get; set; }
        public string? Prazo { get; set; }
        public string? FormaPagamento { get; set; }
        public string? ObjetivoRelacionado { get; set; }
        public string OrigemDaDecisao { get; set; } = "Pergunta do usuario";
        public string TextoOriginalUsuario { get; set; } = string.Empty;
        public string TextoInterpretado { get; set; } = string.Empty;
        public int GrauConfiancaInterpretacao { get; set; }
    }
}
