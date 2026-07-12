namespace MinhasFinancas.Application.Interfaces
{
    using MinhasFinancas.Application.DTOs.MfScoreLaboratorio;

    public interface IBenchmarkMfScoreService
    {
        Task<BenchmarkCenarioMfScoreLaboratorioDTO?> BuscarCenarioAsync(string codigoCenario);
    }

    public interface IMfScoreLaboratorioAppService
    {
        Task<RetornoGenerico> BuscarUsuariosAsync();
        Task<RetornoGenerico> BuscarScoreUsuarioAsync(string usuarioId);
        Task<RetornoGenerico> GerarBaseSimulacaoAsync();
        Task<RetornoGenerico> LimparBaseSimulacaoAsync();
    }
}
