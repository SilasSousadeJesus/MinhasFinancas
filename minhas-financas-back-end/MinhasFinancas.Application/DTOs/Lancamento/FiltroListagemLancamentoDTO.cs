namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class FiltroListagemLancamentoDTO
    {
        public string BuscaDescricao { get; set; } = string.Empty;
        public int? Tipo { get; set; }
        public Guid? CategoriaId { get; set; }
        public Guid? ContaId { get; set; }
        public Guid? CartaoId { get; set; }
        public bool? Realizado { get; set; }
        public DateTime? DataInicialLancamento { get; set; }
        public DateTime? DataFinalLancamento { get; set; }
        public DateTime? DataInicialPagamento { get; set; }
        public DateTime? DataFinalPagamento { get; set; }
        public string OrdenarPor { get; set; } = "data";
        public string Direcao { get; set; } = "desc";
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;
    }
}
