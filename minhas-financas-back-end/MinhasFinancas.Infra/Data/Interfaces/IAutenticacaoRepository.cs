using MinhasFinancas.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhasFinancas.Infra.Data.Interfaces
{
    public interface IAutenticacaoRepository
    {
        Task<Usuario?> BuscarUsuarioPorEmail(string email);
    }
}
