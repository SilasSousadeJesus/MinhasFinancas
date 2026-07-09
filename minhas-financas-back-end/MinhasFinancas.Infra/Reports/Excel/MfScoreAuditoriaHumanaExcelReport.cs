using ClosedXML.Excel;
using MinhasFinancas.CrossCutting.Reports;

namespace MinhasFinancas.Infra.Reports.Excel
{
    public class MfScoreAuditoriaHumanaExcelReport : ExcelReportBase, IExcelReport<MfScoreAuditoriaHumanaExcelReportData>
    {
        public MfScoreAuditoriaHumanaExcelReport(ExcelWorkbookFactory workbookFactory, ExcelStyleHelper styleHelper)
            : base(workbookFactory, styleHelper)
        {
        }

        public ArquivoRelatorioDTO Gerar(MfScoreAuditoriaHumanaExcelReportData model)
        {
            using var workbook = WorkbookFactory.Criar();

            MontarInstrucoes(workbook.Worksheets.Add("Instrucoes"), model);
            MontarResumoPersonas(workbook.Worksheets.Add("Resumo das Personas"), model);
            MontarDadosEntrada(workbook.Worksheets.Add("Dados de Entrada"), model);
            MontarIndicadores(workbook.Worksheets.Add("Indicadores"), model);
            MontarPilares(workbook.Worksheets.Add("Pilares"), model);
            MontarPenalizacoes(workbook.Worksheets.Add("Penalizacoes"), model);
            MontarAvaliacaoHumanaDetalhada(workbook.Worksheets.Add("Avaliacao Humana"), model);
            MontarReferenciaAutomatica(workbook.Worksheets.Add("Referencia Automatica"), model);

            return CriarArquivo(workbook, model.NomeArquivo);
        }

        private void MontarInstrucoes(IXLWorksheet worksheet, MfScoreAuditoriaHumanaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:F1"));
            worksheet.Cell("A1").Value = "Auditoria Humana do MF Score";

            StyleHelper.AplicarSubtitulo(worksheet.Range("A2:F2"));
            worksheet.Cell("A2").Value = "Use esta planilha para avaliar cada persona como um consultor financeiro, sem depender da aprovacao automatica.";

            var instrucoes = new[]
            {
                "1. Comece pela aba Resumo das Personas e avalie a coerencia geral de cada caso.",
                "2. Use Dados de Entrada, Indicadores, Pilares e Penalizacoes para entender como o motor chegou ao score calculado.",
                "3. Preencha manualmente os campos de nota humana, faixa humana e observacoes de auditoria.",
                "4. A aba Referencia Automatica existe apenas como apoio secundario e nao deve guiar a avaliacao cega inicial.",
                "5. Quando uma persona estiver madura o suficiente, ela pode evoluir para caso canonico oficial do sistema."
            };

            worksheet.Cell("A4").Value = "Data de geracao";
            worksheet.Cell("B4").Value = model.DataGeracao;
            worksheet.Cell("A5").Value = "Versao do MF Score";
            worksheet.Cell("B5").Value = model.VersaoMfScore;

            StyleHelper.AplicarResumoRotulo(worksheet.Cell("A4"));
            StyleHelper.AplicarResumoRotulo(worksheet.Cell("A5"));
            worksheet.Cell("B4").Style.DateFormat.Format = ExcelNumberFormats.DateTime;
            worksheet.Cell("B4").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell("B4").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBEAFE");
            worksheet.Cell("B5").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell("B5").Style.Fill.BackgroundColor = XLColor.FromHtml("#DBEAFE");

            StyleHelper.AplicarTituloSecao(worksheet.Range("A7:F7"));
            worksheet.Cell("A7").Value = "Como auditar";

            for (var indice = 0; indice < instrucoes.Length; indice++)
            {
                worksheet.Cell(indice + 8, 1).Value = instrucoes[indice];
                worksheet.Range(indice + 8, 1, indice + 8, 6).Merge();
                worksheet.Range(indice + 8, 1, indice + 8, 6).Style.Alignment.WrapText = true;
                worksheet.Range(indice + 8, 1, indice + 8, 6).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Range(indice + 8, 1, indice + 8, 6).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Range(indice + 8, 1, indice + 8, 6).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            }

            worksheet.FinalizarLayout(3);
        }

        private void MontarResumoPersonas(IXLWorksheet worksheet, MfScoreAuditoriaHumanaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:U1"));
            worksheet.Cell("A1").Value = "Resumo das Personas";

            EscreverCabecalho(
                worksheet,
                3,
                "Persona",
                "Objetivo da persona",
                "Descricao",
                "Score calculado",
                "Classificacao calculada",
                "Risco calculado",
                "Fluxo de Caixa",
                "Liquidez e Reserva",
                "Endividamento e Obrigacoes",
                "Patrimonio",
                "Planejamento e Disciplina",
                "Indicadores criticos",
                "Penalizacoes aplicadas",
                "Nota humana sugerida",
                "Faixa humana minima",
                "Faixa humana maxima",
                "Motor foi severo demais?",
                "Motor foi permissivo demais?",
                "Principal problema percebido",
                "Observacoes do auditor",
                "Aprovar como padrao oficial? Sim/Não");

            var linha = 4;
            foreach (var persona in model.Personas)
            {
                worksheet.Cell(linha, 1).Value = persona.Persona;
                worksheet.Cell(linha, 2).Value = persona.Objetivo;
                worksheet.Cell(linha, 3).Value = persona.Descricao;
                worksheet.Cell(linha, 4).Value = persona.ScoreCalculado;
                worksheet.Cell(linha, 5).Value = persona.ClassificacaoCalculada;
                worksheet.Cell(linha, 6).Value = persona.RiscoCalculado;
                worksheet.Cell(linha, 7).Value = persona.FluxoDeCaixa;
                worksheet.Cell(linha, 8).Value = persona.LiquidezEReserva;
                worksheet.Cell(linha, 9).Value = persona.EndividamentoEObrigacoes;
                worksheet.Cell(linha, 10).Value = persona.Patrimonio;
                worksheet.Cell(linha, 11).Value = persona.PlanejamentoEDisciplina;
                worksheet.Cell(linha, 12).Value = persona.IndicadoresCriticosResumo;
                worksheet.Cell(linha, 13).Value = persona.PenalizacoesAplicadasResumo;
                worksheet.Cell(linha, 14).Value = string.Empty;
                worksheet.Cell(linha, 15).Value = string.Empty;
                worksheet.Cell(linha, 16).Value = string.Empty;
                worksheet.Cell(linha, 17).Value = string.Empty;
                worksheet.Cell(linha, 18).Value = string.Empty;
                worksheet.Cell(linha, 19).Value = string.Empty;
                worksheet.Cell(linha, 20).Value = string.Empty;
                worksheet.Cell(linha, 21).Value = string.Empty;

                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 21));
                worksheet.Range(linha, 14, linha, 21).Style.Fill.BackgroundColor = XLColor.FromHtml("#FEF3C7");
                worksheet.Range(linha, 14, linha, 21).Style.Alignment.WrapText = true;
                linha++;
            }

            AplicarFiltroETamanho(worksheet, 3, 21);
        }

        private void MontarDadosEntrada(IXLWorksheet worksheet, MfScoreAuditoriaHumanaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:O1"));
            worksheet.Cell("A1").Value = "Dados de Entrada";

            EscreverCabecalho(
                worksheet,
                3,
                "Persona",
                "Renda mensal",
                "Receitas no periodo",
                "Despesas mensais",
                "Despesas futuras 30 dias",
                "Despesas futuras 90 dias",
                "Despesas futuras 180 dias",
                "Despesas futuras 12 meses",
                "Reserva",
                "Patrimonio bruto",
                "Passivos",
                "Patrimonio liquido",
                "Perfil financeiro configurado?",
                "Plano estrategico?",
                "Compromissos?");

            var linha = 4;
            foreach (var persona in model.Personas)
            {
                worksheet.Cell(linha, 1).Value = persona.Persona;
                worksheet.Cell(linha, 2).Value = persona.DadosEntrada.RendaMensal;
                worksheet.Cell(linha, 3).Value = persona.DadosEntrada.ReceitasNoPeriodo;
                worksheet.Cell(linha, 4).Value = persona.DadosEntrada.DespesasMensais;
                worksheet.Cell(linha, 5).Value = persona.DadosEntrada.DespesasFuturas30Dias;
                worksheet.Cell(linha, 6).Value = persona.DadosEntrada.DespesasFuturas90Dias;
                worksheet.Cell(linha, 7).Value = persona.DadosEntrada.DespesasFuturas180Dias;
                worksheet.Cell(linha, 8).Value = persona.DadosEntrada.DespesasFuturas12Meses;
                worksheet.Cell(linha, 9).Value = persona.DadosEntrada.Reserva;
                worksheet.Cell(linha, 10).Value = persona.DadosEntrada.PatrimonioBruto;
                worksheet.Cell(linha, 11).Value = persona.DadosEntrada.Passivos;
                worksheet.Cell(linha, 12).Value = persona.DadosEntrada.PatrimonioLiquido;
                worksheet.Cell(linha, 13).Value = persona.DadosEntrada.PerfilFinanceiroConfigurado;
                worksheet.Cell(linha, 14).Value = persona.DadosEntrada.PlanoEstrategico;
                worksheet.Cell(linha, 15).Value = persona.DadosEntrada.Compromissos;
                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 15));
                linha++;
            }

            for (var coluna = 2; coluna <= 12; coluna++)
            {
                worksheet.Column(coluna).AplicarFormatoMoeda();
            }

            AplicarFiltroETamanho(worksheet, 3, 15);
        }

        private void MontarIndicadores(IXLWorksheet worksheet, MfScoreAuditoriaHumanaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:I1"));
            worksheet.Cell("A1").Value = "Indicadores";

            EscreverCabecalho(
                worksheet,
                3,
                "Persona",
                "Indicador",
                "Valor atual",
                "Valor ideal",
                "Percentual",
                "Status",
                "Descricao",
                "Observacao",
                "Pilar relacionado");

            var linha = 4;
            foreach (var indicador in model.Personas.SelectMany(persona => persona.Indicadores))
            {
                worksheet.Cell(linha, 1).Value = indicador.Persona;
                worksheet.Cell(linha, 2).Value = indicador.Indicador;
                worksheet.Cell(linha, 3).Value = indicador.ValorAtual;
                worksheet.Cell(linha, 4).Value = indicador.ValorIdeal;
                worksheet.Cell(linha, 5).Value = indicador.Percentual;
                worksheet.Cell(linha, 6).Value = indicador.Status;
                worksheet.Cell(linha, 7).Value = indicador.Descricao;
                worksheet.Cell(linha, 8).Value = indicador.Observacao;
                worksheet.Cell(linha, 9).Value = indicador.PilarRelacionado;
                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 9));
                linha++;
            }

            worksheet.Column(3).AplicarFormatoMoeda();
            worksheet.Column(4).AplicarFormatoMoeda();
            worksheet.Column(5).Style.NumberFormat.Format = "0.00%";
            AplicarFiltroETamanho(worksheet, 3, 9);
        }

        private void MontarPilares(IXLWorksheet worksheet, MfScoreAuditoriaHumanaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:F1"));
            worksheet.Cell("A1").Value = "Pilares";

            EscreverCabecalho(
                worksheet,
                3,
                "Persona",
                "Pilar",
                "Nota do pilar",
                "Peso do pilar",
                "Contribuicao para score base",
                "Observacao");

            var linha = 4;
            foreach (var pilar in model.Personas.SelectMany(persona => persona.Pilares))
            {
                worksheet.Cell(linha, 1).Value = pilar.Persona;
                worksheet.Cell(linha, 2).Value = pilar.Pilar;
                worksheet.Cell(linha, 3).Value = pilar.NotaPilar;
                worksheet.Cell(linha, 4).Value = pilar.PesoPilar;
                worksheet.Cell(linha, 5).Value = pilar.ContribuicaoScoreBase;
                worksheet.Cell(linha, 6).Value = pilar.Observacao;
                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 6));
                linha++;
            }

            worksheet.Column(4).Style.NumberFormat.Format = "0.00";
            worksheet.Column(5).Style.NumberFormat.Format = "0.00";
            AplicarFiltroETamanho(worksheet, 3, 6);
        }

        private void MontarPenalizacoes(IXLWorksheet worksheet, MfScoreAuditoriaHumanaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:F1"));
            worksheet.Cell("A1").Value = "Penalizacoes e Regras Criticas";

            EscreverCabecalho(
                worksheet,
                3,
                "Persona",
                "Regra critica",
                "Indicador relacionado",
                "Valor",
                "Penalizacao",
                "Justificativa");

            var linha = 4;
            foreach (var penalizacao in model.Personas.SelectMany(persona => persona.Penalizacoes))
            {
                worksheet.Cell(linha, 1).Value = penalizacao.Persona;
                worksheet.Cell(linha, 2).Value = penalizacao.RegraCritica;
                worksheet.Cell(linha, 3).Value = penalizacao.IndicadorRelacionado;
                worksheet.Cell(linha, 4).Value = penalizacao.Valor;
                worksheet.Cell(linha, 5).Value = penalizacao.Penalizacao;
                worksheet.Cell(linha, 6).Value = penalizacao.Justificativa;
                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 6));
                linha++;
            }

            worksheet.Column(4).AplicarFormatoMoeda();
            worksheet.Column(5).AplicarFormatoMoeda();
            AplicarFiltroETamanho(worksheet, 3, 6);
        }

        private void MontarAvaliacaoHumanaDetalhada(IXLWorksheet worksheet, MfScoreAuditoriaHumanaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:B1"));
            worksheet.Cell("A1").Value = "Avaliacao Humana Detalhada";

            var linha = 3;
            foreach (var persona in model.Personas)
            {
                StyleHelper.AplicarTituloSecao(worksheet.Range(linha, 1, linha, 2));
                worksheet.Cell(linha, 1).Value = persona.Persona;
                linha++;

                var perguntas = new[]
                {
                    "Nota que um consultor financeiro daria",
                    "Faixa esperada sugerida",
                    "O score calculado parece coerente?",
                    "O que o motor acertou?",
                    "O que o motor errou?",
                    "O motor foi severo demais?",
                    "O motor foi permissivo demais?",
                    "Quais pilares parecem mal calibrados?",
                    "Quais penalizacoes parecem exageradas?",
                    "Quais penalizacoes parecem fracas?",
                    "Essa persona deve virar caso canonico?",
                    "Observacoes finais"
                };

                foreach (var pergunta in perguntas)
                {
                    worksheet.Cell(linha, 1).Value = pergunta;
                    worksheet.Cell(linha, 2).Value = string.Empty;
                    worksheet.Cell(linha, 1).Style.Font.Bold = true;
                    worksheet.Cell(linha, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(linha, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(linha, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FEF3C7");
                    worksheet.Cell(linha, 2).Style.Alignment.WrapText = true;
                    linha++;
                }

                linha++;
            }

            worksheet.Column(2).Width = 80;
            worksheet.FinalizarLayout(2);
        }

        private void MontarReferenciaAutomatica(IXLWorksheet worksheet, MfScoreAuditoriaHumanaExcelReportData model)
        {
            StyleHelper.AplicarTitulo(worksheet.Range("A1:F1"));
            worksheet.Cell("A1").Value = "Referencia Automatica";

            StyleHelper.AplicarSubtitulo(worksheet.Range("A2:F2"));
            worksheet.Cell("A2").Value = "Use esta aba apenas depois da avaliacao humana inicial, para comparar a faixa automatica atual com a leitura manual.";

            EscreverCabecalho(
                worksheet,
                4,
                "Persona",
                "Score esperado minimo atual",
                "Score esperado maximo atual",
                "Score calculado",
                "Status automatico atual",
                "Observacao");

            var linha = 5;
            foreach (var persona in model.Personas)
            {
                var statusAtual = persona.ScoreCalculado >= persona.ScoreEsperadoMinAtual && persona.ScoreCalculado <= persona.ScoreEsperadoMaxAtual
                    ? "OK"
                    : "FALHA";

                worksheet.Cell(linha, 1).Value = persona.Persona;
                worksheet.Cell(linha, 2).Value = persona.ScoreEsperadoMinAtual;
                worksheet.Cell(linha, 3).Value = persona.ScoreEsperadoMaxAtual;
                worksheet.Cell(linha, 4).Value = persona.ScoreCalculado;
                worksheet.Cell(linha, 5).Value = statusAtual;
                worksheet.Cell(linha, 6).Value = "Referencia secundaria para calibracao posterior.";
                StyleHelper.AplicarLinhaTabela(worksheet.Range(linha, 1, linha, 6));
                linha++;
            }

            AplicarFiltroETamanho(worksheet, 4, 6);
        }

        private void EscreverCabecalho(IXLWorksheet worksheet, int linha, params string[] colunas)
        {
            for (var indice = 0; indice < colunas.Length; indice++)
            {
                worksheet.Cell(linha, indice + 1).Value = colunas[indice];
            }

            StyleHelper.AplicarCabecalho(worksheet.Range(linha, 1, linha, colunas.Length));
        }

        private static void AplicarFiltroETamanho(IXLWorksheet worksheet, int linhaCabecalho, int quantidadeColunas)
        {
            worksheet.Range(linhaCabecalho, 1, Math.Max(linhaCabecalho + 1, worksheet.LastRowUsed()?.RowNumber() ?? linhaCabecalho), quantidadeColunas)
                .SetAutoFilter();

            worksheet.Rows().Style.Alignment.WrapText = true;
            worksheet.FinalizarLayout(linhaCabecalho);
        }
    }
}
