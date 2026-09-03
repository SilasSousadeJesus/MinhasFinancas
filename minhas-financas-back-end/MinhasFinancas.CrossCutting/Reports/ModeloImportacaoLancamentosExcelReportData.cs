namespace MinhasFinancas.CrossCutting.Reports
{
    public class ModeloImportacaoLancamentosExcelReportData
    {
        public string NomeArquivo { get; set; } = "modelo-importacao-lancamentos.xlsx";
        public List<string> Categorias { get; set; } = [];
        public List<string> Subcategorias { get; set; } = [];
        public List<string> Contas { get; set; } = [];
        public List<string> Cartoes { get; set; } = [];
    }
}
