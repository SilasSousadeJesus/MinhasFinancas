namespace MinhasFinancas.Application.Resources.SeedModels
{
    public class CategoriaInicialSeed
    {
        public string NomeCategoria { get; set; } = string.Empty;
        public string Icone { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public List<string> SubCategorias { get; set; } = new();
    }
}
