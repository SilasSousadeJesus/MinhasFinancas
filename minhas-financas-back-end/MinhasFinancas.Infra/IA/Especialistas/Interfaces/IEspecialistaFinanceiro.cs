using MinhasFinancas.Infra.IA.Modelos;
using MinhasFinancas.Infra.IA.Especialistas.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas.Interfaces
{
    public interface IEspecialistaFinanceiro
    {
        string Nome { get; }
        ParecerEspecialistaIA Avaliar(ContextoAssistenteFinanceiro contexto);
    }
}
