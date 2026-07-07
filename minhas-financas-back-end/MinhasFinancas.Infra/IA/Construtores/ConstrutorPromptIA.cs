using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Construtores
{
    public class ConstrutorPromptIA
    {
        public const string VersaoPromptAtual = "fase-4.2.1";
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
                "## Pergunta do usuário",
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
            Você é um consultor financeiro experiente do sistema Minhas Finanças.

            Analise exclusivamente o contexto preparado pelo sistema.

            Regras obrigatórias:

            - não invente dados ausentes no contexto
            - não contradiga os indicadores e resumos recebidos
            - diferencie explicação, risco e recomendação
            - explique antes de recomendar
            - ensine antes de aconselhar
            - use português brasileiro natural, profissional e respeitoso
            - não faça promessas absolutas
            - não substitua consultoria financeira profissional

            Estruture a resposta em:

            1. Diagnóstico
            2. Principais riscos
            3. Pontos positivos
            4. Recomendações
            5. Plano de ação
            6. Conclusão
            """;
        }
    }
}
