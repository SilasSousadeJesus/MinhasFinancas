using System.Globalization;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Interpretadores
{
    public class InterpretadorMemoriaFinanceira
    {
        private static readonly CultureInfo Cultura = new("pt-BR");

        public InterpretacaoMemoriaFinanceiraIA Interpretar(IEnumerable<MemoriaFinanceiraResumidaIA>? memorias)
        {
            var lista = memorias?
                .OrderBy(x => x.PeriodoReferencia)
                .ThenBy(x => x.DataGeracao)
                .ToList() ?? [];

            if (lista.Count == 0)
            {
                return new InterpretacaoMemoriaFinanceiraIA
                {
                    PossuiHistorico = false,
                    PossuiEvolucaoComparavel = false,
                    ResumoEvolucao = "Ainda nao existem analises anteriores suficientes para avaliar a evolucao financeira do usuario.",
                    Narrativas = ["- Esta sera a primeira leitura historica com continuidade analitica do sistema."],
                    MemoriaFinanceiraCompacta = ["- Nenhuma analise anterior registrada."]
                };
            }

            var interpretacao = new InterpretacaoMemoriaFinanceiraIA
            {
                PossuiHistorico = true,
                PossuiEvolucaoComparavel = lista.Count > 1,
                MemoriaFinanceiraCompacta = lista
                    .OrderByDescending(x => x.PeriodoReferencia)
                    .ThenByDescending(x => x.DataGeracao)
                    .Select(FormatarMemoriaCompacta)
                    .ToList()
            };

            if (lista.Count == 1)
            {
                var unica = lista[0];
                interpretacao.ResumoEvolucao =
                    $"Existe apenas uma analise anterior registrada, referente a {unica.PeriodoReferencia.ToString("MMMM 'de' yyyy", Cultura)}, com pontuacao {unica.PontuacaoSaudeFinanceira}/100 e classificacao {unica.ClassificacaoSaudeFinanceira}.";

                var narrativas = new List<string>
                {
                    "- Ainda nao ha base suficiente para falar em tendencia, melhora ou piora historica."
                };

                var prioridade = BuscarPrimeiroItem(unica.Prioridades);
                if (!string.IsNullOrWhiteSpace(prioridade))
                {
                    narrativas.Add($"- A principal prioridade registrada naquela leitura foi: {prioridade}.");
                }

                var risco = BuscarPrimeiroItem(unica.PrincipaisRiscos);
                if (!string.IsNullOrWhiteSpace(risco))
                {
                    narrativas.Add($"- O principal risco observado naquela analise foi: {risco}.");
                }

                interpretacao.Narrativas = narrativas;
                return interpretacao;
            }

            var primeira = lista.First();
            var ultima = lista.Last();
            var variacaoPontuacao = ultima.PontuacaoSaudeFinanceira - primeira.PontuacaoSaudeFinanceira;

            interpretacao.ResumoEvolucao = MontarResumoPontuacao(lista.Count, primeira, ultima, variacaoPontuacao);

            var narrativasEvolucao = new List<string>
            {
                $"- {MontarLeituraClassificacao(primeira, ultima)}"
            };

            var prioridadesRecorrentes = BuscarItensRecorrentes(lista.SelectMany(x => x.Prioridades));
            if (prioridadesRecorrentes.Count > 0)
            {
                var prioridadeAtual = prioridadesRecorrentes
                    .FirstOrDefault(item => ultima.Prioridades.Any(prioridade => Equivale(prioridade, item)));

                narrativasEvolucao.Add(prioridadeAtual is null
                    ? $"- As prioridades mais recorrentes nas ultimas analises foram: {FormatarLista(prioridadesRecorrentes)}."
                    : $"- {prioridadeAtual} permanece como prioridade recorrente e ainda nao foi concluida.");
            }

            var recomendacoesRecorrentes = BuscarItensRecorrentes(lista.SelectMany(x => x.PrincipaisRecomendacoes));
            if (recomendacoesRecorrentes.Count > 0)
            {
                narrativasEvolucao.Add($"- As recomendacoes mais repetidas ao longo do historico foram: {FormatarLista(recomendacoesRecorrentes)}.");
            }

            var riscosRecorrentes = BuscarItensRecorrentes(lista.SelectMany(x => x.PrincipaisRiscos));
            if (riscosRecorrentes.Count > 0)
            {
                narrativasEvolucao.Add($"- Os riscos recorrentes mais presentes no historico foram: {FormatarLista(riscosRecorrentes)}.");
            }

            var pontosPositivosRecorrentes = BuscarItensRecorrentes(lista.SelectMany(x => x.PrincipaisPontosPositivos));
            if (pontosPositivosRecorrentes.Count > 0)
            {
                narrativasEvolucao.Add($"- Os aspectos positivos mais consistentes ao longo das analises foram: {FormatarLista(pontosPositivosRecorrentes)}.");
            }

            narrativasEvolucao.Add($"- {MontarLeituraTendencia(variacaoPontuacao)}");

            interpretacao.Narrativas = narrativasEvolucao;
            return interpretacao;
        }

        private static string MontarResumoPontuacao(
            int quantidadeAnalises,
            MemoriaFinanceiraResumidaIA primeira,
            MemoriaFinanceiraResumidaIA ultima,
            int variacaoPontuacao)
        {
            var periodoInicial = primeira.PeriodoReferencia.ToString("MMMM 'de' yyyy", Cultura);
            var periodoFinal = ultima.PeriodoReferencia.ToString("MMMM 'de' yyyy", Cultura);

            if (variacaoPontuacao >= 4)
            {
                return $"A saude financeira evoluiu de {primeira.PontuacaoSaudeFinanceira} para {ultima.PontuacaoSaudeFinanceira} pontos nas ultimas {quantidadeAnalises} analises, saindo de {periodoInicial} para {periodoFinal} com melhora consistente.";
            }

            if (variacaoPontuacao <= -4)
            {
                return $"A saude financeira recuou de {primeira.PontuacaoSaudeFinanceira} para {ultima.PontuacaoSaudeFinanceira} pontos nas ultimas {quantidadeAnalises} analises, indicando perda de equilibrio entre {periodoInicial} e {periodoFinal}.";
            }

            return $"A saude financeira oscilou pouco nas ultimas {quantidadeAnalises} analises, permanecendo entre {primeira.PontuacaoSaudeFinanceira} e {ultima.PontuacaoSaudeFinanceira} pontos no periodo de {periodoInicial} a {periodoFinal}.";
        }

        private static string MontarLeituraClassificacao(
            MemoriaFinanceiraResumidaIA primeira,
            MemoriaFinanceiraResumidaIA ultima)
        {
            if (string.Equals(primeira.ClassificacaoSaudeFinanceira, ultima.ClassificacaoSaudeFinanceira, StringComparison.OrdinalIgnoreCase))
            {
                return $"A classificacao permaneceu em {ultima.ClassificacaoSaudeFinanceira}, o que sugere continuidade do mesmo nivel geral de saude financeira.";
            }

            return $"A classificacao saiu de {primeira.ClassificacaoSaudeFinanceira} para {ultima.ClassificacaoSaudeFinanceira}, mostrando mudanca perceptivel na leitura geral da situacao financeira.";
        }

        private static string MontarLeituraTendencia(int variacaoPontuacao)
        {
            if (variacaoPontuacao >= 4)
            {
                return "A tendencia historica recente e positiva, com sinais de evolucao acumulada entre as leituras.";
            }

            if (variacaoPontuacao <= -4)
            {
                return "A tendencia historica recente exige atencao, porque o desempenho piorou em vez de consolidar os avancos anteriores.";
            }

            return "A tendencia historica recente sugere estabilidade, sem melhora estrutural forte nem deterioracao relevante.";
        }

        private static List<string> BuscarItensRecorrentes(IEnumerable<string> itens)
        {
            return itens
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .GroupBy(NormalizarTexto)
                .Select(grupo => new
                {
                    Texto = grupo.First().Trim(),
                    Quantidade = grupo.Count()
                })
                .Where(item => item.Quantidade >= 2)
                .OrderByDescending(item => item.Quantidade)
                .ThenBy(item => item.Texto)
                .Take(3)
                .Select(item => item.Texto)
                .ToList();
        }

        private static string? BuscarPrimeiroItem(IEnumerable<string> itens)
        {
            return itens.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item))?.Trim();
        }

        private static string FormatarMemoriaCompacta(MemoriaFinanceiraResumidaIA memoria)
        {
            var prioridades = memoria.Prioridades
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Take(2)
                .ToList();

            var riscos = memoria.PrincipaisRiscos
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Take(2)
                .ToList();

            return $"- {memoria.PeriodoReferencia:MM/yyyy} | {memoria.PontuacaoSaudeFinanceira}/100 | {memoria.ClassificacaoSaudeFinanceira} | Prioridades: {FormatarLista(prioridades, "nenhuma prioridade")} | Riscos: {FormatarLista(riscos, "nenhum risco destacado")}";
        }

        private static string FormatarLista(IReadOnlyCollection<string> itens, string fallback = "nenhum registro")
        {
            return itens.Count == 0 ? fallback : string.Join("; ", itens);
        }

        private static string NormalizarTexto(string texto)
        {
            return texto.Trim().ToUpperInvariant();
        }

        private static bool Equivale(string origem, string comparacao)
        {
            return string.Equals(NormalizarTexto(origem), NormalizarTexto(comparacao), StringComparison.Ordinal);
        }
    }
}
