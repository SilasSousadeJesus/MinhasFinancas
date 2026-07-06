using System.Globalization;
using System.Text;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Construtores
{
    public class ConstrutorContextoIA
    {
        public ContextoAssistenteFinanceiro Construir(ResumoFinanceiroIA resumoFinanceiroIA, string? perguntaUsuario = null)
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

            var secoes = new List<string>
            {
                MontarSecao(
                    "Resumo Executivo",
                    [
                        $"Data de referência: {resumoFinanceiroIA.DataReferencia:dd/MM/yyyy}",
                        $"Resumo do sistema: {resumoFinanceiroIA.ResumoExecutivo}"
                    ]),
                MontarSecao(
                    "Saúde Financeira",
                    [
                        $"Pontuação geral: {resumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral}/100",
                        $"Classificação atual: {resumoFinanceiroIA.SaudeFinanceira.Classificacao}"
                    ]),
                MontarSecao(
                    "Pontos de Atenção Técnicos",
                    pontosAtencao,
                    "- Nenhum ponto de atenção técnico relevante foi identificado pelo sistema."),
                MontarSecao(
                    "Prioridades Imediatas",
                    prioridades.Select(item => $"- {item}"),
                    "- Nenhuma prioridade imediata foi registrada."),
                MontarSecao(
                    "Destaques Positivos",
                    destaques.Select(item => $"- {item}"),
                    "- Nenhum destaque positivo foi registrado."),
                MontarSecao(
                    "Indicadores em Atenção",
                    indicadoresEmAtencao,
                    "- Nenhum indicador está em faixa de atenção ou crítica."),
                MontarSecao(
                    "Indicadores Positivos",
                    indicadoresPositivos,
                    "- Nenhum indicador positivo foi destacado nesta leitura."),
                MontarSecao(
                    "Indicadores Financeiros Consolidados",
                    todosIndicadores,
                    "- Não há indicadores consolidados disponíveis."),
                MontarSecao(
                    "Insights Financeiros Prioritários",
                    insightsPrioritarios,
                    "- Nenhum insight prioritário está disponível."),
                MontarSecao(
                    "Insights Positivos",
                    insightsPositivos,
                    "- Nenhum insight positivo está disponível."),
                MontarSecao(
                    "Cobertura Atual do Contexto",
                    [
                        "- Perfil financeiro: já refletido de forma indireta nos indicadores, na saúde financeira e nas prioridades.",
                        "- Patrimônio: já refletido pelos indicadores de patrimônio líquido atual e percentual do patrimônio-alvo.",
                        "- Fluxo de caixa: já refletido pelos indicadores de economia mensal, percentual de economia e comprometimento da renda.",
                        "- Tendências, histórico detalhado, radar financeiro operacional, projeções, simulações e detalhamento patrimonial ainda não são enviados como blocos próprios nesta fase.",
                        "- Quando algum dado não estiver explicitamente descrito no contexto, a análise deve se limitar às informações estruturadas recebidas."
                    ])
            };

            if (!string.IsNullOrWhiteSpace(perguntaUsuario))
            {
                secoes.Add(MontarSecao("Pergunta do Usuário", [$"- {perguntaUsuario}"]));
            }

            return new ContextoAssistenteFinanceiro
            {
                DataReferencia = resumoFinanceiroIA.DataReferencia,
                PontuacaoSaudeFinanceira = resumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral,
                ClassificacaoSaudeFinanceira = resumoFinanceiroIA.SaudeFinanceira.Classificacao,
                PrioridadesImediatas = prioridades,
                DestaquesPositivos = destaques,
                InsightsPrioritarios = insightsPrioritarios,
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
            return $"- [{insight.Tipo}] {insight.Titulo} | Descrição: {insight.Descricao} | Ação sugerida: {insight.AcaoSugerida}";
        }

        private static string FormatarPontoAtencao(PontoAtencaoSaudeFinanceira pontoAtencao)
        {
            return $"- {pontoAtencao.Nome} | Status: {FormatarStatus(pontoAtencao.Status)} | Descrição: {pontoAtencao.Descricao} | Observação: {pontoAtencao.Observacao}";
        }

        private static string FormatarIndicador(IndicadorFinanceiro indicador, CultureInfo cultura)
        {
            return $"- {indicador.Nome} | Atual: {FormatarValor(indicador.ValorAtual, indicador.Formato, cultura)} | Ideal: {FormatarValor(indicador.ValorIdeal, indicador.Formato, cultura)} | Percentual: {indicador.Percentual:N2}% | Status: {FormatarStatus(indicador.Status)} | Descrição: {indicador.Descricao} | Observação: {indicador.Observacao}";
        }

        private static string FormatarStatus(StatusIndicadorFinanceiro status)
        {
            return status switch
            {
                StatusIndicadorFinanceiro.Excelente => "Excelente",
                StatusIndicadorFinanceiro.Bom => "Bom",
                StatusIndicadorFinanceiro.Atencao => "Atenção",
                _ => "Crítico"
            };
        }

        private static string FormatarValor(decimal valor, FormatoValorIndicadorFinanceiro formato, CultureInfo cultura)
        {
            return formato switch
            {
                FormatoValorIndicadorFinanceiro.Percentual => $"{valor:N2}%",
                FormatoValorIndicadorFinanceiro.Meses => $"{valor:N2} mês(es)",
                _ => valor.ToString("C", cultura)
            };
        }
    }
}
