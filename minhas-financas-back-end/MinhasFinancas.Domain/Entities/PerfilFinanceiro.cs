namespace MinhasFinancas.Domain.Entities
{
    public class PerfilFinanceiro
    {
        public Guid Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }

        public virtual Usuario? Usuario { get; set; }
        public virtual List<ConfiguracaoPerfilFinanceiro> Configuracoes { get; set; } = [];
    }
}
