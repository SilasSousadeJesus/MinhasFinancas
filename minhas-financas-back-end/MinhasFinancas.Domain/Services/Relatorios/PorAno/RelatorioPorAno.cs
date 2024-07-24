using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.Relatorios.PorAno.ClassesDoRelatorio;

namespace MinhasFinancas.Domain.Services.Relatorios.PorAno
{
    public class RelatorioPorAno
    {
        private int anoCorrente;
        private int UmAnoAtras;
        private int DoisAnoAtras;
        private int TresAnosAtras;
        private int QuatroAnosAtras;
        private int CincoAnosAtras;
 
        private int mesCorrente;
        private int mesAnterior;


        public List<ValorPorAno> DespesaPorAno { get; set; }
        public List<ValorPorAno> ReceitaPorAno { get; set; }
        public List<ValorPorAno> InvestimentoPorAno { get; set; }
        public List<ValorPorAno> PorcentagemDespesasDaReceitaPorAno { get; set; }
        public List<ValorPorAno> PorcentagemInvestimentoDaReceitaPorAno { get; set; }
        public List<ValorPatrimonio> PatrimonioPorAno { get; set; }


        public RelatorioPorAno(List<Lancamento> listaLancamentos, List<Categoria> listaCategorias)
        {
            anoCorrente = DateTime.Now.Year;
            UmAnoAtras = DateTime.Now.Year - 1;
            DoisAnoAtras = DateTime.Now.Year - 2;
            TresAnosAtras = DateTime.Now.Year - 3;
            QuatroAnosAtras = DateTime.Now.Year - 4;
            CincoAnosAtras = DateTime.Now.Year - 5;

            mesCorrente = DateTime.Now.Month;
            mesAnterior = mesCorrente == 1 ? 12 : mesCorrente - 1;

            DespesaPorAno = ComparacaoValorPorAno(listaLancamentos, EnumTipoLancamento.Despesa);
            ReceitaPorAno = ComparacaoValorPorAno(listaLancamentos, EnumTipoLancamento.Receita);
            InvestimentoPorAno = ComparacaoValorPorAno(listaLancamentos, EnumTipoLancamento.Investimento);

            PorcentagemDespesasDaReceitaPorAno = ComparacaoPorcentagemDaReceitaPorAno(listaLancamentos, EnumTipoLancamento.Despesa);
            PorcentagemInvestimentoDaReceitaPorAno = ComparacaoPorcentagemDaReceitaPorAno(listaLancamentos, EnumTipoLancamento.Investimento);

            PatrimonioPorAno = ValorPatrimonialPorAno(listaLancamentos);

        }

        private List<ValorPorAno> ComparacaoValorPorAno(List<Lancamento> listaLancamentos, EnumTipoLancamento tipoLancamento) {

            var  listaValorPorAno = new List<ValorPorAno>();

            var lancamentosPorAno = listaLancamentos
                .Where(l => l.Tipo == tipoLancamento)
                .GroupBy(l => l.DataPagamento.Year)
                .OrderBy(g => g.Key) 
                .ToList();

            var lista = lancamentosPorAno
                .Select(g => new ValorPorAno
                {
                    Ano = g.Key.ToString(),
                    Valor = g.Sum(l => l.Valor)
                })
                .ToList();

            return lista;
        }

        private List<ValorPorAno> ComparacaoPorcentagemDaReceitaPorAno(List<Lancamento> listaLancamentos, EnumTipoLancamento tipoLancamento)
        {
            var listaValorPorAno = new List<ValorPorAno>();

            var lancamentosPorAno = listaLancamentos
                .Where(l => l.Tipo == EnumTipoLancamento.Receita || l.Tipo == tipoLancamento)
                .GroupBy(l => l.DataPagamento.Year)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var ano in lancamentosPorAno)
            {
                var despesa = ano.Where(x => x.Tipo == tipoLancamento).Select(x => x.Valor).Sum();
                var receita = ano.Where(x => x.Tipo == EnumTipoLancamento.Receita).Select(x => x.Valor).Sum();

                var resultado = new ValorPorAno()
                {
                    Ano = ano.Key.ToString(),
                    Valor = receita > 0 ? Math.Round((despesa / receita) * 100, 2) : 0,
                };

                listaValorPorAno.Add(resultado);
            }
            return listaValorPorAno;
        }

        public List<ValorPatrimonio> ValorPatrimonialPorAno(List<Lancamento> listaLancamentos)
        {
            var listaValorPorAno = new List<ValorPatrimonio>();
            return listaValorPorAno;
        }

        //public List<ValorPorAno> ComparacaoValorMesAMesAnoPassadoECorrente(EnumTipoLancamento tipoLancamento)
        //{
        //}

        //public List<ValorPorAno> ComparacaoValorCategoriaAnoPorAno(EnumTipoLancamento tipoLancamento, EnumTipoCategoria tipoCategoria)
        //{
        //}
    }
}


