using MinhasFinancas.Infra.IA.Especialistas.Interfaces;
using MinhasFinancas.Infra.IA.Especialistas.Modelos;
using MinhasFinancas.Infra.IA.Modelos;

namespace MinhasFinancas.Infra.IA.Especialistas
{
    public class EspecialistasFinanceirosService : IEspecialistasFinanceirosService
    {
        private readonly IReadOnlyCollection<IEspecialistaFinanceiro> _especialistas;

        public EspecialistasFinanceirosService(IEnumerable<IEspecialistaFinanceiro> especialistas)
        {
            _especialistas = especialistas.ToList();
        }

        public List<ParecerEspecialistaIA> Avaliar(ContextoAssistenteFinanceiro contexto)
        {
            return _especialistas
                .Select(especialista => especialista.Avaliar(contexto))
                .Where(parecer => !string.IsNullOrWhiteSpace(parecer.NomeEspecialista))
                .ToList();
        }
    }
}
