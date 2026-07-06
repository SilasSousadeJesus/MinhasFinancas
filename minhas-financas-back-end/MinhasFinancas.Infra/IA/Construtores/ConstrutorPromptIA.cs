using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Construtores
{
    public class ConstrutorPromptIA
    {
        private readonly string _caminhoPromptBase;

        public ConstrutorPromptIA()
        {
            _caminhoPromptBase = Path.Combine(AppContext.BaseDirectory, "IA", "Prompts", "PromptAnaliseFinanceira.md");
        }

        public RequisicaoIA Construir(ContextoAssistenteFinanceiro contexto)
        {
            var promptSistema = CarregarPromptBase();
            var perguntaUsuario = string.IsNullOrWhiteSpace(contexto.PerguntaUsuario)
                ? "Gere uma análise aprofundada com base somente no contexto recebido."
                : contexto.PerguntaUsuario;

            var secoes = new List<string>
            {
                promptSistema,
                "## Contexto financeiro preparado pelo sistema",
                contexto.ContextoTextual,
                "## Instrução do usuário",
                perguntaUsuario
            };

            return new RequisicaoIA
            {
                PromptSistema = promptSistema,
                ContextoTextual = contexto.ContextoTextual,
                PerguntaUsuario = perguntaUsuario,
                PromptCompleto = string.Join(Environment.NewLine + Environment.NewLine, secoes),
                ModeloSugerido = string.Empty,
                Temperatura = 0.2m
            };
        }

        private string CarregarPromptBase()
        {
            if (File.Exists(_caminhoPromptBase))
            {
                return File.ReadAllText(_caminhoPromptBase);
            }

            return """
            Você é um assistente financeiro.
            Analise exclusivamente o contexto preparado pelo sistema.
            Não invente dados.
            Diferencie fatos, inferências e recomendações.
            Seja objetivo.
            Não faça promessas absolutas.
            Não substitua consultoria financeira profissional.
            """;
        }
    }
}
