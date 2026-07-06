namespace MinhasFinancas.Application.Interfaces
{
    public interface IAssistenteFinanceiroAppService
    {
        Task<RetornoGenerico> GerarAnaliseAsync(
            string usuarioId,
            string? perguntaUsuario,
            CancellationToken cancellationToken = default);
    }
}
