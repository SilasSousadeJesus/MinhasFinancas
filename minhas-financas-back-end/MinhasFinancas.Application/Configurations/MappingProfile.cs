using AutoMapper;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CadastroBancoDTO, Banco>();
            CreateMap<EditarBancoDTO, Banco>();
        }
    }
}
