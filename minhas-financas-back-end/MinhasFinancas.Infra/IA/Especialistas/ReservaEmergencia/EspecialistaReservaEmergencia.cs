using MinhasFinancas.Infra.IA.Especialistas.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas.ReservaEmergencia
{
    public class EspecialistaReservaEmergencia : EspecialistaFinanceiroBase
    {
        public override string Nome => "Especialista em Reserva de Emergência";

        public override ParecerEspecialistaIA Avaliar(ContextoAssistenteFinanceiro contexto)
        {
            var focoReserva = ContemEmLista(contexto.PrioridadesImediatas, "reserva", "emergencia", "emergência") ||
                              ContemTexto(contexto.ResumoExecutivo, "reserva", "emergencia", "emergência");

            var situacao = focoReserva
                ? "A reserva de emergência segue como peça central da proteção financeira do usuário."
                : "A reserva de emergência ainda não parece ocupar o centro da leitura atual, apesar de continuar sendo estratégica.";

            var conclusao = focoReserva
                ? "A proteção contra imprevistos ainda deve ser fortalecida antes de ampliar novas frentes financeiras."
                : "A reserva precisa ganhar prioridade mais explícita na alocação da sobra mensal.";

            var riscos = new[]
            {
                "Uma reserva insuficiente reduz a capacidade de enfrentar imprevistos sem recorrer a crédito."
            };

            var pontosPositivos = focoReserva
                ? new[] { "A proteção financeira já está sendo tratada como prioridade explícita." }
                : new[] { "Existe oportunidade clara de estruturar uma base de proteção mais robusta." };

            var recomendacoes = new[]
            {
                "Destinar a capacidade mensal disponível para fortalecer a reserva.",
                "Evitar que a reserva fique em segundo plano diante de novas compras."
            };

            return CriarParecer(Nome, situacao, conclusao, riscos, pontosPositivos, recomendacoes, "Crítica", []);
        }
    }
}
