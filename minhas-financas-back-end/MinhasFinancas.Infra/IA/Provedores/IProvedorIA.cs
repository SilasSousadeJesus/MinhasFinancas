using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Provedores
{
    public interface IProvedorIA
    {
        Task<RespostaIA> GerarRespostaAsync(RequisicaoIA requisicao, CancellationToken cancellationToken = default);
    }
}
