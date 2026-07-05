using ClosedXML.Excel;
using MinhasFinancas.CrossCutting.Reports;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public abstract class ExcelReportBase
    {
        protected readonly ExcelWorkbookFactory WorkbookFactory;
        protected readonly ExcelStyleHelper StyleHelper;

        protected ExcelReportBase(ExcelWorkbookFactory workbookFactory, ExcelStyleHelper styleHelper)
        {
            WorkbookFactory = workbookFactory;
            StyleHelper = styleHelper;
        }

        protected ArquivoRelatorioDTO CriarArquivo(XLWorkbook workbook, string nomeArquivo)
        {
            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);

            return new ArquivoRelatorioDTO
            {
                NomeArquivo = nomeArquivo,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Conteudo = memoryStream.ToArray(),
            };
        }
    }
}
