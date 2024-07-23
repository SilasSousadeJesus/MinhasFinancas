using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Domain.Services.Relatorios.Patrimonio
{
    public class RelatorioPatrimonial
    {
        private int anoCorrente;
        private int UmAnoAtras;
        private int DoisAnoAtras;
        private int TresAnosAtras;
        private int QuatroAnosAtras;
        private int CincoAnosAtras;

        private int mesCorrente;
        private int mesAnterior;
        private List<Lancamento> lancamentos;
        private List<Categoria> categorias;

        public RelatorioPatrimonial(List<Lancamento> listaLancamentos)
        {
            anoCorrente = DateTime.Now.Year;
            UmAnoAtras = DateTime.Now.Year - 1;
            DoisAnoAtras = DateTime.Now.Year - 2;
            TresAnosAtras = DateTime.Now.Year - 3;
            QuatroAnosAtras = DateTime.Now.Year - 4;
            CincoAnosAtras = DateTime.Now.Year - 5;

            mesCorrente = DateTime.Now.Month;
            mesAnterior = mesCorrente == 1 ? 12 : mesCorrente - 1;
            lancamentos = listaLancamentos;

        }
    }
}
