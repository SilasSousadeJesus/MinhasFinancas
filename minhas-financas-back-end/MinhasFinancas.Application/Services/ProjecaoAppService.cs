using AutoMapper;
using MinhasFinancas.Application.DTOs.Projecao;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Globalization;
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

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = $"{lista.Count} projeção(ões) encontrada(s)",
                    MensagemUsuario = $"{lista.Count} projeção(ões) encontrada(s)",
                    Dados = lista
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel listar as projeções.");
            }
        }

        public async Task<RetornoGenerico> BuscarUmAsync(string usuarioId, Guid projecaoId)
        {
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
                    return new RetornoGenerico
                    {
                        Sucesso = false,
                        HttpStatusCode = HttpStatusCode.NotFound,
                        MensagemSistema = "Projeção não encontrada.",
                        MensagemUsuario = "Projeção não encontrada.",
                        Dados = null
                    };
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
                        .ToList(),
                    RendasExtrasMensais = projecao.RendasExtrasMensais
                        .OrderBy(x => x.MesReferencia)
                        .Select(x => new RendaExtraMensalProjecaoDTO
                        {
                            MesReferencia = x.MesReferencia.ToString("yyyy-MM"),
                            Valor = x.Valor
                        })
                        .ToList()
                };

                if (PodeCalcular(projecao))
                {
                    var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
                    detalhe.ResultadoAtual = CalcularResultado(projecao, lancamentos);
                }

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = "Projeção encontrada.",
                    MensagemUsuario = "Projeção encontrada.",
                    Dados = detalhe
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel buscar a projeção.");
            }
        }

        public async Task<RetornoGenerico> CadastrarAsync(CadastrarProjecaoDTO projecaoDTO)
        {
            try
            {
                var validacao = await ValidarUsuarioAsync(projecaoDTO.UsuarioId);
                if (validacao != null)
                {
                    return validacao;
                }

                var projecao = MontarEntidadeCadastro(projecaoDTO);
                await _projecaoRepository.CadastrarElementoAsync(projecao);

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = "Projeção cadastrada com sucesso.",
                    MensagemUsuario = "Projeção cadastrada com sucesso.",
                    Dados = projecao.Id
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel cadastrar a projeção.");
            }
        }

        public async Task<RetornoGenerico> EditarAsync(string usuarioId, Guid projecaoId, EditarProjecaoDTO projecaoDTO)
        {
            try
            {
                var busca = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (busca == null)
                {
                    return new RetornoGenerico
                    {
                        Sucesso = false,
                        HttpStatusCode = HttpStatusCode.NotFound,
                        MensagemSistema = "Projeção não encontrada.",
                        MensagemUsuario = "Projeção não encontrada.",
                        Dados = null
                    };
                }

                var projecao = MontarEntidadeEdicao(usuarioId, projecaoId, projecaoDTO, busca.DataCadastro);
                await _projecaoRepository.EditarElementoAsync(projecao);

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = "Projeção atualizada com sucesso.",
                    MensagemUsuario = "Projeção atualizada com sucesso.",
                    Dados = null
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel editar a projeção.");
            }
        }

        public async Task<RetornoGenerico> DeletarAsync(string usuarioId, Guid projecaoId)
        {
            try
            {
                var projecao = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (projecao == null)
                {
                    return new RetornoGenerico
                    {
                        Sucesso = false,
                        HttpStatusCode = HttpStatusCode.NotFound,
                        MensagemSistema = "Projeção não encontrada.",
                        MensagemUsuario = "Projeção não encontrada.",
                        Dados = null
                    };
                }

                await _projecaoRepository.DeletarElementoAsync(projecao);

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = "Projeção excluída com sucesso.",
                    MensagemUsuario = "Projeção excluída com sucesso.",
                    Dados = null
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel excluir a projeção.");
            }
        }

        public async Task<RetornoGenerico> CalcularAsync(string usuarioId, CalcularProjecaoDTO calcularProjecaoDTO)
        {
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
                        .ToList(),
                    RendasExtrasMensais = MapearRendasExtrasMensais(Guid.NewGuid(), calcularProjecaoDTO.RendasExtrasMensais)
                };

                var validacaoCalculo = ValidarProjecao(projecao);
                if (validacaoCalculo != null)
                {
                    return validacaoCalculo;
                }

                var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
                var resultado = CalcularResultado(projecao, lancamentos);

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = "Projeção calculada com sucesso.",
                    MensagemUsuario = "Projeção calculada.",
                    Dados = resultado
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel calcular a projeção.");
            }
        }

        public async Task<RetornoGenerico> CalcularAsync(string usuarioId, Guid projecaoId)
        {
            try
            {
                var projecao = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (projecao == null)
                {
                    return new RetornoGenerico
                    {
                        Sucesso = false,
                        HttpStatusCode = HttpStatusCode.NotFound,
                        MensagemSistema = "Projeção não encontrada.",
                        MensagemUsuario = "Projeção não encontrada.",
                        Dados = null
                    };
                }

                var validacao = ValidarProjecao(projecao);
                if (validacao != null)
                {
                    return validacao;
                }

                var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
                var resultado = CalcularResultado(projecao, lancamentos);

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = "Projeção calculada com sucesso.",
                    MensagemUsuario = "Projeção calculada.",
                    Dados = resultado
                };
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
            projecao.RendasExtrasMensais = MapearRendasExtrasMensais(projecao.Id, dto.RendasExtrasMensais);

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
            projecao.RendasExtrasMensais = MapearRendasExtrasMensais(projecaoId, dto.RendasExtrasMensais);

            return projecao;
        }

        private List<RendaExtraProjecaoMensal> MapearRendasExtrasMensais(
            Guid projecaoId,
            List<RendaExtraMensalProjecaoDTO>? rendasExtrasMensais)
        {
            return (rendasExtrasMensais ?? new List<RendaExtraMensalProjecaoDTO>())
                .Where(x => !string.IsNullOrWhiteSpace(x.MesReferencia) && x.Valor > decimal.Zero)
                .Select(x => new RendaExtraProjecaoMensal
                {
                    Id = Guid.NewGuid(),
                    ProjecaoId = projecaoId,
                    MesReferencia = ParseMesReferencia(x.MesReferencia),
                    Valor = x.Valor
                })
                .ToList();
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

            var rendaBaseTotal = projecao.Rendas
                .Where(x => x.ValorMensal > decimal.Zero)
                .Sum(x => x.ValorMensal);

            if (rendaBaseTotal <= decimal.Zero)
            {
                return CriarRetornoValidacao("Nenhuma renda válida foi informada.", "Informe pelo menos uma renda base maior que zero.");
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
                && projecao.Rendas.Any(x => x.ValorMensal > decimal.Zero)
                && !string.IsNullOrWhiteSpace(projecao.Nome);
        }

        private ResultadoProjecaoDTO CalcularResultado(Projecao projecao, List<Lancamento> lancamentos)
        {
            var rendaManualTotal = projecao.Rendas
                .Where(x => x.ValorMensal > decimal.Zero)
                .Sum(x => x.ValorMensal);

            var rendasExtrasPorMes = projecao.RendasExtrasMensais
                .GroupBy(x => x.MesReferencia.ToString("yyyy-MM"))
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Sum(x => x.Valor));

            var dataBase = new DateTime(projecao.DataInicial.Year, projecao.DataInicial.Month, 1);
            var mesesLimite = projecao.MesesLimite <= 0 ? 60 : projecao.MesesLimite;

            var lancamentosProjetaveis = lancamentos
                .Where(x =>
                    x.DataPagamento.Date >= dataBase &&
                    x.Tipo == EnumTipoLancamento.Despesa)
                .ToList();

            var lancamentosPorMes = lancamentosProjetaveis
                .GroupBy(x => new { x.DataPagamento.Year, x.DataPagamento.Month })
                .ToDictionary(
                    grupo => $"{grupo.Key.Year:D4}-{grupo.Key.Month:D2}",
                    grupo => grupo.Sum(x => x.Valor));

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
                resultado.PercentualConcluido = 100;
                return resultado;
            }

            for (var indiceMes = 0; indiceMes < mesesLimite; indiceMes++)
            {
                var mesAtual = dataBase.AddMonths(indiceMes);
                var chaveMes = $"{mesAtual.Year:D4}-{mesAtual.Month:D2}";

                var rendaExtraMensal = rendasExtrasPorMes.TryGetValue(chaveMes, out var rendaExtra)
                    ? rendaExtra
                    : decimal.Zero;
                var dividasTotais = lancamentosPorMes.TryGetValue(chaveMes, out var valorDoMes)
                    ? valorDoMes
                    : decimal.Zero;
                var receitaTotalMes = rendaManualTotal + rendaExtraMensal;
                var sobraDoMes = receitaTotalMes - dividasTotais;

                acumuladoAtual += sobraDoMes;

                var linha = new LinhaResultadoProjecaoDTO
                {
                    MesReferencia = chaveMes,
                    DividasTotais = dividasTotais,
                    RendaExtraMensal = rendaExtraMensal,
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
            resultado.PercentualConcluido = projecao.ValorObjetivo <= decimal.Zero
                ? decimal.Zero
                : Math.Min(100, Math.Max(0, (acumuladoAtual / projecao.ValorObjetivo) * 100));

            return resultado;
        }

        private DateTime ParseMesReferencia(string mesReferencia)
        {
            if (DateTime.TryParseExact(
                $"{mesReferencia}-01",
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var data))
            {
                return data;
            }

            throw new InvalidOperationException($"Mes de referencia invalido: {mesReferencia}");
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
