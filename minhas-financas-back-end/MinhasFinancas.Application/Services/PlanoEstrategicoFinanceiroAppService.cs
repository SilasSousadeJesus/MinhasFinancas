using System.Net;
using MinhasFinancas.Application.DTOs.PlanoEstrategicoFinanceiro;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class PlanoEstrategicoFinanceiroAppService : IPlanoEstrategicoFinanceiroAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IPlanoEstrategicoFinanceiroRepository _repository;

        public PlanoEstrategicoFinanceiroAppService(
            IUsuarioAppService usuarioAppService,
            IPlanoEstrategicoFinanceiroRepository repository)
        {
            _usuarioAppService = usuarioAppService;
            _repository = repository;
        }

        public async Task<RetornoGenerico> BuscarTodosAsync(string usuarioId)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var planos = await _repository.BuscarTodosOsElementosAsync(usuarioId);
                var dados = planos.Select(MapearResumo).ToList();

                return new RetornoGenerico(true, $"{dados.Count} plano(s) encontrado(s).", $"{dados.Count} plano(s) encontrado(s).", HttpStatusCode.OK, dados);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível listar os planos estratégicos.");
            }
        }

        public async Task<RetornoGenerico> BuscarVigenteAsync(string usuarioId)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var plano = await _repository.BuscarVigenteAsync(usuarioId);
                if (plano == null)
                {
                    return CriarNaoEncontrado("Nenhum plano estratégico vigente foi encontrado.");
                }

                return new RetornoGenerico(true, "Plano estratégico vigente carregado com sucesso.", "Plano estratégico vigente carregado com sucesso.", HttpStatusCode.OK, MapearDetalhe(plano));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível carregar o plano estratégico vigente.");
            }
        }

        public async Task<RetornoGenerico> BuscarUmAsync(string usuarioId, Guid planoId)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var plano = await _repository.BuscarUmElementoAsync(usuarioId, planoId);
                if (plano == null)
                {
                    return CriarNaoEncontrado("Plano estratégico não encontrado.");
                }

                return new RetornoGenerico(true, "Plano estratégico carregado com sucesso.", "Plano estratégico carregado com sucesso.", HttpStatusCode.OK, MapearDetalhe(plano));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível carregar o plano estratégico.");
            }
        }

        public async Task<RetornoGenerico> CadastrarAsync(string usuarioId, SalvarPlanoEstrategicoFinanceiroDTO dto)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var validacaoDto = ValidarDto(dto);
                if (validacaoDto != null)
                {
                    return validacaoDto;
                }

                var agora = DateTime.UtcNow;
                var plano = CriarPlano(usuarioId, dto, agora, 1, Guid.NewGuid());
                plano.PlanoRaizId = plano.Id;

                await _repository.AdicionarAsync(plano);
                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(true, "Plano estratégico criado com sucesso.", "Plano estratégico criado com sucesso.", HttpStatusCode.OK, MapearDetalhe(plano));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível criar o plano estratégico.");
            }
        }

        public async Task<RetornoGenerico> AtualizarVersaoAsync(string usuarioId, Guid planoId, SalvarPlanoEstrategicoFinanceiroDTO dto)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var validacaoDto = ValidarDto(dto);
                if (validacaoDto != null)
                {
                    return validacaoDto;
                }

                var planoAtual = await _repository.BuscarUmElementoGerenciadoAsync(usuarioId, planoId);
                if (planoAtual == null)
                {
                    return CriarNaoEncontrado("Plano estratégico não encontrado.");
                }

                if (!planoAtual.Ativo)
                {
                    return CriarErroNegocio("Somente a versão vigente pode ser atualizada.");
                }

                var agora = DateTime.UtcNow;
                planoAtual.Ativo = false;
                planoAtual.DataFimVigencia = agora;
                planoAtual.DataAtualizacao = agora;

                var novaVersao = CriarPlano(usuarioId, dto, agora, planoAtual.NumeroVersao + 1, planoAtual.PlanoRaizId);
                await _repository.AdicionarAsync(novaVersao);
                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(true, "Nova versão do plano estratégico criada com sucesso.", "Nova versão do plano estratégico criada com sucesso.", HttpStatusCode.OK, MapearDetalhe(novaVersao));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível atualizar o plano estratégico.");
            }
        }

        public async Task<RetornoGenerico> InativarAsync(string usuarioId, Guid planoId)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var plano = await _repository.BuscarUmElementoGerenciadoAsync(usuarioId, planoId);
                if (plano == null)
                {
                    return CriarNaoEncontrado("Plano estratégico não encontrado.");
                }

                plano.Ativo = false;
                plano.DataFimVigencia = DateTime.UtcNow;
                plano.DataAtualizacao = DateTime.UtcNow;

                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(true, "Plano estratégico inativado com sucesso.", "Plano estratégico inativado com sucesso.", HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível inativar o plano estratégico.");
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
                Sucesso = false,
                HttpStatusCode = HttpStatusCode.NotFound,
                MensagemSistema = buscaPorUsuario.MensagemSistema,
                MensagemUsuario = buscaPorUsuario.MensagemUsuario,
                Dados = null
            };
        }

        private static RetornoGenerico? ValidarDto(SalvarPlanoEstrategicoFinanceiroDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return CriarErroNegocio("O plano estratégico precisa ter um nome.");
            }

            if (dto.Objetivos == null || dto.Objetivos.Count == 0)
            {
                return CriarErroNegocio("Informe pelo menos um objetivo estratégico.");
            }

            if (dto.Objetivos.Any(objetivo => string.IsNullOrWhiteSpace(objetivo.Titulo)))
            {
                return CriarErroNegocio("Todos os objetivos precisam ter um título.");
            }

            if (dto.Objetivos.Any(objetivo => objetivo.Ordem <= 0))
            {
                return CriarErroNegocio("A ordem dos objetivos precisa ser maior que zero.");
            }

            return null;
        }

        private static PlanoEstrategicoFinanceiro CriarPlano(string usuarioId, SalvarPlanoEstrategicoFinanceiroDTO dto, DateTime agora, int numeroVersao, Guid planoRaizId)
        {
            var plano = new PlanoEstrategicoFinanceiro
            {
                Id = Guid.NewGuid(),
                PlanoRaizId = planoRaizId,
                UsuarioId = usuarioId,
                Nome = dto.Nome.Trim(),
                Descricao = string.IsNullOrWhiteSpace(dto.Descricao) ? null : dto.Descricao.Trim(),
                Observacao = string.IsNullOrWhiteSpace(dto.Observacao) ? null : dto.Observacao.Trim(),
                NumeroVersao = numeroVersao,
                DataInicioVigencia = dto.DataInicioVigencia?.Date ?? agora.Date,
                DataFimVigencia = null,
                DataCadastro = agora,
                DataAtualizacao = agora,
                Ativo = true
            };

            plano.Objetivos = dto.Objetivos
                .OrderBy(x => x.Ordem)
                .Select(objetivo => new ObjetivoPlanoEstrategico
                {
                    Id = Guid.NewGuid(),
                    PlanoEstrategicoFinanceiroId = plano.Id,
                    Titulo = objetivo.Titulo.Trim(),
                    Descricao = string.IsNullOrWhiteSpace(objetivo.Descricao) ? null : objetivo.Descricao.Trim(),
                    Prioridade = objetivo.Prioridade,
                    Status = objetivo.Status,
                    Ordem = objetivo.Ordem,
                    DataAlvo = objetivo.DataAlvo,
                    ValorAlvo = objetivo.ValorAlvo,
                    ValorAtual = objetivo.ValorAtual,
                    Observacao = string.IsNullOrWhiteSpace(objetivo.Observacao) ? null : objetivo.Observacao.Trim(),
                    DataCriacao = agora
                })
                .ToList();

            return plano;
        }

        private static PlanoEstrategicoFinanceiroResumoDTO MapearResumo(PlanoEstrategicoFinanceiro plano)
        {
            return new PlanoEstrategicoFinanceiroResumoDTO
            {
                Id = plano.Id,
                PlanoRaizId = plano.PlanoRaizId,
                Nome = plano.Nome,
                Descricao = plano.Descricao,
                NumeroVersao = plano.NumeroVersao,
                DataInicioVigencia = plano.DataInicioVigencia,
                DataFimVigencia = plano.DataFimVigencia,
                DataCadastro = plano.DataCadastro,
                DataAtualizacao = plano.DataAtualizacao,
                Ativo = plano.Ativo,
                QuantidadeObjetivos = plano.Objetivos.Count
            };
        }

        private static PlanoEstrategicoFinanceiroDetalheDTO MapearDetalhe(PlanoEstrategicoFinanceiro plano)
        {
            return new PlanoEstrategicoFinanceiroDetalheDTO
            {
                Id = plano.Id,
                PlanoRaizId = plano.PlanoRaizId,
                Nome = plano.Nome,
                Descricao = plano.Descricao,
                Observacao = plano.Observacao,
                NumeroVersao = plano.NumeroVersao,
                DataInicioVigencia = plano.DataInicioVigencia,
                DataFimVigencia = plano.DataFimVigencia,
                DataCadastro = plano.DataCadastro,
                DataAtualizacao = plano.DataAtualizacao,
                Ativo = plano.Ativo,
                Objetivos = plano.Objetivos
                    .OrderBy(x => x.Ordem)
                    .ThenBy(x => x.Titulo)
                    .Select(MapearObjetivo)
                    .ToList()
            };
        }

        private static ObjetivoPlanoEstrategicoDTO MapearObjetivo(ObjetivoPlanoEstrategico objetivo)
        {
            return new ObjetivoPlanoEstrategicoDTO
            {
                Id = objetivo.Id,
                Titulo = objetivo.Titulo,
                Descricao = objetivo.Descricao,
                Prioridade = objetivo.Prioridade,
                Status = objetivo.Status,
                Ordem = objetivo.Ordem,
                DataAlvo = objetivo.DataAlvo,
                ValorAlvo = objetivo.ValorAlvo,
                ValorAtual = objetivo.ValorAtual,
                Observacao = objetivo.Observacao
            };
        }

        private static RetornoGenerico CriarErro(Exception ex, string mensagemUsuario)
        {
            return new RetornoGenerico(false, ex.ToString(), mensagemUsuario, HttpStatusCode.InternalServerError, null);
        }

        private static RetornoGenerico CriarErroNegocio(string mensagem)
        {
            return new RetornoGenerico(false, mensagem, mensagem, HttpStatusCode.BadRequest, null);
        }

        private static RetornoGenerico CriarNaoEncontrado(string mensagem)
        {
            return new RetornoGenerico(false, mensagem, mensagem, HttpStatusCode.NotFound, null);
        }
    }
}
