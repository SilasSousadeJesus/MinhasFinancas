namespace MinhasFinancas.API.Jobs
{
    public interface IBemPatrimonialJobs
    {
        Task AtualizacaoAnualDePermanencia();
        Task FilaJobs();
    }
}
