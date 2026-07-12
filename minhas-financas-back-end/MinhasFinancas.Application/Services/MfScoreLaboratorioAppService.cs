using MinhasFinancas.Application.DTOs.MfScore;
using MinhasFinancas.Application.DTOs.MfScoreLaboratorio;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Enums;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class MfScoreLaboratorioAppService : IMfScoreLaboratorioAppService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMfScoreCalculoAppService _mfScoreCalculoAppService;
        private readonly IGeradorBaseSimulacaoMfScoreService _geradorBaseSimulacaoMfScoreService;
        private readonly IBenchmarkMfScoreService _benchmarkMfScoreService;

        public MfScoreLaboratorioAppService(
            IUsuarioRepository usuarioRepository,
            IMfScoreCalculoAppService mfScoreCalculoAppService,
            IGeradorBaseSimulacaoMfScoreService geradorBaseSimulacaoMfScoreService,
            IBenchmarkMfScoreService benchmarkMfScoreService)
        {
            _usuarioRepository = usuarioRepository;
            _mfScoreCalculoAppService = mfScoreCalculoAppService;
            _geradorBaseSimulacaoMfScoreService = geradorBaseSimulacaoMfScoreService;
            _benchmarkMfScoreService = benchmarkMfScoreService;
        }

        public async Task<RetornoGenerico> BuscarUsuariosAsync()
        {
            try
            {
                var usuarios = await _usuarioRepository.BuscarUsuariosParaLaboratorioAsync();
                var dados = usuarios.Select(MapearUsuario).ToList();

                return new RetornoGenerico(
                    true,
                    $"{dados.Count} usuário(s) disponível(is) para auditoria do MF Score.",
                    $"{dados.Count} usuário(s) carregado(s) com sucesso.",
                    HttpStatusCode.OK,
                    dados);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível carregar os usuários do laboratório do MF Score.");
            }
        }

        public async Task<RetornoGenerico> BuscarScoreUsuarioAsync(string usuarioId)
        {
            try
            {
                var usuario = await _usuarioRepository.BuscarResumoUsuarioAsync(usuarioId);
                if (usuario == null)
                {
                    return new RetornoGenerico(false, "Usuário não encontrado.", "Usuário não encontrado.", HttpStatusCode.NotFound, null);
                }

                var calculo = await _mfScoreCalculoAppService.CalcularAsync(usuarioId);
                if (calculo == null)
                {
                    return new RetornoGenerico(
                        false,
                        "O motor financeiro não conseguiu montar o contexto completo do usuário selecionado.",
                        "Não foi possível calcular o MF Score deste usuário.",
                        HttpStatusCode.BadRequest,
                        null);
                }

                var dados = await MapearDetalheAsync(usuario, calculo);

                return new RetornoGenerico(
                    true,
                    "MF Score do usuário carregado com sucesso.",
                    "MF Score do usuário carregado com sucesso.",
                    HttpStatusCode.OK,
                    dados);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível calcular o MF Score do usuário selecionado.");
            }
        }

        public async Task<RetornoGenerico> GerarBaseSimulacaoAsync()
        {
            try
            {
                var dados = await _geradorBaseSimulacaoMfScoreService.GerarAsync();

                return new RetornoGenerico(
                    true,
                    $"{dados.QuantidadeUsuariosGerados} usuário(s) sintético(s) gerado(s) para {dados.QuantidadeCenarios} cenário(s).",
                    "Base Oficial de Simulação do MF Score gerada com sucesso.",
                    HttpStatusCode.Created,
                    dados);
            }
            catch (InvalidOperationException ex)
            {
                return new RetornoGenerico(false, ex.Message, ex.Message, HttpStatusCode.Conflict, null);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível gerar a Base Oficial de Simulação do MF Score.");
            }
        }

        public async Task<RetornoGenerico> LimparBaseSimulacaoAsync()
        {
            try
            {
                var dados = await _geradorBaseSimulacaoMfScoreService.LimparAsync();

                return new RetornoGenerico(
                    true,
                    $"{dados.QuantidadeUsuariosRemovidos} usuário(s) sintético(s) removido(s).",
                    "Base Oficial de Simulação do MF Score removida com sucesso.",
                    HttpStatusCode.OK,
                    dados);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível limpar a Base Oficial de Simulação do MF Score.");
            }
        }

        private static UsuarioMfScoreLaboratorioDTO MapearUsuario(Usuario usuario)
        {
            return new UsuarioMfScoreLaboratorioDTO
            {
                UsuarioId = usuario.Id,
                Nome = string.IsNullOrWhiteSpace(usuario.Nome) ? "Usuário sem nome" : usuario.Nome,
                Email = usuario.Email ?? string.Empty,
                DataCadastro = null,
                EhUsuarioSintetico = usuario.EhUsuarioSintetico,
                OrigemUsuario = usuario.OrigemUsuario ?? string.Empty,
                CodigoCenario = usuario.CodigoCenarioSimulacao ?? string.Empty,
                VersaoBase = usuario.VersaoBaseSimulacao ?? string.Empty,
                DataGeracaoBase = usuario.DataGeracaoBaseSimulacao,
                DescricaoCenario = usuario.DescricaoCenarioSimulacao ?? string.Empty,
                ObjetivoCenario = usuario.ObjetivoCenarioSimulacao ?? string.Empty
            };
        }

        private async Task<MfScoreLaboratorioDetalheDTO> MapearDetalheAsync(Usuario usuario, ResultadoCalculoMfScoreInternoDTO calculo)
        {
            var mfScore = calculo.PainelSaude.Resumo.MfScore;
            var analiseCalibracao = await MontarAnaliseCalibracaoAsync(usuario, calculo);

            return new MfScoreLaboratorioDetalheDTO
            {
                Usuario = MapearUsuario(usuario),
                VersaoModelo = MfScoreCalculoAppService.VersaoModeloAtual,
                MfScoreBase = mfScore.PontuacaoBase,
                MfScoreFinal = mfScore.PontuacaoFinal,
                Classificacao = mfScore.Classificacao,
                Risco = mfScore.Risco,
                PenalidadeTotal = mfScore.PenalidadeTotal,
                Descricao = mfScore.Descricao,
                Tendencia = new TendenciaMfScoreLaboratorioDTO
                {
                    Direcao = mfScore.Tendencia.Direcao.ToString(),
                    Descricao = mfScore.Tendencia.Descricao,
                    HistoricoNotas = mfScore.Tendencia.HistoricoNotas
                },
                ResumoExecutivoDosPilares = mfScore.ResumoExecutivoDosPilares,
                Pilares = mfScore.Pilares.Select(MapearPilar).ToList(),
                Indicadores = calculo.PainelIndicadores.Todos.Select(MapearIndicador).ToList(),
                IndicadoresCriticos = mfScore.IndicadoresCriticos.Select(MapearIndicadorCritico).ToList(),
                Penalizacoes = mfScore.IndicadoresCriticos.Select(MapearPenalizacao).ToList(),
                RegrasCriticasAplicadas = mfScore.RegrasCriticasAplicadas,
                DadosEntrada = MapearDadosEntrada(calculo),
                ObservacoesLimitacoes = MontarObservacoesLimitacao(calculo),
                AnaliseCalibracao = analiseCalibracao
            };
        }

        private static PilarMfScoreLaboratorioDTO MapearPilar(PilarMfScoreFinanceiro pilar)
        {
            return new PilarMfScoreLaboratorioDTO
            {
                Codigo = pilar.Codigo.ToString(),
                Nome = pilar.Nome,
                Peso = pilar.Peso,
                Nota = pilar.Nota,
                Descricao = pilar.Descricao,
                Indicadores = pilar.Indicadores
            };
        }

        private static IndicadorMfScoreLaboratorioDTO MapearIndicador(IndicadorFinanceiro indicador)
        {
            return new IndicadorMfScoreLaboratorioDTO
            {
                Codigo = indicador.Codigo.ToString(),
                Nome = indicador.Nome,
                ValorAtual = indicador.ValorAtual,
                ValorIdeal = indicador.ValorIdeal,
                Percentual = indicador.Percentual,
                ValorObrigacoesPrevistas = indicador.ValorObrigacoesPrevistas,
                ValorReceitaPrevista = indicador.ValorReceitaPrevista,
                PercentualComprometimento = indicador.PercentualComprometimento,
                Status = indicador.Status.ToString(),
                Descricao = indicador.Descricao,
                Observacao = indicador.Observacao,
                Formato = indicador.Formato.ToString()
            };
        }

        private static IndicadorCriticoMfScoreLaboratorioDTO MapearIndicadorCritico(IndicadorCriticoMfScoreFinanceiro indicador)
        {
            return new IndicadorCriticoMfScoreLaboratorioDTO
            {
                Codigo = indicador.CodigoIndicador.ToString(),
                Nome = indicador.Nome,
                Motivo = indicador.Motivo,
                Penalidade = indicador.Penalidade,
                PilarRelacionado = indicador.PilarRelacionado
            };
        }

        private static PenalizacaoMfScoreLaboratorioDTO MapearPenalizacao(IndicadorCriticoMfScoreFinanceiro indicador)
        {
            return new PenalizacaoMfScoreLaboratorioDTO
            {
                Nome = indicador.Nome,
                Motivo = indicador.Motivo,
                Penalidade = indicador.Penalidade,
                PilarRelacionado = indicador.PilarRelacionado
            };
        }

        private static DadosEntradaMfScoreLaboratorioDTO MapearDadosEntrada(ResultadoCalculoMfScoreInternoDTO calculo)
        {
            var dataReferencia = calculo.DataReferencia;
            var lancamentos = calculo.ContextoAnalise.Lancamentos.ToList();
            var receitasDoMes = lancamentos
                .Where(x => x.Tipo == EnumTipoLancamento.Receita && MesmoMes(x.DataVencimento, dataReferencia))
                .Sum(x => x.Valor);
            var despesasDoMes = lancamentos
                .Where(x => x.Tipo == EnumTipoLancamento.Despesa && MesmoMes(x.DataVencimento, dataReferencia))
                .Sum(x => x.Valor);

            return new DadosEntradaMfScoreLaboratorioDTO
            {
                DataReferencia = dataReferencia,
                QuantidadeLancamentos = lancamentos.Count,
                QuantidadeReceitas = lancamentos.Count(x => x.Tipo == EnumTipoLancamento.Receita),
                QuantidadeDespesas = lancamentos.Count(x => x.Tipo == EnumTipoLancamento.Despesa),
                ReceitaMensalConsiderada = receitasDoMes,
                DespesaMensalConsiderada = despesasDoMes,
                QuantidadeAtivos = calculo.ContextoAnalise.Ativos.Count,
                QuantidadePassivos = calculo.ContextoAnalise.Passivos.Count,
                ValorAtivosConsiderados = SomarValoresAtivos(calculo.ContextoAnalise.Ativos),
                ValorPassivosConsiderados = SomarValoresPassivos(calculo.ContextoAnalise.Passivos),
                QuantidadeMetas = calculo.ContextoAnalise.Metas.Count,
                PossuiPerfilFinanceiroConfigurado = calculo.ContextoComplementar.QuantidadeParametrosPlanejamentoConfigurados > 0,
                PossuiPlanoEstrategicoVigente = calculo.ContextoComplementar.PossuiPlanoEstrategicoVigente,
                QuantidadeObjetivosPlanoAtivos = calculo.ContextoComplementar.QuantidadeObjetivosPlanoAtivos,
                QuantidadeObjetivosPlanoAltaPrioridade = calculo.ContextoComplementar.QuantidadeObjetivosPlanoAltaPrioridade,
                QuantidadeObjetivosPlanoConcluidos = calculo.ContextoComplementar.QuantidadeObjetivosPlanoConcluidos,
                PossuiCompromissosFinanceiros = calculo.ContextoComplementar.PossuiCompromissosFinanceiros,
                QuantidadeCompromissosEmAndamento = calculo.ContextoComplementar.QuantidadeCompromissosEmAndamento,
                QuantidadeCompromissosConcluidos = calculo.ContextoComplementar.QuantidadeCompromissosConcluidos,
                QuantidadeCompromissosCancelados = calculo.ContextoComplementar.QuantidadeCompromissosCancelados,
                PossuiFluxoMensalNegativoAtual = calculo.ContextoComplementar.PossuiFluxoMensalNegativoAtual,
                MesesConsecutivosFluxoNegativo = calculo.ContextoComplementar.MesesConsecutivosFluxoNegativo,
                PossuiInadimplencia = calculo.ContextoComplementar.PossuiInadimplencia,
                NivelInadimplencia = calculo.ContextoComplementar.NivelInadimplencia,
                DiasMaximosAtraso = calculo.ContextoComplementar.DiasMaximosAtraso,
                ValorTotalEmAtraso = calculo.ContextoComplementar.ValorTotalEmAtraso,
                PercentualValorEmAtrasoSobreRenda = calculo.ContextoComplementar.PercentualValorEmAtrasoSobreRenda,
                PossuiCuraRecenteInadimplencia = calculo.ContextoComplementar.PossuiCuraRecenteInadimplencia,
                QuantidadeOcorrenciasAtrasoRecente = calculo.ContextoComplementar.QuantidadeOcorrenciasAtrasoRecente,
                QuantidadeMesesComOcorrenciaAtrasoRecente = calculo.ContextoComplementar.QuantidadeMesesComOcorrenciaAtrasoRecente,
                PossuiDadosEssenciaisInsuficientes = calculo.ContextoComplementar.PossuiDadosEssenciaisInsuficientes,
                QuantidadeParametrosPlanejamentoConfigurados = calculo.ContextoComplementar.QuantidadeParametrosPlanejamentoConfigurados,
                TotalParametrosPlanejamentoEsperados = calculo.ContextoComplementar.TotalParametrosPlanejamentoEsperados,
                PerfilFinanceiroBasicoCompleto = calculo.ContextoComplementar.PerfilFinanceiroBasicoCompleto,
                NotaConfiguracaoPlanejamento = calculo.ContextoComplementar.NotaConfiguracaoPlanejamento,
                NotaPlanoEstrategico = calculo.ContextoComplementar.NotaPlanoEstrategico,
                NotaCompromissosFinanceiros = calculo.ContextoComplementar.NotaCompromissosFinanceiros
            };
        }

        private async Task<AnaliseCalibracaoMfScoreLaboratorioDTO> MontarAnaliseCalibracaoAsync(
            Usuario usuario,
            ResultadoCalculoMfScoreInternoDTO calculo)
        {
            if (!usuario.EhUsuarioSintetico || string.IsNullOrWhiteSpace(usuario.CodigoCenarioSimulacao))
            {
                return new AnaliseCalibracaoMfScoreLaboratorioDTO
                {
                    Disponivel = false,
                    Mensagem = "A análise de calibração está disponível apenas para cenários oficiais do benchmark do MF Score."
                };
            }

            var benchmark = await _benchmarkMfScoreService.BuscarCenarioAsync(usuario.CodigoCenarioSimulacao);
            if (benchmark == null)
            {
                return new AnaliseCalibracaoMfScoreLaboratorioDTO
                {
                    Disponivel = false,
                    Mensagem = $"O cenário {usuario.CodigoCenarioSimulacao} não foi encontrado no benchmark oficial."
                };
            }

            var mfScore = calculo.PainelSaude.Resumo.MfScore;
            var scoreAtual = mfScore.PontuacaoFinal;
            var dentroDaFaixa = scoreAtual >= benchmark.FaixaAceitavelMinima && scoreAtual <= benchmark.FaixaAceitavelMaxima;
            var diferencaAtual = scoreAtual - benchmark.NotaHumanaReferencia;
            var piorPilar = mfScore.Pilares.OrderBy(item => item.Nota).FirstOrDefault();
            var melhorPilar = mfScore.Pilares.OrderByDescending(item => item.Nota).FirstOrDefault();
            var indicadores = calculo.PainelIndicadores.Todos.ToList();

            return new AnaliseCalibracaoMfScoreLaboratorioDTO
            {
                Disponivel = true,
                Benchmark = benchmark,
                DiferencaAtual = diferencaAtual,
                DentroDaFaixaEsperada = dentroDaFaixa,
                SituacaoFaixa = benchmark.Status == "Cenário inválido"
                    ? "Cenário ainda inválido para calibração definitiva."
                    : dentroDaFaixa
                        ? "Dentro da faixa esperada."
                        : "Fora da faixa esperada.",
                AnalisesPilares = MontarAnalisesPilares(mfScore.Pilares, indicadores, benchmark, scoreAtual),
                IndicadoresQuePuxaramParaBaixo = SelecionarIndicadoresNegativos(indicadores, mfScore.Pilares)
                    .Select(item => item.Nome)
                    .Take(4)
                    .ToList(),
                PrincipaisPontosPositivos = SelecionarIndicadoresPositivos(indicadores, mfScore.Pilares)
                    .Select(item => item.Nome)
                    .Take(4)
                    .ToList(),
                DiagnosticoFinal = MontarDiagnosticoFinal(benchmark, scoreAtual, dentroDaFaixa, piorPilar, melhorPilar),
                RecomendacaoProximaCalibracao = MontarRecomendacaoCalibracao(benchmark, dentroDaFaixa, piorPilar)
            };
        }

        private static List<string> MontarObservacoesLimitacao(ResultadoCalculoMfScoreInternoDTO calculo)
        {
            var observacoes = new List<string>();
            var contexto = calculo.ContextoComplementar;

            if (contexto.PossuiDadosEssenciaisInsuficientes)
            {
                observacoes.Add("O motor identificou dados essenciais insuficientes para uma leitura totalmente confiável.");
            }

            if (!contexto.PerfilFinanceiroBasicoCompleto)
            {
                observacoes.Add("O perfil financeiro básico ainda não está completo, o que limita a nota máxima do pilar Planejamento.");
            }

            if (!contexto.PossuiPlanoEstrategicoVigente)
            {
                observacoes.Add("O usuário não possui plano estratégico vigente, então esse sinal opcional não entrou no cálculo do planejamento.");
            }

            if (!contexto.PossuiCompromissosFinanceiros)
            {
                observacoes.Add("O usuário não possui compromissos financeiros ativos, então esse sinal opcional foi ignorado no pilar Planejamento.");
            }

            if (calculo.ContextoAnalise.Metas.Count == 0)
            {
                observacoes.Add("Não há metas cadastradas no contexto atual do cálculo.");
            }

            if (contexto.HistoricoPontuacoesFinais.Count == 0)
            {
                observacoes.Add("Ainda não existe histórico mensal suficiente para enriquecer a tendência com série real.");
            }

            return observacoes;
        }

        private static List<AnalisePilarCalibracaoMfScoreLaboratorioDTO> MontarAnalisesPilares(
            IReadOnlyCollection<PilarMfScoreFinanceiro> pilares,
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            BenchmarkCenarioMfScoreLaboratorioDTO benchmark,
            int scoreAtual)
        {
            var abaixoDoEsperado = scoreAtual < benchmark.FaixaAceitavelMinima;
            var acimaDoEsperado = scoreAtual > benchmark.FaixaAceitavelMaxima;
            var ordem = pilares.OrderBy(item => item.Nota).Select((pilar, indice) => new { pilar, indice }).ToList();

            return ordem
                .Select(item =>
                {
                    var indicadoresDoPilar = indicadores
                        .Where(indicador => PertenceAoPilar(indicador.Codigo, item.pilar.Codigo))
                        .ToList();
                    var possuiCritico = indicadoresDoPilar.Any(indicador => indicador.Status == StatusIndicadorFinanceiro.Critico);
                    var possuiAtencao = indicadoresDoPilar.Any(indicador => indicador.Status == StatusIndicadorFinanceiro.Atencao);

                    string diagnostico;

                    if (benchmark.Status == "Cenário inválido")
                    {
                        diagnostico = item.indice == 0
                            ? "Leitura provisória: este pilar ajuda a explicar o score atual, mas o cenário ainda precisa ser reconstruído no benchmark."
                            : "Leitura provisória por depender de um cenário ainda inválido para calibração definitiva.";
                    }
                    else if (abaixoDoEsperado)
                    {
                        diagnostico = item.indice switch
                        {
                            0 => "Principal responsável pela nota inferior ao esperado.",
                            1 when possuiCritico || possuiAtencao => "Também contribui de forma relevante para uma leitura mais conservadora do que a referência humana.",
                            _ when possuiCritico => "Está mais conservador do que o esperado para este cenário.",
                            _ when possuiAtencao => "Levemente conservador em relação à faixa esperada.",
                            _ when item.pilar.Nota >= 80 => "Muito próximo do esperado.",
                            _ => "Dentro do esperado, com pouca influência sobre a diferença final."
                        };
                    }
                    else if (acimaDoEsperado)
                    {
                        diagnostico = item.pilar.Nota >= 85
                            ? "Pode estar benevolente demais para este cenário."
                            : "Sem sinais de benevolência excessiva relevantes.";
                    }
                    else
                    {
                        diagnostico = item.pilar.Nota >= 80
                            ? "Dentro do esperado para este cenário."
                            : possuiCritico || possuiAtencao
                                ? "Ainda conservador em alguns sinais, mas sem romper a faixa aceitável."
                                : "Muito próximo do esperado.";
                    }

                    return new AnalisePilarCalibracaoMfScoreLaboratorioDTO
                    {
                        CodigoPilar = item.pilar.Codigo.ToString(),
                        NomePilar = item.pilar.Nome,
                        NotaPilar = item.pilar.Nota,
                        Diagnostico = diagnostico
                    };
                })
                .OrderBy(item => item.NotaPilar)
                .ToList();
        }

        private static List<IndicadorFinanceiro> SelecionarIndicadoresNegativos(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            IReadOnlyCollection<PilarMfScoreFinanceiro> pilares)
        {
            return indicadores
                .Where(indicador => indicador.Status == StatusIndicadorFinanceiro.Critico || indicador.Status == StatusIndicadorFinanceiro.Atencao)
                .OrderByDescending(indicador => ObterPesoStatusNegativo(indicador.Status))
                .ThenBy(indicador => ObterNotaPilar(indicador.Codigo, pilares))
                .ThenByDescending(indicador => Math.Abs(indicador.Percentual))
                .ToList();
        }

        private static List<IndicadorFinanceiro> SelecionarIndicadoresPositivos(
            IReadOnlyCollection<IndicadorFinanceiro> indicadores,
            IReadOnlyCollection<PilarMfScoreFinanceiro> pilares)
        {
            return indicadores
                .Where(indicador => indicador.Status == StatusIndicadorFinanceiro.Excelente || indicador.Status == StatusIndicadorFinanceiro.Bom)
                .OrderByDescending(indicador => ObterPesoStatusPositivo(indicador.Status))
                .ThenByDescending(indicador => ObterNotaPilar(indicador.Codigo, pilares))
                .ThenByDescending(indicador => Math.Abs(indicador.Percentual))
                .ToList();
        }

        private static int ObterPesoStatusNegativo(StatusIndicadorFinanceiro status)
        {
            return status switch
            {
                StatusIndicadorFinanceiro.Critico => 3,
                StatusIndicadorFinanceiro.Atencao => 2,
                StatusIndicadorFinanceiro.Bom => 1,
                _ => 0
            };
        }

        private static int ObterPesoStatusPositivo(StatusIndicadorFinanceiro status)
        {
            return status switch
            {
                StatusIndicadorFinanceiro.Excelente => 2,
                StatusIndicadorFinanceiro.Bom => 1,
                _ => 0
            };
        }

        private static int ObterNotaPilar(CodigoIndicadorFinanceiro codigoIndicador, IReadOnlyCollection<PilarMfScoreFinanceiro> pilares)
        {
            var codigoPilar = MapearPilar(codigoIndicador);
            return pilares.FirstOrDefault(item => item.Codigo == codigoPilar)?.Nota ?? 0;
        }

        private static bool PertenceAoPilar(CodigoIndicadorFinanceiro codigoIndicador, CodigoPilarMfScoreFinanceiro codigoPilar)
        {
            return MapearPilar(codigoIndicador) == codigoPilar;
        }

        private static CodigoPilarMfScoreFinanceiro MapearPilar(CodigoIndicadorFinanceiro codigoIndicador)
        {
            return codigoIndicador switch
            {
                CodigoIndicadorFinanceiro.EconomiaMensal or
                CodigoIndicadorFinanceiro.PercentualEconomia or
                CodigoIndicadorFinanceiro.ComprometimentoRenda => CodigoPilarMfScoreFinanceiro.FluxoDeCaixa,

                CodigoIndicadorFinanceiro.ReservaEmergenciaAtual or
                CodigoIndicadorFinanceiro.ReservaEmergenciaIdeal or
                CodigoIndicadorFinanceiro.CapacidadeFormacaoReserva => CodigoPilarMfScoreFinanceiro.LiquidezEReserva,

                CodigoIndicadorFinanceiro.Endividamento or
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo or
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo90Dias or
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo180Dias or
                CodigoIndicadorFinanceiro.ComprometimentoFinanceiroFuturo365Dias => CodigoPilarMfScoreFinanceiro.EndividamentoEObrigacoes,

                CodigoIndicadorFinanceiro.PatrimonioLiquidoAtual or
                CodigoIndicadorFinanceiro.PercentualPatrimonioAlvo => CodigoPilarMfScoreFinanceiro.Patrimonio,

                _ => CodigoPilarMfScoreFinanceiro.PlanejamentoEDisciplina
            };
        }

        private static string MontarDiagnosticoFinal(
            BenchmarkCenarioMfScoreLaboratorioDTO benchmark,
            int scoreAtual,
            bool dentroDaFaixa,
            PilarMfScoreFinanceiro? piorPilar,
            PilarMfScoreFinanceiro? melhorPilar)
        {
            if (benchmark.Status == "Cenário inválido")
            {
                return $"O benchmark oficial ainda considera este cenário inválido. O score calculado foi {scoreAtual}/1000, mas a principal ação agora não é recalibrar o motor e sim reconstruir a massa sintética para que ela represente corretamente o objetivo declarado.";
            }

            if (dentroDaFaixa)
            {
                return $"O cenário ficou dentro da faixa esperada do benchmark. O motor já apresenta comportamento compatível com a referência humana, com destaque positivo para {NomePilarOuPadrao(melhorPilar)} e sem distorções suficientes para justificar recalibração imediata.";
            }

            if (scoreAtual < benchmark.FaixaAceitavelMinima)
            {
                return $"O cenário ficou abaixo da nota esperada principalmente porque o pilar {NomePilarOuPadrao(piorPilar)} está mais conservador do que a referência humana do benchmark. O motor continua coerente conceitualmente, mas este é o melhor candidato para revisão pontual na próxima calibração.";
            }

            return $"O cenário ficou acima da faixa esperada do benchmark. O principal ponto de atenção agora é confirmar se o pilar {NomePilarOuPadrao(melhorPilar)} não está benevolente demais para este tipo de risco.";
        }

        private static string MontarRecomendacaoCalibracao(
            BenchmarkCenarioMfScoreLaboratorioDTO benchmark,
            bool dentroDaFaixa,
            PilarMfScoreFinanceiro? piorPilar)
        {
            if (benchmark.Status == "Cenário inválido")
            {
                return "Revisar e reconstruir a massa sintética deste cenário antes de qualquer ajuste numérico no motor.";
            }

            if (dentroDaFaixa)
            {
                return "Manter este cenário como referência estável e usar sua faixa para detectar regressões nas próximas versões.";
            }

            return $"Avaliar primeiro e de forma isolada o pilar {NomePilarOuPadrao(piorPilar)} antes de alterar qualquer outro componente do motor.";
        }

        private static string NomePilarOuPadrao(PilarMfScoreFinanceiro? pilar)
        {
            return pilar?.Nome ?? "sem pilar definido";
        }

        private static bool MesmoMes(DateTime data, DateTime referencia)
        {
            return data.Year == referencia.Year && data.Month == referencia.Month;
        }

        private static decimal SomarValoresAtivos(IReadOnlyCollection<BemPatrimonial> ativos)
        {
            return ativos.Sum(ativo => ativo.DataPermanencia
                .OrderByDescending(x => x.DataPermanencia)
                .FirstOrDefault()?.Valor ?? 0m);
        }

        private static decimal SomarValoresPassivos(IReadOnlyCollection<Passivo> passivos)
        {
            return passivos.Sum(passivo => passivo.DataPermanencia?
                .OrderByDescending(x => x.DataPermanencia)
                .FirstOrDefault()?.Valor ?? 0m);
        }

        private static RetornoGenerico CriarErro(Exception ex, string mensagemUsuario)
        {
            return new RetornoGenerico(false, ex.ToString(), mensagemUsuario, HttpStatusCode.InternalServerError, null);
        }
    }
}
