using MinhasFinancas.Infra.IA.Especialistas.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas.Dividas
{
    public class EspecialistaDividas : EspecialistaFinanceiroBase
    {
        public override string Nome => "Especialista em Dívidas";

        public override ParecerEspecialistaIA Avaliar(ContextoAssistenteFinanceiro contexto)
        {
            var temPressao = ContemEmLista(
                contexto.ConsistenciaEstrategica.MotivosDesfavoraveis,
                "divida", "dívida", "juros", "cartao", "cartão", "financiamento", "parcelamento") ||
                ContemTexto(contexto.ResumoExecutivo, "divida", "dívida", "juros", "cartao", "cartão", "parcelamento") ||
                ContemEmLista(contexto.CompromissosFinanceiros, "divida", "dívida", "juros", "cartao", "cartão", "parcelamento");

            var situacao = temPressao
                ? "O contexto indica pressão relevante de dívidas e compromissos de crédito, com impacto direto na folga mensal."
                : "Não há sinal dominante de pressão por dívidas no contexto consolidado atual.";

            var conclusao = temPressao
                ? "A redução do custo financeiro das dívidas continua sendo um ponto crítico para preservar capacidade de crescimento."
                : "As dívidas não aparecem como a principal pressão do momento, mas continuam merecendo monitoramento.";

            var riscos = temPressao
                ? new[]
                {
                    "O custo financeiro pode crescer se compromissos caros continuarem ativos.",
                    "Parcelamentos e cartão de crédito podem reduzir a margem de manobra mensal."
                }
                : new[] { "Não há alerta forte de endividamento adicional no cenário atual." };

            var pontosPositivos = temPressao
                ? new[] { "Existe espaço para ganho rápido caso os compromissos mais caros sejam priorizados." }
                : new[] { "O contexto não mostra expansão agressiva de dívida no momento." };

            var recomendacoes = temPressao
                ? new[]
                {
                    "Priorizar a redução dos encargos mais caros.",
                    "Evitar novas parcelas até recuperar folga mensal."
                }
                : new[] { "Manter vigilância sobre novos compromissos de crédito." };

            var observacoes = new List<string>
            {
                $"Prioridade derivada do contexto: {(temPressao ? "Crítica" : "Estratégica")}"
            };

            return CriarParecer(Nome, situacao, conclusao, riscos, pontosPositivos, recomendacoes, temPressao ? "Crítica" : "Estratégica", observacoes);
        }
    }
}
