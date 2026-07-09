using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore.Personas
{
    public class VidaFinanceiraExcelentePersonaMfScore : PersonaMfScoreBase
    {
        public override CenarioMfScore CriarCenario()
        {
            var lancamentos = new List<Domain.Entities.Lancamento>();
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Receita, 15000m, 12, 5, "Salario principal"));
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Despesa, 4000m, 12, 10, "Custo de vida"));

            var ativos = new List<Domain.Entities.BemPatrimonial>
            {
                CriarAtivo("Conta principal", EnumBemPatrimonial.DinheiroEmConta, 30000m),
                CriarAtivo("Investimentos", EnumBemPatrimonial.Investimento, 90000m),
                CriarAtivo("Imovel residencial", EnumBemPatrimonial.Imovel, 280000m)
            };

            var configuracao = CriarConfiguracao(25m, 100m, 12, 35m, 30m, 10m, 350000m);

            return new CenarioMfScore
            {
                Nome = "Vida Financeira Excelente",
                Descricao = "Alta renda, reserva elevada, patrimonio forte, sem dividas e fluxo de caixa amplamente positivo.",
                ScoreEsperadoMin = 90,
                ScoreEsperadoMax = 100,
                Justificativa = "Combina protecao, liquidez, patrimonio e baixa pressao financeira em todos os horizontes.",
                Contexto = CriarContexto(lancamentos, ativos, [], configuracao),
                DadosEntrada = CriarDadosEntrada(15000m, 4000m, 120000m, 400000m, 0m, 4000m, 12000m, 24000m, 48000m)
            };
        }
    }
}
