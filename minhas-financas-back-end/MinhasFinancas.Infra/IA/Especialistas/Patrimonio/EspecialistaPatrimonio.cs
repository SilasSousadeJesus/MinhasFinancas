using MinhasFinancas.Infra.IA.Especialistas.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas.Patrimonio
{
    public class EspecialistaPatrimonio : EspecialistaFinanceiroBase
    {
        public override string Nome => "Especialista em Patrimônio";

        public override ParecerEspecialistaIA Avaliar(ContextoAssistenteFinanceiro contexto)
        {
            var focoPatrimonio = ContemEmLista(contexto.InsightsPrioritarios, "patrimonio", "patrimônio", "investimento", "ativo") ||
                                 ContemTexto(contexto.ResumoExecutivo, "patrimonio", "patrimônio", "investimento", "ativo");

            var situacao = focoPatrimonio
                ? "O contexto mostra que patrimônio e construção de ativos já fazem parte da leitura financeira atual."
                : "O patrimônio ainda não aparece como destaque dominante no contexto atual, o que pede atenção estrutural.";

            var conclusao = focoPatrimonio
                ? "Existe base para continuar convertendo resultados mensais em patrimônio mais sólido."
                : "O patrimônio ainda precisa ganhar mais protagonismo dentro da estratégia financeira do usuário.";

            var riscos = focoPatrimonio
                ? new[] { "Se a conversão de sobra em patrimônio perder ritmo, o avanço de longo prazo pode desacelerar." }
                : new[]
                {
                    "A ausência de foco patrimonial pode deixar a evolução financeira concentrada apenas no curto prazo."
                };

            var pontosPositivos = focoPatrimonio
                ? new[] { "Há indicação de construção de base patrimonial relevante." }
                : new[] { "Ainda existe espaço claro para transformar renda em ativo de longo prazo." };

            var recomendacoes = focoPatrimonio
                ? new[]
                {
                    "Manter disciplina na conversão de excedentes em patrimônio.",
                    "Acompanhar a evolução do patrimônio ao longo dos meses."
                }
                : new[]
                {
                    "Dar mais visibilidade ao patrimônio na estratégia financeira.",
                    "Transformar parte da sobra em ativos de longo prazo."
                };

            return CriarParecer(Nome, situacao, conclusao, riscos, pontosPositivos, recomendacoes, "Estratégica", []);
        }
    }
}
