using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using System.Globalization;

namespace MinhasFinancas.Domain.Services.DashBoard
{
    public class Dashboard
    {
        private int anoCorrente;
        private int mesCorrente;
        private int mesAnterior;

        public Dashboard(List<Lancamento> listaLancamentos)
        {
            anoCorrente = DateTime.Now.Year;
            mesCorrente = DateTime.Now.Month;
            mesAnterior = mesCorrente == 1 ? 12 : mesCorrente - 1;

            Calcular(listaLancamentos);
        }


        public ReceitaDashBoard Receita { get; set; }
        public DespesaDashBoard Despesa { get; set; }
        public InvestimentoDashBoard Investimento { get; set; }
        public ResultadoDashBoard Resultado { get; set; }
        public List<ReceitaDespesaMensal> ReceitasDespesasMensais { get; set; }





        private void Calcular(List<Lancamento> listaLancamentos)
        {
            Receita = CalcularReceitas(listaLancamentos);
            Despesa = CalcularDespesas(listaLancamentos);
            Investimento = CalcularInvestimentos(listaLancamentos);
            Resultado = CalcularResultados(listaLancamentos);
            ReceitasDespesasMensais = CalcularReceitasDespesasMensais(listaLancamentos);
        }

        private ReceitaDashBoard CalcularReceitas(List<Lancamento> listaLancamentos)
        {
            var receitaAnoCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.Tipo == EnumTipoLancamento.Receita)
                .Sum(x => x.Valor);

            var receitaMesCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesCorrente && x.Tipo == EnumTipoLancamento.Receita)
                .Sum(x => x.Valor);

            var receitaMesAnterior = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesAnterior && x.Tipo == EnumTipoLancamento.Receita)
                .Sum(x => x.Valor);

            return new ReceitaDashBoard
            {
                ReceitaAnoCorrente = receitaAnoCorrente.ToString("C", new CultureInfo("pt-BR")),
                ReceitaMesCorrente = receitaMesCorrente.ToString("C", new CultureInfo("pt-BR")),
                ReceitaMesPassado = receitaMesAnterior.ToString("C", new CultureInfo("pt-BR"))
            };
        }

        private DespesaDashBoard CalcularDespesas(List<Lancamento> listaLancamentos)
        {
            var despesaAnoCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.Tipo == EnumTipoLancamento.Despesa)
                .Sum(x => x.Valor);

            var despesaMesCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesCorrente && x.Tipo == EnumTipoLancamento.Despesa)
                .Sum(x => x.Valor);

            var despesaMesAnterior = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesAnterior && x.Tipo == EnumTipoLancamento.Despesa)
                .Sum(x => x.Valor);

            return new DespesaDashBoard
            {
                DespesasAnoCorrente = despesaAnoCorrente.ToString("C", new CultureInfo("pt-BR")),
                DespesasMesCorrente = despesaMesCorrente.ToString("C", new CultureInfo("pt-BR")),
                DespesasMesPassado = despesaMesAnterior.ToString("C", new CultureInfo("pt-BR"))
            };
        }

        private InvestimentoDashBoard CalcularInvestimentos(List<Lancamento> listaLancamentos)
        {
            var InvestimentoAnoCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.Tipo == EnumTipoLancamento.Investimento)
                .Sum(x => x.Valor);

            var InvestimentoMesCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesCorrente && x.Tipo == EnumTipoLancamento.Investimento)
                .Sum(x => x.Valor);

            var InvestimentoMesAnterior = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesAnterior && x.Tipo == EnumTipoLancamento.Investimento)
                .Sum(x => x.Valor);

            return new InvestimentoDashBoard
            {
                InvestimentoAnoCorrente = InvestimentoAnoCorrente.ToString("C", new CultureInfo("pt-BR")),
                InvestimentoMesCorrente = InvestimentoMesCorrente.ToString("C", new CultureInfo("pt-BR")),
                InvestimentoMesPassado = InvestimentoMesAnterior.ToString("C", new CultureInfo("pt-BR"))
            };
        }

        private ResultadoDashBoard CalcularResultados(List<Lancamento> listaLancamentos)
        {
            var ResultadoAnoCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.Tipo == EnumTipoLancamento.Investimento)
                .Sum(x => x.Valor);

            var ResultadoMesCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesCorrente && x.Tipo == EnumTipoLancamento.Investimento)
                .Sum(x => x.Valor);

            var ResultadoMesAnterior = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesAnterior && x.Tipo == EnumTipoLancamento.Investimento)
                .Sum(x => x.Valor);

            return new ResultadoDashBoard
            {
                ResultadoAnoCorrente = ResultadoAnoCorrente.ToString("C", new CultureInfo("pt-BR")),
                ResultadoMesCorrente = ResultadoMesCorrente.ToString("C", new CultureInfo("pt-BR")),
                ResultadoMesPassado = ResultadoMesAnterior.ToString("C", new CultureInfo("pt-BR"))
            };
        }

        public List<ReceitaDespesaMensal> CalcularReceitasDespesasMensais(List<Lancamento> listaLancamentos)
        {
            var receitasDespesasMensais = new Dictionary<string, (decimal Receita, decimal Despesa)>();

            foreach (var lancamento in listaLancamentos)
            {
                var key = $"{lancamento.DataPagamento.Year}-{lancamento.DataPagamento.Month:00}";

                if (!receitasDespesasMensais.ContainsKey(key))
                {
                    receitasDespesasMensais[key] = (0, 0);
                }

                if (lancamento.Tipo == EnumTipoLancamento.Receita)
                {
                    receitasDespesasMensais[key] = (receitasDespesasMensais[key].Receita + lancamento.Valor, receitasDespesasMensais[key].Despesa);
                }
                else if (lancamento.Tipo == EnumTipoLancamento.Despesa)
                {
                    receitasDespesasMensais[key] = (receitasDespesasMensais[key].Receita, receitasDespesasMensais[key].Despesa + lancamento.Valor);
                }
            }

            return receitasDespesasMensais.Select(kvp => new ReceitaDespesaMensal
            {
                MesAno = kvp.Key,
                Receita = kvp.Value.Receita.ToString("C", new CultureInfo("pt-BR")),
                Despesa = kvp.Value.Despesa.ToString("C", new CultureInfo("pt-BR"))
            }).ToList();
        }
    }

}
