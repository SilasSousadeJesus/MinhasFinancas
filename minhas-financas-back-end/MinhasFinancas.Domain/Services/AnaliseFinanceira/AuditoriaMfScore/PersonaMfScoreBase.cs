using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.AuditoriaMfScore
{
    public abstract class PersonaMfScoreBase : IPersonaMfScore
    {
        protected static readonly DateTime DataReferenciaBase = new(2026, 7, 1);

        public abstract CenarioMfScore CriarCenario();

        protected static ContextoAnaliseFinanceira CriarContexto(
            IEnumerable<Lancamento> lancamentos,
            IEnumerable<BemPatrimonial> ativos,
            IEnumerable<Passivo> passivos,
            ConfiguracaoPerfilFinanceiro? configuracaoPerfilFinanceiro,
            IEnumerable<Meta>? metas = null,
            DateTime? dataReferencia = null)
        {
            return new ContextoAnaliseFinanceira
            {
                DataReferencia = dataReferencia ?? DataReferenciaBase,
                Lancamentos = lancamentos.ToList(),
                Ativos = ativos.ToList(),
                Passivos = passivos.ToList(),
                Metas = metas?.ToList() ?? [],
                ConfiguracaoPerfilFinanceiro = configuracaoPerfilFinanceiro
            };
        }

        protected static ConfiguracaoPerfilFinanceiro CriarConfiguracao(
            decimal percentualEconomiaMensalDesejado,
            decimal percentualReservaEmergenciaDesejado,
            int mesesReservaEmergenciaDesejados,
            decimal percentualMaximoComprometimentoRenda,
            decimal percentualMaximoEndividamento,
            decimal percentualMinimoInvestimento,
            decimal? patrimonioLiquidoAlvo)
        {
            return new ConfiguracaoPerfilFinanceiro
            {
                Id = Guid.NewGuid(),
                PerfilFinanceiroId = Guid.NewGuid(),
                DataInicioVigencia = DataReferenciaBase.AddMonths(-1),
                DataCriacao = DataReferenciaBase.AddMonths(-1),
                PercentualEconomiaMensalDesejado = percentualEconomiaMensalDesejado,
                PercentualReservaEmergenciaDesejado = percentualReservaEmergenciaDesejado,
                MesesReservaEmergenciaDesejados = mesesReservaEmergenciaDesejados,
                PercentualMaximoComprometimentoRenda = percentualMaximoComprometimentoRenda,
                PercentualMaximoEndividamento = percentualMaximoEndividamento,
                PercentualMinimoInvestimento = percentualMinimoInvestimento,
                PatrimonioLiquidoAlvo = patrimonioLiquidoAlvo,
                OrigemPerfilFinanceiro = EnumOrigemPerfilFinanceiro.PersonalizadoPeloUsuario
            };
        }

        protected static List<Lancamento> CriarLancamentosMensais(
            EnumTipoLancamento tipo,
            decimal valor,
            int quantidadeMeses,
            int diaVencimento,
            string descricaoBase,
            DateTime? dataInicial = null)
        {
            var inicio = dataInicial ?? DataReferenciaBase;
            var status = tipo == EnumTipoLancamento.Receita
                ? EnumStatusLancamento.Pendente
                : EnumStatusLancamento.Pendente;

            return Enumerable.Range(0, quantidadeMeses)
                .Select(indice =>
                {
                    var referencia = inicio.AddMonths(indice);
                    var dataVencimento = AjustarDia(referencia, diaVencimento);

                    return new Lancamento
                    {
                        Id = Guid.NewGuid(),
                        Valor = valor,
                        Descricao = $"{descricaoBase} {indice + 1}/{quantidadeMeses}",
                        Observacao = descricaoBase,
                        DataVencimento = dataVencimento,
                        DataLancamento = inicio.AddDays(-1),
                        StatusLancamento = status,
                        FrequenciaLancamento = EnumTipoFrequenciaLancamento.Pontual,
                        Tipo = tipo,
                        Vinculo = EnumVinculoLancamento.Avulso
                    };
                })
                .ToList();
        }

        protected static Lancamento CriarLancamentoAvulso(
            EnumTipoLancamento tipo,
            decimal valor,
            DateTime dataVencimento,
            string descricao,
            EnumStatusLancamento status = EnumStatusLancamento.Pendente)
        {
            return new Lancamento
            {
                Id = Guid.NewGuid(),
                Valor = valor,
                Descricao = descricao,
                Observacao = descricao,
                DataVencimento = dataVencimento,
                DataLancamento = dataVencimento.AddDays(-10),
                StatusLancamento = status,
                FrequenciaLancamento = EnumTipoFrequenciaLancamento.Pontual,
                Tipo = tipo,
                Vinculo = EnumVinculoLancamento.Avulso
            };
        }

        protected static BemPatrimonial CriarAtivo(string nome, EnumBemPatrimonial tipo, decimal valorAtual)
        {
            var id = Guid.NewGuid();

            return new BemPatrimonial
            {
                Id = id,
                NomeBemPatrimonial = nome,
                Descricao = nome,
                Ativo = true,
                Permanencia = true,
                DataCadastro = DataReferenciaBase.AddMonths(-6),
                DataAquisicao = DataReferenciaBase.AddYears(-1),
                Tipo = tipo,
                DataPermanencia =
                [
                    new PermanenciaBemMaterial
                    {
                        Id = Guid.NewGuid(),
                        BemPatrimonialId = id,
                        DataPermanencia = DataReferenciaBase,
                        Valor = valorAtual
                    }
                ]
            };
        }

        protected static Passivo CriarPassivo(string nome, EnumPassivo tipo, decimal valorAtual)
        {
            var id = Guid.NewGuid();

            return new Passivo
            {
                Id = id,
                NomePassivo = nome,
                Descricao = nome,
                Ativo = true,
                Permanencia = true,
                DataCadastro = DataReferenciaBase.AddMonths(-6),
                DataInicio = DataReferenciaBase.AddYears(-1),
                Tipo = tipo,
                DataPermanencia =
                [
                    new PermanenciaPassivo
                    {
                        Id = Guid.NewGuid(),
                        PassivoId = id,
                        DataPermanencia = DataReferenciaBase,
                        Valor = valorAtual
                    }
                ]
            };
        }

        protected static Meta CriarMeta(string nomeMeta, decimal valorFinal, decimal valorAtual, DateTime dataFim)
        {
            var meta = new Meta
            {
                NomeMeta = nomeMeta,
                ValorFinal = valorFinal,
                ValorAtual = valorAtual,
                DataInicio = DataReferenciaBase,
                DataFim = dataFim
            };

            meta.CalcularDiferenca();
            return meta;
        }

        protected static DadosEntradaPersonaMfScore CriarDadosEntrada(
            decimal renda,
            decimal despesas,
            decimal reserva,
            decimal patrimonio,
            decimal passivos,
            decimal obrigacoesFuturas30Dias,
            decimal obrigacoesFuturas90Dias,
            decimal obrigacoesFuturas180Dias,
            decimal obrigacoesFuturas12Meses)
        {
            return new DadosEntradaPersonaMfScore
            {
                Renda = renda,
                Despesas = despesas,
                Reserva = reserva,
                Patrimonio = patrimonio,
                Passivos = passivos,
                ObrigacoesFuturas30Dias = obrigacoesFuturas30Dias,
                ObrigacoesFuturas90Dias = obrigacoesFuturas90Dias,
                ObrigacoesFuturas180Dias = obrigacoesFuturas180Dias,
                ObrigacoesFuturas12Meses = obrigacoesFuturas12Meses
            };
        }

        private static DateTime AjustarDia(DateTime data, int dia)
        {
            var ultimoDia = DateTime.DaysInMonth(data.Year, data.Month);
            return new DateTime(data.Year, data.Month, Math.Min(dia, ultimoDia));
        }
    }
}
