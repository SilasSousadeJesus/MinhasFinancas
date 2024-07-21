namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> BuscarTodosOsElementosAsync(string id);
        Task<T> BuscarUmElementoAsync(string idPatrono, Guid id);
        Task CadastrarElementoAsync(T elemento);
        Task DeletarElementoAsync(T elemento);
        Task EditarElementoAsync(T elemento);
    }
}
