using AutoMapper;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.DTOs.Cartao;
using MinhasFinancas.Application.DTOs.Categoria;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CadastrarBancoDTO, Banco>();
            CreateMap<EditarBancoDTO, Banco>();

            CreateMap<CadastrarCartaoDTO, Cartao>();
            CreateMap<EditarCartaoDTO, Cartao>();

            CreateMap<CadastrarCategoriaDTO, Categoria>();
            CreateMap<EditarCategoriaDTO, Categoria>();

            CreateMap<CadastrarLancamento, Lancamento>();
            CreateMap<EditarLancamento, Lancamento>();
        }
    }
}
