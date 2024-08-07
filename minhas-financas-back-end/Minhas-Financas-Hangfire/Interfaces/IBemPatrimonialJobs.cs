namespace Minhas_Financas_Hangfire.Interfaces
{
    public interface IBemPatrimonialJobs
    {
        Task AtualizacaoAnualDePermanencia();
        Task FilaJobs();
    }
}
