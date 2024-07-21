using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Domain.Services
{
    public class Dashboard
    {
        public Dashboard() { }

        public Dashboard(List<Lancamento> listaLancamentos)
        {
            CalcularReceitas(listaLancamentos);
        }

        public string ReceitaAnoCorrente { get; set; } = string.Empty;
        public string ReceitaMesCorrente { get; set; } = string.Empty;
        public string ReceitaMesPassado { get; set; } = string.Empty;










        public void CalcularReceitas(List<Lancamento> listaLancamentos)
        {
            // Implementar a lógica de cálculo aqui
            ReceitaAnoCorrente = CalcularReceitaAnoCorrente(listaLancamentos);
            ReceitaMesCorrente = CalcularReceitaMesCorrente(listaLancamentos);
            ReceitaMesPassado = CalcularReceitaMesPassado(listaLancamentos);
        }

        private string CalcularReceitaAnoCorrente(List<Lancamento> listaLancamentos)
        {
            // Lógica para calcular a receita do ano corrente
            return "Resultado do Ano Corrente";
        }

        private string CalcularReceitaMesCorrente(List<Lancamento> listaLancamentos)
        {
            // Lógica para calcular a receita do mês corrente
            return "Resultado do Mês Corrente";
        }

        private string CalcularReceitaMesPassado(List<Lancamento> listaLancamentos)
        {
            // Lógica para calcular a receita do mês passado
            return "Resultado do Mês Passado";
        }
    }
}
