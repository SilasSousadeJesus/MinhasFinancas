namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class ResultadoPaginadoDTO<T>
    {
        public List<T> Itens { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalItens { get; set; }
        public int TotalPaginas { get; set; }
    }
}
