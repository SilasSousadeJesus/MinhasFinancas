using AutoMapper;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MinhasFinancas.Application.Services
{
    public class LancamentoAppService : ILancamentoAppService
    {

        private readonly IMapper _mapper;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IContaAppService _contaAppService;
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IBemPatrimonialAppService  _bemPatrimonialAppService;
        public LancamentoAppService(IMapper mapper, ILancamentoRepository lancamentoRepository, IUsuarioAppService usuarioAppService, IContaAppService contaAppService, IBemPatrimonialAppService bemPatrimonialAppService)
        {
            _mapper = mapper;
            _lancamentoRepository = lancamentoRepository;
            _usuarioAppService = usuarioAppService;
            _contaAppService = contaAppService;
            _bemPatrimonialAppService = bemPatrimonialAppService;   
        }

        public async Task<RetornoGenerico> BuscarTodosOsElementosAsync(string id)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorusuario = await _usuarioAppService.BuscarUmUsuario(id);

                if (!buscaPorusuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorusuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorusuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorusuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var lista = await _lancamentoRepository.BuscarTodosOsElementosAsync(id);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = $"{lista.Count} elemento(s) encontrado(s)";
                retorno.MensagemUsuario = $"{lista.Count} elemento(s)  encontrado(s)";
                retorno.Dados = lista;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel buscar a lista de lançamentos";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarTodosOsElementosAsync(string id, FiltroListagemLancamentoDTO filtro)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorusuario = await _usuarioAppService.BuscarUmUsuario(id);

                if (!buscaPorusuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorusuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorusuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorusuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var lista = await _lancamentoRepository.BuscarTodosOsElementosAsync(id);
                var query = lista.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filtro.BuscaDescricao))
                {
                    var busca = filtro.BuscaDescricao.Trim().ToLower();
                    query = query.Where(x => x.Descricao.ToLower().Contains(busca));
                }

                if (filtro.Tipo.HasValue)
                {
                    query = query.Where(x => (int)x.Tipo == filtro.Tipo.Value);
                }

                if (filtro.CategoriaId.HasValue)
                {
                    query = query.Where(x => x.CategoriaId == filtro.CategoriaId.Value);
                }

                if (filtro.ContaId.HasValue)
                {
                    query = query.Where(x => x.ContaId == filtro.ContaId.Value);
                }

                if (filtro.CartaoId.HasValue)
                {
                    query = query.Where(x => x.CartaoId == filtro.CartaoId.Value);
                }

                if (filtro.StatusLancamento.HasValue)
                {
                    query = query.Where(x => (int)x.StatusLancamento == filtro.StatusLancamento.Value);
                }

                if (filtro.DataInicialLancamento.HasValue)
                {
                    var dataInicial = filtro.DataInicialLancamento.Value.Date;
                    query = query.Where(x => x.DataLancamento.Date >= dataInicial);
                }

                if (filtro.DataFinalLancamento.HasValue)
                {
                    var dataFinal = filtro.DataFinalLancamento.Value.Date;
                    query = query.Where(x => x.DataLancamento.Date <= dataFinal);
                }

                if (filtro.DataInicialVencimento.HasValue)
                {
                    var dataInicialVencimento = filtro.DataInicialVencimento.Value.Date;
                    query = query.Where(x => x.DataVencimento.Date >= dataInicialVencimento);
                }

                if (filtro.DataFinalVencimento.HasValue)
                {
                    var dataFinalVencimento = filtro.DataFinalVencimento.Value.Date;
                    query = query.Where(x => x.DataVencimento.Date <= dataFinalVencimento);
                }

                if (filtro.DataInicialEfetivacao.HasValue)
                {
                    var dataInicialEfetivacao = filtro.DataInicialEfetivacao.Value.Date;
                    query = query.Where(x => x.DataEfetivacao.HasValue && x.DataEfetivacao.Value.Date >= dataInicialEfetivacao);
                }

                if (filtro.DataFinalEfetivacao.HasValue)
                {
                    var dataFinalEfetivacao = filtro.DataFinalEfetivacao.Value.Date;
                    query = query.Where(x => x.DataEfetivacao.HasValue && x.DataEfetivacao.Value.Date <= dataFinalEfetivacao);
                }

                var ordenarPor = filtro.OrdenarPor.Trim().ToLower();
                var direcao = filtro.Direcao.Trim().ToLower();
                var asc = direcao == "asc";

                query = ordenarPor switch
                {
                    "valor" => asc ? query.OrderBy(x => x.Valor) : query.OrderByDescending(x => x.Valor),
                    _ => asc ? query.OrderBy(x => x.DataVencimento) : query.OrderByDescending(x => x.DataVencimento),
                };

                var pagina = filtro.Pagina < 1 ? 1 : filtro.Pagina;
                var tamanhoPagina = filtro.TamanhoPagina < 1 ? 10 : filtro.TamanhoPagina;
                var totalItens = query.Count();
                var totalPaginas = totalItens == 0 ? 1 : (int)Math.Ceiling(totalItens / (double)tamanhoPagina);
                var itens = query.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToList();

                var resultado = new ResultadoPaginadoDTO<Lancamento>
                {
                    Itens = itens,
                    PaginaAtual = pagina,
                    TamanhoPagina = tamanhoPagina,
                    TotalItens = totalItens,
                    TotalPaginas = totalPaginas,
                };

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = $"{totalItens} elemento(s) encontrado(s)";
                retorno.MensagemUsuario = $"{totalItens} elemento(s) encontrado(s)";
                retorno.Dados = resultado;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel buscar a lista de lançamentos";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarFluxoCaixaSimplesAsync(string usuarioId, int ano, int mes)
        {
            var retorno = new RetornoGenerico();

            try
            {
                if (mes < 1 || mes > 12)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.BadRequest;
                    retorno.MensagemSistema = "Mês informado fora do intervalo permitido.";
                    retorno.MensagemUsuario = "Informe um mes valido.";
                    retorno.Dados = null;
                    return retorno;
                }

                var buscaPorUsuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);

                if (!buscaPorUsuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorUsuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorUsuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorUsuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var dataInicial = new DateTime(ano, mes, 1);
                var dataFinal = dataInicial.AddMonths(1).AddDays(-1);

                var lancamentosDoMes = await _lancamentoRepository.BuscarPorPeriodoVencimentoAsync(
                    usuarioId,
                    dataInicial,
                    dataFinal);

                var lancamentosValidos = lancamentosDoMes
                    .Where(x => x.StatusLancamento != EnumStatusLancamento.Cancelado)
                    .ToList();

                var receitas = lancamentosValidos
                    .Where(x => x.Tipo == EnumTipoLancamento.Receita)
                    .OrderBy(x => x.DataVencimento)
                    .ThenBy(x => x.Descricao)
                    .Select(MapearItemFluxoCaixaSimples)
                    .ToList();

                var despesas = lancamentosValidos
                    .Where(x => x.Tipo == EnumTipoLancamento.Despesa)
                    .OrderBy(x => x.DataVencimento)
                    .ThenBy(x => x.Descricao)
                    .Select(MapearItemFluxoCaixaSimples)
                    .ToList();

                var resultado = new FluxoCaixaSimplesDTO
                {
                    Ano = ano,
                    Mes = mes,
                    ReceitasTotal = receitas.Sum(x => x.Valor),
                    DespesasTotal = despesas.Sum(x => x.Valor),
                    SaldoMes = receitas.Sum(x => x.Valor) - despesas.Sum(x => x.Valor),
                    Receitas = receitas,
                    Despesas = despesas,
                };

                var referencia = dataInicial.ToString("MMMM yyyy", new CultureInfo("pt-BR"));

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = $"Fluxo de caixa simples carregado para {referencia}.";
                retorno.MensagemUsuario = $"Fluxo de caixa simples carregado para {referencia}.";
                retorno.Dados = resultado;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível carregar o fluxo de caixa simples.";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid lancamentoId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var lancamento = await _lancamentoRepository.BuscarUmElementoAsync(usuarioId, lancamentoId);

                retorno.Sucesso = lancamento != null ? true : false;
                retorno.HttpStatusCode = lancamento != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = lancamento != null ? "Lançamento encontrado" : "Lançamento não encontrado";
                retorno.MensagemUsuario = lancamento != null ? "Lançamento encontrado" : "Lançamento não encontrado";
                retorno.Dados = lancamento;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel encontrar a lancamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> CadastrarElementoAsync(CadastrarLancamentoDTO elementoDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorUsuario = await _usuarioAppService.BuscarUmUsuario(elementoDTO.UsuarioId);

                if (!buscaPorUsuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorUsuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorUsuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorUsuario.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                var lancamentos = GerarLancamentosProgramados(elementoDTO);

                foreach (var lancamento in lancamentos)
                {
                    await AplicarImpactosFinanceirosSeNecessarioAsync(lancamento);
                }

                if (lancamentos.Count == 1)
                {
                    await _lancamentoRepository.CadastrarElementoAsync(lancamentos[0]);
                }
                else
                {
                    await _lancamentoRepository.CadastrarElementosAsync(lancamentos);
                }

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = lancamentos.Count == 1
                    ? "Lançamento cadastrado com sucesso"
                    : $"{lancamentos.Count} lançamentos cadastrados com sucesso";
                retorno.MensagemUsuario = lancamentos.Count == 1
                    ? "Lançamento cadastrado"
                    : $"{lancamentos.Count} lançamentos cadastrados";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel criar o Lançamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        private List<Lancamento> GerarLancamentosProgramados(CadastrarLancamentoDTO elementoDTO)
        {
            ValidarRecorrencia(elementoDTO);
            PrepararCadastroInicial(elementoDTO);

            return elementoDTO.FrequenciaLancamento switch
            {
                EnumTipoFrequenciaLancamento.Parcelado => GerarLancamentosParcelados(elementoDTO),
                EnumTipoFrequenciaLancamento.Fixo => GerarLancamentosFixos(elementoDTO),
                EnumTipoFrequenciaLancamento.DiaUtil => GerarLancamentosDiaUtil(elementoDTO),
                _ => new List<Lancamento> { CriarLancamentoBase(elementoDTO) },
            };
        }

        private void ValidarRecorrencia(CadastrarLancamentoDTO elementoDTO)
        {
            if (elementoDTO.FrequenciaLancamento == EnumTipoFrequenciaLancamento.Parcelado
                && (!elementoDTO.QuantidadeParcelas.HasValue || elementoDTO.QuantidadeParcelas.Value <= 1))
            {
                throw new InvalidOperationException("Lançamento parcelado exige quantidade de parcelas maior que 1.");
            }

            if (elementoDTO.FrequenciaLancamento == EnumTipoFrequenciaLancamento.DiaUtil
                && (!elementoDTO.NumeroDiaUtil.HasValue || elementoDTO.NumeroDiaUtil.Value <= 0))
            {
                throw new InvalidOperationException("Lançamento dia útil exige um número de dia útil maior que zero.");
            }
        }

        private List<Lancamento> GerarLancamentosParcelados(CadastrarLancamentoDTO elementoDTO)
        {
            var quantidadeParcelas = elementoDTO.QuantidadeParcelas!.Value;
            var lancamentos = new List<Lancamento>(quantidadeParcelas);
            var grupoParcelamentoId = Guid.NewGuid();
            var valorBase = Math.Round(elementoDTO.Valor / quantidadeParcelas, 2, MidpointRounding.AwayFromZero);
            var acumulado = 0m;

            for (var parcela = 1; parcela <= quantidadeParcelas; parcela++)
            {
                var valorParcela = parcela == quantidadeParcelas
                    ? elementoDTO.Valor - acumulado
                    : valorBase;

                acumulado += valorParcela;

                var lancamento = CriarLancamentoBase(
                    elementoDTO,
                    parcela - 1,
                    $"{elementoDTO.Descricao} {parcela}/{quantidadeParcelas}",
                    valorParcela);

                lancamento.GrupoParcelamentoId = grupoParcelamentoId;
                lancamento.NumeroParcela = parcela;
                lancamento.TotalParcelas = quantidadeParcelas;

                lancamentos.Add(lancamento);
            }

            return lancamentos;
        }

        private List<Lancamento> GerarLancamentosFixos(CadastrarLancamentoDTO elementoDTO)
        {
            return GerarLancamentosMensaisProgramados(
                elementoDTO,
                12,
                null,
                EnumTipoProgramacaoLancamento.Fixo,
                (_, lancamento) => lancamento);
        }

        private List<Lancamento> GerarLancamentosDiaUtil(CadastrarLancamentoDTO elementoDTO)
        {
            return GerarLancamentosMensaisProgramados(
                elementoDTO,
                12,
                null,
                EnumTipoProgramacaoLancamento.DiaUtil,
                (_, lancamento) =>
                {
                    var numeroDiaUtil = elementoDTO.NumeroDiaUtil!.Value;
                    lancamento.DataVencimento = CalcularDataDiaUtil(lancamento.DataVencimento, numeroDiaUtil);
                    lancamento.NumeroDiaUtil = numeroDiaUtil;
                    return lancamento;
                });
        }

        private List<Lancamento> GerarLancamentosMensaisProgramados(
            CadastrarLancamentoDTO elementoDTO,
            int quantidadeMeses,
            decimal? valor,
            EnumTipoProgramacaoLancamento tipoProgramacao,
            Func<int, Lancamento, Lancamento> configurarLancamento)
        {
            var lancamentos = new List<Lancamento>(quantidadeMeses);
            var grupoLancamentoProgramadoId = Guid.NewGuid();

            for (var indice = 0; indice < quantidadeMeses; indice++)
            {
                var lancamento = CriarLancamentoBase(elementoDTO, indice, valor: valor);
                lancamento.GrupoLancamentoProgramadoId = grupoLancamentoProgramadoId;
                lancamento.TipoProgramacao = tipoProgramacao;
                lancamentos.Add(configurarLancamento(indice, lancamento));
            }

            return lancamentos;
        }

        private static DateTime CalcularDataDiaUtil(DateTime dataBase, int numeroDiaUtil)
        {
            var dataAtual = new DateTime(dataBase.Year, dataBase.Month, 1);
            var contadorDiaUtil = 0;

            while (dataAtual.Month == dataBase.Month)
            {
                if (dataAtual.DayOfWeek != DayOfWeek.Saturday && dataAtual.DayOfWeek != DayOfWeek.Sunday)
                {
                    contadorDiaUtil++;

                    if (contadorDiaUtil == numeroDiaUtil)
                    {
                        return dataAtual;
                    }
                }

                dataAtual = dataAtual.AddDays(1);
            }

            throw new InvalidOperationException(
                $"Não foi possível localizar o {numeroDiaUtil}º dia útil em {dataBase:MM/yyyy}.");
        }

        private Lancamento CriarLancamentoBase(
            CadastrarLancamentoDTO elementoDTO,
            int mesesParaAdicionar = 0,
            string? descricao = null,
            decimal? valor = null)
        {
            var lancamento = _mapper.Map<Lancamento>(elementoDTO);
            lancamento.Id = Guid.NewGuid();
            lancamento.Descricao = descricao ?? elementoDTO.Descricao;
            lancamento.Valor = valor ?? elementoDTO.Valor;
            lancamento.DataVencimento = elementoDTO.DataVencimento.AddMonths(mesesParaAdicionar);
            lancamento.DataLancamento = elementoDTO.DataLancamento;

            return lancamento;
        }

        private static void PrepararCadastroInicial(CadastrarLancamentoDTO elementoDTO)
        {
            elementoDTO.StatusLancamento = EnumStatusLancamento.Pendente;
            elementoDTO.DataEfetivacao = null;
        }

        private static void ValidarStatusLancamento(Lancamento lancamento)
        {
            if (lancamento.StatusLancamento == EnumStatusLancamento.Pendente || lancamento.StatusLancamento == EnumStatusLancamento.Cancelado)
            {
                lancamento.DataEfetivacao = null;
            }

            if ((lancamento.StatusLancamento == EnumStatusLancamento.Pago || lancamento.StatusLancamento == EnumStatusLancamento.Recebido)
                && !lancamento.DataEfetivacao.HasValue)
            {
                throw new InvalidOperationException("Lancamentos efetivados exigem DataEfetivacao preenchida.");
            }

            if (EhMovimentoDeEntrada(lancamento.Tipo) && lancamento.StatusLancamento == EnumStatusLancamento.Pago)
            {
                throw new InvalidOperationException("Movimentos de entrada nao podem usar o status Pago.");
            }

            if (EhMovimentoDeSaida(lancamento.Tipo) && lancamento.StatusLancamento == EnumStatusLancamento.Recebido)
            {
                throw new InvalidOperationException("Movimentos de saida nao podem usar o status Recebido.");
            }
        }

        private static EnumStatusLancamento DeterminarStatusDeEfetivacao(Lancamento lancamento)
        {
            if (lancamento.StatusLancamento == EnumStatusLancamento.Cancelado)
            {
                throw new InvalidOperationException("Lancamentos cancelados nao podem ser efetivados.");
            }

            if (lancamento.StatusLancamento == EnumStatusLancamento.Pago || lancamento.StatusLancamento == EnumStatusLancamento.Recebido)
            {
                throw new InvalidOperationException("Este lancamento ja foi efetivado.");
            }

            return lancamento.Tipo switch
            {
                EnumTipoLancamento.Despesa => EnumStatusLancamento.Pago,
                EnumTipoLancamento.Receita => EnumStatusLancamento.Recebido,
                _ => throw new InvalidOperationException("A efetivacao rapida esta disponivel apenas para receitas e despesas."),
            };
        }

        private static bool EhMovimentoDeEntrada(EnumTipoLancamento tipo)
        {
            return tipo == EnumTipoLancamento.Receita
                || tipo == EnumTipoLancamento.Deposito
                || tipo == EnumTipoLancamento.InvestimentoSaque;
        }

        private static bool EhMovimentoDeSaida(EnumTipoLancamento tipo)
        {
            return tipo == EnumTipoLancamento.Despesa
                || tipo == EnumTipoLancamento.Saque
                || tipo == EnumTipoLancamento.InvestimentoDeposito
                || tipo == EnumTipoLancamento.Transferencia;
        }

        private async Task AplicarImpactosFinanceirosSeNecessarioAsync(Lancamento lancamento)
        {
            if (lancamento.ContaId == null)
            {
                return;
            }

            var buscarContaVinculada = await _contaAppService.BuscarUmElementoAsync(lancamento.UsuarioId, (Guid)lancamento.ContaId);
            List<BemPatrimonial> buscarBensMateriais = _bemPatrimonialAppService.BuscarTodosOsElementosAsync(lancamento.UsuarioId).Result.Dados;
            var investimentos = buscarBensMateriais.FirstOrDefault(x => x.Tipo == EnumBemPatrimonial.Investimento);
            var dinheiroEmConta = buscarBensMateriais.FirstOrDefault(x => x.Tipo == EnumBemPatrimonial.DinheiroEmConta);

            if (buscarContaVinculada?.Dados == null || investimentos == null || dinheiroEmConta == null)
            {
                return;
            }

            PermanenciaBemMaterial permancenciaInvestimento = _bemPatrimonialAppService.BuscarUltimaDataPermanencia(investimentos.Id).Result.Dados;
            PermanenciaBemMaterial permancenciaDinheiroEmConta = _bemPatrimonialAppService.BuscarUltimaDataPermanencia(dinheiroEmConta.Id).Result.Dados;

            var contaDTO = new EditarContaDTO()
            {
                Descricao = buscarContaVinculada.Dados.Descricao,
                Instituicao = buscarContaVinculada.Dados.Instituicao,
                NomeConta = buscarContaVinculada.Dados.NomeConta,
                Saldo = buscarContaVinculada.Dados.Saldo,
                Tipo = buscarContaVinculada.Dados.Tipo,
                SaldoInvestimento = buscarContaVinculada.Dados.SaldoInvestimento
            };

            switch (lancamento.Tipo)
            {
                case EnumTipoLancamento.InvestimentoDeposito:
                    permancenciaInvestimento.Valor += lancamento.Valor;
                    contaDTO.SaldoInvestimento += lancamento.Valor;
                    await _contaAppService.EditarElementoAsync(lancamento.UsuarioId, (Guid)lancamento.ContaId, contaDTO);
                    await _bemPatrimonialAppService.EditarUltimaDataPermanencia(permancenciaInvestimento);
                    break;

                case EnumTipoLancamento.InvestimentoSaque:
                    permancenciaInvestimento.Valor -= lancamento.Valor;
                    contaDTO.SaldoInvestimento -= lancamento.Valor;
                    await _contaAppService.EditarElementoAsync(lancamento.UsuarioId, (Guid)lancamento.ContaId, contaDTO);
                    await _bemPatrimonialAppService.EditarUltimaDataPermanencia(permancenciaInvestimento);
                    break;

                case EnumTipoLancamento.Saque:
                    contaDTO.Saldo -= lancamento.Valor;
                    permancenciaDinheiroEmConta.Valor -= lancamento.Valor;
                    await _contaAppService.EditarElementoAsync(lancamento.UsuarioId, (Guid)lancamento.ContaId, contaDTO);
                    await _bemPatrimonialAppService.EditarUltimaDataPermanencia(permancenciaDinheiroEmConta);
                    break;

                case EnumTipoLancamento.Deposito:
                    contaDTO.Saldo += lancamento.Valor;
                    permancenciaDinheiroEmConta.Valor += lancamento.Valor;
                    await _contaAppService.EditarElementoAsync(lancamento.UsuarioId, (Guid)lancamento.ContaId, contaDTO);
                    await _bemPatrimonialAppService.EditarUltimaDataPermanencia(permancenciaDinheiroEmConta);
                    break;
            }
        }

        public async Task<RetornoGenerico> DeletarElementoAsync(string idPatrono, Guid idElemento)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorLancamento = await BuscarUmElementoAsync(idPatrono, idElemento);

                if (!buscaPorLancamento.Sucesso)
                {
                    retorno.Sucesso = buscaPorLancamento.Sucesso;
                    retorno.HttpStatusCode = buscaPorLancamento.HttpStatusCode;
                    retorno.MensagemSistema = buscaPorLancamento.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorLancamento.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                await _lancamentoRepository.DeletarElementoAsync(buscaPorLancamento.Dados);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Lancamento deletada com sucesso";
                retorno.MensagemUsuario = "Lancamento Deletada";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel deletar o Lancamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarLancamentoDTO elementoDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorBanco = await BuscarUmElementoAsync(idPatrono, elementoId);

                if (!buscaPorBanco.Sucesso)
                {
                    retorno.Sucesso = buscaPorBanco.Sucesso;
                    retorno.HttpStatusCode = buscaPorBanco.HttpStatusCode;
                    retorno.MensagemSistema = buscaPorBanco.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorBanco.MensagemUsuario;
                    retorno.Dados = null;

                    return retorno;
                }

                var lancamento = _mapper.Map<Lancamento>(elementoDTO);
                lancamento.Id = elementoId;
                lancamento.UsuarioId = idPatrono;
                ValidarStatusLancamento(lancamento);

                await _lancamentoRepository.EditarElementoAsync(lancamento);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Lançamento Editado com sucesso";
                retorno.MensagemUsuario = "Lançamento Editado";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel editar o Lançamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> EfetivarLancamentoAsync(string usuarioId, Guid lancamentoId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorLancamento = await BuscarUmElementoAsync(usuarioId, lancamentoId);

                if (!buscaPorLancamento.Sucesso || buscaPorLancamento.Dados is not Lancamento lancamento)
                {
                    retorno.Sucesso = buscaPorLancamento.Sucesso;
                    retorno.HttpStatusCode = buscaPorLancamento.HttpStatusCode;
                    retorno.MensagemSistema = buscaPorLancamento.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorLancamento.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var novoStatus = DeterminarStatusDeEfetivacao(lancamento);

                lancamento.StatusLancamento = novoStatus;
                lancamento.DataEfetivacao = DateTime.Now;

                await _lancamentoRepository.EditarElementoAsync(lancamento);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Lancamento efetivado com sucesso";
                retorno.MensagemUsuario = novoStatus == EnumStatusLancamento.Pago
                    ? "Lancamento marcado como pago"
                    : "Lancamento marcado como recebido";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = ex is InvalidOperationException
                    ? HttpStatusCode.BadRequest
                    : HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = ex is InvalidOperationException
                    ? ex.Message
                    : "Não foi possível efetivar o lançamento";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> BuscarLancamentosPorCategoriaAsync(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var buscaPorusuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);

                if (!buscaPorusuario.Sucesso)
                {
                    retorno.Sucesso = buscaPorusuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = buscaPorusuario.MensagemSistema;
                    retorno.MensagemUsuario = buscaPorusuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var lista = await _lancamentoRepository.BuscarLancamentosPorCategoriaAsync(usuarioId);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = $"{lista.Count} elemento(s) encontrado(s)";
                retorno.MensagemUsuario = $"{lista.Count} elemento(s)  encontrado(s)";
                retorno.Dados = lista;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel buscar a lista de elementos";
                retorno.Dados = null;
                return retorno;
            }
        }

        private static FluxoCaixaSimplesItemDTO MapearItemFluxoCaixaSimples(Lancamento lancamento)
        {
            return new FluxoCaixaSimplesItemDTO
            {
                Id = lancamento.Id,
                Descricao = lancamento.Descricao,
                Categoria = lancamento.Categoria?.NomeCategoria,
                Valor = lancamento.Valor,
                DataVencimento = lancamento.DataVencimento,
            };
        }

    }
}
