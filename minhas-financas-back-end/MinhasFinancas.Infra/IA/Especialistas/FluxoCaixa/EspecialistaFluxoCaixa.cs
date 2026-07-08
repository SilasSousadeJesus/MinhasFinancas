using MinhasFinancas.Infra.IA.Especialistas.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas.FluxoCaixa
{
    public class EspecialistaFluxoCaixa : EspecialistaFinanceiroBase
    {
        public override string Nome => "Especialista em Fluxo de Caixa";

        public override ParecerEspecialistaIA Avaliar(ContextoAssistenteFinanceiro contexto)
        {
            var positivo = ContemEmLista(contexto.DestaquesPositivos, "economia", "sobra", "saldo", "folga") ||
                           ContemTexto(contexto.ResumoExecutivo, "economia", "sobra", "saldo", "folga");

            var pressionado = ContemEmLista(contexto.PrioridadesImediatas, "gasto", "despesa", "comprometimento", "reserva") ||
                              ContemEmLista(contexto.ConsistenciaEstrategica.MotivosDesfavoraveis, "gasto", "despesa", "comprometimento");

            var situacao = positivo
                ? "O fluxo de caixa mostra capacidade de gerar sobra e sustentar decisões financeiras mais conscientes."
                : "O fluxo de caixa exige leitura atenta, porque a folga mensal ainda pode estar limitada.";

            var conclusao = pressionado
                ? "A folga de caixa deve continuar sendo protegida para evitar que decisões de curto prazo reduzam a capacidade de execução."
                : "O fluxo de caixa atual oferece base para organizar prioridades e avançar com mais previsibilidade.";

            var riscos = pressionado
                ? new[]
                {
                    "Uma nova pressão de despesas pode reduzir rapidamente a margem disponível.",
                    "Compromissos futuros precisam ser acompanhados para não consumir a sobra planejada."
                }
                : new[] { "Sem disciplina, a sobra mensal pode ser absorvida por decisões de consumo pouco prioritárias." };

            var pontosPositivos = positivo
                ? new[] { "Existe sinal de capacidade real de geração de sobra mensal." }
                : new[] { "O cenário ainda pode ser organizado com maior previsibilidade." };

            var recomendacoes = pressionado
                ? new[]
                {
                    "Proteger a folga mensal antes de assumir novas pressões.",
                    "Priorizar despesas que preservem a execução do plano."
                }
                : new[]
                {
                    "Direcionar a sobra mensal para prioridades estratégicas.",
                    "Manter o ritmo de acompanhamento do caixa."
                };

            return CriarParecer(Nome, situacao, conclusao, riscos, pontosPositivos, recomendacoes, pressionado ? "Estratégica" : "Longo prazo", []);
        }
    }
}
