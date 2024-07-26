using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.Relatorios.PorCategoria.ClassesDoRelatorio;

namespace MinhasFinancas.Domain.Services.Relatorios.PorCategoria
{
    public class RelatorioPorCategoria
    {
        private int anoCorrente;
        private int anoPassado;
        private int mesCorrente;
        private int mesAnterior;
        private List<Lancamento> lancamentos;
        private List<Categoria> categorias;

        public List<CategoriaPorcentagemRelatorio> DespesasPorcentagemCategoriaMesAtual { get; set; }
        public List<CategoriaPorcentagemRelatorio> DespesasPorcentagemCategoriaMesPassado { get; set; }
        public List<CategoriaPorcentagemRelatorio> DespesasPorcentagemCategoriaAnoAtual { get; set; }
        public List<CategoriaPorcentagemRelatorio> DespesasPorcentagemCategoriaAnoPassado { get; set; }
        public List<CategoriaDetalhamentoRelatorio> DespesasCategoriaDetalhamentoRelatorioAnoCorrente { get; set; }
        public List<CategoriaDetalhamentoRelatorio> DespesasCategoriaDetalhamentoRelatorioAnoPassado { get; set; }
        public List<CategoriaDetalhamentoRelatorio> DespesasCategoriaDetalhamentoRelatorioMesCorrente { get; set; }
        public List<CategoriaDetalhamentoRelatorio> DespesasCategoriaDetalhamentoRelatorioMesPassado { get; set; }


        public List<CategoriaPorcentagemRelatorio> ReceitasPorcentagemCategoriaMesAtual { get; set; }
        public List<CategoriaPorcentagemRelatorio> ReceitasPorcentagemCategoriaMesPassado { get; set; }
        public List<CategoriaPorcentagemRelatorio> ReceitasPorcentagemCategoriaAnoAtual { get; set; }
        public List<CategoriaPorcentagemRelatorio> ReceitasPorcentagemCategoriaAnoPassado { get; set; }
        public List<CategoriaDetalhamentoRelatorio> ReceitasCategoriaDetalhamentoRelatorioAnoCorrente { get; set; }
        public List<CategoriaDetalhamentoRelatorio> ReceitasCategoriaDetalhamentoRelatorioAnoPassado { get; set; }
        public List<CategoriaDetalhamentoRelatorio> ReceitasCategoriaDetalhamentoRelatorioMesCorrente { get; set; }
        public List<CategoriaDetalhamentoRelatorio> ReceitasCategoriaDetalhamentoRelatorioMesPassado { get; set; }


        public List<CategoriaPorcentagemRelatorio> InvestimentosPorcentagemCategoriaMesAtual { get; set; }
        public List<CategoriaPorcentagemRelatorio> InvestimentosPorcentagemCategoriaMesPassado { get; set; }
        public List<CategoriaPorcentagemRelatorio> InvestimentosPorcentagemCategoriaAnoAtual { get; set; }
        public List<CategoriaPorcentagemRelatorio> InvestimentosPorcentagemCategoriaAnoPassado { get; set; }
        public List<CategoriaDetalhamentoRelatorio> InvestimentosCategoriaDetalhamentoRelatorioAnoCorrente { get; set; }
        public List<CategoriaDetalhamentoRelatorio> InvestimentosCategoriaDetalhamentoRelatorioAnoPassado { get; set; }
        public List<CategoriaDetalhamentoRelatorio> InvestimentosCategoriaDetalhamentoRelatorioMesCorrente { get; set; }
        public List<CategoriaDetalhamentoRelatorio> InvestimentosCategoriaDetalhamentoRelatorioMesPassado { get; set; }

        public RelatorioPorCategoria(List<Lancamento> listaLancamentos, List<Categoria> listaCategoria)
        {
            anoCorrente = DateTime.Now.Year;
            anoPassado = DateTime.Now.Year - 1;
            mesCorrente = DateTime.Now.Month;
            mesAnterior = mesCorrente == 1 ? 12 : mesCorrente - 1;
            lancamentos = listaLancamentos;
            categorias = listaCategoria;

            DespesasPorcentagemCategoriaMesAtual = CalcularPorcentagemPorCategoria(anoCorrente, mesCorrente, EnumTipoLancamento.Despesa, EnumTipoCategoria.Despesa);
            DespesasPorcentagemCategoriaMesPassado = CalcularPorcentagemPorCategoria(anoCorrente, mesAnterior, EnumTipoLancamento.Despesa, EnumTipoCategoria.Despesa);
            DespesasPorcentagemCategoriaAnoAtual = CalcularPorcentagemPorCategoria(anoCorrente, 0, EnumTipoLancamento.Despesa, EnumTipoCategoria.Despesa);
            DespesasPorcentagemCategoriaAnoPassado = CalcularPorcentagemPorCategoria(anoPassado, 0, EnumTipoLancamento.Despesa, EnumTipoCategoria.Despesa);

            ReceitasPorcentagemCategoriaMesAtual = CalcularPorcentagemPorCategoria(anoCorrente, mesCorrente, EnumTipoLancamento.Receita, EnumTipoCategoria.Receita);
            ReceitasPorcentagemCategoriaMesPassado = CalcularPorcentagemPorCategoria(anoCorrente, mesAnterior, EnumTipoLancamento.Receita, EnumTipoCategoria.Receita);
            ReceitasPorcentagemCategoriaAnoAtual = CalcularPorcentagemPorCategoria(anoCorrente, 0, EnumTipoLancamento.Receita, EnumTipoCategoria.Receita);
            ReceitasPorcentagemCategoriaAnoPassado = CalcularPorcentagemPorCategoria(anoPassado, 0, EnumTipoLancamento.Receita, EnumTipoCategoria.Receita);

            InvestimentosPorcentagemCategoriaMesAtual = CalcularPorcentagemPorCategoria(anoCorrente, mesCorrente, EnumTipoLancamento.InvestimentoDeposito, EnumTipoCategoria.Investimento);
            InvestimentosPorcentagemCategoriaMesPassado = CalcularPorcentagemPorCategoria(anoCorrente, mesAnterior, EnumTipoLancamento.InvestimentoDeposito, EnumTipoCategoria.Investimento);
            InvestimentosPorcentagemCategoriaAnoAtual = CalcularPorcentagemPorCategoria(anoCorrente, 0, EnumTipoLancamento.InvestimentoDeposito, EnumTipoCategoria.Investimento);
            InvestimentosPorcentagemCategoriaAnoPassado = CalcularPorcentagemPorCategoria(anoPassado, 0, EnumTipoLancamento.InvestimentoDeposito, EnumTipoCategoria.Investimento);



            DespesasCategoriaDetalhamentoRelatorioAnoCorrente = CalcularPorcentagemPorCategoria(anoCorrente, 0, EnumTipoLancamento.Despesa, EnumTipoCategoria.Despesa, false);
            DespesasCategoriaDetalhamentoRelatorioAnoPassado = CalcularPorcentagemPorCategoria(anoPassado, 0, EnumTipoLancamento.Despesa, EnumTipoCategoria.Despesa, false);
            DespesasCategoriaDetalhamentoRelatorioMesCorrente = CalcularPorcentagemPorCategoria(anoCorrente, mesCorrente, EnumTipoLancamento.Despesa, EnumTipoCategoria.Despesa, true);
            DespesasCategoriaDetalhamentoRelatorioMesPassado = CalcularPorcentagemPorCategoria(anoCorrente, mesAnterior, EnumTipoLancamento.Despesa, EnumTipoCategoria.Despesa, true);

            ReceitasCategoriaDetalhamentoRelatorioAnoCorrente = CalcularPorcentagemPorCategoria(anoCorrente, 0, EnumTipoLancamento.Receita, EnumTipoCategoria.Receita, false);
            ReceitasCategoriaDetalhamentoRelatorioAnoPassado = CalcularPorcentagemPorCategoria(anoPassado, 0, EnumTipoLancamento.Receita, EnumTipoCategoria.Receita, false);
            ReceitasCategoriaDetalhamentoRelatorioMesCorrente = CalcularPorcentagemPorCategoria(anoCorrente, mesCorrente, EnumTipoLancamento.Receita, EnumTipoCategoria.Receita, true);
            ReceitasCategoriaDetalhamentoRelatorioMesPassado = CalcularPorcentagemPorCategoria(anoCorrente, mesAnterior, EnumTipoLancamento.Receita, EnumTipoCategoria.Receita, true);

            InvestimentosCategoriaDetalhamentoRelatorioAnoCorrente = CalcularPorcentagemPorCategoria(anoCorrente, 0, EnumTipoLancamento.InvestimentoDeposito, EnumTipoCategoria.Investimento, false);
            InvestimentosCategoriaDetalhamentoRelatorioAnoPassado = CalcularPorcentagemPorCategoria(anoPassado, 0, EnumTipoLancamento.InvestimentoDeposito, EnumTipoCategoria.Investimento, false);
            InvestimentosCategoriaDetalhamentoRelatorioMesCorrente = CalcularPorcentagemPorCategoria(anoCorrente, mesCorrente, EnumTipoLancamento.InvestimentoDeposito, EnumTipoCategoria.Investimento, true);
            InvestimentosCategoriaDetalhamentoRelatorioMesPassado = CalcularPorcentagemPorCategoria(anoCorrente, mesAnterior, EnumTipoLancamento.InvestimentoDeposito, EnumTipoCategoria.Investimento, true);
        }


        // Porcentagem de cada categoria no valor total;
        private List<CategoriaPorcentagemRelatorio> CalcularPorcentagemPorCategoria(int ano, int mes, EnumTipoLancamento tipoLancamento, EnumTipoCategoria tipoCategoria)
        {
            var lancamentosFiltrados = lancamentos
                .Where(l => l.DataPagamento.Year == ano && (mes == 0 || l.DataPagamento.Month == mes) && l.Tipo == tipoLancamento)
                .ToList();

            var totalDespesas = lancamentosFiltrados.Sum(l => l.Valor);

            var categoriasFiltradasPorTipo = categorias.Where(x => x.Tipo == tipoCategoria);

            var porcentagemPorCategoria = categoriasFiltradasPorTipo
                .Select(c => new CategoriaPorcentagemRelatorio
                {
                    NomeCategoria = c.NomeCategoria,
                    Porcentagem = totalDespesas == 0 ? 0 : lancamentosFiltrados.Where(l => l.CategoriaId == c.Id).Sum(l => l.Valor) / totalDespesas * 100
                })
                .ToList();

            return porcentagemPorCategoria;
        }


        // Detalhamento dos valores por categorio
        private List<CategoriaDetalhamentoRelatorio> CalcularPorcentagemPorCategoria(int ano, int mes, EnumTipoLancamento tipoLancamento, EnumTipoCategoria tipoCategoria, bool isMensal)
        {
            var lancamentosFiltrados = lancamentos
                .Where(l => l.DataPagamento.Year == ano && (mes == 0 || l.DataPagamento.Month == mes) && l.Tipo == tipoLancamento)
                .ToList();

            var categoriasFiltradasPorTipo = categorias.Where(x => x.Tipo == tipoCategoria);

            var detalhamentoPorCategoria = categoriasFiltradasPorTipo
                .Select(c => new CategoriaDetalhamentoRelatorio
                {
                    NomeCategoria = c.NomeCategoria,
                    Detalhamentos = isMensal ? CalcularDetalhamentoSemanal(lancamentosFiltrados, c.Id, mes) : CalcularDetalhamentoMensal(lancamentosFiltrados, c.Id)
                })
                .ToList();

            return detalhamentoPorCategoria;
        }

        private List<Detalhamento> CalcularDetalhamentoSemanal(List<Lancamento> lancamentos, Guid categoriaId, int mes)
        {
            var semanas = new List<Detalhamento>();
            for (int semana = 1; semana <= 4; semana++)
            {
                var inicioSemana = new DateTime(DateTime.Now.Year, mes, (semana - 1) * 7 + 1);
                var fimSemana = inicioSemana.AddDays(6);

                var valorSemana = lancamentos
                    .Where(l => l.CategoriaId == categoriaId && l.DataPagamento >= inicioSemana && l.DataPagamento <= fimSemana)
                    .Sum(l => l.Valor);

                semanas.Add(new Detalhamento
                {
                    Periodo = $"Semana {semana}",
                    Valor = valorSemana
                });
            }
            return semanas;
        }

        private List<Detalhamento> CalcularDetalhamentoMensal(List<Lancamento> lancamentos, Guid categoriaId)
        {
            var meses = new List<Detalhamento>();
            for (int mes = 1; mes <= 12; mes++)
            {
                var valorMes = lancamentos
                    .Where(l => l.CategoriaId == categoriaId && l.DataPagamento.Month == mes)
                    .Sum(l => l.Valor);

                meses.Add(new Detalhamento
                {
                    Periodo = new DateTime(DateTime.Now.Year, mes, 1).ToString("MMMM"),
                    Valor = valorMes
                });
            }
            return meses;
        }
    }
}
