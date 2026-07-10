using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore.Personas
{
    public class PlanejamentoExcelentePersonaMfScore : PersonaMfScoreBase
    {
        public override CenarioMfScore CriarCenario()
        {
            var lancamentos = new List<Domain.Entities.Lancamento>();
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Receita, 9000m, 12, 5, "Receita principal"));
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Despesa, 4500m, 12, 10, "Despesas controladas"));

            var ativos = new List<Domain.Entities.BemPatrimonial>
            {
                CriarAtivo("Reserva financeira", EnumBemPatrimonial.DinheiroEmConta, 18000m),
                CriarAtivo("Investimentos regulares", EnumBemPatrimonial.Investimento, 12000m),
                CriarAtivo("Veiculo quitado", EnumBemPatrimonial.Automovel, 20000m)
            };

            var passivos = new List<Domain.Entities.Passivo>
            {
                CriarPassivo("Passivo residual", EnumPassivo.ObrigacaoFinanceira, 5000m)
            };

            var configuracao = CriarConfiguracao(25m, 100m, 6, 35m, 35m, 15m, 100000m);
            var metas = new List<Domain.Entities.Meta>
            {
                CriarMeta("Reserva de seguranca", 50000m, 30000m, DataReferenciaBase.AddYears(2))
            };

            return new CenarioMfScore
            {
                Nome = "Planejamento excelente",
                Descricao = "Perfil bem configurado, meta registrada e bons sinais de organizacao financeira, sem deixar o planejamento dominar o score.",
                ScoreEsperadoMin = 720,
                ScoreEsperadoMax = 840,
                Justificativa = "Planejamento bem configurado deve fortalecer a nota, mas continua subordinado ao fluxo real, a liquidez, ao endividamento e ao progresso patrimonial.",
                Observacoes = "Caso canonico revisto na calibracao v2.1: o pilar de planejamento agora considera explicitamente os parametros basicos do perfil financeiro, sem sobrepor a realidade operacional.",
                Contexto = CriarContexto(lancamentos, ativos, passivos, configuracao, metas),
                DadosEntrada = CriarDadosEntrada(9000m, 4500m, 30000m, 50000m, 5000m, 4500m, 13500m, 27000m, 54000m)
            };
        }
    }
}
