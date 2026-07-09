using ClosedXML.Excel;
using MinhasFinancas.CrossCutting.Reports;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public class MfScoreAuditoriaExcelReport : ExcelReportBase, IExcelReport<MfScoreAuditoriaExcelReportData>
    {
        public MfScoreAuditoriaExcelReport(ExcelWorkbookFactory workbookFactory, ExcelStyleHelper styleHelper)
            : base(workbookFactory, styleHelper)
        {
        }

        public ArquivoRelatorioDTO Gerar(MfScoreAuditoriaExcelReportData model)
        {
            using var workbook = WorkbookFactory.Criar();

            MontarResumo(workbook.Worksheets.Add("Resumo"), model);
            MontarCenarios(workbook.Worksheets.Add("Cenarios"), model);
            MontarPilares(workbook.Worksheets.Add("Pilares"), model);
            MontarIndicadoresCriticos(workbook.Worksheets.Add("Indicadores Criticos"), model);
            MontarDadosEntrada(workbook.Worksheets.Add("Dados de Entrada"), model);

            return CriarArquivo(workbook, model.NomeArquivo);
        }

        private void MontarResumo(IXLWorksheet worksheet, MfScoreAuditoriaExcelReportData model)
        {
            var totalCenarios = model.Cenarios.Count;
            var cenariosOk = model.Cenarios.Count(cenario => cenario.Status == "OK");
            var cenariosFalha = totalCenarios - cenariosOk;
            var percentualAprovacao = totalCenarios > 0
                ? (decimal)cenariosOk / totalCenarios
                : 0m;

            StyleHelper.AplicarTitulo(worksheet.Range("A1:B1"));
            worksheet.Cell("A1").Value = "Auditoria do MF Score";

            StyleHelper.AplicarSubtitulo(worksheet.Range("A2:B2"));
            worksheet.Cell("A2").Value = "Resumo geral da auditoria interna por personas oficiais";

            worksheet.Cell("A4").Value = "Total de cenarios";
            worksheet.Cell("B4").Value = totalCenarios;
            worksheet.Cell("A5").Value = "Cenarios OK";
            worksheet.Cell("B5").Value = cenariosOk;
            worksheet.Cell("A6").Value = "Cenarios com falha";
            worksheet.Cell("B6").Value = cenariosFalha;
            worksheet.Cell("A7").Value = "Percentual de aprovacao";
            worksheet.Cell("B7").Value = percentualAprovacao;
            worksheet.Cell("A8").Value = "Data de geracao";
            worksheet.Cell("B8").Value = model.DataGeracao;
            worksheet.Cell("A9").Value = "Versao do MF Score";
            worksheet.Cell("B9").Value = model.VersaoMfScore;

            for (var linha = 4; linha <= 9; linha++)
            {
                StyleHelper.AplicarResumoRotulo(worksheet.Cell(linha, 1));

                if (linha == 7)
                {
                    worksheet.Cell(linha, 2).Style.NumberFormat.Format = "0.00%";
                    worksheet.Cell(linha, 2).Style.Font.Bold = true;
                    worksheet.Cell(linha, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(linha, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#DBEAFE");
                    worksheet.Cell(linha, 2).Value = percentualAprovacao;
                    continue;
                }

                if (linha == 8)
                {
                    worksheet.Cell(linha, 2).Style.DateFormat.Format = ExcelNumberFormats.DateTime;
                    worksheet.Cell(linha, 2).Style.Font.Bold = true;
                    worksheet.Cell(linha, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(linha, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#DBEAFE");
                    worksheet.Cell(linha, 2).Value = model.DataGeracao;
                    continue;
                }

                if (linha == 9)
                {
                    worksheet.Cell(linha, 2).Style.Font.Bold = true;
                    worksheet.Cell(linha, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(linha, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#DBEAFE");
                    worksheet.Cell(linha, 2).Value = model.VersaoMfScore;
                    continue;
                }

                worksheet.Cell(linha, 2).Style.Font.Bold = true;
                worksheet.Cell(linha, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(linha, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#DBEAFE");
            }

            worksheet.FinalizarLayout(3);
        }

        private void MontarCenarios(IXLWorksheet worksheet, MfScoreAuditoriaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:J1"));
            worksheet.Cell("A1").Value = "Cenarios auditados";

            EscreverCabecalho(worksheet, 3, "Persona", "Descricao", "Score minimo", "Score maximo", "Score obtido", "Status", "Classificacao", "Risco", "Justificativa", "Observacoes");

            var linha = 4;
            foreach (var cenario in model.Cenarios)
            {
                worksheet.Cell(linha, 1).Value = cenario.Persona;
                worksheet.Cell(linha, 2).Value = cenario.Descricao;
                worksheet.Cell(linha, 3).Value = cenario.ScoreEsperadoMin;
                worksheet.Cell(linha, 4).Value = cenario.ScoreEsperadoMax;
                worksheet.Cell(linha, 5).Value = cenario.ScoreObtido;
                worksheet.Cell(linha, 6).Value = cenario.Status;
                worksheet.Cell(linha, 7).Value = cenario.Classificacao;
                worksheet.Cell(linha, 8).Value = cenario.Risco;
                worksheet.Cell(linha, 9).Value = cenario.Justificativa;
                worksheet.Cell(linha, 10).Value = cenario.Observacoes;

                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 10));
                AplicarStatus(worksheet.Cell(linha, 6), cenario.Status);
                linha++;
            }

            worksheet.FinalizarLayout(3);
        }

        private void MontarPilares(IXLWorksheet worksheet, MfScoreAuditoriaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:F1"));
            worksheet.Cell("A1").Value = "Notas dos pilares";

            EscreverCabecalho(worksheet, 3, "Persona", "Fluxo de Caixa", "Liquidez e Reserva", "Endividamento e Obrigacoes", "Patrimonio", "Planejamento e Disciplina");

            var linha = 4;
            foreach (var cenario in model.Cenarios)
            {
                worksheet.Cell(linha, 1).Value = cenario.Persona;
                worksheet.Cell(linha, 2).Value = cenario.FluxoDeCaixa;
                worksheet.Cell(linha, 3).Value = cenario.LiquidezEReserva;
                worksheet.Cell(linha, 4).Value = cenario.EndividamentoEObrigacoes;
                worksheet.Cell(linha, 5).Value = cenario.Patrimonio;
                worksheet.Cell(linha, 6).Value = cenario.PlanejamentoEDisciplina;
                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 6));
                linha++;
            }

            worksheet.FinalizarLayout(3);
        }

        private void MontarIndicadoresCriticos(IXLWorksheet worksheet, MfScoreAuditoriaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:E1"));
            worksheet.Cell("A1").Value = "Indicadores criticos";

            EscreverCabecalho(worksheet, 3, "Persona", "Indicador", "Valor", "Penalizacao", "Observacao");

            var linha = 4;
            foreach (var cenario in model.Cenarios)
            {
                if (cenario.IndicadoresCriticos.Count == 0)
                {
                    worksheet.Cell(linha, 1).Value = cenario.Persona;
                    worksheet.Cell(linha, 2).Value = "Nenhum indicador critico";
                    worksheet.Cell(linha, 5).Value = "Nenhuma penalizacao aplicada neste cenario.";
                    StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 5));
                    linha++;
                    continue;
                }

                foreach (var indicadorCritico in cenario.IndicadoresCriticos)
                {
                    worksheet.Cell(linha, 1).Value = indicadorCritico.Persona;
                    worksheet.Cell(linha, 2).Value = indicadorCritico.Indicador;
                    worksheet.Cell(linha, 3).Value = indicadorCritico.Valor;
                    worksheet.Cell(linha, 4).Value = indicadorCritico.Penalidade;
                    worksheet.Cell(linha, 5).Value = indicadorCritico.Observacao;
                    StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 5));
                    linha++;
                }
            }

            worksheet.Column(3).AplicarFormatoMoeda();
            worksheet.Column(4).AplicarFormatoMoeda();
            worksheet.FinalizarLayout(3);
        }

        private void MontarDadosEntrada(IXLWorksheet worksheet, MfScoreAuditoriaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:J1"));
            worksheet.Cell("A1").Value = "Dados de entrada";

            EscreverCabecalho(
                worksheet,
                3,
                "Persona",
                "Renda",
                "Despesas",
                "Reserva",
                "Patrimonio",
                "Passivos",
                "Obrigacoes 30 dias",
                "Obrigacoes 90 dias",
                "Obrigacoes 180 dias",
                "Obrigacoes 12 meses");

            var linha = 4;
            foreach (var cenario in model.Cenarios)
            {
                worksheet.Cell(linha, 1).Value = cenario.Persona;
                worksheet.Cell(linha, 2).Value = cenario.DadosEntrada.Renda;
                worksheet.Cell(linha, 3).Value = cenario.DadosEntrada.Despesas;
                worksheet.Cell(linha, 4).Value = cenario.DadosEntrada.Reserva;
                worksheet.Cell(linha, 5).Value = cenario.DadosEntrada.Patrimonio;
                worksheet.Cell(linha, 6).Value = cenario.DadosEntrada.Passivos;
                worksheet.Cell(linha, 7).Value = cenario.DadosEntrada.ObrigacoesFuturas30Dias;
                worksheet.Cell(linha, 8).Value = cenario.DadosEntrada.ObrigacoesFuturas90Dias;
                worksheet.Cell(linha, 9).Value = cenario.DadosEntrada.ObrigacoesFuturas180Dias;
                worksheet.Cell(linha, 10).Value = cenario.DadosEntrada.ObrigacoesFuturas12Meses;
                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 10));
                linha++;
            }

            for (var coluna = 2; coluna <= 10; coluna++)
            {
                worksheet.Column(coluna).AplicarFormatoMoeda();
            }

            worksheet.FinalizarLayout(3);
        }

        private void EscreverCabecalho(IXLWorksheet worksheet, int linha, params string[] colunas)
        {
            for (var indice = 0; indice < colunas.Length; indice++)
            {
                worksheet.Cell(linha, indice + 1).Value = colunas[indice];
            }

            StyleHelper.AplicarCabecalho(worksheet.Range(linha, 1, linha, colunas.Length));
        }

        private static void AplicarStatus(IXLCell cell, string status)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Fill.BackgroundColor = status == "OK"
                ? XLColor.FromHtml("#DCFCE7")
                : XLColor.FromHtml("#FEE2E2");
            cell.Style.Font.FontColor = status == "OK"
                ? XLColor.FromHtml("#166534")
                : XLColor.FromHtml("#991B1B");
        }
    }
}
