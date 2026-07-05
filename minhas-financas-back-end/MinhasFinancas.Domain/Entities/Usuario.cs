

using Microsoft.AspNetCore.Identity;

namespace MinhasFinancas.Domain.Entities
{
    public class Usuario : IdentityUser
    {
        public string? Nome  { get; set; }
        public virtual List<Conta>? Bancos { get; set; }
        public virtual List<Cartao>? Cartoes { get; set; }
        public virtual List<Lancamento>? Lancamentos { get; set; }
        public virtual List<Categoria>? Categorias { get; set; }
        public virtual List<BemPatrimonial>? BensPatrimoniais { get; set; }
        public virtual List<Passivo>? Passivos { get; set; }
        public virtual List<SnapshotPatrimonial>? SnapshotsPatrimoniais { get; set; }
        public virtual List<Meta>? Metas { get; set; }
        public virtual List<PerfilFinanceiro>? PerfisFinanceiros { get; set; }
        public virtual List<Projecao>? Projecoes { get; set; }
        public virtual List<SimulacaoFinanceira>? SimulacoesFinanceiras { get; set; }
    }
}
