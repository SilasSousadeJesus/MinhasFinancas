using MinhasFinancas.Application.DTOs.BemPatrimonial;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Services.Sorteios;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class SorteiosAppService : ISorteiosAppService
    {
        public SorteiosAppService()
        {

        }

        public async Task<RetornoGenerico> MegaSena()
        {
            var retorno = new RetornoGenerico();

            try
            {
                var sorteios = new Sorteios();

                var numMegaSena = sorteios.MegaSena();

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "numeros para jogo de mega sena gerados com sucesso";
                retorno.MensagemUsuario = "numeros para jogo de mega sena gerados com sucesso";
                retorno.Dados = numMegaSena;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possivel gerar os numeros na mega cena";
                retorno.Dados = null;
                return retorno;
            }
        }
    }
}
