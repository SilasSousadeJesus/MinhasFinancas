using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore.Personas
{
    public class InadimplenciaPersonaMfScore : PersonaMfScoreBase
    {
        public override CenarioMfScore CriarCenario()
        {
            var dataReferencia = new DateTime(2026, 7, 15);
            var lancamentos = new List<Domain.Entities.Lancamento>
            {
                CriarLancamentoAvulso(EnumTipoLancamento.Receita, 4500m, new DateTime(2026, 7, 5), "Receita principal julho"),
                CriarLancamentoAvulso(EnumTipoLancamento.Despesa, 3200m, new DateTime(2026, 7, 3), "Aluguel atrasado"),
                CriarLancamentoAvulso(EnumTipoLancamento.Despesa, 1800m, new DateTime(2026, 7, 8), "Cartao vencido"),
                CriarLancamentoAvulso(EnumTipoLancamento.Despesa, 1600m, new DateTime(2026, 7, 20), "Obrigacoes restantes de julho"),
                CriarLancamentoAvulso(EnumTipoLancamento.Receita, 4500m, new DateTime(2026, 8, 5), "Receita principal agosto"),
                CriarLancamentoAvulso(EnumTipoLancamento.Despesa, 4200m, new DateTime(2026, 8, 10), "Pressao de agosto")
            };

            var ativos = new List<Domain.Entities.BemPatrimonial>
            {
                CriarAtivo("Conta zerada", EnumBemPatrimonial.DinheiroEmConta, 0m)
            };

            var passivos = new List<Domain.Entities.Passivo>
            {
                CriarPassivo("Dividas acumuladas", EnumPassivo.Divida, 25000m)
            };

            var configuracao = CriarConfiguracao(20m, 100m, 6, 35m, 35m, 10m, 70000m);

            return new CenarioMfScore
            {
                Nome = "Inadimplencia",
                Descricao = "Atrasos relevantes, pressao de caixa imediata, reserva inexistente e passivos elevados.",
                ScoreEsperadoMin = 0,
                ScoreEsperadoMax = 49,
                Justificativa = "Mesmo sem um campo explicito de inadimplencia no modelo, a melhor aproximacao deve produzir penalizacao forte.",
                Observacoes = "Limitacao conhecida: o dominio ainda nao possui um marcador explicito de inadimplencia; o cenario usa atrasos pendentes e passivos altos como aproximacao.",
                Contexto = CriarContexto(lancamentos, ativos, passivos, configuracao, dataReferencia: dataReferencia),
                DadosEntrada = CriarDadosEntrada(4500m, 6600m, 0m, 0m, 25000m, 5800m, 10000m, 10000m, 10000m)
            };
        }
    }
}
