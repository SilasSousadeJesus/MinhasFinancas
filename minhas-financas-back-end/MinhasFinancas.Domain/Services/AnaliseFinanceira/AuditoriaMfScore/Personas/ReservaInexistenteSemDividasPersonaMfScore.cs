using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore.Personas
{
    public class ReservaInexistenteSemDividasPersonaMfScore : PersonaMfScoreBase
    {
        public override CenarioMfScore CriarCenario()
        {
            var lancamentos = new List<Domain.Entities.Lancamento>();
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Receita, 7000m, 12, 5, "Receita mensal"));
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Despesa, 4200m, 12, 10, "Despesas recorrentes"));

            var ativos = new List<Domain.Entities.BemPatrimonial>
            {
                CriarAtivo("Equipamento de trabalho", EnumBemPatrimonial.Equipamento, 5000m)
            };

            var configuracao = CriarConfiguracao(20m, 100m, 6, 35m, 30m, 10m, 90000m);

            return new CenarioMfScore
            {
                Nome = "Reserva inexistente sem dividas",
                Descricao = "Sem dividas relevantes, fluxo razoavel, mas sem protecao liquida para imprevistos.",
                ScoreEsperadoMin = 50,
                ScoreEsperadoMax = 79,
                Justificativa = "A ausencia de reserva impede score excelente, mesmo com fluxo razoavel e sem passivos relevantes.",
                Contexto = CriarContexto(lancamentos, ativos, [], configuracao),
                DadosEntrada = CriarDadosEntrada(7000m, 4200m, 0m, 5000m, 0m, 4200m, 12600m, 25200m, 50400m)
            };
        }
    }
}
