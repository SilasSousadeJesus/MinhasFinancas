using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore.Personas
{
    public class BoaRendaReservaZeroCartaoAltoPersonaMfScore : PersonaMfScoreBase
    {
        public override CenarioMfScore CriarCenario()
        {
            var lancamentos = new List<Domain.Entities.Lancamento>();
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Receita, 8000m, 12, 5, "Salario"));
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Despesa, 5500m, 12, 10, "Fatura e custo fixo"));

            var ativos = new List<Domain.Entities.BemPatrimonial>
            {
                CriarAtivo("Conta corrente", EnumBemPatrimonial.DinheiroEmConta, 0m),
                CriarAtivo("Equipamentos", EnumBemPatrimonial.Equipamento, 12000m)
            };

            var passivos = new List<Domain.Entities.Passivo>
            {
                CriarPassivo("Cartao rotativo acumulado", EnumPassivo.Divida, 18000m)
            };

            var configuracao = CriarConfiguracao(20m, 100m, 6, 35m, 35m, 10m, 80000m);

            return new CenarioMfScore
            {
                Nome = "Boa renda, reserva zero e cartao alto",
                Descricao = "Renda boa, mas sem reserva e com forte pressao de cartao e obrigacoes recorrentes.",
                ScoreEsperadoMin = 600,
                ScoreEsperadoMax = 740,
                Justificativa = "A renda sustenta algum equilibrio, mas a ausencia de reserva e a pressao recorrente impedem um score alto.",
                Contexto = CriarContexto(lancamentos, ativos, passivos, configuracao),
                DadosEntrada = CriarDadosEntrada(8000m, 5500m, 0m, 12000m, 18000m, 5500m, 16500m, 33000m, 66000m)
            };
        }
    }
}
