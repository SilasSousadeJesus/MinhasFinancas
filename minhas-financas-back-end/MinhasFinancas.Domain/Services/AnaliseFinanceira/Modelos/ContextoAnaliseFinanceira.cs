using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos
{
    public class ContextoAnaliseFinanceira
    {
        public DateTime DataReferencia { get; set; }
        public IReadOnlyCollection<Lancamento> Lancamentos { get; set; } = [];
        public IReadOnlyCollection<BemPatrimonial> Ativos { get; set; } = [];
        public IReadOnlyCollection<Passivo> Passivos { get; set; } = [];
        public IReadOnlyCollection<Meta> Metas { get; set; } = [];
        public ConfiguracaoPerfilFinanceiro? ConfiguracaoPerfilFinanceiro { get; set; }
    }
}
