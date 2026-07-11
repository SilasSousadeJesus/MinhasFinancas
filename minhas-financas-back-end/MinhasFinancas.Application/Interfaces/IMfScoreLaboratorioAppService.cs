namespace MinhasFinancas.Application.Interfaces
{
    public interface IMfScoreLaboratorioAppService
    {
        Task<RetornoGenerico> BuscarUsuariosAsync();
        Task<RetornoGenerico> BuscarScoreUsuarioAsync(string usuarioId);
        Task<RetornoGenerico> GerarBaseSimulacaoAsync();
        Task<RetornoGenerico> LimparBaseSimulacaoAsync();
    }
}
