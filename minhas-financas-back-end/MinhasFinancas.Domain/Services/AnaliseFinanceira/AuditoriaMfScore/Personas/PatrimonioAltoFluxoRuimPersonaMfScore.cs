using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore.Personas
{
    public class PatrimonioAltoFluxoRuimPersonaMfScore : PersonaMfScoreBase
    {
        public override CenarioMfScore CriarCenario()
        {
            var lancamentos = new List<Domain.Entities.Lancamento>();
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Receita, 12000m, 12, 5, "Receita principal"));
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Despesa, 10500m, 12, 12, "Despesas pesadas"));

            var ativos = new List<Domain.Entities.BemPatrimonial>
            {
                CriarAtivo("Imovel comercial", EnumBemPatrimonial.Imovel, 450000m),
                CriarAtivo("Investimentos iliquidos", EnumBemPatrimonial.Investimento, 120000m),
                CriarAtivo("Conta reserva curta", EnumBemPatrimonial.DinheiroEmConta, 3000m)
            };

            var passivos = new List<Domain.Entities.Passivo>
            {
                CriarPassivo("Financiamento patrimonial", EnumPassivo.Financiamento, 220000m)
            };

            var configuracao = CriarConfiguracao(25m, 100m, 6, 35m, 40m, 10m, 500000m);

            return new CenarioMfScore
            {
                Nome = "Patrimonio alto com fluxo ruim",
                Descricao = "Patrimonio forte, mas com folga mensal pequena, liquidez baixa e comprometimento elevado.",
                ScoreEsperadoMin = 55,
                ScoreEsperadoMax = 75,
                Justificativa = "O patrimonio reduz o risco estrutural, mas nao pode mascarar fluxo apertado e pressao recorrente.",
                Contexto = CriarContexto(lancamentos, ativos, passivos, configuracao),
                DadosEntrada = CriarDadosEntrada(12000m, 10500m, 3000m, 573000m, 220000m, 10500m, 31500m, 63000m, 126000m)
            };
        }
    }
}
