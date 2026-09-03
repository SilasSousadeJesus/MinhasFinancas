namespace MinhasFinancas.Application.DTOs.Lancamento
{
    public class ResultadoImportacaoLancamentosDTO
    {
        public int TotalLinhas { get; set; }
        public int TotalImportados { get; set; }
        public List<ErroImportacaoLancamentoDTO> Erros { get; set; } = [];
    }

    public class ErroImportacaoLancamentoDTO
    {
        public int Linha { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}
