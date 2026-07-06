using Microsoft.Extensions.Options;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Provedores
{
    public class OpenAIProvider : IProvedorIA
    {
        private readonly ConfiguracaoOpenAI _configuracao;

        public OpenAIProvider(IOptions<ConfiguracaoOpenAI> configuracao)
        {
            _configuracao = configuracao.Value;
        }

        public Task<RespostaIA> GerarRespostaAsync(RequisicaoIA requisicao, CancellationToken cancellationToken = default)
        {
            // Fase 2: provider preparado apenas para contrato e DI, sem chamadas reais.
            return Task.FromResult(new RespostaIA
            {
                Provedor = "OpenAI",
                Modelo = _configuracao.Model,
                FoiSimulada = true,
                Conteudo = "Infraestrutura de IA preparada. Nesta Fase 2 nenhuma chamada real ao provedor foi executada.",
                ObservacaoInfraestrutura = "Resposta simulada gerada pelo OpenAIProvider para validar a arquitetura sem consumo de token."
            });
        }
    }
}
