using System.Net;
using MinhasFinancas.Application.DTOs.CompromissoFinanceiro;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class CompromissoFinanceiroAppService : ICompromissoFinanceiroAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly ICompromissoFinanceiroRepository _repository;
        private readonly IAnaliseFinanceiraHistoricaRepository _analiseFinanceiraHistoricaRepository;

        public CompromissoFinanceiroAppService(
            IUsuarioAppService usuarioAppService,
            ICompromissoFinanceiroRepository repository,
            IAnaliseFinanceiraHistoricaRepository analiseFinanceiraHistoricaRepository)
        {
            _usuarioAppService = usuarioAppService;
            _repository = repository;
            _analiseFinanceiraHistoricaRepository = analiseFinanceiraHistoricaRepository;
        }

        public async Task<RetornoGenerico> BuscarTodosOsElementosAsync(string id)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(id);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var compromissos = await _repository.BuscarTodosOsElementosAsync(id);
                var dados = compromissos.Select(Mapear).ToList();

                return new RetornoGenerico(
                    true,
                    $"{dados.Count} compromisso(s) encontrado(s).",
                    $"{dados.Count} compromisso(s) carregado(s) com sucesso.",
                    HttpStatusCode.OK,
                    dados);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível listar os compromissos financeiros.");
            }
        }

        public async Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid compromissoId)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var compromisso = await _repository.BuscarUmElementoAsync(usuarioId, compromissoId);
                if (compromisso == null)
                {
                    return CriarNaoEncontrado("Compromisso financeiro não encontrado.");
                }

                return new RetornoGenerico(true, "Compromisso financeiro carregado com sucesso.", "Compromisso financeiro carregado com sucesso.", HttpStatusCode.OK, Mapear(compromisso));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível carregar o compromisso financeiro.");
            }
        }

        public async Task<RetornoGenerico> CadastrarElementoAsync(SalvarCompromissoFinanceiroDTO elementoDTO)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(elementoDTO.UsuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var validacaoDto = ValidarDto(elementoDTO);
                if (validacaoDto != null)
                {
                    return validacaoDto;
                }

                var agora = DateTime.UtcNow;
                var compromisso = new CompromissoFinanceiro
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = elementoDTO.UsuarioId,
                    Descricao = elementoDTO.Descricao.Trim(),
                    Origem = elementoDTO.Origem,
                    Status = EnumStatusCompromissoFinanceiro.EmAndamento,
                    AnaliseFinanceiraHistoricaId = elementoDTO.AnaliseFinanceiraHistoricaId,
                    DataCriacao = agora,
                    Observacoes = string.IsNullOrWhiteSpace(elementoDTO.Observacoes) ? null : elementoDTO.Observacoes.Trim(),
                    Ativo = true
                };

                await _repository.AdicionarAsync(compromisso);
                await _repository.SalvarAlteracoesAsync();
                await SincronizarAnaliseFinanceiraHistoricaAsync(elementoDTO, compromisso.Id);

                return new RetornoGenerico(true, "Compromisso financeiro criado com sucesso.", "Compromisso financeiro criado com sucesso.", HttpStatusCode.OK, Mapear(compromisso));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível criar o compromisso financeiro.");
            }
        }

        public async Task<RetornoGenerico> EditarElementoAsync(string usuarioId, Guid compromissoId, SalvarCompromissoFinanceiroDTO elementoDTO)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null)
                {
                    return validacaoUsuario;
                }

                var validacaoDto = ValidarDto(elementoDTO);
                if (validacaoDto != null)
                {
                    return validacaoDto;
                }

                var compromisso = await _repository.BuscarUmElementoGerenciadoAsync(usuarioId, compromissoId);
                if (compromisso == null)
                {
                    return CriarNaoEncontrado("Compromisso financeiro não encontrado.");
                }

                compromisso.Descricao = elementoDTO.Descricao.Trim();
                compromisso.Observacoes = string.IsNullOrWhiteSpace(elementoDTO.Observacoes) ? null : elementoDTO.Observacoes.Trim();

                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(true, "Compromisso financeiro atualizado com sucesso.", "Compromisso financeiro atualizado com sucesso.", HttpStatusCode.OK, Mapear(compromisso));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível editar o compromisso financeiro.");
            }
        }

        public async Task<RetornoGenerico> ConcluirAsync(string usuarioId, Guid compromissoId)
        {
            try
            {
                var compromisso = await BuscarCompromissoGerenciadoAsync(usuarioId, compromissoId);
                if (compromisso == null)
                {
                    return CriarNaoEncontrado("Compromisso financeiro não encontrado.");
                }

                if (compromisso.Status != EnumStatusCompromissoFinanceiro.EmAndamento)
                {
                    return CriarErroNegocio("Somente compromissos em andamento podem ser concluídos.");
                }

                compromisso.Status = EnumStatusCompromissoFinanceiro.Concluido;
                compromisso.DataConclusao = DateTime.UtcNow;
                compromisso.DataCancelamento = null;

                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(true, "Compromisso financeiro concluído com sucesso.", "Compromisso financeiro concluído com sucesso.", HttpStatusCode.OK, Mapear(compromisso));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível concluir o compromisso financeiro.");
            }
        }

        public async Task<RetornoGenerico> CancelarAsync(string usuarioId, Guid compromissoId)
        {
            try
            {
                var compromisso = await BuscarCompromissoGerenciadoAsync(usuarioId, compromissoId);
                if (compromisso == null)
                {
                    return CriarNaoEncontrado("Compromisso financeiro não encontrado.");
                }

                if (compromisso.Status == EnumStatusCompromissoFinanceiro.Cancelado)
                {
                    return CriarErroNegocio("O compromisso já está cancelado.");
                }

                compromisso.Status = EnumStatusCompromissoFinanceiro.Cancelado;
                compromisso.DataCancelamento = DateTime.UtcNow;
                compromisso.DataConclusao = null;

                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(true, "Compromisso financeiro cancelado com sucesso.", "Compromisso financeiro cancelado com sucesso.", HttpStatusCode.OK, Mapear(compromisso));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível cancelar o compromisso financeiro.");
            }
        }

        public async Task<RetornoGenerico> ExcluirAsync(string usuarioId, Guid compromissoId)
        {
            try
            {
                var compromisso = await BuscarCompromissoGerenciadoAsync(usuarioId, compromissoId);
                if (compromisso == null)
                {
                    return CriarNaoEncontrado("Compromisso financeiro não encontrado.");
                }

                compromisso.Ativo = false;
                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(true, "Compromisso financeiro excluído com sucesso.", "Compromisso financeiro excluído com sucesso.", HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível excluir o compromisso financeiro.");
            }
        }

        public Task<RetornoGenerico> DeletarElementoAsync(string idPatrono, Guid idElemento)
        {
            return ExcluirAsync(idPatrono, idElemento);
        }

        private async Task<CompromissoFinanceiro?> BuscarCompromissoGerenciadoAsync(string usuarioId, Guid compromissoId)
        {
            var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
            if (validacaoUsuario != null)
            {
                return null;
            }

            return await _repository.BuscarUmElementoGerenciadoAsync(usuarioId, compromissoId);
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

        private static RetornoGenerico? ValidarDto(SalvarCompromissoFinanceiroDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Descricao))
            {
                return CriarErroNegocio("Informe uma descrição para o compromisso financeiro.");
            }

            return null;
        }

        private static CompromissoFinanceiroDTO Mapear(CompromissoFinanceiro compromisso)
        {
            return new CompromissoFinanceiroDTO
            {
                Id = compromisso.Id,
                UsuarioId = compromisso.UsuarioId,
                Descricao = compromisso.Descricao,
                Origem = compromisso.Origem,
                Status = compromisso.Status,
                AnaliseFinanceiraHistoricaId = compromisso.AnaliseFinanceiraHistoricaId,
                DataCriacao = compromisso.DataCriacao,
                DataConclusao = compromisso.DataConclusao,
                DataCancelamento = compromisso.DataCancelamento,
                Observacoes = compromisso.Observacoes,
                Ativo = compromisso.Ativo
            };
        }

        private async Task SincronizarAnaliseFinanceiraHistoricaAsync(SalvarCompromissoFinanceiroDTO dto, Guid compromissoId)
        {
            if (!dto.AnaliseFinanceiraHistoricaId.HasValue)
            {
                return;
            }

            var analise = await _analiseFinanceiraHistoricaRepository.BuscarUmElementoAsync(dto.UsuarioId, dto.AnaliseFinanceiraHistoricaId.Value);
            if (analise == null)
            {
                return;
            }

            analise.CompromissoFinanceiroId = compromissoId;
            await _analiseFinanceiraHistoricaRepository.EditarElementoAsync(analise);
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
