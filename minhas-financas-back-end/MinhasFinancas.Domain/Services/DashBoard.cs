using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Domain.Services
{
    public class Dashboard
    {
        public Dashboard() { }

        public Dashboard(List<Lancamento> listaLancamentos)
        {
            Calcular(listaLancamentos);
        }

        public string ReceitaAnoCorrente { get; set; } = string.Empty;
        public string ReceitaMesCorrente { get; set; } = string.Empty;
        public string ReceitaMesPassado { get; set; } = string.Empty;


        public string DespesasAnoCorrente { get; set; } = string.Empty;
        public string DespesasMesCorrente { get; set; } = string.Empty;
        public string DespesasMesPassado { get; set; } = string.Empty;


        public string InvestimentoAnoCorrente { get; set; } = string.Empty;
        public string InvestimentoMesCorrente { get; set; } = string.Empty;
        public string InvestimentoMesPassado { get; set; } = string.Empty;




        public string ResultadoAnoCorrente { get; set; } = string.Empty;
        public string ResultadoMesCorrente { get; set; } = string.Empty;
        public string ResultadoMesPassado { get; set; } = string.Empty;





        public void Calcular(List<Lancamento> listaLancamentos)
        {
            // Implementar a lógica de cálculo aqui
            ReceitaAnoCorrente = CalcularReceitas(listaLancamentos);
        }

        private string CalcularReceitas(List<Lancamento> listaLancamentos)
        {
            // Lógica para calcular a receita do mes passado, mes correte e ano corrente
            return "Resultado";
        }
    }
}
