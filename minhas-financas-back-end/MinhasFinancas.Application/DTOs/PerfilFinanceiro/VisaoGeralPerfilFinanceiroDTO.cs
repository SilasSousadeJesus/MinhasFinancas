namespace MinhasFinancas.Application.DTOs.PerfilFinanceiro
{
    public class VisaoGeralPerfilFinanceiroDTO
    {
        public Guid? PerfilId { get; set; }
        public bool UsaPerfilFinanceiroInicial { get; set; }
        public ConfiguracaoPerfilFinanceiroDTO? ConfiguracaoVigente { get; set; }
        public List<ConfiguracaoPerfilFinanceiroDTO> Historico { get; set; } = [];
    }
}
