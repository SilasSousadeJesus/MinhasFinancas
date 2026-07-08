using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.IA.Avaliadores;
using MinhasFinancas.Infra.IA.Construtores;
using MinhasFinancas.Infra.IA.Modelos;
using MinhasFinancas.Infra.IA.Provedores;

namespace MinhasFinancas.Infra.IA
{
    public class AssistenteFinanceiroService
    {
        private readonly ConstrutorContextoIA _construtorContextoIA;
        private readonly ConstrutorPromptIA _construtorPromptIA;
        private readonly IProvedorIA _provedorIA;
        private readonly AvaliadorConsistenciaEstrategica _avaliadorConsistenciaEstrategica;

        public AssistenteFinanceiroService(
            ConstrutorContextoIA construtorContextoIA,
            ConstrutorPromptIA construtorPromptIA,
            IProvedorIA provedorIA,
            AvaliadorConsistenciaEstrategica avaliadorConsistenciaEstrategica)
        {
            _construtorContextoIA = construtorContextoIA;
            _construtorPromptIA = construtorPromptIA;
            _provedorIA = provedorIA;
            _avaliadorConsistenciaEstrategica = avaliadorConsistenciaEstrategica;
        }

        public ContextoAssistenteFinanceiro PrepararContexto(
            ResumoFinanceiroIA resumoFinanceiroIA,
            string? perguntaUsuario = null,
            IEnumerable<MemoriaFinanceiraResumidaIA>? memoriaFinanceira = null,
            DecisaoFinanceiraIA? decisaoFinanceira = null,
            PlanoEstrategicoFinanceiro? planoEstrategicoFinanceiro = null,
            InterpretacaoPlanoEstrategicoIA? interpretacaoPlanoEstrategico = null,
            IEnumerable<CompromissoFinanceiro>? compromissosFinanceiros = null)
        {
            var consistenciaEstrategica = _avaliadorConsistenciaEstrategica.Avaliar(
                resumoFinanceiroIA,
                planoEstrategicoFinanceiro,
                interpretacaoPlanoEstrategico,
                perguntaUsuario,
                decisaoFinanceira);

            return _construtorContextoIA.Construir(
                resumoFinanceiroIA,
                perguntaUsuario,
                memoriaFinanceira,
                decisaoFinanceira,
                interpretacaoPlanoEstrategico,
                consistenciaEstrategica,
                compromissosFinanceiros);
        }

        public RequisicaoIA PrepararRequisicao(
            ResumoFinanceiroIA resumoFinanceiroIA,
            string? perguntaUsuario = null,
            IEnumerable<MemoriaFinanceiraResumidaIA>? memoriaFinanceira = null,
            DecisaoFinanceiraIA? decisaoFinanceira = null,
            PlanoEstrategicoFinanceiro? planoEstrategicoFinanceiro = null,
            InterpretacaoPlanoEstrategicoIA? interpretacaoPlanoEstrategico = null,
            IEnumerable<CompromissoFinanceiro>? compromissosFinanceiros = null)
        {
            var contexto = PrepararContexto(
                resumoFinanceiroIA,
                perguntaUsuario,
                memoriaFinanceira,
                decisaoFinanceira,
                planoEstrategicoFinanceiro,
                interpretacaoPlanoEstrategico,
                compromissosFinanceiros);
            return _construtorPromptIA.Construir(contexto);
        }

        public Task<RespostaIA> GerarRespostaAsync(
            RequisicaoIA requisicao,
            CancellationToken cancellationToken = default)
        {
            return _provedorIA.GerarRespostaAsync(requisicao, cancellationToken);
        }

        public Task<RespostaIA> GerarRespostaAsync(
            ResumoFinanceiroIA resumoFinanceiroIA,
            string? perguntaUsuario = null,
            CancellationToken cancellationToken = default)
        {
            var requisicao = PrepararRequisicao(resumoFinanceiroIA, perguntaUsuario);
            return GerarRespostaAsync(requisicao, cancellationToken);
        }

        public Task<RespostaIA> GerarRespostaSimuladaAsync(
            ResumoFinanceiroIA resumoFinanceiroIA,
            string? perguntaUsuario = null,
            CancellationToken cancellationToken = default)
        {
            return GerarRespostaAsync(resumoFinanceiroIA, perguntaUsuario, cancellationToken);
        }
    }
}
