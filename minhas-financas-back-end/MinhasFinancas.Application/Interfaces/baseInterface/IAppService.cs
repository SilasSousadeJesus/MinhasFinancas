namespace MinhasFinancas.Application.Interfaces.baseInterface
{
    public interface IAppService<TCriacaoDTO, TAtualizacaoDTO> : IAppServiceCriacao<TCriacaoDTO>, IAppServiceAtualizacao<TAtualizacaoDTO>
        where TCriacaoDTO : class
        where TAtualizacaoDTO : class
    {
        Task<RetornoGenerico> BuscarTodosOsElementosAsync(string id);
        Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid BancoId);
        Task<RetornoGenerico> DeletarElementoAsync(string idPatrono, Guid idElemento);
    }
}
