using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class PlanoEstrategicoFinanceiro
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PlanoRaizId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public string UsuarioId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? Descricao { get; set; }

        [MaxLength(4000)]
        public string? Observacao { get; set; }

        public int NumeroVersao { get; set; } = 1;

        public DateTime DataInicioVigencia { get; set; }

        public DateTime? DataFimVigencia { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

        public bool Ativo { get; set; } = true;

        public Usuario? Usuario { get; set; }

        public List<ObjetivoPlanoEstrategico> Objetivos { get; set; } = [];
    }
}
