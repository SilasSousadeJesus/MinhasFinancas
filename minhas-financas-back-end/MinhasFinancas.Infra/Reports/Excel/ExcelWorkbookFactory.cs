using ClosedXML.Excel;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public class ExcelWorkbookFactory
    {
        public XLWorkbook Criar()
        {
            var workbook = new XLWorkbook();
            workbook.Properties.Author = "Minhas Financas";
            workbook.Properties.Company = "Minhas Financas";
            workbook.Properties.Title = "Relatório";
            return workbook;
        }
    }
}
