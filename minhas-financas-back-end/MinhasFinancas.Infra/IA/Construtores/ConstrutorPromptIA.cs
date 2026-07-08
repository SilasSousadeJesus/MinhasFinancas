using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Construtores
{
    public class ConstrutorPromptIA
    {
        public const string VersaoPromptAtual = "fase-4.2.3.3";
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
            - use a secao Plano Estrategico Financeiro como referencia oficial da direcao escolhida pelo usuario
            - use a secao Consistencia Estrategica como avaliacao oficial do alinhamento da decisao
            - nunca recalcular consistencia estrategica; aceite a avaliacao deterministica do sistema
            - nunca contradiga o Avaliador de Consistencia Estrategica
            - nao crie nem altere estrategia; apenas interprete o plano vigente
            - explique por que a decisao esta alinhada ou desalinhada ao plano vigente
            - cite os objetivos impactados quando houver consistencia estrategica
            - quando houver plano estrategico, avalie se a leitura executiva esta alinhada com ele
            - quando nao houver plano vigente, deixe isso explicito sem inventar direcionamento
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
