namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> BuscarTodosOsElementosAsync(string Id);
        Task<T> BuscarUmElementoAsync(string idPatrono, Guid id);
        Task CadastrarElementoAsync(T elemento);
        Task AtualizarElementoAsync(T elemento);
        Task DeletarElementoAsync(T elemento);
        Task EditarElementoAsync(T elemento);
    }
}
