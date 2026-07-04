using AutoMapper;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;
using Microsoft.EntityFrameworkCore;

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

                if (filtro.Realizado.HasValue)
                {
                    query = query.Where(x => x.Realizado == filtro.Realizado.Value);
                }

                if (filtro.DataInicial.HasValue)
                {
                    var dataInicial = filtro.DataInicial.Value.Date;
                    query = query.Where(x => x.DataLancamento.Date >= dataInicial);
                }

                if (filtro.DataFinal.HasValue)
                {
                    var dataFinal = filtro.DataFinal.Value.Date;
                    query = query.Where(x => x.DataLancamento.Date <= dataFinal);
                }

                var ordenarPor = filtro.OrdenarPor.Trim().ToLower();
                var direcao = filtro.Direcao.Trim().ToLower();
                var asc = direcao == "asc";

                query = ordenarPor switch
                {
                    "valor" => asc ? query.OrderBy(x => x.Valor) : query.OrderByDescending(x => x.Valor),
                    _ => asc ? query.OrderBy(x => x.DataLancamento) : query.OrderByDescending(x => x.DataLancamento),
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

        public async Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid lancamentoId)
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

                var lancamento = await _lancamentoRepository.BuscarUmElementoAsync(usuarioId, lancamentoId);

                retorno.Sucesso = lancamento != null ? true : false;
                retorno.HttpStatusCode = lancamento != null ? HttpStatusCode.OK : HttpStatusCode.NotFound;
                retorno.MensagemSistema = lancamento != null ? "lancamento Encontrado" : "lancamento não Encontrado";
                retorno.MensagemUsuario = lancamento != null ? "lancamento Encontrado" : "lancamento não Encontrado";
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

                var lancamento = _mapper.Map<Lancamento>(elementoDTO);

                if (lancamento.ContaId != null)
                {
                    var buscarContaVinculada = await _contaAppService.BuscarUmElementoAsync(lancamento.UsuarioId, (Guid)lancamento.ContaId);
                    List<BemPatrimonial> buscarBensMateriais =  _bemPatrimonialAppService.BuscarTodosOsElementosAsync(lancamento.UsuarioId).Result.Dados;
                    var investimentos = buscarBensMateriais.Where(x => x.Tipo == EnumBemPatrimonial.Investimento).FirstOrDefault();
                    var dinheiroEmConta = buscarBensMateriais.Where(x => x.Tipo == EnumBemPatrimonial.DinheiroEmConta).FirstOrDefault();

                    PermanenciaBemMaterial permancenciaInvestimento =  _bemPatrimonialAppService.BuscarUltimaDataPermanencia(investimentos.Id).Result.Dados;
                    PermanenciaBemMaterial permancenciaDinheiroEmConta =  _bemPatrimonialAppService.BuscarUltimaDataPermanencia(dinheiroEmConta.Id).Result.Dados;

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
                            await _contaAppService.EditarElementoAsync(elementoDTO.UsuarioId, (Guid)lancamento.ContaId, contaDTO);                         
                            await _bemPatrimonialAppService.EditarUltimaDataPermanencia(permancenciaInvestimento);
                            break;

                        case EnumTipoLancamento.InvestimentoSaque:
                            permancenciaInvestimento.Valor -= lancamento.Valor;
                            contaDTO.SaldoInvestimento -= lancamento.Valor;
                            await _contaAppService.EditarElementoAsync(elementoDTO.UsuarioId, (Guid)lancamento.ContaId, contaDTO);
                            await _bemPatrimonialAppService.EditarUltimaDataPermanencia(permancenciaInvestimento);
                            break;

                        case EnumTipoLancamento.Saque:
                            contaDTO.Saldo -= lancamento.Valor;
                            permancenciaDinheiroEmConta.Valor -= lancamento.Valor;
                            await _contaAppService.EditarElementoAsync(elementoDTO.UsuarioId, (Guid)lancamento.ContaId, contaDTO);
                            await _bemPatrimonialAppService.EditarUltimaDataPermanencia(permancenciaInvestimento);
                            break;

                        case EnumTipoLancamento.Deposito:
                            contaDTO.Saldo += lancamento.Valor;
                            permancenciaDinheiroEmConta.Valor += lancamento.Valor;
                            await _contaAppService.EditarElementoAsync(elementoDTO.UsuarioId, (Guid)lancamento.ContaId, contaDTO);
                            await _bemPatrimonialAppService.EditarUltimaDataPermanencia(permancenciaInvestimento);
                            break;
                    }
                }

                await _lancamentoRepository.CadastrarElementoAsync(lancamento);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Lançamento cadastrado com sucesso";
                retorno.MensagemUsuario = "Lançamento cadastrado";
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

                var categoria = _mapper.Map<Lancamento>(elementoDTO);
                categoria.Id = elementoId;
                categoria.UsuarioId = idPatrono;

                await _lancamentoRepository.EditarElementoAsync(categoria);

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

    }
}
