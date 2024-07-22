using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.Resources
{
    public static class CategoriasSubCategorias
    {
        public static List<Categoria> ConstrutorCategoriasSubCategorias(string UsuarioId) {

           var listaCategoriasSubcategorias =  new List<Categoria>
                                        {
                                            // receitas                                                
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Salario",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Aposentadoria",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },
                                             new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Dividendo",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },
                                             new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Décimo Terceiro Salário",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },

                                             new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Férias",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },

                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Horas Extras",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Outra Receita",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },
                                             new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Renda Extra",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },
                                             new Categoria
                                            {

                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Resgate de Investimento",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Receita.png",
                                                NomeCategoria = "Restituição de Imposto",
                                                Tipo = EnumTipoCategoria.Receita,
                                                UsuarioId = UsuarioId
                                            },

                                            // Despesas
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Casa",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                                SubCategorias = new List<SubCategoria> {
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Condominio",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                     new SubCategoria{
                                                        NomeSubCategoria = "Celular / telefone",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Consumo de Água",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                   new SubCategoria{
                                                        NomeSubCategoria = "Móveis",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                   new SubCategoria{
                                                        NomeSubCategoria = "Eletricidade",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                   new SubCategoria{
                                                        NomeSubCategoria = "Gás",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                   new SubCategoria{
                                                        NomeSubCategoria = "Internet",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },new SubCategoria{
                                                        NomeSubCategoria = "TV",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },new SubCategoria{
                                                        NomeSubCategoria = "Streaming",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    }
                                                   ,new SubCategoria{
                                                        NomeSubCategoria = "Impostos Residenciais",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    }
                                                   ,new SubCategoria{
                                                        NomeSubCategoria = "Manutenção",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },new SubCategoria{
                                                        NomeSubCategoria = "Pagamento da Casa",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    }
                                                   ,new SubCategoria{
                                                        NomeSubCategoria = "Faxina da Casa",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    }
                                                    ,new SubCategoria{
                                                        NomeSubCategoria = "Outros Gastos com a Casa",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    }
                                                }
                                            },
                                           new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Alimentação",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                                SubCategorias = new List<SubCategoria>{
                                                        new SubCategoria{
                                                        NomeSubCategoria = "Feira",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Padaria",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Restaurante",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Mercado",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Outros Alimentos",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                }
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Transporte",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                                SubCategorias = new List<SubCategoria>{
                                                        new SubCategoria{
                                                        NomeSubCategoria = "Combustível",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Estacionamento",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Pagamento de Transporte",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Taxas de Transporte",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Limpeza de Transporte",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Licença",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Conserto de Transporte",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                     new SubCategoria{
                                                        NomeSubCategoria = "Transporte",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "IPVA",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                     new SubCategoria{
                                                        NomeSubCategoria = "Taxas de Seguro",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Outros Gastos com Transporte",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Financiamento Veiculo",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                }
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Educação",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                                 SubCategorias = new List<SubCategoria>{
                                                        new SubCategoria{
                                                        NomeSubCategoria = "Material",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Entrada",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Curso",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Transporte para Educação",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Outros Gastos com Educação",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                }
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Cuidados Pessoais",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                              SubCategorias = new List<SubCategoria>{
                                                        new SubCategoria{
                                                        NomeSubCategoria = "Academia",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Corte de Cabelo",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Unhas",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Outros Cuidados Pessoais",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                }
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Saúde",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                              SubCategorias = new List<SubCategoria>{
                                                        new SubCategoria{
                                                        NomeSubCategoria = "Plano de Saúde",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Dentista",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Consultas Avulso",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    }
                                                }
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Entretetimento",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                              SubCategorias = new List<SubCategoria>{
                                                        new SubCategoria{
                                                        NomeSubCategoria = "Cinema",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Shows",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Outras Atividades de Lazer",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                }
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Vestuario",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                                SubCategorias = new List<SubCategoria>{
                                                        new SubCategoria{
                                                        NomeSubCategoria = "Acessórios",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Calçados",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Roupas",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Outros Gastos com Roupas",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                }
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Doações/Presentes",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Animais de Estimação",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId,
                                                SubCategorias = new List<SubCategoria>{
                                                        new SubCategoria{
                                                        NomeSubCategoria = "Ração",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Medicamentos",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Veterinarios",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                    new SubCategoria{
                                                        NomeSubCategoria = "Outros Gastos com Animais",
                                                        CategoriaId = Guid.Empty,
                                                        Id = Guid.NewGuid()
                                                    },
                                                }
                                            } ,
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Despesa.png",
                                                NomeCategoria = "Outras Despesas",
                                                Tipo = EnumTipoCategoria.Despesa,
                                                UsuarioId = UsuarioId
                                            },

                                            // Investimento
                                             new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Economias",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Fundo de Emergência",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Fundo de Investimento",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "LCI/LCA",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId,
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Outra Renda",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Previdência Privada",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                           },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Renda Fixa",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Renda Fixa",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Renda Variável",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            },
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "Tesouro Direto",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            }                                         ,
                                            new Categoria
                                            {
                                                Id = Guid.NewGuid(),
                                                Icone = "Investimento.png",
                                                NomeCategoria = "RDB",
                                                Tipo = EnumTipoCategoria.Investimento,
                                                UsuarioId = UsuarioId
                                            }
                                        };

            foreach (var categoria in listaCategoriasSubcategorias)
            {
                if (categoria.SubCategorias != null && categoria.SubCategorias.Any())
                {
                    foreach (var item in categoria.SubCategorias)
                    {
                        item.CategoriaId = categoria.Id;
                    }
                }
            }
            return listaCategoriasSubcategorias;
        }
    }
}
