using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Domain.Services.Relatorios
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
        }





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
                    Porcentagem = totalDespesas == 0 ? 0 : (lancamentosFiltrados.Where(l => l.CategoriaId == c.Id).Sum(l => l.Valor) / totalDespesas) * 100
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


    }
}
