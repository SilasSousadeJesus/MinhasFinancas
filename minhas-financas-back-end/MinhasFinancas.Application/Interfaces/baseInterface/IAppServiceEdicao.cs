namespace MinhasFinancas.Application.Interfaces.baseInterface
{
    public interface IAppServiceAtualizacao<TAtualizacaoDTO> where TAtualizacaoDTO : class
    {
        Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, TAtualizacaoDTO elementoDTO);
    }
}
