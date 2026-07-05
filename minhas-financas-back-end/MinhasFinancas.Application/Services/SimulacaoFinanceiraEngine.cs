using MinhasFinancas.Application.DTOs.SimulacaoFinanceira;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.Services
{
    public class SimulacaoFinanceiraEngine
    {
        public ResultadoSimulacaoFinanceiraDTO Calcular(
            SimulacaoFinanceira simulacao,
            List<Lancamento> lancamentosReais)
        {
            var resultado = new ResultadoSimulacaoFinanceiraDTO();
            var dataBase = new DateTime(simulacao.DataInicial.Year, simulacao.DataInicial.Month, 1);
            var quantidadeMeses = Math.Min(12, simulacao.QuantidadeMeses <= 0 ? 12 : simulacao.QuantidadeMeses);
            var acoesAtivas = simulacao.Acoes.Where(x => x.Ativa).ToList();

            var lancamentosNoPeriodo = lancamentosReais
                .Where(x =>
                    x.StatusLancamento != EnumStatusLancamento.Cancelado &&
                    x.DataVencimento.Date >= dataBase.Date &&
                    x.DataVencimento.Date < dataBase.AddMonths(quantidadeMeses).Date)
                .ToList();

            for (var indiceMes = 0; indiceMes < quantidadeMeses; indiceMes++)
            {
                var mesAtual = dataBase.AddMonths(indiceMes);
                var inicioMes = new DateTime(mesAtual.Year, mesAtual.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);
                var chaveMes = $"{inicioMes.Year:D4}-{inicioMes.Month:D2}";

                var receitasReais = lancamentosNoPeriodo
                    .Where(x =>
                        x.Tipo == EnumTipoLancamento.Receita &&
                        x.DataVencimento.Date >= inicioMes.Date &&
                        x.DataVencimento.Date <= fimMes.Date)
                    .Sum(x => x.Valor);

                var despesasReais = lancamentosNoPeriodo
                    .Where(x =>
                        x.Tipo == EnumTipoLancamento.Despesa &&
                        x.DataVencimento.Date >= inicioMes.Date &&
                        x.DataVencimento.Date <= fimMes.Date)
                    .Sum(x => x.Valor);

                var receitasSimuladas = acoesAtivas
                    .Where(x => EhReceita(x.TipoAcao))
                    .Sum(x => CalcularImpactoMensal(x, inicioMes));

                var despesasSimuladas = acoesAtivas
                    .Where(x => EhDespesa(x.TipoAcao))
                    .Sum(x => CalcularImpactoMensal(x, inicioMes));

                var saldoReal = receitasReais - despesasReais;
                var saldoSimulado = (receitasReais + receitasSimuladas) - (despesasReais + despesasSimuladas);
                var diferenca = saldoSimulado - saldoReal;

                resultado.Linhas.Add(new LinhaResultadoSimulacaoFinanceiraDTO
                {
                    MesReferencia = chaveMes,
                    ReceitasReais = receitasReais,
                    DespesasReais = despesasReais,
                    SaldoReal = saldoReal,
                    ReceitasSimuladas = receitasSimuladas,
                    DespesasSimuladas = despesasSimuladas,
                    SaldoSimulado = saldoSimulado,
                    Diferenca = diferenca
                });
            }

            resultado.TotalReceitasReais = resultado.Linhas.Sum(x => x.ReceitasReais);
            resultado.TotalDespesasReais = resultado.Linhas.Sum(x => x.DespesasReais);
            resultado.SaldoRealAcumulado = resultado.Linhas.Sum(x => x.SaldoReal);
            resultado.TotalReceitasSimuladas = resultado.Linhas.Sum(x => x.ReceitasSimuladas);
            resultado.TotalDespesasSimuladas = resultado.Linhas.Sum(x => x.DespesasSimuladas);
            resultado.SaldoSimuladoAcumulado = resultado.Linhas.Sum(x => x.SaldoSimulado);
            resultado.DiferencaAcumulada = resultado.Linhas.Sum(x => x.Diferenca);

            return resultado;
        }

        private static decimal CalcularImpactoMensal(AcaoSimulacaoFinanceira acao, DateTime inicioMes)
        {
            var mesAcao = new DateTime(acao.DataInicial.Year, acao.DataInicial.Month, 1);

            return acao.TipoAcao switch
            {
                EnumTipoAcaoSimulacaoFinanceira.ReceitaUnica or EnumTipoAcaoSimulacaoFinanceira.DespesaUnica
                    => mesAcao == inicioMes ? acao.Valor : decimal.Zero,

                EnumTipoAcaoSimulacaoFinanceira.ReceitaRecorrenteMensal or EnumTipoAcaoSimulacaoFinanceira.DespesaRecorrenteMensal
                    => EstaDentroDaRecorrencia(acao, inicioMes) ? acao.Valor : decimal.Zero,

                EnumTipoAcaoSimulacaoFinanceira.DespesaParcelada
                    => EstaDentroDoParcelamento(acao, inicioMes)
                        ? DividirValorParcelado(acao.Valor, acao.QuantidadeParcelas ?? 1)
                        : decimal.Zero,

                _ => decimal.Zero
            };
        }

        private static bool EstaDentroDaRecorrencia(AcaoSimulacaoFinanceira acao, DateTime inicioMes)
        {
            var dataInicialMes = new DateTime(acao.DataInicial.Year, acao.DataInicial.Month, 1);
            var dataFinalMes = acao.DataFinal.HasValue
                ? new DateTime(acao.DataFinal.Value.Year, acao.DataFinal.Value.Month, 1)
                : (DateTime?)null;

            if (inicioMes < dataInicialMes)
            {
                return false;
            }

            return !dataFinalMes.HasValue || inicioMes <= dataFinalMes.Value;
        }

        private static bool EstaDentroDoParcelamento(AcaoSimulacaoFinanceira acao, DateTime inicioMes)
        {
            if (!acao.QuantidadeParcelas.HasValue || acao.QuantidadeParcelas.Value <= 1)
            {
                return false;
            }

            var dataInicialMes = new DateTime(acao.DataInicial.Year, acao.DataInicial.Month, 1);
            var diferencaMeses = ((inicioMes.Year - dataInicialMes.Year) * 12) + inicioMes.Month - dataInicialMes.Month;

            return diferencaMeses >= 0 && diferencaMeses < acao.QuantidadeParcelas.Value;
        }

        private static decimal DividirValorParcelado(decimal valorTotal, int quantidadeParcelas)
        {
            if (quantidadeParcelas <= 0)
            {
                return decimal.Zero;
            }

            return Math.Round(valorTotal / quantidadeParcelas, 2);
        }

        private static bool EhReceita(EnumTipoAcaoSimulacaoFinanceira tipoAcao)
        {
            return tipoAcao == EnumTipoAcaoSimulacaoFinanceira.ReceitaUnica
                || tipoAcao == EnumTipoAcaoSimulacaoFinanceira.ReceitaRecorrenteMensal;
        }

        private static bool EhDespesa(EnumTipoAcaoSimulacaoFinanceira tipoAcao)
        {
            return tipoAcao == EnumTipoAcaoSimulacaoFinanceira.DespesaUnica
                || tipoAcao == EnumTipoAcaoSimulacaoFinanceira.DespesaRecorrenteMensal
                || tipoAcao == EnumTipoAcaoSimulacaoFinanceira.DespesaParcelada;
        }
    }
}
