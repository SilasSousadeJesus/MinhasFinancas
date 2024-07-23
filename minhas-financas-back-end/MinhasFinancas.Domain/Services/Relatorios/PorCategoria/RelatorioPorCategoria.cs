using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;

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



        public RelatorioPorCategoria(List<Lancamento> listaLancamentos, List<Categoria> listaCategoria)
        {
            anoCorrente = DateTime.Now.Year;
            anoPassado = DateTime.Now.Year - 1;
            mesCorrente = DateTime.Now.Month;
            mesAnterior = mesCorrente == 1 ? 12 : mesCorrente - 1;
            lancamentos = listaLancamentos;
            categorias = listaCategoria;

            DespesasPorcentagemCategoriaMesAtual = CalcularDespesasPorcentagemCategoriaMesAtual();
            DespesasPorcentagemCategoriaMesPassado = CalcularDespesasPorcentagemCategoriaMesPassado();
            DespesasPorcentagemCategoriaAnoAtual = CalcularDespesasPorcentagemCategoriaAnoAtual();
            DespesasPorcentagemCategoriaAnoPassado = CalcularDespesasPorcentagemCategoriaAnoPassado();


            DespesasCategoriaDetalhamentoRelatorioAnoCorrente = CalcularPorcentagemPorCategoria(anoCorrente, 0, EnumTipoLancamento.Despesa, false);
            DespesasCategoriaDetalhamentoRelatorioAnoPassado = CalcularPorcentagemPorCategoria(anoPassado, 0, EnumTipoLancamento.Despesa, false);
            DespesasCategoriaDetalhamentoRelatorioMesCorrente = CalcularPorcentagemPorCategoria(anoCorrente, mesCorrente, EnumTipoLancamento.Despesa, true);
            DespesasCategoriaDetalhamentoRelatorioMesPassado = CalcularPorcentagemPorCategoria(anoCorrente, mesAnterior, EnumTipoLancamento.Despesa, true);
        }




        // Porcentagem da categoria no valor total;
        private List<CategoriaPorcentagemRelatorio> CalcularPorcentagemPorCategoria(int ano, int mes, EnumTipoLancamento tipoLancamento)
        {
            var lancamentosFiltrados = lancamentos
                .Where(l => l.DataPagamento.Year == ano && (mes == 0 || l.DataPagamento.Month == mes) && l.Tipo == tipoLancamento)
                .ToList();

            var totalDespesas = lancamentosFiltrados.Sum(l => l.Valor);

            var porcentagemPorCategoria = categorias
                .Select(c => new CategoriaPorcentagemRelatorio
                {
                    NomeCategoria = c.NomeCategoria,
                    Porcentagem = totalDespesas == 0 ? 0 : lancamentosFiltrados.Where(l => l.CategoriaId == c.Id).Sum(l => l.Valor) / totalDespesas * 100
                })
                .ToList();

            return porcentagemPorCategoria;
        }

        public List<CategoriaPorcentagemRelatorio> CalcularDespesasPorcentagemCategoriaAnoAtual()
        {
            return CalcularPorcentagemPorCategoria(anoCorrente, 0, EnumTipoLancamento.Despesa);
        }

        public List<CategoriaPorcentagemRelatorio> CalcularDespesasPorcentagemCategoriaAnoPassado()
        {
            return CalcularPorcentagemPorCategoria(anoPassado, 0, EnumTipoLancamento.Despesa);
        }
        public List<CategoriaPorcentagemRelatorio> CalcularDespesasPorcentagemCategoriaMesAtual()
        {
            return CalcularPorcentagemPorCategoria(anoCorrente, mesCorrente, EnumTipoLancamento.Despesa);
        }

        public List<CategoriaPorcentagemRelatorio> CalcularDespesasPorcentagemCategoriaMesPassado()
        {
            return CalcularPorcentagemPorCategoria(anoCorrente, mesAnterior, EnumTipoLancamento.Despesa);
        }



        // Detalhamento
        private List<CategoriaDetalhamentoRelatorio> CalcularPorcentagemPorCategoria(int ano, int mes, EnumTipoLancamento tipoLancamento, bool isMensal)
        {
            var lancamentosFiltrados = lancamentos
                .Where(l => l.DataPagamento.Year == ano && (mes == 0 || l.DataPagamento.Month == mes) && l.Tipo == tipoLancamento)
                .ToList();

            var detalhamentoPorCategoria = categorias
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
