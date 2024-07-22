namespace MinhasFinancas.Application.Interfaces
{
    public interface IDashboardAppService
    {
        Task<RetornoGenerico> BuscarInformacoesDashboard(string usuarioId);
    }
}
