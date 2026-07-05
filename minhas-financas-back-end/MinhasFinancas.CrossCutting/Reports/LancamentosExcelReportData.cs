namespace MinhasFinancas.CrossCutting.Reports
{
    public class LancamentosExcelReportData
    {
        public string NomeArquivo { get; set; } = string.Empty;
        public string Titulo { get; set; } = "Relatorio de Lancamentos";
        public string Subtitulo { get; set; } = string.Empty;
        public List<LancamentoExcelReportRow> Itens { get; set; } = [];
    }

    public class LancamentoExcelReportRow
    {
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataEfetivacao { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DataLancamento { get; set; }
    }
}
