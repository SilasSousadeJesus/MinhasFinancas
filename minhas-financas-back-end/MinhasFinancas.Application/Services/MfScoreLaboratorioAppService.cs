using MinhasFinancas.Application.DTOs.MfScoreLaboratorio;
using MinhasFinancas.Application.DTOs.MfScore;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class MfScoreLaboratorioAppService : IMfScoreLaboratorioAppService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMfScoreCalculoAppService _mfScoreCalculoAppService;

        public MfScoreLaboratorioAppService(
            IUsuarioRepository usuarioRepository,
            IMfScoreCalculoAppService mfScoreCalculoAppService)
        {
            _usuarioRepository = usuarioRepository;
            _mfScoreCalculoAppService = mfScoreCalculoAppService;
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

                var dados = MapearDetalhe(usuario, calculo);

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

        private static UsuarioMfScoreLaboratorioDTO MapearUsuario(Usuario usuario)
        {
            return new UsuarioMfScoreLaboratorioDTO
            {
                UsuarioId = usuario.Id,
                Nome = string.IsNullOrWhiteSpace(usuario.Nome) ? "Usuário sem nome" : usuario.Nome,
                Email = usuario.Email ?? string.Empty,
                DataCadastro = null
            };
        }

        private static MfScoreLaboratorioDetalheDTO MapearDetalhe(Usuario usuario, ResultadoCalculoMfScoreInternoDTO calculo)
        {
            var mfScore = calculo.PainelSaude.Resumo.MfScore;

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
                ObservacoesLimitacoes = MontarObservacoesLimitacao(calculo)
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
