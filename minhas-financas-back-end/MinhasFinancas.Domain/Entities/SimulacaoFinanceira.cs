using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class SimulacaoFinanceira
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public DateTime DataInicial { get; set; }

        public int QuantidadeMeses { get; set; } = 12;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

        public bool Ativa { get; set; } = true;

        [ForeignKey("UsuarioId")]
        public string UsuarioId { get; set; } = string.Empty;
        public Usuario? Usuario { get; set; }

        public List<AcaoSimulacaoFinanceira> Acoes { get; set; } = new();
    }
}
