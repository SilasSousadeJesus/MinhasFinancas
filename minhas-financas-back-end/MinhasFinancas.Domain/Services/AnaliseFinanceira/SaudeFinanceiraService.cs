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
            { CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva, 1.0m },
            { CodigoIndicadorFinanceiro.ComprometimentoRenda, 1.5m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo, 1.5m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias, 0.75m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias, 0.5m },
            { CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias, 0.25m },
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
                CalcularPilarPlanejamentoEDisciplina(indicadores, contextoComplementar)
            };

            var somaPesos = pilares.Sum(item => item.Peso);
            var pontuacaoBaseNormalizada = somaPesos > 0m
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
            var economiaMensal = Buscar(indicadores, CodigoIndicadorFinanceiro.EconomiaMensal);
            var percentualEconomia = Buscar(indicadores, CodigoIndicadorFinanceiro.PercentualEconomia);
            var comprometimentoRenda = Buscar(indicadores, CodigoIndicadorFinanceiro.ComprometimentoRenda);
            var relevantes = SelecionarIndicadores(indicadores,
                CodigoIndicadorFinanceiro.EconomiaMensal,
                CodigoIndicadorFinanceiro.PercentualEconomia,
                CodigoIndicadorFinanceiro.ComprometimentoRenda);

            var nota = CalcularNotaMediaPonderada([
                (economiaMensal, 0.45m),
                (percentualEconomia, 0.20m),
                (comprometimentoRenda, 0.35m)
            ]);

            if (economiaMensal?.Status == StatusIndicadorFinanceiro.Critico)
            {
                nota = Math.Min(nota, 35);
            }
            else if (economiaMensal?.Status == StatusIndicadorFinanceiro.Atencao &&
                     comprometimentoRenda?.Status == StatusIndicadorFinanceiro.Critico)
            {
                nota = Math.Min(nota, 55);
            }
            else if (economiaMensal?.Status == StatusIndicadorFinanceiro.Excelente &&
                     comprometimentoRenda?.Status is StatusIndicadorFinanceiro.Excelente or StatusIndicadorFinanceiro.Bom)
            {
                nota = Math.Max(nota, 85);
            }

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.FluxoDeCaixa,
                Nome = "Fluxo de Caixa",
                Peso = 30m,
                Nota = nota,
                Descricao = "Mede principalmente a capacidade operacional do mês: se a renda fecha o ciclo com folga, aperto ou déficit, sem duplicar em excesso a leitura dos mesmos sinais.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarLiquidezEReserva(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            var relevantes = SelecionarIndicadores(indicadores,
                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual,
                CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal,
                CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva);
            var reservaAtual = Buscar(indicadores, CodigoIndicadorFinanceiro.ReservaEmergenciaAtual);
            var capacidadeFormacaoReserva = Buscar(indicadores, CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva);
            var notaLiquidez = CalcularNotaMediaLiquidez(reservaAtual, capacidadeFormacaoReserva);

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.LiquidezEReserva,
                Nome = "Liquidez e Reserva",
                Peso = 25m,
                Nota = notaLiquidez,
                Descricao = "Capacidade de suportar imprevistos e manter proteção financeira. A reserva atual continua sendo a base principal, enquanto a velocidade de formação evita falsos positivos em perfis iniciantes com boa folga mensal.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarEndividamentoEObrigacoes(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            var endividamento = Buscar(indicadores, CodigoIndicadorFinanceiro.Endividamento);
            var futuro30 = Buscar(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo);
            var futuro90 = Buscar(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias);
            var futuro180 = Buscar(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias);
            var futuro365 = Buscar(indicadores, CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias);
            var relevantes = SelecionarIndicadores(indicadores,
                CodigoIndicadorFinanceiro.Endividamento,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias,
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias);

            var nota = CalcularNotaMediaPonderada([
                (endividamento, 0.40m),
                (futuro30, 0.30m),
                (futuro90, 0.15m),
                (futuro180, 0.10m),
                (futuro365, 0.05m)
            ]);

            if (endividamento?.Status == StatusIndicadorFinanceiro.Critico &&
                futuro30?.Status == StatusIndicadorFinanceiro.Critico)
            {
                nota = Math.Min(nota, 40);
            }

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.EndividamentoEObrigacoes,
                Nome = "Endividamento e Obrigações",
                Peso = 20m,
                Nota = nota,
                Descricao = "Separa dívida de consumo, financiamento patrimonial, obrigações futuras recorrentes e inadimplência. Financiamento patrimonial continua relevante, mas recebe tratamento diferente do endividamento de consumo.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarPatrimonio(IReadOnlyCollection<IndicadorFinanceiro> indicadores)
        {
            var patrimonioLiquido = Buscar(indicadores, CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual);
            var percentualPatrimonioAlvo = Buscar(indicadores, CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo);
            var relevantes = SelecionarIndicadores(
                indicadores,
                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual,
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo);

            var notaPatrimonio = CalcularNotaMediaPonderada([
                (patrimonioLiquido, 0.85m),
                (percentualPatrimonioAlvo, 0.15m)
            ]);

            if (patrimonioLiquido?.ValorAtual == 0m && percentualPatrimonioAlvo?.ValorAtual == 0m)
            {
                notaPatrimonio = Math.Max(notaPatrimonio, 60);
            }

            if ((patrimonioLiquido?.ValorAtual ?? 0m) > 0m &&
                patrimonioLiquido?.Status is StatusIndicadorFinanceiro.Bom or StatusIndicadorFinanceiro.Excelente)
            {
                notaPatrimonio = Math.Max(notaPatrimonio, 70);
            }

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.Patrimonio,
                Nome = "Patrimônio",
                Peso = 15m,
                Nota = notaPatrimonio,
                Descricao = "Prioriza a situação patrimonial real do usuário. A meta de patrimônio entra como sinal de evolução, mas não deve rebaixar excessivamente quem já possui patrimônio líquido positivo relevante.",
                Indicadores = relevantes.Select(item => item.Nome).ToList()
            };
        }

        private static PilarMfScoreFinanceiro CalcularPilarPlanejamentoEDisciplina(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            ContextoComplementarMfScoreFinanceiro? contextoComplementar)
        {
            var notaConfiguracao = contextoComplementar?.NotaConfiguracaoPlanejamento ?? 10;
            var quantidadeParametrosConfigurados = contextoComplementar?.QuantidadeParametrosPlanejamentoConfigurados ?? 0;
            var totalParametrosEsperados = contextoComplementar?.TotalParametrosPlanejamentoEsperados ?? 5;
            var percentualEconomia = Buscar(indicadores, CodigoIndicadorFinanceiro.PercentualEconomia);
            var capacidadeFormacaoReserva = Buscar(indicadores, CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva);

            var notaConsistencia = CalcularNotaMediaPonderada([
                (percentualEconomia, 0.55m),
                (capacidadeFormacaoReserva, 0.45m)
            ]);

            var notaHigieneFinanceira = 100m;
            if (contextoComplementar?.PossuiFluxoMensalNegativoAtual == true)
            {
                notaHigieneFinanceira -= 20m;
            }

            if ((contextoComplementar?.MesesConsecutivosFluxoNegativo ?? 0) >= 2)
            {
                notaHigieneFinanceira -= 15m;
            }

            if (contextoComplementar?.PossuiInadimplencia == true)
            {
                notaHigieneFinanceira -= 35m;
            }
            else if (contextoComplementar?.PossuiCuraRecenteInadimplencia == true)
            {
                notaHigieneFinanceira -= 10m;
            }

            notaHigieneFinanceira = Math.Clamp(notaHigieneFinanceira, 0m, 100m);

            var sinaisExecucao = new List<decimal>
            {
                (notaConsistencia * 0.60m) + (notaHigieneFinanceira * 0.40m)
            };
            var componentesOpcionais = new List<string>();

            if ((contextoComplementar?.PossuiPlanoEstrategicoVigente ?? false) && contextoComplementar?.NotaPlanoEstrategico is int notaPlano)
            {
                sinaisExecucao.Add(notaPlano);
                componentesOpcionais.Add($"plano vigente com {contextoComplementar.QuantidadeObjetivosPlanoAtivos} objetivo(s) ativo(s)");
            }

            if ((contextoComplementar?.PossuiCompromissosFinanceiros ?? false) && contextoComplementar?.NotaCompromissosFinanceiros is int notaCompromissos)
            {
                sinaisExecucao.Add(notaCompromissos);
                componentesOpcionais.Add($"compromissos financeiros ({contextoComplementar.QuantidadeCompromissosConcluidos} concluído(s), {contextoComplementar.QuantidadeCompromissosEmAndamento} em andamento)");
            }

            var mediaExecucao = sinaisExecucao.Average();
            var notaCalculada = (notaConfiguracao * 0.20m) + (mediaExecucao * 0.80m);
            var tetoPorConfiguracao = ObterTetoPlanejamentoPorConfiguracao(quantidadeParametrosConfigurados);
            var notaFinal = Math.Clamp(
                (int)Math.Round(Math.Min(notaCalculada, tetoPorConfiguracao)),
                0,
                EscalaPilares);

            var descricaoComponentesOpcionais = componentesOpcionais.Count > 0
                ? $" Componentes adicionais considerados: {string.Join("; ", componentesOpcionais)}."
                : " Sem plano estratégico vigente ou compromissos financeiros cadastrados, esses componentes opcionais são ignorados no cálculo.";

            return new PilarMfScoreFinanceiro
            {
                Codigo = CodigoPilarMfScoreFinanceiro.PlanejamentoEDisciplina,
                Nome = "Planejamento e Disciplina",
                Peso = 10m,
                Nota = notaFinal,
                Descricao = $"Combina base mínima de configuração com execução real: consistência da economia, formação de reserva, higiene financeira, aderência ao plano e cumprimento de compromissos. Parâmetros configurados: {quantidadeParametrosConfigurados}/{totalParametrosEsperados}.{descricaoComponentesOpcionais}",
                Indicadores = new List<string>
                {
                    percentualEconomia?.Nome ?? "Percentual de economia",
                    capacidadeFormacaoReserva?.Nome ?? "Capacidade de formação de reserva",
                    "Configuração básica do perfil financeiro",
                    "Plano estratégico financeiro",
                    "Compromissos financeiros"
                }
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
            if (somaPesos <= 0m)
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

        private static int CalcularNotaMediaPonderada(IEnumerable<(IndicadorFinanceiro? Indicador, decimal Peso)> itens)
        {
            var itensValidos = itens
                .Where(item => item.Indicador is not null && item.Peso > 0m)
                .ToList();

            if (itensValidos.Count == 0)
            {
                return 0;
            }

            var somaPesos = itensValidos.Sum(item => item.Peso);
            var somaPonderada = itensValidos.Sum(item => ObterPontuacao(item.Indicador!.Status) * item.Peso);
            return (int)Math.Round(somaPonderada / somaPesos);
        }

        private static int CalcularNotaMediaLiquidez(
            IndicadorFinanceiro? reservaAtual,
            IndicadorFinanceiro? capacidadeFormacaoReserva)
        {
            if (reservaAtual is null && capacidadeFormacaoReserva is null)
            {
                return 0;
            }

            var pontuacaoReserva = reservaAtual is null ? 0m : ObterPontuacao(reservaAtual.Status);
            var pontuacaoCapacidade = capacidadeFormacaoReserva is null ? 0m : ObterPontuacao(capacidadeFormacaoReserva.Status);
            var notaBase = (pontuacaoReserva * 0.7m) + (pontuacaoCapacidade * 0.3m);

            if (pontuacaoReserva <= 25m && capacidadeFormacaoReserva is not null)
            {
                notaBase = capacidadeFormacaoReserva.Status switch
                {
                    StatusIndicadorFinanceiro.Excelente => Math.Max(notaBase, 60m),
                    StatusIndicadorFinanceiro.Bom => Math.Max(notaBase, 55m),
                    _ => notaBase
                };
            }

            return (int)Math.Round(Math.Clamp(notaBase, 0m, EscalaPilares));
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
            var mesesNegativos = contextoComplementar?.MesesConsecutivosFluxoNegativo ?? 0;
            var penalizacaoFluxoNegativo = ObterPenalizacaoFluxoNegativo(mesesNegativos);

            if (penalizacaoFluxoNegativo is not null)
            {
                Adicionar(
                    economiaMensal?.Codigo ?? CodigoIndicadorFinanceiro.EconomiaMensal,
                    "Persistência de fluxo negativo",
                    penalizacaoFluxoNegativo.Value.Motivo,
                    penalizacaoFluxoNegativo.Value.Penalidade,
                    "Fluxo de Caixa");
            }

            var endividamento = Buscar(indicadores, CodigoIndicadorFinanceiro.Endividamento);
            if (contextoComplementar?.PossuiInadimplencia == true)
            {
                var nivelInadimplencia = contextoComplementar.NivelInadimplencia;
                var penalidadeBase = nivelInadimplencia switch
                {
                    1 => 3m,
                    2 => 9m,
                    3 => 17m,
                    4 => 25m,
                    _ => 9m
                };
                var agravamentoReincidencia = contextoComplementar.QuantidadeMesesComOcorrenciaAtrasoRecente switch
                {
                    >= 3 => 4m,
                    >= 2 => 2m,
                    _ => 0m
                };
                var penalidade = penalidadeBase + agravamentoReincidencia;

                var descricaoNivel = nivelInadimplencia switch
                {
                    1 => "Atraso técnico identificado",
                    2 => "Estresse moderado por atraso identificado",
                    3 => "Inadimplência relevante identificada",
                    4 => "Inadimplência grave identificada",
                    _ => "Inadimplência identificada"
                };

                Adicionar(
                    endividamento?.Codigo ?? CodigoIndicadorFinanceiro.Endividamento,
                    "Inadimplência",
                    $"{descricaoNivel}: {contextoComplementar.DiasMaximosAtraso} dia(s) de atraso e {contextoComplementar.PercentualValorEmAtrasoSobreRenda:N2}% da renda mensal comprometida em valores vencidos. Ocorrências recentes de atraso: {contextoComplementar.QuantidadeOcorrenciasAtrasoRecente} em {contextoComplementar.QuantidadeMesesComOcorrenciaAtrasoRecente} mês(es).",
                    penalidade,
                    "Endividamento e Obrigações");
            }
            else if (contextoComplementar?.PossuiCuraRecenteInadimplencia == true)
            {
                var penalidadeCura = contextoComplementar.QuantidadeMesesComOcorrenciaAtrasoRecente >= 2 ? 2m : 1m;

                Adicionar(
                    endividamento?.Codigo ?? CodigoIndicadorFinanceiro.Endividamento,
                    "Cura recente de inadimplência",
                    $"Não existe atraso pendente no momento, mas houve regularização recente de atraso em {contextoComplementar.QuantidadeOcorrenciasAtrasoRecente} ocorrência(s), distribuída(s) por {contextoComplementar.QuantidadeMesesComOcorrenciaAtrasoRecente} mês(es).",
                    penalidadeCura,
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

        private static (string Motivo, decimal Penalidade)? ObterPenalizacaoFluxoNegativo(int mesesConsecutivos)
        {
            return mesesConsecutivos switch
            {
                >= 12 => ("O usuário acumula doze ou mais meses consecutivos no vermelho.", 18m),
                >= 6 => ("O usuário acumula seis ou mais meses consecutivos no vermelho.", 14m),
                >= 3 => ("O usuário acumula três ou mais meses consecutivos no vermelho.", 10m),
                >= 2 => ("O usuário acumula dois meses consecutivos no vermelho.", 6m),
                >= 1 => ("O mês de referência fechou com fluxo de caixa negativo.", 3m),
                _ => null
            };
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

        private static int ObterTetoPlanejamentoPorConfiguracao(int quantidadeParametrosConfigurados)
        {
            return quantidadeParametrosConfigurados switch
            {
                >= 5 => 100,
                4 => 93,
                3 => 85,
                2 => 75,
                1 => 65,
                _ => 55
            };
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
