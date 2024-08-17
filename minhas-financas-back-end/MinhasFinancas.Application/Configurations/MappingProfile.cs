using AutoMapper;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.DTOs.BemPatrimonial;
using MinhasFinancas.Application.DTOs.Cartao;
using MinhasFinancas.Application.DTOs.Categoria;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.DTOs.Meta;
using MinhasFinancas.Application.DTOs.Passivo;
using MinhasFinancas.Domain.Entities;

namespace MinhasFinancas.Application.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CadastrarContaDTO, Conta>();
            CreateMap<EditarContaDTO, Conta>();

            CreateMap<CadastrarCartaoDTO, Cartao>();
            CreateMap<EditarCartaoDTO, Cartao>();

            CreateMap<CadastrarCategoriaDTO, Categoria>();
            CreateMap<EditarCategoriaDTO, Categoria>();

            CreateMap<CadastrarLancamentoDTO, Lancamento>();
            CreateMap<EditarLancamentoDTO, Lancamento>();

            CreateMap<CadastrarBemPatrimonialDTO, BemPatrimonial>();
            CreateMap<EditarBemPatrimonialDTO, BemPatrimonial>();

            CreateMap<CadastrarPassivoDTO, Passivo>();
            CreateMap<EditarPassivoDTO, Passivo>();


            CreateMap<CadastrarMetaDTO, Meta>();
            CreateMap<EditarMetalDTO, Meta>();
        }
    }
}
