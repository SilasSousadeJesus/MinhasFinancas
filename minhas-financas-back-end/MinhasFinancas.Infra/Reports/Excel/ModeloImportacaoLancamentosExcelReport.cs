using ClosedXML.Excel;
using MinhasFinancas.CrossCutting.Reports;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public class ModeloImportacaoLancamentosExcelReport : ExcelReportBase, IExcelReport<ModeloImportacaoLancamentosExcelReportData>
    {
        private const int PrimeiraLinhaDados = LinhaCabecalho + 1;
        private const int UltimaLinhaDados = 505;
        private const int LinhaCabecalho = 5;
        private const string RecursoModeloBase = "MinhasFinancas.Infra.Resources.Modelos.modelo-importacao-lancamentos-base.xlsx";

        public ModeloImportacaoLancamentosExcelReport(ExcelWorkbookFactory workbookFactory, ExcelStyleHelper styleHelper)
            : base(workbookFactory, styleHelper)
        {
        }

        public ArquivoRelatorioDTO Gerar(ModeloImportacaoLancamentosExcelReportData model)
        {
            using var streamModelo = typeof(ModeloImportacaoLancamentosExcelReport).Assembly
                .GetManifestResourceStream(RecursoModeloBase)
                ?? throw new InvalidOperationException("Modelo-base de importação não encontrado.");
            using var workbook = new XLWorkbook(streamModelo);
            var lancamentos = workbook.Worksheet("Lancamentos");
            var listas = workbook.Worksheet("Listas");

            listas.Clear(XLClearOptions.Contents);

            AdicionarLista(listas, 1, "Tipo", ["Despesa", "Receita", "InvestimentoDeposito", "InvestimentoSaque", "Transferencia", "Saque", "Deposito"]);
            AdicionarLista(listas, 2, "Frequencia", ["Pontual", "Fixo", "Parcelado", "DiaUtil"]);
            AdicionarLista(listas, 3, "Categoria", model.Categorias);
            AdicionarLista(listas, 4, "Subcategoria", model.Subcategorias);
            AdicionarLista(listas, 5, "Conta", model.Contas);
            AdicionarLista(listas, 6, "Cartao", model.Cartoes);

            AplicarValidacao(lancamentos, 3, "Listas!$A$2:$A$8");
            AplicarValidacao(lancamentos, 7, "Listas!$B$2:$B$5");
            AplicarValidacao(lancamentos, 10, $"Listas!$C$2:$C${Math.Max(model.Categorias.Count + 1, 2)}");
            AplicarValidacao(lancamentos, 11, $"Listas!$D$2:$D${Math.Max(model.Subcategorias.Count + 1, 2)}");
            AplicarValidacao(lancamentos, 12, $"Listas!$E$2:$E${Math.Max(model.Contas.Count + 1, 2)}");
            AplicarValidacao(lancamentos, 13, $"Listas!$F$2:$F${Math.Max(model.Cartoes.Count + 1, 2)}");

            listas.Visibility = XLWorksheetVisibility.VeryHidden;
            return CriarArquivo(workbook, model.NomeArquivo);
        }

        private static void AdicionarLista(IXLWorksheet worksheet, int coluna, string titulo, IEnumerable<string> valores)
        {
            worksheet.Cell(1, coluna).Value = titulo;
            var linha = 2;
            foreach (var valor in valores.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x))
            {
                worksheet.Cell(linha++, coluna).Value = valor;
            }
        }

        private static void AplicarValidacao(IXLWorksheet worksheet, int coluna, string intervalo)
        {
            var intervaloDados = worksheet.Range(PrimeiraLinhaDados, coluna, UltimaLinhaDados, coluna);
            var validacoesExistentes = worksheet.DataValidations
                .GetAllInRange(intervaloDados.RangeAddress)
                .ToList();

            if (validacoesExistentes.Count == 0)
            {
                intervaloDados.CreateDataValidation().List(intervalo, true);
                return;
            }

            foreach (var validacao in validacoesExistentes)
            {
                validacao.List(intervalo, true);
            }
        }

    }
}
