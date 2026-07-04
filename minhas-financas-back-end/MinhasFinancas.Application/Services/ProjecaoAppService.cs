using AutoMapper;
using MinhasFinancas.Application.DTOs.Projecao;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class ProjecaoAppService : IProjecaoAppService
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IProjecaoRepository _projecaoRepository;

        public ProjecaoAppService(
            IMapper mapper,
            IUsuarioAppService usuarioAppService,
            ILancamentoRepository lancamentoRepository,
            IProjecaoRepository projecaoRepository)
        {
            _mapper = mapper;
            _usuarioAppService = usuarioAppService;
            _lancamentoRepository = lancamentoRepository;
            _projecaoRepository = projecaoRepository;
        }

        public async Task<RetornoGenerico> BuscarTodosAsync(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var validacao = await ValidarUsuarioAsync(usuarioId);
                if (validacao != null)
                {
                    return validacao;
                }

                var projecoes = await _projecaoRepository.BuscarTodosOsElementosAsync(usuarioId);
                var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);

                var lista = projecoes
                    .Select(x =>
                    {
                        var resultado = PodeCalcular(x)
                            ? CalcularResultado(x, lancamentos)
                            : null;

                        return new ProjecaoResumoDTO
                        {
                            Id = x.Id,
                            Nome = x.Nome,
                            DataInicial = x.DataInicial,
                            ValorAcumuladoInicial = x.ValorAcumuladoInicial,
                            ValorObjetivo = x.ValorObjetivo,
                            MesesLimite = x.MesesLimite,
                            QuantidadeRendas = x.Rendas.Count,
                            RendaManualTotal = x.Rendas.Where(r => r.ValorMensal > decimal.Zero).Sum(r => r.ValorMensal),
                            ResultadoAtual = resultado
                        };
                    })
                    .ToList();

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = $"{lista.Count} projeção(ões) encontrada(s)";
                retorno.MensagemUsuario = $"{lista.Count} projeção(ões) encontrada(s)";
                retorno.Dados = lista;
                return retorno;
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel listar as projeções.");
            }
        }

        public async Task<RetornoGenerico> BuscarUmAsync(string usuarioId, Guid projecaoId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var validacao = await ValidarUsuarioAsync(usuarioId);
                if (validacao != null)
                {
                    return validacao;
                }

                var projecao = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (projecao == null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Projeção não encontrada.";
                    retorno.MensagemUsuario = "Projeção não encontrada.";
                    retorno.Dados = null;
                    return retorno;
                }

                var detalhe = new DetalheProjecaoDTO
                {
                    Id = projecao.Id,
                    Nome = projecao.Nome,
                    DataInicial = projecao.DataInicial,
                    ValorAcumuladoInicial = projecao.ValorAcumuladoInicial,
                    ValorObjetivo = projecao.ValorObjetivo,
                    MesesLimite = projecao.MesesLimite,
                    Rendas = projecao.Rendas
                        .OrderBy(x => x.Nome)
                        .Select(x => new RendaProjecaoDTO
                        {
                            Nome = x.Nome,
                            ValorMensal = x.ValorMensal
                        })
                        .ToList()
                };

                if (PodeCalcular(projecao))
                {
                    var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
                    detalhe.ResultadoAtual = CalcularResultado(projecao, lancamentos);
                }

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Projeção encontrada.";
                retorno.MensagemUsuario = "Projeção encontrada.";
                retorno.Dados = detalhe;
                return retorno;
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel buscar a projeção.");
            }
        }

        public async Task<RetornoGenerico> CadastrarAsync(CadastrarProjecaoDTO projecaoDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var validacao = await ValidarUsuarioAsync(projecaoDTO.UsuarioId);
                if (validacao != null)
                {
                    return validacao;
                }

                var projecao = MontarEntidadeCadastro(projecaoDTO);
                await _projecaoRepository.CadastrarElementoAsync(projecao);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Projeção cadastrada com sucesso.";
                retorno.MensagemUsuario = "Projeção cadastrada com sucesso.";
                retorno.Dados = projecao.Id;
                return retorno;
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel cadastrar a projeção.");
            }
        }

        public async Task<RetornoGenerico> EditarAsync(string usuarioId, Guid projecaoId, EditarProjecaoDTO projecaoDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var busca = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (busca == null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Projeção não encontrada.";
                    retorno.MensagemUsuario = "Projeção não encontrada.";
                    retorno.Dados = null;
                    return retorno;
                }

                var projecao = MontarEntidadeEdicao(usuarioId, projecaoId, projecaoDTO, busca.DataCadastro);
                await _projecaoRepository.EditarElementoAsync(projecao);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Projeção atualizada com sucesso.";
                retorno.MensagemUsuario = "Projeção atualizada com sucesso.";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel editar a projeção.");
            }
        }

        public async Task<RetornoGenerico> DeletarAsync(string usuarioId, Guid projecaoId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var projecao = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (projecao == null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Projeção não encontrada.";
                    retorno.MensagemUsuario = "Projeção não encontrada.";
                    retorno.Dados = null;
                    return retorno;
                }

                await _projecaoRepository.DeletarElementoAsync(projecao);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Projeção excluída com sucesso.";
                retorno.MensagemUsuario = "Projeção excluída com sucesso.";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel excluir a projeção.");
            }
        }

        public async Task<RetornoGenerico> CalcularAsync(string usuarioId, CalcularProjecaoDTO calcularProjecaoDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var validacao = await ValidarUsuarioAsync(usuarioId);
                if (validacao != null)
                {
                    return validacao;
                }

                var projecao = new Projecao
                {
                    UsuarioId = usuarioId,
                    DataInicial = calcularProjecaoDTO.DataInicial?.Date
                        ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                    ValorAcumuladoInicial = calcularProjecaoDTO.ValorAcumuladoInicial,
                    ValorObjetivo = calcularProjecaoDTO.ValorObjetivo,
                    MesesLimite = calcularProjecaoDTO.MesesLimite <= 0 ? 60 : calcularProjecaoDTO.MesesLimite,
                    Rendas = calcularProjecaoDTO.Rendas
                        .Select(x => new RendaProjecao
                        {
                            Nome = x.Nome,
                            ValorMensal = x.ValorMensal
                        })
                        .ToList()
                };

                var validacaoCalculo = ValidarProjecao(projecao);
                if (validacaoCalculo != null)
                {
                    return validacaoCalculo;
                }

                var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
                var resultado = CalcularResultado(projecao, lancamentos);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Projeção calculada com sucesso.";
                retorno.MensagemUsuario = "Projeção calculada.";
                retorno.Dados = resultado;
                return retorno;
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel calcular a projeção.");
            }
        }

        public async Task<RetornoGenerico> CalcularAsync(string usuarioId, Guid projecaoId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var projecao = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (projecao == null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Projeção não encontrada.";
                    retorno.MensagemUsuario = "Projeção não encontrada.";
                    retorno.Dados = null;
                    return retorno;
                }

                var validacao = ValidarProjecao(projecao);
                if (validacao != null)
                {
                    return validacao;
                }

                var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
                var resultado = CalcularResultado(projecao, lancamentos);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Projeção calculada com sucesso.";
                retorno.MensagemUsuario = "Projeção calculada.";
                retorno.Dados = resultado;
                return retorno;
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel calcular a projeção.");
            }
        }

        private async Task<RetornoGenerico?> ValidarUsuarioAsync(string usuarioId)
        {
            var buscaPorUsuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);

            if (buscaPorUsuario.Sucesso)
            {
                return null;
            }

            return new RetornoGenerico
            {
                Sucesso = buscaPorUsuario.Sucesso,
                HttpStatusCode = HttpStatusCode.NotFound,
                MensagemSistema = buscaPorUsuario.MensagemSistema,
                MensagemUsuario = buscaPorUsuario.MensagemUsuario,
                Dados = null
            };
        }

        private Projecao MontarEntidadeCadastro(CadastrarProjecaoDTO dto)
        {
            var projecao = _mapper.Map<Projecao>(dto);
            projecao.Id = Guid.NewGuid();
            projecao.DataInicial = dto.DataInicial?.Date
                ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            projecao.MesesLimite = dto.MesesLimite <= 0 ? 60 : dto.MesesLimite;
            projecao.DataCadastro = DateTime.UtcNow;
            projecao.DataAtualizacao = DateTime.UtcNow;
            projecao.Rendas = dto.Rendas
                .Select(x => new RendaProjecao
                {
                    Id = Guid.NewGuid(),
                    ProjecaoId = projecao.Id,
                    Nome = x.Nome.Trim(),
                    ValorMensal = x.ValorMensal
                })
                .ToList();

            return projecao;
        }

        private Projecao MontarEntidadeEdicao(string usuarioId, Guid projecaoId, EditarProjecaoDTO dto, DateTime dataCadastro)
        {
            var projecao = _mapper.Map<Projecao>(dto);
            projecao.Id = projecaoId;
            projecao.UsuarioId = usuarioId;
            projecao.DataCadastro = dataCadastro;
            projecao.DataAtualizacao = DateTime.UtcNow;
            projecao.DataInicial = dto.DataInicial?.Date
                ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            projecao.MesesLimite = dto.MesesLimite <= 0 ? 60 : dto.MesesLimite;
            projecao.Rendas = dto.Rendas
                .Select(x => new RendaProjecao
                {
                    Id = Guid.NewGuid(),
                    ProjecaoId = projecaoId,
                    Nome = x.Nome.Trim(),
                    ValorMensal = x.ValorMensal
                })
                .ToList();

            return projecao;
        }

        private RetornoGenerico? ValidarProjecao(Projecao projecao)
        {
            if (string.IsNullOrWhiteSpace(projecao.Nome))
            {
                return CriarRetornoValidacao("A projeção precisa ter um nome.", "Informe o nome da projeção.");
            }

            if (projecao.ValorObjetivo <= decimal.Zero)
            {
                return CriarRetornoValidacao("O valor objetivo precisa ser maior que zero.", "Informe um objetivo maior que zero.");
            }

            if (!projecao.Rendas.Any() || projecao.Rendas.All(x => x.ValorMensal <= decimal.Zero))
            {
                return CriarRetornoValidacao("Nenhuma renda válida foi informada.", "Informe pelo menos uma renda maior que zero.");
            }

            if (projecao.Rendas.Any(x => string.IsNullOrWhiteSpace(x.Nome)))
            {
                return CriarRetornoValidacao("Existe uma renda sem nome.", "Preencha o nome de todas as rendas.");
            }

            return null;
        }

        private bool PodeCalcular(Projecao projecao)
        {
            return projecao.ValorObjetivo > decimal.Zero
                && projecao.Rendas.Any()
                && projecao.Rendas.Any(x => x.ValorMensal > decimal.Zero)
                && !string.IsNullOrWhiteSpace(projecao.Nome);
        }

        private ResultadoProjecaoDTO CalcularResultado(Projecao projecao, List<Lancamento> lancamentos)
        {
            var rendaManualTotal = projecao.Rendas
                .Where(x => x.ValorMensal > decimal.Zero)
                .Sum(x => x.ValorMensal);

            var dataBase = new DateTime(projecao.DataInicial.Year, projecao.DataInicial.Month, 1);
            var mesesLimite = projecao.MesesLimite <= 0 ? 60 : projecao.MesesLimite;

            var lancamentosProjetaveis = lancamentos
                .Where(x =>
                    x.DataPagamento.Date >= dataBase &&
                    (x.Tipo == EnumTipoLancamento.Despesa || x.Tipo == EnumTipoLancamento.Receita))
                .ToList();

            var lancamentosPorMes = lancamentosProjetaveis
                .GroupBy(x => new { x.DataPagamento.Year, x.DataPagamento.Month })
                .ToDictionary(
                    grupo => $"{grupo.Key.Year:D4}-{grupo.Key.Month:D2}",
                    grupo => new
                    {
                        Receitas = grupo
                            .Where(x => x.Tipo == EnumTipoLancamento.Receita)
                            .Sum(x => x.Valor),
                        Despesas = grupo
                            .Where(x => x.Tipo == EnumTipoLancamento.Despesa)
                            .Sum(x => x.Valor)
                    });

            var resultado = new ResultadoProjecaoDTO
            {
                RendaManualTotal = rendaManualTotal,
                ValorAcumuladoInicial = projecao.ValorAcumuladoInicial,
                ValorObjetivo = projecao.ValorObjetivo
            };

            decimal acumuladoAtual = projecao.ValorAcumuladoInicial;

            if (acumuladoAtual >= projecao.ValorObjetivo)
            {
                resultado.ObjetivoAlcancado = true;
                resultado.MesObjetivo = $"{dataBase.Year:D4}-{dataBase.Month:D2}";
                resultado.QuantidadeMesesParaObjetivo = 0;
                resultado.ValorRestanteParaObjetivo = decimal.Zero;
                return resultado;
            }

            for (var indiceMes = 0; indiceMes < mesesLimite; indiceMes++)
            {
                var mesAtual = dataBase.AddMonths(indiceMes);
                var chaveMes = $"{mesAtual.Year:D4}-{mesAtual.Month:D2}";

                var dadosDoMes = lancamentosPorMes.TryGetValue(chaveMes, out var valorDoMes)
                    ? valorDoMes
                    : null;

                var receitasDosLancamentos = dadosDoMes?.Receitas ?? decimal.Zero;
                var dividasTotais = dadosDoMes?.Despesas ?? decimal.Zero;
                var receitaTotalMes = rendaManualTotal + receitasDosLancamentos;
                var sobraDoMes = receitaTotalMes - dividasTotais;

                acumuladoAtual += sobraDoMes;

                var linha = new LinhaResultadoProjecaoDTO
                {
                    MesReferencia = chaveMes,
                    DividasTotais = dividasTotais,
                    ReceitasDosLancamentos = receitasDosLancamentos,
                    RendaManualTotal = rendaManualTotal,
                    ReceitaTotalMes = receitaTotalMes,
                    SobraDoMes = sobraDoMes,
                    AcumuladoProjetado = acumuladoAtual,
                    ObjetivoAtingidoNoMes = acumuladoAtual >= projecao.ValorObjetivo
                };

                resultado.Linhas.Add(linha);

                if (linha.ObjetivoAtingidoNoMes)
                {
                    resultado.ObjetivoAlcancado = true;
                    resultado.MesObjetivo = chaveMes;
                    resultado.QuantidadeMesesParaObjetivo = indiceMes + 1;
                    break;
                }
            }

            resultado.ValorRestanteParaObjetivo = resultado.ObjetivoAlcancado
                ? decimal.Zero
                : Math.Max(decimal.Zero, projecao.ValorObjetivo - acumuladoAtual);

            return resultado;
        }

        private RetornoGenerico CriarRetornoValidacao(string mensagemSistema, string mensagemUsuario)
        {
            return new RetornoGenerico
            {
                Sucesso = false,
                HttpStatusCode = HttpStatusCode.BadRequest,
                MensagemSistema = mensagemSistema,
                MensagemUsuario = mensagemUsuario,
                Dados = null
            };
        }

        private RetornoGenerico CriarErro(Exception ex, string mensagemUsuario)
        {
            return new RetornoGenerico
            {
                Sucesso = false,
                HttpStatusCode = HttpStatusCode.InternalServerError,
                MensagemSistema = $"{ex}",
                MensagemUsuario = mensagemUsuario,
                Dados = null
            };
        }
    }
}
