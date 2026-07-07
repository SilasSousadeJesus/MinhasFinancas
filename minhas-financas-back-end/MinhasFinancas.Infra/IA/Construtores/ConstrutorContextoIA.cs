using System.Globalization;
using System.Text;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Construtores
{
    public class ConstrutorContextoIA
    {
        public ContextoAssistenteFinanceiro Construir(
            ResumoFinanceiroIA resumoFinanceiroIA,
            string? perguntaUsuario = null,
            IEnumerable<MemoriaFinanceiraResumidaIA>? memoriaFinanceira = null)
        {
            var cultura = new CultureInfo("pt-BR");

            var prioridades = resumoFinanceiroIA.PrioridadesImediatas
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            var destaques = resumoFinanceiroIA.DestaquesPositivos
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            var insightsPrioritarios = resumoFinanceiroIA.Insights.Prioritarios
                .Select(FormatarInsight)
                .ToList();

            var insightsPositivos = resumoFinanceiroIA.Insights.DestaquesPositivos
                .Select(FormatarInsight)
                .ToList();

            var pontosAtencao = resumoFinanceiroIA.SaudeFinanceira.PontosAtencao
                .Select(FormatarPontoAtencao)
                .ToList();

            var indicadoresEmAtencao = resumoFinanceiroIA.Indicadores.Todos
                .Where(indicador => indicador.Status is StatusIndicadorFinanceiro.Atencao or StatusIndicadorFinanceiro.Critico)
                .Select(indicador => FormatarIndicador(indicador, cultura))
                .ToList();

            var indicadoresPositivos = resumoFinanceiroIA.Indicadores.Todos
                .Where(indicador => indicador.Status is StatusIndicadorFinanceiro.Bom or StatusIndicadorFinanceiro.Excelente)
                .Select(indicador => FormatarIndicador(indicador, cultura))
                .ToList();

            var todosIndicadores = resumoFinanceiroIA.Indicadores.Todos
                .Select(indicador => FormatarIndicador(indicador, cultura))
                .ToList();

            var memoriaFinanceiraResumida = memoriaFinanceira?
                .Select(FormatarMemoriaFinanceira)
                .ToList() ?? [];

            var secoes = new List<string>
            {
                MontarSecao(
                    "Resumo Executivo",
                    [
                        $"Data de referÃªncia: {resumoFinanceiroIA.DataReferencia:dd/MM/yyyy}",
                        $"Resumo do sistema: {resumoFinanceiroIA.ResumoExecutivo}"
                    ]),
                MontarSecao(
                    "SaÃºde Financeira",
                    [
                        $"PontuaÃ§Ã£o geral: {resumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral}/100",
                        $"ClassificaÃ§Ã£o atual: {resumoFinanceiroIA.SaudeFinanceira.Classificacao}"
                    ]),
                MontarSecao(
                    "Pontos de AtenÃ§Ã£o TÃ©cnicos",
                    pontosAtencao,
                    "- Nenhum ponto de atenÃ§Ã£o tÃ©cnico relevante foi identificado pelo sistema."),
                MontarSecao(
                    "Prioridades Imediatas",
                    prioridades.Select(item => $"- {item}"),
                    "- Nenhuma prioridade imediata foi registrada."),
                MontarSecao(
                    "Destaques Positivos",
                    destaques.Select(item => $"- {item}"),
                    "- Nenhum destaque positivo foi registrado."),
                MontarSecao(
                    "Indicadores em AtenÃ§Ã£o",
                    indicadoresEmAtencao,
                    "- Nenhum indicador estÃ¡ em faixa de atenÃ§Ã£o ou crÃ­tica."),
                MontarSecao(
                    "Indicadores Positivos",
                    indicadoresPositivos,
                    "- Nenhum indicador positivo foi destacado nesta leitura."),
                MontarSecao(
                    "Indicadores Financeiros Consolidados",
                    todosIndicadores,
                    "- NÃ£o hÃ¡ indicadores consolidados disponÃ­veis."),
                MontarSecao(
                    "Insights Financeiros PrioritÃ¡rios",
                    insightsPrioritarios,
                    "- Nenhum insight prioritÃ¡rio estÃ¡ disponÃ­vel."),
                MontarSecao(
                    "Insights Positivos",
                    insightsPositivos,
                    "- Nenhum insight positivo estÃ¡ disponÃ­vel."),
                MontarSecao(
                    "MemÃ³ria Financeira",
                    memoriaFinanceiraResumida,
                    "NÃ£o existem anÃ¡lises anteriores."),
                MontarSecao(
                    "Cobertura Atual do Contexto",
                    [
                        "- Perfil financeiro: jÃ¡ refletido de forma indireta nos indicadores, na saÃºde financeira e nas prioridades.",
                        "- PatrimÃ´nio: jÃ¡ refletido pelos indicadores de patrimÃ´nio lÃ­quido atual e percentual do patrimÃ´nio-alvo.",
                        "- Fluxo de caixa: jÃ¡ refletido pelos indicadores de economia mensal, percentual de economia e comprometimento da renda.",
                        "- TendÃªncias, radar financeiro operacional, projeÃ§Ãµes, simulaÃ§Ãµes e detalhamento patrimonial ainda nÃ£o sÃ£o enviados como blocos prÃ³prios nesta fase.",
                        "- Quando algum dado nÃ£o estiver explicitamente descrito no contexto, a anÃ¡lise deve se limitar Ã s informaÃ§Ãµes estruturadas recebidas."
                    ])
            };

            if (!string.IsNullOrWhiteSpace(perguntaUsuario))
            {
                secoes.Add(MontarSecao("Pergunta do UsuÃ¡rio", [$"- {perguntaUsuario}"]));
            }

            return new ContextoAssistenteFinanceiro
            {
                DataReferencia = resumoFinanceiroIA.DataReferencia,
                PontuacaoSaudeFinanceira = resumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral,
                ClassificacaoSaudeFinanceira = resumoFinanceiroIA.SaudeFinanceira.Classificacao,
                PrioridadesImediatas = prioridades,
                DestaquesPositivos = destaques,
                InsightsPrioritarios = insightsPrioritarios,
                MemoriaFinanceiraResumida = memoriaFinanceiraResumida,
                ResumoExecutivo = resumoFinanceiroIA.ResumoExecutivo,
                ContextoTextual = string.Join(Environment.NewLine + Environment.NewLine, secoes),
                PerguntaUsuario = perguntaUsuario ?? string.Empty
            };
        }

        private static string MontarSecao(string titulo, IEnumerable<string> linhas, string? vazio = null)
        {
            var conteudo = linhas
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            if (conteudo.Count == 0 && !string.IsNullOrWhiteSpace(vazio))
            {
                conteudo.Add(vazio);
            }

            var builder = new StringBuilder();
            builder.AppendLine($"## {titulo}");

            foreach (var linha in conteudo)
            {
                builder.AppendLine(linha);
            }

            return builder.ToString().TrimEnd();
        }

        private static string FormatarInsight(InsightFinanceiro insight)
        {
            return $"- [{insight.Tipo}] {insight.Titulo} | DescriÃ§Ã£o: {insight.Descricao} | AÃ§Ã£o sugerida: {insight.AcaoSugerida}";
        }

        private static string FormatarPontoAtencao(PontoAtencaoSaudeFinanceira pontoAtencao)
        {
            return $"- {pontoAtencao.Nome} | Status: {FormatarStatus(pontoAtencao.Status)} | DescriÃ§Ã£o: {pontoAtencao.Descricao} | ObservaÃ§Ã£o: {pontoAtencao.Observacao}";
        }

        private static string FormatarIndicador(IndicadorFinanceiro indicador, CultureInfo cultura)
        {
            return $"- {indicador.Nome} | Atual: {FormatarValor(indicador.ValorAtual, indicador.Formato, cultura)} | Ideal: {FormatarValor(indicador.ValorIdeal, indicador.Formato, cultura)} | Percentual: {indicador.Percentual:N2}% | Status: {FormatarStatus(indicador.Status)} | DescriÃ§Ã£o: {indicador.Descricao} | ObservaÃ§Ã£o: {indicador.Observacao}";
        }

        private static string FormatarStatus(StatusIndicadorFinanceiro status)
        {
            return status switch
            {
                StatusIndicadorFinanceiro.Excelente => "Excelente",
                StatusIndicadorFinanceiro.Bom => "Bom",
                StatusIndicadorFinanceiro.Atencao => "AtenÃ§Ã£o",
                _ => "CrÃ­tico"
            };
        }

        private static string FormatarValor(decimal valor, FormatoValorIndicadorFinanceiro formato, CultureInfo cultura)
        {
            return formato switch
            {
                FormatoValorIndicadorFinanceiro.Percentual => $"{valor:N2}%",
                FormatoValorIndicadorFinanceiro.Meses => $"{valor:N2} mÃªs(es)",
                _ => valor.ToString("C", cultura)
            };
        }

        private static string FormatarMemoriaFinanceira(MemoriaFinanceiraResumidaIA memoria)
        {
            var partes = new List<string>
            {
                $"- Data: {memoria.DataGeracao:dd/MM/yyyy}",
                $"PerÃ­odo: {memoria.PeriodoReferencia:MM/yyyy}",
                $"PontuaÃ§Ã£o: {memoria.PontuacaoSaudeFinanceira}/100",
                $"ClassificaÃ§Ã£o: {memoria.ClassificacaoSaudeFinanceira}",
                $"Resumo: {memoria.ResumoExecutivoSistema}",
                $"Riscos: {FormatarListaResumo(memoria.PrincipaisRiscos)}",
                $"Pontos positivos: {FormatarListaResumo(memoria.PrincipaisPontosPositivos)}",
                $"RecomendaÃ§Ãµes: {FormatarListaResumo(memoria.PrincipaisRecomendacoes)}",
                $"Prioridades: {FormatarListaResumo(memoria.Prioridades)}"
            };

            return string.Join(" | ", partes);
        }

        private static string FormatarListaResumo(List<string> itens)
        {
            var filtrados = itens
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();

            return filtrados.Count == 0
                ? "nenhum registro"
                : string.Join("; ", filtrados);
        }
    }
}

