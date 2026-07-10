using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore.Personas
{
    public class ExcelenteFluxoPoucoPatrimonioPersonaMfScore : PersonaMfScoreBase
    {
        public override CenarioMfScore CriarCenario()
        {
            var lancamentos = new List<Domain.Entities.Lancamento>();
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Receita, 8000m, 12, 5, "Salario"));
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Despesa, 4200m, 12, 10, "Despesas mensais"));

            var ativos = new List<Domain.Entities.BemPatrimonial>
            {
                CriarAtivo("Reserva liquida", EnumBemPatrimonial.DinheiroEmConta, 10000m),
                CriarAtivo("Investimento inicial", EnumBemPatrimonial.Investimento, 15000m),
                CriarAtivo("Equipamentos", EnumBemPatrimonial.Equipamento, 5000m)
            };

            var configuracao = CriarConfiguracao(20m, 100m, 6, 35m, 35m, 10m, 120000m);

            return new CenarioMfScore
            {
                Nome = "Excelente fluxo com pouco patrimonio",
                Descricao = "Renda estavel, economia mensal forte, boa reserva, pouco patrimonio acumulado e sem dividas relevantes.",
                ScoreEsperadoMin = 640,
                ScoreEsperadoMax = 780,
                Justificativa = "Fluxo forte e liquidez boa sustentam risco moderado controlado, mas ainda nao compensam comprometimento mensal acima de 50% e patrimonio alvo distante.",
                Observacoes = "Caso canonico revisto na calibracao v2.1: o perfil continua saudavel, mas nao deve ser tratado como risco muito baixo sem maior maturidade patrimonial.",
                Contexto = CriarContexto(lancamentos, ativos, [], configuracao),
                DadosEntrada = CriarDadosEntrada(8000m, 4200m, 25000m, 30000m, 0m, 4200m, 12600m, 25200m, 50400m)
            };
        }
    }
}
