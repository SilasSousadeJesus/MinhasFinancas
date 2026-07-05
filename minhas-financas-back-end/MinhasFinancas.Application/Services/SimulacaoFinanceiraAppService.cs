using MinhasFinancas.Application.DTOs.SimulacaoFinanceira;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class SimulacaoFinanceiraAppService : ISimulacaoFinanceiraAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly ISimulacaoFinanceiraRepository _simulacaoRepository;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly SimulacaoFinanceiraEngine _engine;

        public SimulacaoFinanceiraAppService(
            IUsuarioAppService usuarioAppService,
            ISimulacaoFinanceiraRepository simulacaoRepository,
            ILancamentoRepository lancamentoRepository,
            SimulacaoFinanceiraEngine engine)
        {
            _usuarioAppService = usuarioAppService;
            _simulacaoRepository = simulacaoRepository;
            _lancamentoRepository = lancamentoRepository;
            _engine = engine;
        }

        public async Task<RetornoGenerico> BuscarTodasAsync(string usuarioId)
        {
            try
            {
                var validacao = await ValidarUsuarioAsync(usuarioId);
                if (validacao != null) return validacao;

                var simulacoes = await _simulacaoRepository.BuscarTodosOsElementosAsync(usuarioId);
                var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);

                var lista = simulacoes.Select(simulacao => new SimulacaoFinanceiraResumoDTO
                {
                    Id = simulacao.Id,
                    Nome = simulacao.Nome,
                    Descricao = simulacao.Descricao,
                    DataInicial = simulacao.DataInicial,
                    QuantidadeMeses = simulacao.QuantidadeMeses,
                    Ativa = simulacao.Ativa,
                    QuantidadeAcoes = simulacao.Acoes.Count(x => x.Ativa),
                    ResultadoAtual = PodeCalcular(simulacao)
                        ? _engine.Calcular(simulacao, lancamentos)
                        : null
                }).ToList();

                return new RetornoGenerico(true, $"{lista.Count} simulação(ões) encontrada(s)", $"{lista.Count} simulação(ões) encontrada(s)", HttpStatusCode.OK, lista);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível listar as simulações financeiras.");
            }
        }

        public async Task<RetornoGenerico> BuscarUmaAsync(string usuarioId, Guid simulacaoId)
        {
            try
            {
                var validacao = await ValidarUsuarioAsync(usuarioId);
                if (validacao != null) return validacao;

                var simulacao = await _simulacaoRepository.BuscarUmElementoAsync(usuarioId, simulacaoId);
                if (simulacao == null || !simulacao.Ativa)
                {
                    return CriarNaoEncontrado();
                }

                var detalhe = MapearDetalhe(simulacao);

                if (PodeCalcular(simulacao))
                {
                    var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
                    detalhe.ResultadoAtual = _engine.Calcular(simulacao, lancamentos);
                }

                return new RetornoGenerico(true, "Simulação encontrada.", "Simulação encontrada.", HttpStatusCode.OK, detalhe);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível carregar a simulação financeira.");
            }
        }

        public async Task<RetornoGenerico> CadastrarAsync(CadastrarSimulacaoFinanceiraDTO simulacaoDTO)
        {
            try
            {
                var validacao = await ValidarUsuarioAsync(simulacaoDTO.UsuarioId);
                if (validacao != null) return validacao;

                var simulacao = MontarEntidadeCadastro(simulacaoDTO);
                var validacaoSimulacao = ValidarSimulacao(simulacao);
                if (validacaoSimulacao != null) return validacaoSimulacao;

                await _simulacaoRepository.CadastrarElementoAsync(simulacao);

                return new RetornoGenerico(true, "Simulação cadastrada com sucesso.", "Simulação cadastrada com sucesso.", HttpStatusCode.OK, simulacao.Id);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível cadastrar a simulação financeira.");
            }
        }

        public async Task<RetornoGenerico> EditarAsync(string usuarioId, Guid simulacaoId, EditarSimulacaoFinanceiraDTO simulacaoDTO)
        {
            try
            {
                var existente = await _simulacaoRepository.BuscarUmElementoAsync(usuarioId, simulacaoId);
                if (existente == null || !existente.Ativa)
                {
                    return CriarNaoEncontrado();
                }

                var simulacao = MontarEntidadeEdicao(usuarioId, simulacaoId, simulacaoDTO, existente.DataCriacao, existente.Ativa);
                var validacaoSimulacao = ValidarSimulacao(simulacao);
                if (validacaoSimulacao != null) return validacaoSimulacao;

                await _simulacaoRepository.EditarElementoAsync(simulacao);

                return new RetornoGenerico(true, "Simulação atualizada com sucesso.", "Simulação atualizada com sucesso.", HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível editar a simulação financeira.");
            }
        }

        public async Task<RetornoGenerico> InativarAsync(string usuarioId, Guid simulacaoId)
        {
            try
            {
                var existente = await _simulacaoRepository.BuscarUmElementoAsync(usuarioId, simulacaoId);
                if (existente == null || !existente.Ativa)
                {
                    return CriarNaoEncontrado();
                }

                existente.Ativa = false;
                existente.DataAtualizacao = DateTime.UtcNow;
                await _simulacaoRepository.EditarElementoAsync(existente);

                return new RetornoGenerico(true, "Simulação inativada com sucesso.", "Simulação inativada com sucesso.", HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível inativar a simulação financeira.");
            }
        }

        public async Task<RetornoGenerico> CalcularAsync(string usuarioId, Guid simulacaoId)
        {
            try
            {
                var simulacao = await _simulacaoRepository.BuscarUmElementoAsync(usuarioId, simulacaoId);
                if (simulacao == null || !simulacao.Ativa)
                {
                    return CriarNaoEncontrado();
                }

                var validacaoSimulacao = ValidarSimulacao(simulacao);
                if (validacaoSimulacao != null) return validacaoSimulacao;

                var dataInicial = new DateTime(simulacao.DataInicial.Year, simulacao.DataInicial.Month, 1);
                var dataFinal = dataInicial.AddMonths(Math.Min(12, simulacao.QuantidadeMeses <= 0 ? 12 : simulacao.QuantidadeMeses)).AddDays(-1);
                var lancamentos = await _lancamentoRepository.BuscarPorPeriodoVencimentoAsync(usuarioId, dataInicial, dataFinal);
                var resultado = _engine.Calcular(simulacao, lancamentos);

                return new RetornoGenerico(true, "Simulação calculada com sucesso.", "Simulação calculada com sucesso.", HttpStatusCode.OK, resultado);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível calcular a simulação financeira.");
            }
        }

        private async Task<RetornoGenerico?> ValidarUsuarioAsync(string usuarioId)
        {
            var buscaPorUsuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);
            if (buscaPorUsuario.Sucesso) return null;

            return new RetornoGenerico
            {
                Sucesso = buscaPorUsuario.Sucesso,
                HttpStatusCode = HttpStatusCode.NotFound,
                MensagemSistema = buscaPorUsuario.MensagemSistema,
                MensagemUsuario = buscaPorUsuario.MensagemUsuario,
                Dados = null
            };
        }

        private static SimulacaoFinanceiraDetalheDTO MapearDetalhe(SimulacaoFinanceira simulacao)
        {
            return new SimulacaoFinanceiraDetalheDTO
            {
                Id = simulacao.Id,
                Nome = simulacao.Nome,
                Descricao = simulacao.Descricao,
                DataInicial = simulacao.DataInicial,
                QuantidadeMeses = simulacao.QuantidadeMeses,
                Ativa = simulacao.Ativa,
                Acoes = simulacao.Acoes
                    .Where(x => x.Ativa)
                    .OrderBy(x => x.DataInicial)
                    .Select(x => new AcaoSimulacaoFinanceiraDTO
                    {
                        TipoAcao = x.TipoAcao,
                        Descricao = x.Descricao,
                        Valor = x.Valor,
                        DataInicial = x.DataInicial,
                        DataFinal = x.DataFinal,
                        QuantidadeParcelas = x.QuantidadeParcelas,
                        Observacao = x.Observacao
                    })
                    .ToList()
            };
        }

        private static SimulacaoFinanceira MontarEntidadeCadastro(CadastrarSimulacaoFinanceiraDTO dto)
        {
            var simulacao = new SimulacaoFinanceira
            {
                Id = Guid.NewGuid(),
                UsuarioId = dto.UsuarioId,
                Nome = dto.Nome.Trim(),
                Descricao = dto.Descricao.Trim(),
                DataInicial = new DateTime(dto.DataInicial.Year, dto.DataInicial.Month, 1),
                QuantidadeMeses = dto.QuantidadeMeses,
                Ativa = true,
                DataCriacao = DateTime.UtcNow,
                DataAtualizacao = DateTime.UtcNow
            };

            simulacao.Acoes = dto.Acoes.Select(x => MontarAcao(simulacao.Id, x)).ToList();
            return simulacao;
        }

        private static SimulacaoFinanceira MontarEntidadeEdicao(
            string usuarioId,
            Guid simulacaoId,
            EditarSimulacaoFinanceiraDTO dto,
            DateTime dataCriacao,
            bool ativa)
        {
            var simulacao = new SimulacaoFinanceira
            {
                Id = simulacaoId,
                UsuarioId = usuarioId,
                Nome = dto.Nome.Trim(),
                Descricao = dto.Descricao.Trim(),
                DataInicial = new DateTime(dto.DataInicial.Year, dto.DataInicial.Month, 1),
                QuantidadeMeses = dto.QuantidadeMeses,
                Ativa = ativa,
                DataCriacao = dataCriacao,
                DataAtualizacao = DateTime.UtcNow
            };

            simulacao.Acoes = dto.Acoes.Select(x => MontarAcao(simulacaoId, x)).ToList();
            return simulacao;
        }

        private static AcaoSimulacaoFinanceira MontarAcao(Guid simulacaoId, AcaoSimulacaoFinanceiraDTO dto)
        {
            return new AcaoSimulacaoFinanceira
            {
                Id = Guid.NewGuid(),
                SimulacaoFinanceiraId = simulacaoId,
                TipoAcao = dto.TipoAcao,
                Descricao = dto.Descricao.Trim(),
                Valor = dto.Valor,
                DataInicial = dto.DataInicial.Date,
                DataFinal = dto.DataFinal?.Date,
                QuantidadeParcelas = dto.QuantidadeParcelas,
                Observacao = dto.Observacao.Trim(),
                Ativa = true
            };
        }

        private static RetornoGenerico? ValidarSimulacao(SimulacaoFinanceira simulacao)
        {
            if (string.IsNullOrWhiteSpace(simulacao.Nome))
            {
                return CriarValidacao("A simulação precisa ter um nome.", "Informe o nome da simulação.");
            }

            if (simulacao.QuantidadeMeses <= 0 || simulacao.QuantidadeMeses > 12)
            {
                return CriarValidacao("A quantidade de meses deve ficar entre 1 e 12.", "Use entre 1 e 12 meses para simular.");
            }

            foreach (var acao in simulacao.Acoes)
            {
                if (string.IsNullOrWhiteSpace(acao.Descricao))
                {
                    return CriarValidacao("Existe uma ação sem descrição.", "Preencha a descrição de todas as ações.");
                }

                if (acao.Valor <= decimal.Zero)
                {
                    return CriarValidacao("Existe uma ação com valor inválido.", "Todas as ações precisam ter valor maior que zero.");
                }

                if (acao.TipoAcao == EnumTipoAcaoSimulacaoFinanceira.DespesaParcelada &&
                    (!acao.QuantidadeParcelas.HasValue || acao.QuantidadeParcelas.Value <= 1))
                {
                    return CriarValidacao("A despesa parcelada exige quantidade de parcelas maior que 1.", "Informe uma quantidade de parcelas maior que 1.");
                }

                if ((acao.TipoAcao == EnumTipoAcaoSimulacaoFinanceira.ReceitaRecorrenteMensal ||
                    acao.TipoAcao == EnumTipoAcaoSimulacaoFinanceira.DespesaRecorrenteMensal) &&
                    acao.DataFinal.HasValue &&
                    acao.DataFinal.Value.Date < acao.DataInicial.Date)
                {
                    return CriarValidacao("Existe uma ação recorrente com data final anterior à inicial.", "Revise o período das ações recorrentes.");
                }
            }

            return null;
        }

        private static bool PodeCalcular(SimulacaoFinanceira simulacao)
        {
            return !string.IsNullOrWhiteSpace(simulacao.Nome)
                && simulacao.QuantidadeMeses > 0
                && simulacao.QuantidadeMeses <= 12;
        }

        private static RetornoGenerico CriarValidacao(string mensagemSistema, string mensagemUsuario)
        {
            return new RetornoGenerico(false, mensagemSistema, mensagemUsuario, HttpStatusCode.BadRequest, null);
        }

        private static RetornoGenerico CriarNaoEncontrado()
        {
            return new RetornoGenerico(false, "Simulação não encontrada.", "Simulação não encontrada.", HttpStatusCode.NotFound, null);
        }

        private static RetornoGenerico CriarErro(Exception ex, string mensagemUsuario)
        {
            return new RetornoGenerico(false, $"{ex}", mensagemUsuario, HttpStatusCode.InternalServerError, null);
        }
    }
}
