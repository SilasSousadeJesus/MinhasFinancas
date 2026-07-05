using ClosedXML.Excel;
using MinhasFinancas.CrossCutting.Reports;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public class FluxoCaixaSimplesExcelReport : ExcelReportBase, IExcelReport<FluxoCaixaSimplesExcelReportData>
    {
        public FluxoCaixaSimplesExcelReport(ExcelWorkbookFactory workbookFactory, ExcelStyleHelper styleHelper)
            : base(workbookFactory, styleHelper)
        {
        }

        public ArquivoRelatorioDTO Gerar(FluxoCaixaSimplesExcelReportData model)
        {
            using var workbook = WorkbookFactory.Criar();

            foreach (var mes in model.Meses)
            {
                var worksheet = workbook.Worksheets.Add(mes.NomeAba.SanitizarNomeAba());
                MontarAba(worksheet, mes);
            }

            return CriarArquivo(workbook, model.NomeArquivo);
        }

        private void MontarAba(IXLWorksheet worksheet, FluxoCaixaSimplesExcelSheetData mes)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:D1"));
            worksheet.Cell("A1").Value = "Fluxo de Caixa";

            StyleHelper.AplicarSubtitulo(worksheet.Range("A2:D2"));
            worksheet.Cell("A2").Value = mes.Referencia;

            StyleHelper.AplicarTituloSecao(worksheet.Range("A4:D4"));
            worksheet.Cell("A4").Value = "Resumo";

            worksheet.Cell("A5").Value = "Receitas do mês";
            worksheet.Cell("B5").Value = mes.ReceitasTotal;
            worksheet.Cell("C5").Value = "Despesas do mês";
            worksheet.Cell("D5").Value = mes.DespesasTotal;
            StyleHelper.AplicarResumoRotulo(worksheet.Cell("A5"));
            StyleHelper.AplicarResumoValor(worksheet.Cell("B5"));
            StyleHelper.AplicarResumoRotulo(worksheet.Cell("C5"));
            StyleHelper.AplicarResumoValor(worksheet.Cell("D5"));

            worksheet.Cell("A6").Value = "Saldo do mês";
            worksheet.Cell("B6").Value = mes.SaldoMes;
            StyleHelper.AplicarResumoRotulo(worksheet.Cell("A6"));
            StyleHelper.AplicarResumoValor(worksheet.Cell("B6"));

            var proximaLinha = 8;
            proximaLinha = MontarSecaoTabela(
                worksheet,
                proximaLinha,
                "Receitas previstas",
                mes.Receitas,
                "Total das receitas");

            proximaLinha += 2;

            MontarSecaoTabela(
                worksheet,
                proximaLinha,
                "Despesas previstas",
                mes.Despesas,
                "Total das despesas");

            worksheet.Column(3).AplicarFormatoData();
            worksheet.Column(4).AplicarFormatoMoeda();
            worksheet.FinalizarLayout(9);
        }

        private int MontarSecaoTabela(
            IXLWorksheet worksheet,
            int linhaInicial,
            string tituloSecao,
            IReadOnlyCollection<FluxoCaixaSimplesExcelItemData> itens,
            string totalLabel)
        {
            StyleHelper.AplicarTituloSecao(worksheet.Range(linhaInicial, 1, linhaInicial, 4));
            worksheet.Cell(linhaInicial, 1).Value = tituloSecao;

            var linhaCabecalho = linhaInicial + 1;
            worksheet.Cell(linhaCabecalho, 1).Value = "Descrição";
            worksheet.Cell(linhaCabecalho, 2).Value = "Categoria";
            worksheet.Cell(linhaCabecalho, 3).Value = "Vencimento";
            worksheet.Cell(linhaCabecalho, 4).Value = "Valor";
            StyleHelper.AplicarCabecalho(worksheet.Range(linhaCabecalho, 1, linhaCabecalho, 4));

            var linhaAtual = linhaCabecalho + 1;
            foreach (var item in itens)
            {
                worksheet.Cell(linhaAtual, 1).Value = item.Descricao;
                worksheet.Cell(linhaAtual, 2).Value = item.Categoria;
                worksheet.Cell(linhaAtual, 3).Value = item.DataVencimento;
                worksheet.Cell(linhaAtual, 4).Value = item.Valor;
                StyleHelper.AplicarLinhaTabela(worksheet.Range(linhaAtual, 1, linhaAtual, 4));
                linhaAtual++;
            }

            StyleHelper.AplicarTotalizador(worksheet.Range(linhaAtual, 1, linhaAtual, 4));
            worksheet.Cell(linhaAtual, 1).Value = totalLabel;
            worksheet.Cell(linhaAtual, 4).Value = itens.Sum(x => x.Valor);

            return linhaAtual;
        }
    }
}
