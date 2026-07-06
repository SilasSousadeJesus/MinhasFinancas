namespace MinhasFinancas.Application.Interfaces
{
    public interface IInteligenciaFinanceiraAppService
    {
        Task<RetornoGenerico> BuscarInsightsFinanceiros(string usuarioId);
        Task<RetornoGenerico> BuscarResumoFinanceiroIA(string usuarioId);
    }
}
