using MinhasFinancas.Infra.IA.Modelos;
using MinhasFinancas.Infra.IA.Especialistas.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas
{
    public interface IEspecialistasFinanceirosService
    {
        List<ParecerEspecialistaIA> Avaliar(ContextoAssistenteFinanceiro contexto);
    }
}
