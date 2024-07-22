using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Domain.Services.DashBoard
{
    public class LancamentosPorCategoriaDashboard
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Icone { get; set; }
        public List<Lancamento> Lancamentos { get; set; }
    }
}
