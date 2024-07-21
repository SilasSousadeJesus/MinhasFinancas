using AutoMapper;
using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class LancamentoAppService : ILancamentoAppService
    {

        private readonly IMapper _mapper;
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IUsuarioAppService _usuarioAppService;
        public LancamentoAppService(IMapper mapper, ILancamentoRepository lancamentoRepository, IUsuarioAppService usuarioAppService)
        {
            _mapper = mapper;
            _lancamentoRepository = lancamentoRepository;
            _usuarioAppService = usuarioAppService;
        }

        public Task<RetornoGenerico> BuscarTodosOsElementosAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> BuscarUmElementoAsync(string usuarioId, Guid BancoId)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> CadastrarElementoAsync(CadastrarLancamento elementoDTO)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> DeletarElementoAsync(string idPatrono, Guid idElemento)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGenerico> EditarElementoAsync(string idPatrono, Guid elementoId, EditarLancamento elementoDTO)
        {
            throw new NotImplementedException();
        }
    }
}
