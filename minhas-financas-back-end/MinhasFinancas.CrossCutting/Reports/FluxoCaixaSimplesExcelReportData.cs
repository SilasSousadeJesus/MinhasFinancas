namespace MinhasFinancas.CrossCutting.Reports
{
    public class FluxoCaixaSimplesExcelReportData
    {
        public string NomeArquivo { get; set; } = string.Empty;
        public List<FluxoCaixaSimplesExcelSheetData> Meses { get; set; } = [];
    }

    public class FluxoCaixaSimplesExcelSheetData
    {
        public string NomeAba { get; set; } = string.Empty;
        public string Referencia { get; set; } = string.Empty;
        public decimal ReceitasTotal { get; set; }
        public decimal DespesasTotal { get; set; }
        public decimal SaldoMes { get; set; }
        public List<FluxoCaixaSimplesExcelItemData> Receitas { get; set; } = [];
        public List<FluxoCaixaSimplesExcelItemData> Despesas { get; set; } = [];
    }

    public class FluxoCaixaSimplesExcelItemData
    {
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime DataVencimento { get; set; }
        public decimal Valor { get; set; }
    }
}
