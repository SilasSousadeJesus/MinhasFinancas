using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira
{
    public class SaudeFinanceiraService : ISaudeFinanceiraService
    {
        private const int EscalaPilares = 100;
        private const int EscalaMfScore = 1000;

        private static readonly IReadOnlyDictionary<CodigoIndicadorFinanceiro, decimal> PesosIndicadores = new Dictionary<CodigoIndicadorFinanceiro, decimal>
        {
            { CodigoIndicadorFinanceiro.EconomiaMensal, 1.0m },
            { CodigoIndicadorFinanceiro.PercentualEconomia, 1.0m },
            { CodigoIndicadorFinanceiro.ReservaEmergenciaAtual, 1.5m },
            { CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal, 0.5m },
            { CodigoIndicadorFinanceiro.ComprometimentoRenda, 1.5m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo, 1.5m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias, 1.0m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias, 0.75m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias, 0.5m },
            { CodigoIndicadorFinanceiro.Endividamento, 1.5m },
            { CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual, 1.25m },
            { CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo, 0.75m },
        };

        public PainelSaudeFinanceira GerarPainel(
            PainelIndicadoresFinanceiros indicadores,
            ContextoComplementarMfScoreFinanceiro? contextoComplementar = null)
        {
            var lista = indicadores.Todos;
            var mfScore = CalcularMfScore(lista, contextoComplementar);

            return new PainelSaudeFinanceira
            {
                Resumo = new ResumoSaudeFinanceira
                {
                    PontuacaoGeral = mfScore.PontuacaoFinal,
                    Classificacao = mfScore.Classificacao,
                    MfScore = mfScore,
                    PontosAtencao = lista
                        .Where(indicador => indicador.Status == StatusIndicadorFinanceiro.Atencao || indicador.Status == StatusIndicadorFinanceiro.Critico)
                        .OrderByDescending(indicador => (int)indicador.Status)
                        .Take(3)
                        .Select(indicador => new PontoAtencaoSaudeFinanceira
                        {
                            Nome = indicador.Nome,
                            Status = indicador.Status,
                            Descricao = indicador.Descricao,
                            Observacao = indicador.Observacao
                        })
                        .ToList()
                },
                Indicadores = indicadores
            };
        }

        private static MfScoreFinanceiro CalcularMfScore(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            ContextoComplementarMfScoreFinanceiro? contextoComplementar)
        {
            var pilares = new List<PilarMfScoreFinanceiro>
            {
                CalcularPilarFluxoDeCaixa(indicadores),
                CalcularPilarLiquidezEReserva(indicadores),
                CalcularPilarEndividamentoEObrigacoes(indicadores),
                CalcularPilarPatrimonio(indicadores),
                CalcularPilarPlanejamentoEDisciplina(indicadores)
            };

            var somaPesos = pilares.Sum(item => item.Peso);
            var pontuacaoBaseNormalizada = somaPesos > 0
                ? (int)Math.Round(pilares.Sum(item => item.Nota * item.Peso) / somaPesos)
                : 0;

            var regrasCriticas = new List<string>();
            var indicadoresCriticosNormalizados = MontarIndicadoresCriticos(indicadores, contextoComplementar, regrasCriticas);
            var penalidadeTotalNormalizada = indicadoresCriticosNormalizados.Sum(item => item.Penalidade);
            var pontuacaoFinalNormalizada = Math.Clamp(
                (int)Math.Round(pontuacaoBaseNormalizada - penalidadeTotalNormalizada),
                0,
                EscalaPilares);

            var pontuacaoBase = ConverterEscalaMfScore(pontuacaoBaseNormalizada);
            var pontuacaoFinal = ConverterEscalaMfScore(pontuacaoFinalNormalizada);
            var indicadoresCriticos = indicadoresCriticosNormalizados
                .Select(item => new IndicadorCriticoMfScoreFinanceiro
                {
                    CodigoIndicador = item.CodigoIndicador,
                    Nome = item.Nome,
                    Motivo = item.Motivo,
                    Penalidade = ConverterEscalaMfScore(item.Penalidade),
                    PilarRelacionado = item.PilarRelacionado
                })
                .ToList();
            var penalidadeTotal = indicadoresCriticos.Sum(item => item.Penalidade);

            var classificacao = ObterClassificacao(pontuacaoFinal);
            var risco = ObterRisco(classificacao);
            var tendencia = ObterTendencia(indicadores, pontuacaoFinal, contextoComplementar);
            var resumoExecutivoDosPilares = pilares
                .Select(item => $"{item.Nome}: {InterpretarNotaPilar(item.Nota)}")
                .ToList();

            return new MfScoreFinanceiro
            {
                PontuacaoBase = pontuacaoBase,
                PontuacaoFinal = pontuacaoFinal,
                Classificacao = classificacao,
                Risco = risco,
                Tendencia = tendencia,
                Pilares = pilares,
                IndicadoresCriticos = indicadoresCriticos,
                ResumoExecutivoDosPilares = resumoExecutivoDosPilares,
                RegrasCriticasAplicadas = regrasCriticas,
                PenalidadeTotal = penalidadeTotal,
                Descricao = $"MF Score base {pontuacaoBase}/1000 com penalidade total de {penalidadeTotal:N0} ponto(s), resultando em {pontuacaoFinal}/1000."
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarFluxoDeCaixa(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            var relevantes = SelecionarIndicadores(indicadores,
                CodigoIndicadorFinanceiro.EconomiaMensal,
                CodigoIndicadorFinanceiro.PercentualEconomia,
                CodigoIndicadorFinanceiro.ComprometimentoRenda,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo);

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.FluxoDeCaixa,
                Nome = "Fluxo de Caixa",
                Peso = 30m,
                Nota = CalcularNotaMedia(relevantes),
                Descricao = "Capacidade operacional da vida financeira no curto prazo.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarLiquidezEReserva(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            var relevantes = SelecionarIndicadores(indicadores,
                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual,
                CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal);

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.LiquidezEReserva,
                Nome = "Liquidez e Reserva",
                Peso = 25m,
                Nota = CalcularNotaMedia(relevantes),
                Descricao = "Capacidade de suportar imprevistos e manter proteção financeira.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarEndividamentoEObrigacoes(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            var relevantes = SelecionarIndicadores(indicadores,
                CodigoIndicadorFinanceiro.Endividamento,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias);

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.EndividamentoEObrigacoes,
                Nome = "Endividamento e Obrigações",
                Peso = 20m,
                Nota = CalcularNotaMedia(relevantes),
                Descricao = "Pressão financeira estrutural e peso dos compromissos futuros.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarPatrimonio(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            var relevantes = SelecionarIndicadores(indicadores,
                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual,
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo);

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.Patrimonio,
                Nome = "Patrimônio",
                Peso = 15m,
                Nota = CalcularNotaMedia(relevantes),
                Descricao = "Evolução patrimonial e avanço em relação ao objetivo de longo prazo.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarPlanejamentoEDisciplina(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            var relevantes = SelecionarIndicadores(indicadores,
                CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal,
                CodigoIndicadorFinanceiro.ComprometimentoRenda,
                CodigoIndicadorFinanceiro.Endividamento,
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo);

            var notaBase = CalcularNotaMedia(relevantes);
            var configurados = relevantes.Count(item => item.ValorIdeal > 0);
            var bonusConfiguracao = configurados >= 4 ? 10 : configurados >= 3 ? 5 : 0;
            var notaFinal = Math.Clamp(notaBase + bonusConfiguracao, 0, EscalaPilares);

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.PlanejamentoEDisciplina,
                Nome = "Planejamento e Disciplina",
                Peso = 10m,
                Nota = notaFinal,
                Descricao = "Maturidade financeira, padrão de configuração e capacidade de manter a direção escolhida.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static List<IndicadorFinanceiro> SelecionarIndicadores(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            params CodigoIndicadorFinanceiro[] codigos)
        {
            return codigos
                .Select(codigo => indicadores.FirstOrDefault(item => item.Codigo == codigo))
                .Where(indicador => indicador is not null)
                .Cast<IndicadorFinanceiro>()
                .ToList();
        }

        private static int CalcularNotaMedia(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            if (indicadores.Count == 0)
            {
                return 0;
            }

            var somaPesos = indicadores.Sum(item => PesosIndicadores.TryGetValue(item.Codigo, out var peso) ? peso : 1m);
            if (somaPesos <= 0)
            {
                return 0;
            }

            var somaPonderada = indicadores.Sum(item =>
            {
                var peso = PesosIndicadores.TryGetValue(item.Codigo, out var valorPeso) ? valorPeso : 1m;
                return ObterPontuacao(item.Status) * peso;
            });

            return (int)Math.Round(somaPonderada / somaPesos);
        }

        private static List<IndicadorCriticoMfScoreFinanceiro> MontarIndicadoresCriticos(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            ContextoComplementarMfScoreFinanceiro? contextoComplementar,
            ICollection<string> regrasCriticasAplicadas)
        {
            var criticos = new List<IndicadorCriticoMfScoreFinanceiro>();

            void Adicionar(
                CodigoIndicadorFinanceiro codigo,
                string nome,
                string motivo,
                decimal penalidade,
                string pilar)
            {
                criticos.Add(new IndicadorCriticoMfScoreFinanceiro
                {
                    CodigoIndicador = codigo,
                    Nome = nome,
                    Motivo = motivo,
                    Penalidade = penalidade,
                    PilarRelacionado = pilar
                });

                regrasCriticasAplicadas.Add($"{nome}: {motivo}");
            }

            var patrimonioLiquido = Buscar(indicadores, CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual);
            if (patrimonioLiquido is not null && patrimonioLiquido.ValorAtual < 0m)
            {
                Adicionar(
                    patrimonioLiquido.Codigo,
                    patrimonioLiquido.Nome,
                    "Patrimônio líquido negativo.",
                    10m,
                    "Patrimônio");
            }

            var economiaMensal = Buscar(indicadores, CodigoIndicadorFinanceiro.EconomiaMensal);
            if (contextoComplementar?.PossuiFluxoMensalNegativoAtual == true)
            {
                Adicionar(
                    economiaMensal?.Codigo ?? CodigoIndicadorFinanceiro.EconomiaMensal,
                    economiaMensal?.Nome ?? "Fluxo mensal",
                    "O mês de referência fechou com fluxo de caixa negativo.",
                    8m,
                    "Fluxo de Caixa");
            }

            if ((contextoComplementar?.MesesConsecutivosFluxoNegativo ?? 0) >= 3)
            {
                Adicionar(
                    economiaMensal?.Codigo ?? CodigoIndicadorFinanceiro.EconomiaMensal,
                    "Persistência de fluxo negativo",
                    "O usuário acumula três ou mais meses consecutivos no vermelho.",
                    12m,
                    "Fluxo de Caixa");
            }
            else if ((contextoComplementar?.MesesConsecutivosFluxoNegativo ?? 0) >= 2)
            {
                Adicionar(
                    economiaMensal?.Codigo ?? CodigoIndicadorFinanceiro.EconomiaMensal,
                    "Persistência de fluxo negativo",
                    "O usuário acumula dois meses consecutivos no vermelho.",
                    6m,
                    "Fluxo de Caixa");
            }

            var endividamento = Buscar(indicadores, CodigoIndicadorFinanceiro.Endividamento);
            if (contextoComplementar?.PossuiInadimplencia == true)
            {
                Adicionar(
                    endividamento?.Codigo ?? CodigoIndicadorFinanceiro.Endividamento,
                    "Inadimplência",
                    "Existem despesas vencidas ainda pendentes de pagamento.",
                    15m,
                    "Endividamento e Obrigações");
            }

            var reservaAtual = Buscar(indicadores, CodigoIndicadorFinanceiro.ReservaEmergenciaAtual);
            if (contextoComplementar?.PossuiDadosEssenciaisInsuficientes == true)
            {
                Adicionar(
                    reservaAtual?.Codigo ?? CodigoIndicadorFinanceiro.ReservaEmergenciaAtual,
                    "Dados essenciais insuficientes",
                    "Ainda faltam dados básicos para avaliar o risco financeiro com alta confiança.",
                    3m,
                    "Planejamento e Disciplina");
            }

            return criticos;
        }

        private static IndicadorFinanceiro? Buscar(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            CodigoIndicadorFinanceiro codigo)
        {
            return indicadores.FirstOrDefault(item => item.Codigo == codigo);
        }

        private static TendenciaMfScoreFinanceiro ObterTendencia(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            int pontuacaoFinal,
            ContextoComplementarMfScoreFinanceiro? contextoComplementar)
        {
            var historico = (contextoComplementar?.HistoricoPontuacoesFinais ?? [])
                .Where(nota => nota >= 0)
                .ToList();

            if (historico.Count > 0)
            {
                var ultimoHistorico = historico[^1];
                var diferenca = pontuacaoFinal - ultimoHistorico;
                var direcao = diferenca switch
                {
                    >= 40 => DirecaoTendenciaMfScoreFinanceiro.Positiva,
                    <= -40 => DirecaoTendenciaMfScoreFinanceiro.Negativa,
                    _ => DirecaoTendenciaMfScoreFinanceiro.Neutra
                };

                var descricao = direcao switch
                {
                    DirecaoTendenciaMfScoreFinanceiro.Positiva => "O MF Score melhorou em relação às competências anteriores.",
                    DirecaoTendenciaMfScoreFinanceiro.Negativa => "O MF Score piorou em relação às competências anteriores.",
                    _ => "O MF Score está estável em relação às competências anteriores."
                };

                return new TendenciaMfScoreFinanceiro
                {
                    Direcao = direcao,
                    Descricao = descricao,
                    HistoricoNotas = historico
                };
            }

            var positivos = indicadores.Count(indicador => indicador.Status is StatusIndicadorFinanceiro.Excelente or StatusIndicadorFinanceiro.Bom);
            var negativos = indicadores.Count(indicador => indicador.Status is StatusIndicadorFinanceiro.Atencao or StatusIndicadorFinanceiro.Critico);

            return new TendenciaMfScoreFinanceiro
            {
                Direcao = positivos > negativos
                    ? DirecaoTendenciaMfScoreFinanceiro.Positiva
                    : negativos > positivos
                        ? DirecaoTendenciaMfScoreFinanceiro.Negativa
                        : DirecaoTendenciaMfScoreFinanceiro.Neutra,
                Descricao = pontuacaoFinal >= 700
                    ? "Tendência geral favorável com espaço para fortalecimento."
                    : pontuacaoFinal >= 500
                        ? "Tendência estável, mas ainda sensível a ajustes estruturais."
                        : "Tendência de risco que pede reorganização imediata.",
                HistoricoNotas = historico
            };
        }

        private static string InterpretarNotaPilar(int nota)
        {
            if (nota >= 90)
            {
                return "muito forte";
            }

            if (nota >= 80)
            {
                return "forte";
            }

            if (nota >= 70)
            {
                return "sólido";
            }

            if (nota >= 60)
            {
                return "sob observação";
            }

            if (nota >= 40)
            {
                return "frágil";
            }

            return "crítico";
        }

        private static decimal ObterPontuacao(StatusIndicadorFinanceiro status)
        {
            return status switch
            {
                StatusIndicadorFinanceiro.Excelente => 100m,
                StatusIndicadorFinanceiro.Bom => 80m,
                StatusIndicadorFinanceiro.Atencao => 55m,
                _ => 25m,
            };
        }

        private static string ObterClassificacao(int pontuacao)
        {
            if (pontuacao >= 900)
            {
                return "Excelente";
            }

            if (pontuacao >= 800)
            {
                return "Muito Bom";
            }

            if (pontuacao >= 700)
            {
                return "Bom";
            }

            if (pontuacao >= 600)
            {
                return "Atenção";
            }

            if (pontuacao >= 400)
            {
                return "Crítico";
            }

            return "Muito Crítico";
        }

        private static string ObterRisco(string classificacao)
        {
            return classificacao switch
            {
                "Excelente" => "Risco Muito Baixo",
                "Muito Bom" => "Risco Baixo",
                "Bom" => "Risco Moderado",
                "Atenção" => "Risco Moderado-Alto",
                "Crítico" => "Risco Alto",
                _ => "Risco Muito Alto"
            };
        }

        private static int ConverterEscalaMfScore(decimal notaNormalizada)
        {
            var fator = EscalaMfScore / (decimal)EscalaPilares;
            return (int)Math.Round(notaNormalizada * fator, MidpointRounding.AwayFromZero);
        }
    }
}
