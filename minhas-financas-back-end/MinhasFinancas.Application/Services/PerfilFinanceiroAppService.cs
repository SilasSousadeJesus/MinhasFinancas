using MinhasFinancas.Application.DTOs.PerfilFinanceiro;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class PerfilFinanceiroAppService : IPerfilFinanceiroAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IPerfilFinanceiroRepository _perfilFinanceiroRepository;

        public PerfilFinanceiroAppService(
            IUsuarioAppService usuarioAppService,
            IPerfilFinanceiroRepository perfilFinanceiroRepository)
        {
            _usuarioAppService = usuarioAppService;
            _perfilFinanceiroRepository = perfilFinanceiroRepository;
        }

        public async Task<RetornoGenerico> BuscarVisaoGeralAsync(string usuarioId)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null) return validacaoUsuario;

                var perfil = await _perfilFinanceiroRepository.BuscarPorUsuarioLeituraAsync(usuarioId);
                var dados = MapearVisaoGeral(perfil);

                return new RetornoGenerico(true, "Perfil financeiro carregado com sucesso.", "Perfil financeiro carregado com sucesso.", HttpStatusCode.OK, dados);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível carregar o perfil financeiro.");
            }
        }

        public async Task<RetornoGenerico> SalvarConfiguracaoAsync(string usuarioId, SalvarPerfilFinanceiroDTO configuracaoDTO)
        {
            try
            {
                var validacaoUsuario = await ValidarUsuarioAsync(usuarioId);
                if (validacaoUsuario != null) return validacaoUsuario;

                var validacaoConfiguracao = ValidarConfiguracao(configuracaoDTO);
                if (validacaoConfiguracao != null) return validacaoConfiguracao;

                var perfil = await _perfilFinanceiroRepository.BuscarPorUsuarioLeituraAsync(usuarioId);
                var agora = DateTime.UtcNow;

                if (perfil == null)
                {
                    perfil = new PerfilFinanceiro
                    {
                        Id = Guid.NewGuid(),
                        UsuarioId = usuarioId,
                        DataCriacao = agora,
                        Ativo = true,
                    };

                    perfil.Configuracoes.Add(CriarConfiguracao(perfil.Id, configuracaoDTO, agora));
                    await _perfilFinanceiroRepository.CadastrarAsync(perfil);

                    return new RetornoGenerico(true, "Perfil financeiro criado com sucesso.", "Perfil financeiro criado com sucesso.", HttpStatusCode.OK, MapearVisaoGeral(perfil));
                }

                var configuracaoVigente = perfil.Configuracoes
                    .Where(x => x.DataFimVigencia == null)
                    .OrderByDescending(x => x.DataInicioVigencia)
                    .ThenByDescending(x => x.DataCriacao)
                    .FirstOrDefault();

                if (configuracaoVigente != null && ConfiguracaoEhIgual(configuracaoVigente, configuracaoDTO))
                {
                    return new RetornoGenerico(true, "Nenhuma alteração relevante foi identificada.", "Nenhuma alteração relevante foi identificada.", HttpStatusCode.OK, MapearVisaoGeral(perfil));
                }

                if (configuracaoVigente != null)
                {
                    await _perfilFinanceiroRepository.EncerrarConfiguracaoVigenteAsync(configuracaoVigente.Id, agora);
                }

                await _perfilFinanceiroRepository.AdicionarConfiguracaoAsync(CriarConfiguracao(perfil.Id, configuracaoDTO, agora));
                await _perfilFinanceiroRepository.SalvarAlteracoesAsync();

                var perfilAtualizado = await _perfilFinanceiroRepository.BuscarPorUsuarioLeituraAsync(usuarioId);
                return new RetornoGenerico(true, "Perfil financeiro atualizado com sucesso.", "Perfil financeiro atualizado com sucesso.", HttpStatusCode.OK, MapearVisaoGeral(perfilAtualizado));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível salvar o perfil financeiro.");
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

        private static RetornoGenerico? ValidarConfiguracao(SalvarPerfilFinanceiroDTO dto)
        {
            if (dto.PercentualEconomiaMensalDesejado < 0 ||
                dto.PercentualReservaEmergenciaDesejado < 0 ||
                dto.PercentualMaximoComprometimentoRenda < 0 ||
                dto.PercentualMaximoEndividamento < 0 ||
                dto.PercentualMinimoInvestimento < 0)
            {
                return new RetornoGenerico(false, "Percentuais não podem ser negativos.", "Informe percentuais válidos.", HttpStatusCode.BadRequest, null);
            }

            if (dto.MesesReservaEmergenciaDesejados < 0)
            {
                return new RetornoGenerico(false, "Meses de reserva não podem ser negativos.", "Informe uma quantidade válida de meses de reserva.", HttpStatusCode.BadRequest, null);
            }

            if (dto.PatrimonioLiquidoAlvo.HasValue && dto.PatrimonioLiquidoAlvo.Value < 0)
            {
                return new RetornoGenerico(false, "Patrimônio líquido alvo não pode ser negativo.", "Informe um patrimônio líquido alvo válido.", HttpStatusCode.BadRequest, null);
            }

            return null;
        }

        private static ConfiguracaoPerfilFinanceiro CriarConfiguracao(Guid perfilFinanceiroId, SalvarPerfilFinanceiroDTO dto, DateTime dataReferencia)
        {
            return new ConfiguracaoPerfilFinanceiro
            {
                Id = Guid.NewGuid(),
                PerfilFinanceiroId = perfilFinanceiroId,
                DataInicioVigencia = dataReferencia,
                PercentualEconomiaMensalDesejado = dto.PercentualEconomiaMensalDesejado,
                PercentualReservaEmergenciaDesejado = dto.PercentualReservaEmergenciaDesejado,
                MesesReservaEmergenciaDesejados = dto.MesesReservaEmergenciaDesejados,
                PercentualMaximoComprometimentoRenda = dto.PercentualMaximoComprometimentoRenda,
                PercentualMaximoEndividamento = dto.PercentualMaximoEndividamento,
                PercentualMinimoInvestimento = dto.PercentualMinimoInvestimento,
                PatrimonioLiquidoAlvo = dto.PatrimonioLiquidoAlvo,
                Observacao = dto.Observacao?.Trim(),
                DataCriacao = dataReferencia,
            };
        }

        private static bool ConfiguracaoEhIgual(ConfiguracaoPerfilFinanceiro atual, SalvarPerfilFinanceiroDTO dto)
        {
            return atual.PercentualEconomiaMensalDesejado == dto.PercentualEconomiaMensalDesejado
                && atual.PercentualReservaEmergenciaDesejado == dto.PercentualReservaEmergenciaDesejado
                && atual.MesesReservaEmergenciaDesejados == dto.MesesReservaEmergenciaDesejados
                && atual.PercentualMaximoComprometimentoRenda == dto.PercentualMaximoComprometimentoRenda
                && atual.PercentualMaximoEndividamento == dto.PercentualMaximoEndividamento
                && atual.PercentualMinimoInvestimento == dto.PercentualMinimoInvestimento
                && atual.PatrimonioLiquidoAlvo == dto.PatrimonioLiquidoAlvo
                && string.Equals(atual.Observacao?.Trim() ?? string.Empty, dto.Observacao?.Trim() ?? string.Empty, StringComparison.Ordinal);
        }

        private static VisaoGeralPerfilFinanceiroDTO MapearVisaoGeral(PerfilFinanceiro? perfil)
        {
            var historico = perfil?.Configuracoes
                .OrderByDescending(x => x.DataInicioVigencia)
                .ThenByDescending(x => x.DataCriacao)
                .Select(MapearConfiguracao)
                .ToList() ?? [];

            return new VisaoGeralPerfilFinanceiroDTO
            {
                PerfilId = perfil?.Id,
                ConfiguracaoVigente = historico.FirstOrDefault(x => x.Vigente),
                Historico = historico
            };
        }

        private static ConfiguracaoPerfilFinanceiroDTO MapearConfiguracao(ConfiguracaoPerfilFinanceiro configuracao)
        {
            return new ConfiguracaoPerfilFinanceiroDTO
            {
                Id = configuracao.Id,
                DataInicioVigencia = configuracao.DataInicioVigencia,
                DataFimVigencia = configuracao.DataFimVigencia,
                PercentualEconomiaMensalDesejado = configuracao.PercentualEconomiaMensalDesejado,
                PercentualReservaEmergenciaDesejado = configuracao.PercentualReservaEmergenciaDesejado,
                MesesReservaEmergenciaDesejados = configuracao.MesesReservaEmergenciaDesejados,
                PercentualMaximoComprometimentoRenda = configuracao.PercentualMaximoComprometimentoRenda,
                PercentualMaximoEndividamento = configuracao.PercentualMaximoEndividamento,
                PercentualMinimoInvestimento = configuracao.PercentualMinimoInvestimento,
                PatrimonioLiquidoAlvo = configuracao.PatrimonioLiquidoAlvo,
                Observacao = configuracao.Observacao,
                Vigente = configuracao.DataFimVigencia == null,
            };
        }

        private static RetornoGenerico CriarErro(Exception ex, string mensagemUsuario)
        {
            return new RetornoGenerico(false, ex.ToString(), mensagemUsuario, HttpStatusCode.InternalServerError, null);
        }
    }
}
