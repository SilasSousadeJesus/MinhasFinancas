using ClosedXML.Excel;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public static class ExcelExtensions
    {
        public static void AplicarFormatoMoeda(this IXLColumn column)
        {
            column.Style.NumberFormat.Format = ExcelNumberFormats.Currency;
        }

        public static void AplicarFormatoData(this IXLColumn column)
        {
            column.Style.DateFormat.Format = ExcelNumberFormats.Date;
        }

        public static void FinalizarLayout(this IXLWorksheet worksheet, int linhaCongelada)
        {
            worksheet.SheetView.FreezeRows(linhaCongelada);
            worksheet.Columns().AdjustToContents();
        }

        public static string SanitizarNomeAba(this string value)
        {
            var invalidos = new[] { '[', ']', '*', '?', '/', '\\', ':' };
            var sanitizado = new string(value.Where(x => !invalidos.Contains(x)).ToArray());
            return string.IsNullOrWhiteSpace(sanitizado) ? "Relatorio" : sanitizado[..Math.Min(sanitizado.Length, 31)];
        }
    }
}
