namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> BuscarTodosOsElementosAsync();
        Task<T> BuscarUmElementoAsync(int id);
        Task CadastrarElementoAsync(T elemento);
        Task AtualizarElementoAsync(T elemento);
        Task DeletarElementoAsync(T elemento);
        Task EditarElementoAsync(T elemento);
    }
}
