using ClosedXML.Excel;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public class ExcelStyleHelper
    {
        private readonly XLColor _tituloBackground = XLColor.FromHtml("#0F172A");
        private readonly XLColor _cabecalhoBackground = XLColor.FromHtml("#E2E8F0");
        private readonly XLColor _secaoBackground = XLColor.FromHtml("#F8FAFC");
        private readonly XLColor _resumoBackground = XLColor.FromHtml("#DBEAFE");
        private readonly XLColor _totalBackground = XLColor.FromHtml("#DCFCE7");

        public void AplicarTitulo(IXLRange range)
        {
            range.Merge();
            range.Style.Font.Bold = true;
            range.Style.Font.FontSize = 16;
            range.Style.Font.FontColor = XLColor.White;
            range.Style.Fill.BackgroundColor = _tituloBackground;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        public void AplicarSubtitulo(IXLRange range)
        {
            range.Merge();
            range.Style.Font.FontSize = 11;
            range.Style.Font.FontColor = XLColor.FromHtml("#334155");
            range.Style.Fill.BackgroundColor = _secaoBackground;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        public void AplicarTituloSecao(IXLRange range)
        {
            range.Merge();
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = _secaoBackground;
            range.Style.Font.FontColor = XLColor.FromHtml("#0F172A");
        }

        public void AplicarCabecalho(IXLRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = _cabecalhoBackground;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        public void AplicarLinhaTabela(IXLRange range)
        {
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        public void AplicarResumoRotulo(IXLCell cell)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = _resumoBackground;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        public void AplicarResumoValor(IXLCell cell)
        {
            cell.Style.Fill.BackgroundColor = _resumoBackground;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.NumberFormat.Format = ExcelNumberFormats.Currency;
            cell.Style.Font.Bold = true;
        }

        public void AplicarTotalizador(IXLRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.BackgroundColor = _totalBackground;
            range.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            range.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            range.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            range.Style.Border.RightBorder = XLBorderStyleValues.Thin;
        }
    }
}
