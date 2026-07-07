using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Construtores
{
    public class ConstrutorPromptIA
    {
        public const string VersaoPromptAtual = "fase-4.2.2.1";
        private readonly string _caminhoPromptBase;

        public ConstrutorPromptIA()
        {
            _caminhoPromptBase = Path.Combine(AppContext.BaseDirectory, "IA", "Prompts", "PromptAnaliseFinanceira.md");
        }

        public RequisicaoIA Construir(ContextoAssistenteFinanceiro contexto)
        {
            var promptSistema = CarregarPromptBase();
            var perguntaUsuario = string.IsNullOrWhiteSpace(contexto.PerguntaUsuario)
                ? "Gere uma analise aprofundada com base somente no contexto recebido."
                : contexto.PerguntaUsuario;

            var secoes = new List<string>
            {
                promptSistema,
                "## Contexto financeiro preparado pelo sistema",
                contexto.ContextoTextual,
                "## Pergunta do usuario",
                perguntaUsuario
            };

            return new RequisicaoIA
            {
                PromptSistema = promptSistema,
                ContextoTextual = contexto.ContextoTextual,
                PerguntaUsuario = perguntaUsuario,
                PromptCompleto = string.Join(Environment.NewLine + Environment.NewLine, secoes),
                ModeloSugerido = string.Empty,
                VersaoPrompt = VersaoPromptAtual,
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
            Voce e um consultor financeiro experiente do sistema Minhas Financas.

            Analise exclusivamente o contexto preparado pelo sistema.

            Regras obrigatorias:

            - nao invente dados ausentes no contexto
            - nao contradiga os indicadores e resumos recebidos
            - diferencie explicacao, risco e recomendacao
            - explique antes de recomendar
            - ensine antes de aconselhar
            - use a secao Evolucao Financeira como interpretacao oficial da memoria do sistema
            - quando fizer sentido, reconheca continuidade, melhora, estabilidade ou recorrencia
            - se mantiver uma recomendacao anterior, explicite isso com frases como "continuamos recomendando" ou "mantemos como prioridade"
            - use portugues brasileiro natural, profissional e respeitoso
            - nao faca promessas absolutas
            - nao substitua consultoria financeira profissional

            Estruture a resposta em:

            1. Diagnostico
            2. Principais riscos
            3. Pontos positivos
            4. Recomendacoes
            5. Plano de acao
            6. Conclusao
            """;
        }
    }
}
