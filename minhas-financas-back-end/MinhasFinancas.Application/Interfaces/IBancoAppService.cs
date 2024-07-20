using MinhasFinancas.Application.DTOs.Banco;
using MinhasFinancas.Application.DTOs.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhasFinancas.Application.Interfaces
{
    public interface IBancoAppService
    {
        Task<RetornoGenerico> CadastrarBanco(CadastroBancoDTO cadastroBancoDTO);
        Task<RetornoGenerico> BuscarUmBanco(string usuarioId, Guid BancoId);
        Task<RetornoGenerico> BuscarTodosBancos(string usuarioId);
        Task<RetornoGenerico> EditarBancos(string usuarioId, Guid BancoId, EditarBancoDTO EditarBancoDTO);
        Task<RetornoGenerico> DeletarBanco(string usuarioId, Guid BancoId);
    }
}
