using System.Globalization;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public class ResumoFinanceiroIAService : IResumoFinanceiroIAService
    {
        private static readonly CodigoIndicadorFinanceiro[] OrdemPrioridadeAtencao =
        [
            CodigoIndicadorFinanceiro.ReservaEmergenciaAtual,
            CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva,
            CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo,
            CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias,
            CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias,
            CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias,
            CodigoIndicadorFinanceiro.Endividamento,
            CodigoIndicadorFinanceiro.ComprometimentoRenda,
            CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo,
            CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual,
            CodigoIndicadorFinanceiro.PercentualEconomia,
            CodigoIndicadorFinanceiro.EconomiaMensal
        ];

        private static readonly CodigoIndicadorFinanceiro[] OrdemPrioridadeForca =
        [
            CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual,
            CodigoIndicadorFinanceiro.PercentualEconomia,
            CodigoIndicadorFinanceiro.EconomiaMensal,
            CodigoIndicadorFinanceiro.ReservaEmergenciaAtual,
            CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva,
            CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo,
            CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias,
            CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias,
            CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias,
            CodigoIndicadorFinanceiro.Endividamento,
            CodigoIndicadorFinanceiro.ComprometimentoRenda,
            CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo
        ];

        public ResumoFinanceiroIA GerarResumo(
            DateTime dataReferencia,
            PainelSaudeFinanceira painelSaudeFinanceira,
            PainelInsightsFinanceiros painelInsightsFinanceiros)
        {
            var prioridades = MontarPrioridadesImediatas(painelSaudeFinanceira);
            var destaques = MontarDestaquesPositivos(painelSaudeFinanceira, painelInsightsFinanceiros);

            return new ResumoFinanceiroIA
            {
                DataReferencia = dataReferencia,
                SaudeFinanceira = painelSaudeFinanceira.Resumo,
                Indicadores = painelSaudeFinanceira.Indicadores,
                Insights = painelInsightsFinanceiros,
                ResumoExecutivo = MontarResumoExecutivo(dataReferencia, painelSaudeFinanceira),
                PrioridadesImediatas = prioridades,
                DestaquesPositivos = destaques
            };
        }

        private static List<string> MontarPrioridadesImediatas(PainelSaudeFinanceira painelSaudeFinanceira)
        {
            var indicadores = painelSaudeFinanceira.Indicadores.Todos;
            var prioridades = new List<string>();

            AdicionarPrioridadeConfiguracao(indicadores, prioridades);

            foreach (var codigo in OrdemPrioridadeAtencao)
            {
                var indicador = indicadores.FirstOrDefault(item =>
                    item.Codigo == codigo &&
                    (item.Status == StatusIndicadorFinanceiro.Atencao || item.Status == StatusIndicadorFinanceiro.Critico));

                if (indicador is null)
                {
                    continue;
                }

                if (indicador.ValorIdeal <= 0 && IndicadorDependeDeConfiguracao(indicador.Codigo))
                {
                    continue;
                }

                var prioridade = ObterTextoPrioridade(indicador);

                if (!string.IsNullOrWhiteSpace(prioridade) && !prioridades.Contains(prioridade))
                {
                    prioridades.Add(prioridade);
                }
            }

            if (prioridades.Count == 0)
            {
                prioridades.Add("Manter a disciplina financeira atual.");
            }

            return prioridades.Take(3).ToList();
        }

        private static bool IndicadorDependeDeConfiguracao(CodigoIndicadorFinanceiro codigo)
        {
            return codigo is CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal
                or CodigoIndicadorFinanceiro.ComprometimentoRenda
                or CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo
                or CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias
                or CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias
                or CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias
                or CodigoIndicadorFinanceiro.Endividamento
                or CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo;
        }

        private static void AdicionarPrioridadeConfiguracao(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            ICollection<string> prioridades)
        {
            var reservaIdeal = indicadores.FirstOrDefault(item => item.Codigo == CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal);

            if (reservaIdeal is not null && reservaIdeal.ValorIdeal <= 0)
            {
                prioridades.Add("Configurar a meta de reserva de emergência.");
            }

            var comprometimentoRenda = indicadores.FirstOrDefault(item => item.Codigo == CodigoIndicadorFinanceiro.ComprometimentoRenda);

            if (comprometimentoRenda is not null && comprometimentoRenda.ValorIdeal <= 0)
            {
                prioridades.Add("Definir limite de comprometimento da renda.");
            }

            var endividamento = indicadores.FirstOrDefault(item => item.Codigo == CodigoIndicadorFinanceiro.Endividamento);

            if (endividamento is not null && endividamento.ValorIdeal <= 0)
            {
                prioridades.Add("Definir limite de endividamento.");
            }

            var patrimonioAlvo = indicadores.FirstOrDefault(item => item.Codigo == CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo);

            if (patrimonioAlvo is not null && patrimonioAlvo.ValorIdeal <= 0)
            {
                prioridades.Add("Definir o patrimônio líquido alvo.");
            }
        }

        private static List<string> MontarDestaquesPositivos(
            PainelSaudeFinanceira painelSaudeFinanceira,
            PainelInsightsFinanceiros painelInsightsFinanceiros)
        {
            var destaques = new List<string>();

            foreach (var codigo in OrdemPrioridadeForca)
            {
                var indicador = painelSaudeFinanceira.Indicadores.Todos.FirstOrDefault(item =>
                    item.Codigo == codigo &&
                    (item.Status == StatusIndicadorFinanceiro.Excelente || item.Status == StatusIndicadorFinanceiro.Bom));

                if (indicador is null)
                {
                    continue;
                }

                var destaque = ObterTextoDestaque(indicador);

                if (!string.IsNullOrWhiteSpace(destaque) && !destaques.Contains(destaque))
                {
                    destaques.Add(destaque);
                }
            }

            if (destaques.Count == 0)
            {
                destaques.AddRange(painelInsightsFinanceiros.DestaquesPositivos
                    .Select(insight => insight.Titulo)
                    .Where(titulo => !string.IsNullOrWhiteSpace(titulo))
                    .Distinct()
                    .Take(2));
            }

            return destaques.Take(2).ToList();
        }

        private static string MontarResumoExecutivo(
            DateTime dataReferencia,
            PainelSaudeFinanceira painelSaudeFinanceira)
        {
            var mesReferencia = dataReferencia.ToString("MMMM 'de' yyyy", new CultureInfo("pt-BR"));
            var indicadores = painelSaudeFinanceira.Indicadores.Todos;
            var pontoForte = SelecionarIndicador(indicadores, OrdemPrioridadeForca, StatusIndicadorFinanceiro.Excelente, StatusIndicadorFinanceiro.Bom);
            var pontoAtencao = SelecionarIndicador(indicadores, OrdemPrioridadeAtencao, StatusIndicadorFinanceiro.Critico, StatusIndicadorFinanceiro.Atencao);

            var abertura = ObterAberturaResumo(painelSaudeFinanceira.Resumo.Classificacao);
            var fraseForte = ObterFraseResumoPontoForte(pontoForte);
            var fraseAtencao = ObterFraseResumoPontoAtencao(pontoAtencao);

            if (!string.IsNullOrWhiteSpace(fraseForte) && !string.IsNullOrWhiteSpace(fraseAtencao))
            {
                return $"{abertura} Em {mesReferencia}, {fraseForte} Ao mesmo tempo, {fraseAtencao}";
            }

            if (!string.IsNullOrWhiteSpace(fraseAtencao))
            {
                return $"{abertura} Em {mesReferencia}, {fraseAtencao}";
            }

            if (!string.IsNullOrWhiteSpace(fraseForte))
            {
                return $"{abertura} Em {mesReferencia}, {fraseForte}";
            }

            return $"{abertura} Em {mesReferencia}, os principais indicadores seguem sem desvios relevantes.";
        }

        private static IndicadorFinanceiro? SelecionarIndicador(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            IEnumerable<CodigoIndicadorFinanceiro> ordem,
            params StatusIndicadorFinanceiro[] statusPermitidos)
        {
            foreach (var codigo in ordem)
            {
                var indicador = indicadores.FirstOrDefault(item =>
                    item.Codigo == codigo &&
                    statusPermitidos.Contains(item.Status));

                if (indicador is not null)
                {
                    if (indicador.ValorIdeal <= 0 && IndicadorDependeDeConfiguracao(indicador.Codigo))
                    {
                        continue;
                    }

                    return indicador;
                }
            }

            return indicadores.FirstOrDefault(item =>
                statusPermitidos.Contains(item.Status) &&
                !(item.ValorIdeal <= 0 && IndicadorDependeDeConfiguracao(item.Codigo)));
        }

        private static string ObterAberturaResumo(string classificacao)
        {
            return classificacao switch
            {
                "Excelente" => "Sua situação financeira apresenta um cenário bastante sólido neste momento.",
                "Muito Bom" => "Sua situação financeira apresenta uma base muito consistente neste momento.",
                "Boa" => "Sua situação financeira mostra sinais consistentes de equilíbrio.",
                "Crítica" => "O momento financeiro pede cautela e reorganização das prioridades.",
                "Muito Crítico" => "O momento financeiro exige correção imediata e reorganização profunda das prioridades.",
                _ => "Sua situação financeira exige alguns ajustes importantes neste momento."
            };
        }

        private static string ObterFraseResumoPontoForte(IndicadorFinanceiro? indicador)
        {
            if (indicador is null)
            {
                return string.Empty;
            }

            return indicador.Codigo switch
            {
                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual
                    => "a construção patrimonial já demonstra consistência e favorece uma base mais sólida de longo prazo.",
                CodigoIndicadorFinanceiro.PercentualEconomia
                    => "a disciplina de economia vem ajudando a transformar renda em avanço financeiro concreto.",
                CodigoIndicadorFinanceiro.EconomiaMensal
                    => "a geração de sobra mensal contribui para sustentar o planejamento com mais previsibilidade.",
                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual or CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal
                    => "a proteção de liquidez já oferece um colchão mais seguro para lidar com imprevistos.",
                CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva
                    => "a velocidade atual de formação da reserva indica boa capacidade de recompor proteção mesmo em fase inicial de patrimônio.",
                CodigoIndicadorFinanceiro.Endividamento
                    => "a exposição atual a dívidas e passivos permanece controlada e preserva margem para decisões futuras.",
                CodigoIndicadorFinanceiro.ComprometimentoRenda
                    => "o orçamento ainda mantém boa capacidade para absorver compromissos sem pressionar excessivamente a renda.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo
                    => "os compromissos dos próximos 30 dias ainda cabem com folga na estrutura de renda atual.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias
                    => "a pressão financeira acumulada dos próximos 90 dias segue administrável, mas já pede acompanhamento atento.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias
                    => "a pressão acumulada dos próximos 180 dias ainda parece sustentável, embora mereça planejamento para evitar acúmulo de peso.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias
                    => "a pressão de longo prazo continua compatível com a renda projetada, mas exige disciplina para não perder fôlego.",
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo
                    => "o patrimônio avança de forma compatível com o objetivo traçado para o longo prazo.",
                _ => string.Empty
            };
        }

        private static string ObterFraseResumoPontoAtencao(IndicadorFinanceiro? indicador)
        {
            if (indicador is null)
            {
                return string.Empty;
            }

            return indicador.Codigo switch
            {
                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual or CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal
                    => "a reserva de emergência ainda não oferece cobertura suficiente para atravessar imprevistos com tranquilidade.",
                CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva
                    => "a reserva ainda demoraria além do ideal para ser concluída no ritmo atual, o que prolonga a vulnerabilidade a imprevistos.",
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo
                    => "o patrimônio permanece distante do objetivo definido e ainda exige constância nos aportes.",
                CodigoIndicadorFinanceiro.Endividamento
                    => "a exposição atual a dívidas e passivos ainda reduz a margem para crescimento e limita decisões futuras.",
                CodigoIndicadorFinanceiro.ComprometimentoRenda
                    => "uma parcela elevada da renda continua comprometida, o que reduz a flexibilidade do mês.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo
                    => "os compromissos dos próximos 30 dias já começam a limitar a folga do caixa futuro.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias
                    => "a pressão financeira acumulada dos próximos 90 dias começa a reduzir a folga disponível para reagir a imprevistos.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias
                    => "a pressão financeira acumulada dos próximos 180 dias já merece atenção para não comprometer o planejamento.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias
                    => "a pressão financeira acumulada de longo prazo ainda pede disciplina para não transformar previsibilidade em aperto futuro.",
                CodigoIndicadorFinanceiro.PercentualEconomia
                    => "a taxa de economia ainda está abaixo do ritmo necessário para acelerar sua evolução financeira.",
                CodigoIndicadorFinanceiro.EconomiaMensal
                    => "a sobra mensal segue frágil e pressiona a estabilidade do curto prazo.",
                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual
                    => "a base patrimonial ainda está em fase inicial de consolidação.",
                _ => string.Empty
            };
        }

        private static string ObterTextoPrioridade(IndicadorFinanceiro indicador)
        {
            return indicador.Codigo switch
            {
                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual or CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal
                    => "Fortalecer a reserva de emergência.",
                CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva
                    => "Acelerar a formação da reserva de emergência.",
                CodigoIndicadorFinanceiro.Endividamento
                    => "Reduzir a exposição a dívidas e passivos.",
                CodigoIndicadorFinanceiro.ComprometimentoRenda
                    => "Reduzir o comprometimento da renda.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo
                    => "Rever os compromissos dos próximos 30 dias.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias
                    => "Organizar a pressão financeira acumulada dos próximos 90 dias.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias
                    => "Planejar a pressão financeira acumulada dos próximos 180 dias.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias
                    => "Revisar a pressão financeira acumulada de longo prazo.",
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo
                    => "Aproximar o patrimônio do objetivo definido.",
                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual
                    => "Reverter o patrimônio líquido negativo.",
                CodigoIndicadorFinanceiro.PercentualEconomia
                    => "Ampliar a capacidade de poupança.",
                CodigoIndicadorFinanceiro.EconomiaMensal
                    => "Recuperar a sobra mensal.",
                _ => string.Empty
            };
        }

        private static string ObterTextoDestaque(IndicadorFinanceiro indicador)
        {
            return indicador.Codigo switch
            {
                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual
                    => "Crescimento consistente do patrimônio.",
                CodigoIndicadorFinanceiro.PercentualEconomia
                    => "Boa disciplina de economia.",
                CodigoIndicadorFinanceiro.EconomiaMensal
                    => "Sobra mensal saudável.",
                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual or CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal
                    => "Proteção financeira de curto prazo mais robusta.",
                CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva
                    => "Boa velocidade de formação da reserva.",
                CodigoIndicadorFinanceiro.Endividamento
                    => "Endividamento patrimonial sob controle.",
                CodigoIndicadorFinanceiro.ComprometimentoRenda
                    => "Boa folga no orçamento mensal.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo
                    => "Boa previsibilidade dos compromissos futuros.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias
                    => "Boa previsibilidade da pressão financeira de 90 dias.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias
                    => "Boa leitura da pressão financeira acumulada de médio prazo.",
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias
                    => "Boa previsibilidade da pressão financeira acumulada de longo prazo.",
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo
                    => "Evolução consistente rumo ao patrimônio-alvo.",
                _ => indicador.Nome
            };
        }
    }
}

