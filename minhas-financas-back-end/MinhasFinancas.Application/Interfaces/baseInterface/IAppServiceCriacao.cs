namespace MinhasFinancas.Application.Interfaces.baseInterface
{
    public interface IAppServiceCriacao<TCriacaoDTO> where TCriacaoDTO : class
    {
        Task<RetornoGenerico> CadastrarElementoAsync(TCriacaoDTO elementoDTO);
    }
}
