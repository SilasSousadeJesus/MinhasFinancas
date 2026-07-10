using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class PerfilFinanceiroInicialService : IPerfilFinanceiroInicialService
    {
        private const decimal PercentualEconomiaMensalDesejadoPadrao = 20m;
        private const decimal PercentualReservaEmergenciaDesejadoPadrao = 100m;
        private const int MesesReservaEmergenciaDesejadosPadrao = 6;
        private const decimal PercentualMaximoComprometimentoRendaPadrao = 50m;
        private const decimal PercentualMaximoEndividamentoPadrao = 50m;
        private const decimal PercentualMinimoInvestimentoPadrao = 10m;
        private const decimal PatrimonioLiquidoAlvoPadrao = 0m;

        private readonly IPerfilFinanceiroRepository _perfilFinanceiroRepository;

        public PerfilFinanceiroInicialService(IPerfilFinanceiroRepository perfilFinanceiroRepository)
        {
            _perfilFinanceiroRepository = perfilFinanceiroRepository;
        }

        public async Task<PerfilFinanceiro> GarantirPerfilFinanceiroValidoAsync(string usuarioId)
        {
            var perfil = await _perfilFinanceiroRepository.BuscarPorUsuarioLeituraAsync(usuarioId);
            var agora = DateTime.UtcNow;

            if (perfil == null)
            {
                var novoPerfil = CriarPerfilInicial(usuarioId, agora);
                await _perfilFinanceiroRepository.CadastrarAsync(novoPerfil);
                return novoPerfil;
            }

            var configuracaoVigente = ObterConfiguracaoVigente(perfil);
            if (configuracaoVigente != null)
            {
                return perfil;
            }

            var configuracaoMaisRecente = perfil.Configuracoes
                .OrderByDescending(x => x.DataInicioVigencia)
                .ThenByDescending(x => x.DataCriacao)
                .FirstOrDefault();

            var novaConfiguracao = configuracaoMaisRecente == null
                ? CriarConfiguracaoInicial(perfil.Id, agora)
                : CriarConfiguracaoComMesmosParametros(configuracaoMaisRecente, agora);

            await _perfilFinanceiroRepository.AdicionarConfiguracaoAsync(novaConfiguracao);
            await _perfilFinanceiroRepository.SalvarAlteracoesAsync();

            var perfilAtualizado = await _perfilFinanceiroRepository.BuscarPorUsuarioLeituraAsync(usuarioId);
            return perfilAtualizado ?? perfil;
        }

        public static PerfilFinanceiro CriarPerfilInicial(string usuarioId, DateTime dataReferencia)
        {
            var perfilId = Guid.NewGuid();

            return new PerfilFinanceiro
            {
                Id = perfilId,
                UsuarioId = usuarioId,
                DataCriacao = dataReferencia,
                Ativo = true,
                Configuracoes =
                [
                    CriarConfiguracaoInicial(perfilId, dataReferencia)
                ]
            };
        }

        public static ConfiguracaoPerfilFinanceiro CriarConfiguracaoInicial(Guid perfilFinanceiroId, DateTime dataReferencia)
        {
            return new ConfiguracaoPerfilFinanceiro
            {
                Id = Guid.NewGuid(),
                PerfilFinanceiroId = perfilFinanceiroId,
                DataInicioVigencia = dataReferencia,
                PercentualEconomiaMensalDesejado = PercentualEconomiaMensalDesejadoPadrao,
                PercentualReservaEmergenciaDesejado = PercentualReservaEmergenciaDesejadoPadrao,
                MesesReservaEmergenciaDesejados = MesesReservaEmergenciaDesejadosPadrao,
                PercentualMaximoComprometimentoRenda = PercentualMaximoComprometimentoRendaPadrao,
                PercentualMaximoEndividamento = PercentualMaximoEndividamentoPadrao,
                PercentualMinimoInvestimento = PercentualMinimoInvestimentoPadrao,
                PatrimonioLiquidoAlvo = PatrimonioLiquidoAlvoPadrao,
                Observacao = "Perfil financeiro inicial criado automaticamente pelo sistema.",
                DataCriacao = dataReferencia,
                OrigemPerfilFinanceiro = EnumOrigemPerfilFinanceiro.PerfilInicialSistema
            };
        }

        public static ConfiguracaoPerfilFinanceiro CriarConfiguracaoComMesmosParametros(
            ConfiguracaoPerfilFinanceiro configuracaoOrigem,
            DateTime dataReferencia,
            EnumOrigemPerfilFinanceiro? origemSobrescrita = null)
        {
            return new ConfiguracaoPerfilFinanceiro
            {
                Id = Guid.NewGuid(),
                PerfilFinanceiroId = configuracaoOrigem.PerfilFinanceiroId,
                DataInicioVigencia = dataReferencia,
                PercentualEconomiaMensalDesejado = configuracaoOrigem.PercentualEconomiaMensalDesejado,
                PercentualReservaEmergenciaDesejado = configuracaoOrigem.PercentualReservaEmergenciaDesejado,
                MesesReservaEmergenciaDesejados = configuracaoOrigem.MesesReservaEmergenciaDesejados,
                PercentualMaximoComprometimentoRenda = configuracaoOrigem.PercentualMaximoComprometimentoRenda,
                PercentualMaximoEndividamento = configuracaoOrigem.PercentualMaximoEndividamento,
                PercentualMinimoInvestimento = configuracaoOrigem.PercentualMinimoInvestimento,
                PatrimonioLiquidoAlvo = configuracaoOrigem.PatrimonioLiquidoAlvo,
                Observacao = configuracaoOrigem.Observacao,
                DataCriacao = dataReferencia,
                OrigemPerfilFinanceiro = origemSobrescrita ?? configuracaoOrigem.OrigemPerfilFinanceiro
            };
        }

        private static ConfiguracaoPerfilFinanceiro? ObterConfiguracaoVigente(PerfilFinanceiro perfil)
        {
            return perfil.Configuracoes
                .Where(x => x.DataFimVigencia == null)
                .OrderByDescending(x => x.DataInicioVigencia)
                .ThenByDescending(x => x.DataCriacao)
                .FirstOrDefault();
        }
    }
}
