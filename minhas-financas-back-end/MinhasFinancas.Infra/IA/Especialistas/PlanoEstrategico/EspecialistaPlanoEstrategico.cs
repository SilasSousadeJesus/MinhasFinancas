using MinhasFinancas.Infra.IA.Especialistas.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas.PlanoEstrategico
{
    public class EspecialistaPlanoEstrategico : EspecialistaFinanceiroBase
    {
        public override string Nome => "Especialista em Plano Estratégico";

        public override ParecerEspecialistaIA Avaliar(ContextoAssistenteFinanceiro contexto)
        {
            var planoPresente = contexto.InterpretacaoPlanoEstrategico.PossuiPlanoVigente;

            var situacao = planoPresente
                ? $"Existe um plano estratégico vigente orientando a leitura financeira atual, com {contexto.InterpretacaoPlanoEstrategico.PrioridadesEstrategicas.Count} prioridade(s) destacada(s)."
                : "Ainda não existe um plano estratégico vigente para orientar a leitura do assistente.";

            var conclusao = planoPresente
                ? "A direção escolhida pelo usuário já pode ser usada como referência para interpretar prioridades e riscos."
                : "Sem plano vigente, a análise estratégica fica menos precisa e mais dependente do momento operacional.";

            IEnumerable<string> riscos = planoPresente
                ? contexto.ConsistenciaEstrategica.MotivosDesfavoraveis.Count > 0
                    ? contexto.ConsistenciaEstrategica.MotivosDesfavoraveis
                    : new[] { "Sem conflitos explícitos, mas o plano precisa continuar sendo acompanhado com disciplina." }
                : new[] { "A ausência de plano vigente reduz a clareza de direção para decisões de médio e longo prazo." };

            IEnumerable<string> pontosPositivos = planoPresente
                ? contexto.InterpretacaoPlanoEstrategico.PrioridadesEstrategicas.Count > 0
                    ? contexto.InterpretacaoPlanoEstrategico.PrioridadesEstrategicas
                    : new[] { "Há um plano vigente que já organiza a direção escolhida pelo usuário." }
                : new[] { "Ainda há oportunidade de formalizar uma direção estratégica mais clara." };

            IEnumerable<string> recomendacoes = planoPresente
                ? new[]
                {
                    "Manter o plano como referência principal para decisões relevantes.",
                    "Revisar o alinhamento entre plano, compromissos e fluxo mensal."
                }
                : new[]
                {
                    "Formalizar um plano estratégico para dar direção às próximas decisões.",
                    "Definir prioridades de longo prazo com mais clareza."
                };

            return CriarParecer(
                Nome,
                situacao,
                conclusao,
                riscos,
                pontosPositivos,
                recomendacoes,
                planoPresente ? "Estratégica" : "Longo prazo",
                contexto.InterpretacaoPlanoEstrategico.AlertasEstrategicos);
        }
    }
}
