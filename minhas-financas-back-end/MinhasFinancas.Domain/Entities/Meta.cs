using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class Meta
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NomeMeta { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorFinal { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorAtual { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorParaChegarNaMeta { get; set; } = decimal.Zero;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcentagemDaMeta { get; set; } = decimal.Zero;

        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public bool MetaAlcancada { get; set; } = false;



        [ForeignKey("UsuarioId")]
        public string? UsuarioId { get; set; }
        public List<AporteMeta> AportesMeta { get; set; } = new List<AporteMeta>();


        public void CalcularDiferenca() {
            this.ValorParaChegarNaMeta = this.ValorFinal - this.ValorAtual;
            if (this.ValorFinal > 0)
            {
                this.PorcentagemDaMeta = (this.ValorAtual / this.ValorFinal) * 100;
            }
            else
            {
                this.PorcentagemDaMeta = 0;
            }
            this.MetaAlcancada = this.ValorAtual >= this.ValorFinal;
        }
    }
}
