namespace MinhasFinancas.Domain.Services.Relatorios.PorCategoria.ClassesDoRelatorio
{
    public class CategoriaDetalhamentoRelatorio
    {
        public string NomeCategoria { get; set; }
        public List<Detalhamento> Detalhamentos { get; set; }
    }

    public class Detalhamento
    {
        public string Periodo { get; set; }
        public decimal Valor { get; set; }
    }

}
