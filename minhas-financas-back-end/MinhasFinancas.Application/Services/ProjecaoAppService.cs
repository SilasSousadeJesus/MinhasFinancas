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
                            AtreladaADespesas = x.AtreladaADespesas,
                            QuantidadeRendas = x.Rendas.Count,
                            RendaManualTotal = SomarRendaBase(x),
                            ResultadoAtual = resultado
                        };
                    })
                    .ToList();

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = $"{lista.Count} projecao(oes) encontrada(s)",
                    MensagemUsuario = $"{lista.Count} projecao(oes) encontrada(s)",
                    Dados = lista
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel listar as projecoes.");
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
                    return CriarNaoEncontrado();
                }

                var detalhe = new DetalheProjecaoDTO
                {
                    Id = projecao.Id,
                    Nome = projecao.Nome,
                    DataInicial = projecao.DataInicial,
                    ValorAcumuladoInicial = projecao.ValorAcumuladoInicial,
                    ValorObjetivo = projecao.ValorObjetivo,
                    MesesLimite = projecao.MesesLimite,
                    AtreladaADespesas = projecao.AtreladaADespesas,
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
                        .ToList(),
                    DividasManuaisMensais = projecao.DividasManuaisMensais
                        .OrderBy(x => x.MesReferencia)
                        .Select(x => new DividaManualMensalProjecaoDTO
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
                    MensagemSistema = "Projecao encontrada.",
                    MensagemUsuario = "Projecao encontrada.",
                    Dados = detalhe
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel buscar a projecao.");
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
                    MensagemSistema = "Projecao cadastrada com sucesso.",
                    MensagemUsuario = "Projecao cadastrada com sucesso.",
                    Dados = projecao.Id
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel cadastrar a projecao.");
            }
        }

        public async Task<RetornoGenerico> EditarAsync(string usuarioId, Guid projecaoId, EditarProjecaoDTO projecaoDTO)
        {
            try
            {
                var busca = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (busca == null)
                {
                    return CriarNaoEncontrado();
                }

                var projecao = MontarEntidadeEdicao(usuarioId, projecaoId, projecaoDTO, busca.DataCadastro);
                await _projecaoRepository.EditarElementoAsync(projecao);

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = "Projecao atualizada com sucesso.",
                    MensagemUsuario = "Projecao atualizada com sucesso.",
                    Dados = null
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel editar a projecao.");
            }
        }

        public async Task<RetornoGenerico> DeletarAsync(string usuarioId, Guid projecaoId)
        {
            try
            {
                var projecao = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (projecao == null)
                {
                    return CriarNaoEncontrado();
                }

                await _projecaoRepository.DeletarElementoAsync(projecao);

                return new RetornoGenerico
                {
                    Sucesso = true,
                    HttpStatusCode = HttpStatusCode.OK,
                    MensagemSistema = "Projecao excluida com sucesso.",
                    MensagemUsuario = "Projecao excluida com sucesso.",
                    Dados = null
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel excluir a projecao.");
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

                var projecaoId = Guid.NewGuid();
                var projecao = new Projecao
                {
                    Id = projecaoId,
                    UsuarioId = usuarioId,
                    DataInicial = calcularProjecaoDTO.DataInicial?.Date
                        ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                    ValorAcumuladoInicial = calcularProjecaoDTO.ValorAcumuladoInicial,
                    ValorObjetivo = calcularProjecaoDTO.ValorObjetivo,
                    MesesLimite = calcularProjecaoDTO.MesesLimite <= 0 ? 60 : calcularProjecaoDTO.MesesLimite,
                    AtreladaADespesas = calcularProjecaoDTO.AtreladaADespesas,
                    Rendas = calcularProjecaoDTO.Rendas
                        .Select(x => new RendaProjecao
                        {
                            Nome = x.Nome,
                            ValorMensal = x.ValorMensal
                        })
                        .ToList(),
                    RendasExtrasMensais = MapearRendasExtrasMensais(projecaoId, calcularProjecaoDTO.RendasExtrasMensais),
                    DividasManuaisMensais = MapearDividasManuaisMensais(projecaoId, calcularProjecaoDTO.DividasManuaisMensais)
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
                    MensagemSistema = "Projecao calculada com sucesso.",
                    MensagemUsuario = "Projecao calculada.",
                    Dados = resultado
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel calcular a projecao.");
            }
        }

        public async Task<RetornoGenerico> CalcularAsync(string usuarioId, Guid projecaoId)
        {
            try
            {
                var projecao = await _projecaoRepository.BuscarUmElementoAsync(usuarioId, projecaoId);
                if (projecao == null)
                {
                    return CriarNaoEncontrado();
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
                    MensagemSistema = "Projecao calculada com sucesso.",
                    MensagemUsuario = "Projecao calculada.",
                    Dados = resultado
                };
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Nao foi possivel calcular a projecao.");
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
            projecao.AtreladaADespesas = dto.AtreladaADespesas;
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
            projecao.DividasManuaisMensais = MapearDividasManuaisMensais(projecao.Id, dto.DividasManuaisMensais);

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
            projecao.AtreladaADespesas = dto.AtreladaADespesas;
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
            projecao.DividasManuaisMensais = MapearDividasManuaisMensais(projecaoId, dto.DividasManuaisMensais);

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

        private List<DividaManualProjecaoMensal> MapearDividasManuaisMensais(
            Guid projecaoId,
            List<DividaManualMensalProjecaoDTO>? dividasManuaisMensais)
        {
            return (dividasManuaisMensais ?? new List<DividaManualMensalProjecaoDTO>())
                .Where(x => !string.IsNullOrWhiteSpace(x.MesReferencia) && x.Valor >= decimal.Zero)
                .Select(x => new DividaManualProjecaoMensal
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
                return CriarRetornoValidacao("A projecao precisa ter um nome.", "Informe o nome da projecao.");
            }

            if (projecao.ValorObjetivo <= decimal.Zero)
            {
                return CriarRetornoValidacao("O valor objetivo precisa ser maior que zero.", "Informe um objetivo maior que zero.");
            }

            if (SomarRendaBase(projecao) <= decimal.Zero)
            {
                return CriarRetornoValidacao("Nenhuma renda valida foi informada.", "Informe pelo menos uma renda base maior que zero.");
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
                && SomarRendaBase(projecao) > decimal.Zero
                && !string.IsNullOrWhiteSpace(projecao.Nome);
        }

        private decimal SomarRendaBase(Projecao projecao)
        {
            return projecao.Rendas
                .Where(x => x.ValorMensal > decimal.Zero)
                .Sum(x => x.ValorMensal);
        }

        private ResultadoProjecaoDTO CalcularResultado(Projecao projecao, List<Lancamento> lancamentos)
        {
            var rendaManualTotal = SomarRendaBase(projecao);
            var dataBase = new DateTime(projecao.DataInicial.Year, projecao.DataInicial.Month, 1);
            var mesesLimite = projecao.MesesLimite <= 0 ? 60 : projecao.MesesLimite;

            var rendasExtrasPorMes = projecao.RendasExtrasMensais
                .GroupBy(x => x.MesReferencia.ToString("yyyy-MM"))
                .ToDictionary(grupo => grupo.Key, grupo => grupo.Sum(x => x.Valor));

            var dividasManuaisPorMes = projecao.DividasManuaisMensais
                .GroupBy(x => x.MesReferencia.ToString("yyyy-MM"))
                .ToDictionary(grupo => grupo.Key, grupo => grupo.Sum(x => x.Valor));

            var lancamentosPorMes = lancamentos
                .Where(x =>
                    x.DataVencimento.Date >= dataBase &&
                    x.Tipo == EnumTipoLancamento.Despesa)
                .GroupBy(x => new { x.DataVencimento.Year, x.DataVencimento.Month })
                .ToDictionary(
                    grupo => $"{grupo.Key.Year:D4}-{grupo.Key.Month:D2}",
                    grupo => grupo.Sum(x => x.Valor));

            var resultado = new ResultadoProjecaoDTO
            {
                RendaManualTotal = rendaManualTotal,
                ValorAcumuladoInicial = projecao.ValorAcumuladoInicial,
                ValorObjetivo = projecao.ValorObjetivo,
                ValorRestanteParaObjetivo = Math.Max(decimal.Zero, projecao.ValorObjetivo - projecao.ValorAcumuladoInicial),
                PercentualConcluido = projecao.ValorObjetivo <= decimal.Zero
                    ? decimal.Zero
                    : Math.Min(100, Math.Max(0, (projecao.ValorAcumuladoInicial / projecao.ValorObjetivo) * 100))
            };

            decimal acumuladoAtual = projecao.ValorAcumuladoInicial;

            if (acumuladoAtual >= projecao.ValorObjetivo)
            {
                resultado.ObjetivoAlcancado = true;
                resultado.MesObjetivo = $"{dataBase.Year:D4}-{dataBase.Month:D2}";
                resultado.QuantidadeMesesParaObjetivo = 0;
                return resultado;
            }

            for (var indiceMes = 0; indiceMes < mesesLimite; indiceMes++)
            {
                var mesAtual = dataBase.AddMonths(indiceMes);
                var chaveMes = $"{mesAtual.Year:D4}-{mesAtual.Month:D2}";

                var rendaExtraMensal = rendasExtrasPorMes.TryGetValue(chaveMes, out var rendaExtra)
                    ? rendaExtra
                    : decimal.Zero;

                var dividasTotais = projecao.AtreladaADespesas
                    ? (lancamentosPorMes.TryGetValue(chaveMes, out var valorDoMes) ? valorDoMes : decimal.Zero)
                    : (dividasManuaisPorMes.TryGetValue(chaveMes, out var valorManual) ? valorManual : decimal.Zero);

                var receitaTotalMes = rendaManualTotal + rendaExtraMensal;
                var sobraDoMes = receitaTotalMes - dividasTotais;

                acumuladoAtual += sobraDoMes;

                var linha = new LinhaResultadoProjecaoDTO
                {
                    MesReferencia = chaveMes,
                    DividasTotais = dividasTotais,
                    DividasEditaveis = !projecao.AtreladaADespesas,
                    RendaExtraMensal = rendaExtraMensal,
                    RendaManualTotal = rendaManualTotal,
                    ReceitaTotalMes = receitaTotalMes,
                    SobraDoMes = sobraDoMes,
                    AcumuladoProjetado = acumuladoAtual,
                    ObjetivoAtingidoNoMes = acumuladoAtual >= projecao.ValorObjetivo
                };

                resultado.Linhas.Add(linha);

                if (linha.ObjetivoAtingidoNoMes && !resultado.ObjetivoAlcancado)
                {
                    resultado.ObjetivoAlcancado = true;
                    resultado.MesObjetivo = chaveMes;
                    resultado.QuantidadeMesesParaObjetivo = indiceMes + 1;
                }
            }

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

        private RetornoGenerico CriarNaoEncontrado()
        {
            return new RetornoGenerico
            {
                Sucesso = false,
                HttpStatusCode = HttpStatusCode.NotFound,
                MensagemSistema = "Projecao nao encontrada.",
                MensagemUsuario = "Projecao nao encontrada.",
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
