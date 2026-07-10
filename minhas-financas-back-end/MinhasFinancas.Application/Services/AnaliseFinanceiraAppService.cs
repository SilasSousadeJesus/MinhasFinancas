using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class AnaliseFinanceiraAppService : IAnaliseFinanceiraAppService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IBemMaterialRepository _bemMaterialRepository;
        private readonly IPassivoRepository _passivoRepository;
        private readonly IPerfilFinanceiroInicialService _perfilFinanceiroInicialService;
        private readonly IPlanoEstrategicoFinanceiroRepository _planoEstrategicoFinanceiroRepository;
        private readonly ICompromissoFinanceiroRepository _compromissoFinanceiroRepository;
        private readonly IIndicadoresFinanceirosService _indicadoresFinanceirosService;

        public AnaliseFinanceiraAppService(
            IUsuarioRepository usuarioRepository,
            ILancamentoRepository lancamentoRepository,
            IBemMaterialRepository bemMaterialRepository,
            IPassivoRepository passivoRepository,
            IPerfilFinanceiroInicialService perfilFinanceiroInicialService,
            IPlanoEstrategicoFinanceiroRepository planoEstrategicoFinanceiroRepository,
            ICompromissoFinanceiroRepository compromissoFinanceiroRepository,
            IIndicadoresFinanceirosService indicadoresFinanceirosService)
        {
            _usuarioRepository = usuarioRepository;
            _lancamentoRepository = lancamentoRepository;
            _bemMaterialRepository = bemMaterialRepository;
            _passivoRepository = passivoRepository;
            _perfilFinanceiroInicialService = perfilFinanceiroInicialService;
            _planoEstrategicoFinanceiroRepository = planoEstrategicoFinanceiroRepository;
            _compromissoFinanceiroRepository = compromissoFinanceiroRepository;
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

        public async Task<PainelIndicadoresFinanceiros?> BuscarPainelIndicadoresInternoAsync(string usuarioId, DateTime? dataReferencia = null)
        {
            var contexto = await BuscarContextoAnaliseInternoAsync(usuarioId, dataReferencia);
            if (contexto is null)
            {
                return null;
            }

            return _indicadoresFinanceirosService.Calcular(contexto);
        }

        public async Task<ContextoAnaliseFinanceira?> BuscarContextoAnaliseInternoAsync(string usuarioId, DateTime? dataReferencia = null)
        {
            var usuarioExiste = await _usuarioRepository.ExisteUsuarioAsync(usuarioId);

            if (!usuarioExiste)
            {
                return null;
            }

            var lancamentos = await _lancamentoRepository.BuscarTodosOsElementosAsync(usuarioId);
            var ativos = await _bemMaterialRepository.BuscarTodosOsElementosAsync(usuarioId);
            var passivos = await _passivoRepository.BuscarTodosOsElementosAsync(usuarioId);
            var perfilFinanceiro = await _perfilFinanceiroInicialService.GarantirPerfilFinanceiroValidoAsync(usuarioId);
            var planoEstrategicoVigente = await _planoEstrategicoFinanceiroRepository.BuscarVigenteAsync(usuarioId);
            var compromissosFinanceiros = await _compromissoFinanceiroRepository.BuscarTodosOsElementosAsync(usuarioId);

            var configuracaoVigente = perfilFinanceiro?.Configuracoes
                .Where(x => x.DataFimVigencia == null)
                .OrderByDescending(x => x.DataInicioVigencia)
                .ThenByDescending(x => x.DataCriacao)
                .FirstOrDefault();

            return new ContextoAnaliseFinanceira
            {
                DataReferencia = (dataReferencia ?? DateTime.Today).Date,
                Lancamentos = lancamentos,
                Ativos = ativos,
                Passivos = passivos,
                PlanoEstrategicoFinanceiroVigente = planoEstrategicoVigente,
                CompromissosFinanceiros = compromissosFinanceiros,
                ConfiguracaoPerfilFinanceiro = configuracaoVigente,
            };
        }
    }
}
