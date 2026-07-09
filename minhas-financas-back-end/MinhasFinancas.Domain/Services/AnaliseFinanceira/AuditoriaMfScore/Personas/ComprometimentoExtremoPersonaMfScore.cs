using MinhasFinancas.CrossCutting.Util.Enum;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore.Personas
{
    public class ComprometimentoExtremoPersonaMfScore : PersonaMfScoreBase
    {
        public override CenarioMfScore CriarCenario()
        {
            var lancamentos = new List<Domain.Entities.Lancamento>();
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Receita, 10000m, 12, 5, "Receita recorrente"));
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Despesa, 8800m, 12, 10, "Obrigacoes principais"));
            lancamentos.AddRange(CriarLancamentosMensais(EnumTipoLancamento.Despesa, 700m, 12, 20, "Despesas variaveis fixas"));

            var ativos = new List<Domain.Entities.BemPatrimonial>
            {
                CriarAtivo("Conta reserva minima", EnumBemPatrimonial.DinheiroEmConta, 1000m),
                CriarAtivo("Veiculo", EnumBemPatrimonial.Automovel, 18000m)
            };

            var passivos = new List<Domain.Entities.Passivo>
            {
                CriarPassivo("Emprestimo de consumo", EnumPassivo.Emprestimo, 35000m)
            };

            var configuracao = CriarConfiguracao(20m, 100m, 6, 35m, 35m, 10m, 150000m);

            return new CenarioMfScore
            {
                Nome = "Comprometimento extremo",
                Descricao = "Renda fortemente comprometida, baixa folga mensal, reserva pequena e pressao recorrente alta.",
                ScoreEsperadoMin = 0,
                ScoreEsperadoMax = 59,
                Justificativa = "Comprometimento acima de 80 por cento e pressao futura forte devem limitar severamente a nota final.",
                Contexto = CriarContexto(lancamentos, ativos, passivos, configuracao),
                DadosEntrada = CriarDadosEntrada(10000m, 9500m, 1000m, 19000m, 35000m, 9500m, 28500m, 57000m, 114000m)
            };
        }
    }
}
