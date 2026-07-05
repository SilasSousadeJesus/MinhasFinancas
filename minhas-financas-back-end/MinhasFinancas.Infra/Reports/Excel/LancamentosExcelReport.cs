using ClosedXML.Excel;
using MinhasFinancas.CrossCutting.Reports;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public class LancamentosExcelReport : ExcelReportBase, IExcelReport<LancamentosExcelReportData>
    {
        public LancamentosExcelReport(ExcelWorkbookFactory workbookFactory, ExcelStyleHelper styleHelper)
            : base(workbookFactory, styleHelper)
        {
        }

        public ArquivoRelatorioDTO Gerar(LancamentosExcelReportData model)
        {
            using var workbook = WorkbookFactory.Criar();
            var worksheet = workbook.Worksheets.Add("Lançamentos");

            StyleHelper.AplicarTitulo(worksheet.Range("A1:G1"));
            worksheet.Cell("A1").Value = model.Titulo;

            StyleHelper.AplicarSubtitulo(worksheet.Range("A2:G2"));
            worksheet.Cell("A2").Value = model.Subtitulo;

            worksheet.Cell("A4").Value = "Descrição";
            worksheet.Cell("B4").Value = "Tipo";
            worksheet.Cell("C4").Value = "Valor";
            worksheet.Cell("D4").Value = "Vencimento";
            worksheet.Cell("E4").Value = "Efetivação do pagamento/recebimento";
            worksheet.Cell("F4").Value = "Status";
            worksheet.Cell("G4").Value = "Data do lançamento";
            StyleHelper.AplicarCabecalho(worksheet.Range("A4:G4"));

            var linhaAtual = 5;
            foreach (var item in model.Itens)
            {
                worksheet.Cell(linhaAtual, 1).Value = item.Descricao;
                worksheet.Cell(linhaAtual, 2).Value = item.Tipo;
                worksheet.Cell(linhaAtual, 3).Value = item.Valor;
                worksheet.Cell(linhaAtual, 4).Value = item.DataVencimento;
                worksheet.Cell(linhaAtual, 5).Value = item.DataEfetivacao;
                worksheet.Cell(linhaAtual, 6).Value = item.Status;
                worksheet.Cell(linhaAtual, 7).Value = item.DataLancamento;

                StyleHelper.AplicarLinhaTabela(worksheet.Range(linhaAtual, 1, linhaAtual, 7));
                linhaAtual++;
            }

            StyleHelper.AplicarTotalizador(worksheet.Range(linhaAtual, 1, linhaAtual, 7));
            worksheet.Cell(linhaAtual, 1).Value = "Total de lançamentos";
            worksheet.Cell(linhaAtual, 3).Value = model.Itens.Sum(x => x.Valor);
            worksheet.Cell(linhaAtual, 7).Value = model.Itens.Count;

            worksheet.Column(3).AplicarFormatoMoeda();
            worksheet.Column(4).AplicarFormatoData();
            worksheet.Column(5).AplicarFormatoData();
            worksheet.Column(7).AplicarFormatoData();
            worksheet.FinalizarLayout(4);

            return CriarArquivo(workbook, model.NomeArquivo);
        }
    }
}
