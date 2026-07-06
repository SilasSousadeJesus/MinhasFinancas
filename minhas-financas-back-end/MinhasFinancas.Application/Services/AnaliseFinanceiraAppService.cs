using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class AnaliseFinanceiraAppService : IAnaliseFinanceiraAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IBemMaterialRepository _bemMaterialRepository;
        private readonly IPassivoRepository _passivoRepository;
        private readonly IPerfilFinanceiroRepository _perfilFinanceiroRepository;
        private readonly IIndicadoresFinanceirosService _indicadoresFinanceirosService;

        public AnaliseFinanceiraAppService(
            IUsuarioAppService usuarioAppService,
            ILancamentoRepository lancamentoRepository,
            IBemMaterialRepository bemMaterialRepository,
            IPassivoRepository passivoRepository,
            IPerfilFinanceiroRepository perfilFinanceiroRepository,
            IIndicadoresFinanceirosService indicadoresFinanceirosService)
        {
            _usuarioAppService = usuarioAppService;
            _lancamentoRepository = lancamentoRepository;
            _bemMaterialRepository = bemMaterialRepository;
            _passivoRepository = passivoRepository;
            _perfilFinanceiroRepository = perfilFinanceiroRepository;
            _indicadoresFinanceirosService = indicadoresFinanceirosService;
        }

        public async Task<RetornoGenerico> BuscarIndicadoresFinanceiros(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var painel = await BuscarPainelIndicadoresInternoAsync(usuarioId);

                if (painel is null)
                {
                    retorno.Sucesso = false;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = "Usuário não encontrado para análise financeira.";
                    retorno.MensagemUsuario = "Não foi possível encontrar os indicadores financeiros.";
                    retorno.Dados = null;
                    return retorno;
                }

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Indicadores financeiros calculados com sucesso.";
                retorno.MensagemUsuario = "Indicadores financeiros calculados com sucesso.";
                retorno.Dados = painel;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível calcular os indicadores financeiros.";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<PainelIndicadoresFinanceiros?> BuscarPainelIndicadoresInternoAsync(string usuarioId)
        {
            var buscaUsuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);

            if (!buscaUsuario.Sucesso)
            {
                return null;
            }

            var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
            var ativos = await _bemMaterialRepository.BuscarTodosOsElementosAsync(usuarioId);
            var passivos = await _passivoRepository.BuscarTodosOsElementosAsync(usuarioId);
            var perfilFinanceiro = await _perfilFinanceiroRepository.BuscarPorUsuarioLeituraAsync(usuarioId);

            var configuracaoVigente = perfilFinanceiro?.Configuracoes
                .Where(x => x.DataFimVigencia == null)
                .OrderByDescending(x => x.DataInicioVigencia)
                .ThenByDescending(x => x.DataCriacao)
                .FirstOrDefault();

            return _indicadoresFinanceirosService.Calcular(new ContextoAnaliseFinanceira
            {
                DataReferencia = DateTime.Today,
                Lancamentos = lancamentos,
                Ativos = ativos,
                Passivos = passivos,
                ConfiguracaoPerfilFinanceiro = configuracaoVigente,
            });
        }
    }
}
