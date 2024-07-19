using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Domain.Entities
{
    public class Tag
    {

        // RELACIONAMENTO
        [ForeignKey("UsuarioId")]
        public Guid UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }


        // RELACIONAMENTO
        [ForeignKey("LancamentoId")]
        public Guid LancamentoId { get; set; }
        public virtual Lancamento LancamentoId { get; set; }
    }
}
