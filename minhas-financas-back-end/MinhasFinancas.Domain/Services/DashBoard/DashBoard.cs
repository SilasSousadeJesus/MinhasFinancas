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
        public ContasApagarDashboard ContasApagarDashboard { get; set; }
        public List<ReceitaDespesaMensal> ReceitasDespesasMensais { get; set; }
        public List<InvestimentoMensal> AcumuloInvestimentoMensal { get; set; }
        public List<LancamentosPorCategoriaDashboard> LancamentosPorCategoriaDeDespesaDashboard { get; set; }

        private void Calcular(List<Lancamento> listaLancamentos)
        {
            Receita = CalcularReceitas(listaLancamentos);
            Despesa = CalcularDespesas(listaLancamentos);
            Investimento = CalcularInvestimentos(listaLancamentos);
            Resultado = CalcularResultados(listaLancamentos);
            ReceitasDespesasMensais = CalcularReceitasDespesasMensais(listaLancamentos);
            ContasApagarDashboard = CalcularContasApagar(listaLancamentos);
            AcumuloInvestimentoMensal = CalcularAcumuloInvestimento(listaLancamentos);
            LancamentosPorCategoriaDeDespesaDashboard = AgruparLancamentosPorCategoriaDespesa(listaLancamentos);
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
            var receitaAnoCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.Tipo == EnumTipoLancamento.Receita)
                .Sum(x => x.Valor);

            var despesaAnoCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.Tipo == EnumTipoLancamento.Despesa)
                .Sum(x => x.Valor);


            var receitaMesCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesCorrente && x.Tipo == EnumTipoLancamento.Receita)
                .Sum(x => x.Valor);

            var despesaMesCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesCorrente && x.Tipo == EnumTipoLancamento.Despesa)
                .Sum(x => x.Valor);


            var receitaMesAnterior = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesAnterior && x.Tipo == EnumTipoLancamento.Receita)
                .Sum(x => x.Valor);

            var despesaMesAnterior = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesAnterior && x.Tipo == EnumTipoLancamento.Despesa)
                .Sum(x => x.Valor);


            var ano = Math.Round((despesaAnoCorrente / receitaAnoCorrente) * 100, 1);
            var mesAtual = Math.Round((despesaMesCorrente / receitaMesCorrente) * 100, 1);
            var mesPassado = Math.Round((despesaMesAnterior / receitaMesAnterior) * 100, 1);

            return new ResultadoDashBoard
            {
                ResultadoAnoCorrente = $"{ano}",
                ResultadoMesCorrente = $"{mesAtual}",
                ResultadoMesPassado = $"{mesPassado}"
            };
        }

        private ContasApagarDashboard CalcularContasApagar(List<Lancamento> listaLancamentos)
        {
            var ContasApagarAnoCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.Tipo == EnumTipoLancamento.Despesa && !x.Realizado)
                .Sum(x => x.Valor);

            var ContasApagarMesCorrente = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesCorrente && x.Tipo == EnumTipoLancamento.Despesa && !x.Realizado)
                .Sum(x => x.Valor);

            var ContasApagarMesAnterior = listaLancamentos
                .Where(x => x.DataPagamento.Year == anoCorrente && x.DataPagamento.Month == mesAnterior && x.Tipo == EnumTipoLancamento.Despesa && !x.Realizado)
                .Sum(x => x.Valor);

            return new ContasApagarDashboard
            {
                ContasApagarAnoCorrente = ContasApagarAnoCorrente.ToString("C", new CultureInfo("pt-BR")),
                ContasApagarMesCorrente = ContasApagarMesCorrente.ToString("C", new CultureInfo("pt-BR")),
                ContasApagarMesPassado = ContasApagarMesAnterior.ToString("C", new CultureInfo("pt-BR"))
            };
        }

        private List<ReceitaDespesaMensal> CalcularReceitasDespesasMensais(List<Lancamento> listaLancamentos)
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

        private List<InvestimentoMensal> CalcularAcumuloInvestimento(List<Lancamento> listaLancamentos)
        {
            var investimentosMensais = new Dictionary<string, decimal>();
            var acumulado = 0m;

            foreach (var lancamento in listaLancamentos
                .Where(x => x.Tipo == EnumTipoLancamento.Investimento)
                .OrderBy(x => x.DataPagamento))
            {
                var key = $"{lancamento.DataPagamento.Year}-{lancamento.DataPagamento.Month:00}";

                if (!investimentosMensais.ContainsKey(key))
                {
                    investimentosMensais[key] = 0;
                }

                investimentosMensais[key] += lancamento.Valor;
            }

            var resultado = new List<InvestimentoMensal>();

            foreach (var key in investimentosMensais.Keys.OrderBy(k => k))
            {
                acumulado += investimentosMensais[key];
                resultado.Add(new InvestimentoMensal
                {
                    Chave = key,
                    Valor = acumulado.ToString("C", new CultureInfo("pt-BR")),
                });
            }

            return resultado;
        }

        public List<LancamentosPorCategoriaDashboard> AgruparLancamentosPorCategoriaDespesa(List<Lancamento> lancamentos)
        {
            var lancamentosFiltrados = lancamentos.Where(x=> x.Tipo == EnumTipoLancamento.Despesa).ToList();

            var agrupados = lancamentosFiltrados
                .GroupBy(l => l.Categoria)
                .Select(g => new LancamentosPorCategoriaDashboard
                {
                    Id = g.Key.Id,
                    Nome = g.Key.NomeCategoria,
                    Icone = g.Key.Icone,
                    Lancamentos = g.Select(l =>
                    {
                        l.Categoria = null;
                        return l;
                    }).ToList()
                })
                .ToList();

            return agrupados;
        }

    }

}
