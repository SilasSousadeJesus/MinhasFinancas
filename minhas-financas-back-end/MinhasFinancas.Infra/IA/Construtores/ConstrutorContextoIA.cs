using System.Globalization;
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

            var indicadores = resumoFinanceiroIA.Indicadores.Todos
                .Select(indicador =>
                    $"- {indicador.Nome}: atual {FormatarValor(indicador.ValorAtual, indicador.Formato, cultura)}, ideal {FormatarValor(indicador.ValorIdeal, indicador.Formato, cultura)}, status {FormatarStatus(indicador.Status)}, observação {indicador.Observacao}")
                .ToList();

            var linhas = new List<string>
            {
                $"Data de referência: {resumoFinanceiroIA.DataReferencia:dd/MM/yyyy}",
                $"Pontuação de saúde financeira: {resumoFinanceiroIA.SaudeFinanceira.PontuacaoGeral}/100",
                $"Classificação de saúde financeira: {resumoFinanceiroIA.SaudeFinanceira.Classificacao}",
                $"Resumo executivo do sistema: {resumoFinanceiroIA.ResumoExecutivo}",
                $"Prioridades imediatas: {(prioridades.Count > 0 ? string.Join(" | ", prioridades) : "Nenhuma prioridade imediata registrada.")}",
                $"Destaques positivos: {(destaques.Count > 0 ? string.Join(" | ", destaques) : "Nenhum destaque positivo registrado.")}",
                "Indicadores financeiros:",
                string.Join(Environment.NewLine, indicadores),
                "Insights prioritários:",
                insightsPrioritarios.Count > 0
                    ? string.Join(Environment.NewLine, insightsPrioritarios)
                    : "- Nenhum insight prioritário disponível."
            };

            if (!string.IsNullOrWhiteSpace(perguntaUsuario))
            {
                linhas.Add($"Pergunta do usuário: {perguntaUsuario}");
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
                ContextoTextual = string.Join(Environment.NewLine, linhas),
                PerguntaUsuario = perguntaUsuario ?? string.Empty
            };
        }

        private static string FormatarInsight(InsightFinanceiro insight)
        {
            return $"- [{insight.Tipo}] {insight.Titulo} | Descrição: {insight.Descricao} | Ação sugerida: {insight.AcaoSugerida}";
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
