using MinhasFinancas.Application.DTOs.Lancamento;
using MinhasFinancas.Application.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class LancamentoAppService : ILancamentoAppService
    {
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
