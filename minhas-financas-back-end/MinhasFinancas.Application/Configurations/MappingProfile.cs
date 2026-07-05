using AutoMapper;
using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.DTOs.BemPatrimonial;
using MinhasFinancas.Application.DTOs.Cartao;
using MinhasFinancas.Application.DTOs.Categoria;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.DTOs.Meta;
using MinhasFinancas.Application.DTOs.Passivo;
using MinhasFinancas.Application.DTOs.Patrimonio;
using MinhasFinancas.Application.DTOs.Projecao;
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
            CreateMap<CadastrarSubCategoriaDTO, SubCategoria>();
            CreateMap<EditarSubCategoriaDTO, SubCategoria>();

            CreateMap<CadastrarLancamentoDTO, Lancamento>();
            CreateMap<EditarLancamentoDTO, Lancamento>();

            CreateMap<CadastrarBemPatrimonialDTO, BemPatrimonial>();
            CreateMap<EditarBemPatrimonialDTO, BemPatrimonial>();

            CreateMap<CadastrarPassivoDTO, Passivo>()
                .ForMember(dest => dest.NomePassivo, opt => opt.MapFrom(src => src.NomeBemPatrimonial));
            CreateMap<EditarPassivoDTO, Passivo>()
                .ForMember(dest => dest.NomePassivo, opt => opt.MapFrom(src => src.NomeBemPatrimonial));

            CreateMap<CadastrarSnapshotPatrimonialDTO, SnapshotPatrimonial>();


            CreateMap<CadastrarMetaDTO, Meta>();
            CreateMap<EditarMetalDTO, Meta>();

            CreateMap<CadastrarProjecaoDTO, Projecao>();
            CreateMap<EditarProjecaoDTO, Projecao>();
            CreateMap<RendaProjecaoDTO, RendaProjecao>();

            CreateMap<RendaExtraMensalProjecaoDTO, RendaExtraProjecaoMensal>()
                    .ForMember(dest => dest.MesReferencia,
                         opt => opt.MapFrom(src => DateTime.Parse(src.MesReferencia)));

            CreateMap<DividaManualMensalProjecaoDTO, DividaManualProjecaoMensal>()
                    .ForMember(dest => dest.MesReferencia,
                         opt => opt.MapFrom(src => DateTime.Parse(src.MesReferencia)));
        }
    }
}
