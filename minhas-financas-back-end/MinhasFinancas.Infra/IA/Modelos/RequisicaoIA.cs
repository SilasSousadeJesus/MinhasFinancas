namespace MinhasFinancas.Infra.IA.Modelos
{
    public class RequisicaoIA
    {
        public string PromptSistema { get; set; } = string.Empty;
        public string ContextoTextual { get; set; } = string.Empty;
        public string PerguntaUsuario { get; set; } = string.Empty;
        public string PromptCompleto { get; set; } = string.Empty;
        public string ModeloSugerido { get; set; } = string.Empty;
        public string VersaoPrompt { get; set; } = string.Empty;
        public decimal Temperatura { get; set; }
    }
}
