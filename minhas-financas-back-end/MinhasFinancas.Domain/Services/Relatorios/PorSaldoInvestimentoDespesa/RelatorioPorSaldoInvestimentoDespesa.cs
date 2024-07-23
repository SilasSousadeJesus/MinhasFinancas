using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.Relatorios.PorSaldoInvestimentoDespesa.ClassesDoRelatorio;

namespace MinhasFinancas.Domain.Services.Relatorios.PorSaldoInvestimentoDespesa
{
    public class RelatorioPorSaldoInvestimentoDespesa
    {
        private int anoCorrente;
        private int anoPassado;
        private int mesCorrente;
        private int mesAnterior;
        private List<Lancamento> lancamentos;

        public RelatorioPorSaldoInvestimentoDespesa(List<Lancamento> listaLancamentos)
        {
            anoCorrente = DateTime.Now.Year;
            anoPassado = DateTime.Now.Year - 1;
            mesCorrente = DateTime.Now.Month;
            mesAnterior = mesCorrente == 1 ? 12 : mesCorrente - 1;
            lancamentos = listaLancamentos;

        }

        public SaldoReceitaInvestimento SaldoReceitaInvestimento { get; set; }
        public DespesasEmRelacaoReceita DespesasEmRelacaoReceita { get; set; }
        public PagamentoDividasRelacaoReceita PagamentoDividasRelacaoReceita { get; set; }
        public InvestimentoRelacaoReceita InvestimentoRelacaoReceita { get; set; }

    }
}
