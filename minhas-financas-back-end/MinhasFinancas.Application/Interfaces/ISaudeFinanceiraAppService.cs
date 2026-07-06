namespace MinhasFinancas.Application.Interfaces
{
    public interface ISaudeFinanceiraAppService
    {
        Task<RetornoGenerico> BuscarSaudeFinanceira(string usuarioId);
    }
}
