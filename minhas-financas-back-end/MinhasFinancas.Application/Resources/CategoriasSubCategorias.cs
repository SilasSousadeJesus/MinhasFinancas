using MinhasFinancas.Application.Resources.SeedModels;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using System.Text.Json;

namespace MinhasFinancas.Application.Resources
{
    public static class CategoriasSubCategorias
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static List<Categoria> ConstrutorCategoriasSubCategorias(string usuarioId)
        {
            var arquivo = Path.Combine(AppContext.BaseDirectory, "Resources", "categorias-iniciais.json");

            if (!File.Exists(arquivo))
            {
                throw new FileNotFoundException("Arquivo de categorias iniciais não encontrado.", arquivo);
            }

            var json = File.ReadAllText(arquivo);
            var seeds = JsonSerializer.Deserialize<List<CategoriaInicialSeed>>(json, JsonOptions) ?? new();

            var categorias = seeds.Select(seed =>
            {
                var categoriaId = Guid.NewGuid();

                return new Categoria
                {
                    Id = categoriaId,
                    Icone = seed.Icone,
                    NomeCategoria = seed.NomeCategoria,
                    Tipo = Enum.Parse<EnumTipoCategoria>(seed.Tipo, ignoreCase: true),
                    UsuarioId = usuarioId,
                    SubCategorias = seed.SubCategorias.Select(nome => new SubCategoria
                    {
                        Id = Guid.NewGuid(),
                        NomeSubCategoria = nome,
                        CategoriaId = categoriaId
                    }).ToList()
                };
            }).ToList();

            return categorias;
        }
    }
}
