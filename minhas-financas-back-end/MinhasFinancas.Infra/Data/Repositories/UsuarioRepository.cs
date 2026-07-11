using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;



namespace MinhasFinancas.Infra.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {

        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteUsuarioAsync(string usuarioId)
        {
            return await _context.Users.AsNoTracking().AnyAsync(x => x.Id == usuarioId);
        }

        public async Task<List<string>> BuscarIdsUsuariosAtivosAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync();
        }

        public async Task<List<Usuario>> BuscarUsuariosParaLaboratorioAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(x => x.Nome ?? x.Email)
                .ThenBy(x => x.Email)
                .ToListAsync();
        }

        public async Task<List<Usuario>> BuscarUsuariosSinteticosAsync()
        {
            return await _context.Users
                .Where(x => x.EhUsuarioSintetico)
                .OrderBy(x => x.CodigoCenarioSimulacao)
                .ThenBy(x => x.Email)
                .ToListAsync();
        }

        public async Task<Usuario?> BuscarResumoUsuarioAsync(string usuarioId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == usuarioId);
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }


        public async Task DeletarUsuarioESeusDados(Usuario usuario)
        {
            var categorias = await _context.Categoria.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var cartoes = await _context.Cartao.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var contas = await _context.Conta.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var lancamentos = await _context.Lancamento.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var lancamentosFixos = await _context.LancamentoFixo.Where(x => lancamentos.Select(l => l.Id).Contains(x.LancamentoId)).ToListAsync();
            var lancamentosParcelados = await _context.LancamentoParcelado.Where(x => lancamentos.Select(l => l.Id).Contains(x.LancamentoId)).ToListAsync();
            var bemPatrimonial = await _context.BemPatrimonial.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var permanenciasBens = await _context.PermanenciaBemMaterial.Where(x => bemPatrimonial.Select(b => b.Id).Contains(x.BemPatrimonialId)).ToListAsync();
            var passivos = await _context.Passivo.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var permanenciasPassivos = await _context.Set<PermanenciaPassivo>().Where(x => passivos.Select(p => p.Id).Contains(x.PassivoId)).ToListAsync();
            var metas = await _context.Meta.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var aportesMetas = await _context.Set<AporteMeta>().Where(x => metas.Select(m => m.Id).Contains(x.MetaId)).ToListAsync();
            var snapshots = await _context.SnapshotPatrimonial.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var perfis = await _context.PerfilFinanceiro.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var configuracoesPerfis = await _context.ConfiguracaoPerfilFinanceiro.Where(x => perfis.Select(p => p.Id).Contains(x.PerfilFinanceiroId)).ToListAsync();
            var planos = await _context.PlanoEstrategicoFinanceiro.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var objetivosPlanos = await _context.ObjetivoPlanoEstrategico.Where(x => planos.Select(p => p.Id).Contains(x.PlanoEstrategicoFinanceiroId)).ToListAsync();
            var compromissos = await _context.CompromissoFinanceiro.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var projecoes = await _context.Projecao.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var rendasProjecao = await _context.RendaProjecao.Where(x => projecoes.Select(p => p.Id).Contains(x.ProjecaoId)).ToListAsync();
            var rendasExtras = await _context.RendaExtraProjecaoMensal.Where(x => projecoes.Select(p => p.Id).Contains(x.ProjecaoId)).ToListAsync();
            var dividasProjecao = await _context.DividaManualProjecaoMensal.Where(x => projecoes.Select(p => p.Id).Contains(x.ProjecaoId)).ToListAsync();
            var simulacoes = await _context.SimulacaoFinanceira.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var acoesSimulacao = await _context.AcaoSimulacaoFinanceira.Where(x => simulacoes.Select(s => s.Id).Contains(x.SimulacaoFinanceiraId)).ToListAsync();
            var analises = await _context.AnaliseFinanceiraHistorica.Where(x => x.UsuarioId == usuario.Id).ToListAsync();
            var historicosMfScore = await _context.HistoricoMfScore.Where(x => x.UsuarioId == usuario.Id).ToListAsync();

            if (usuario == null)
            {
                return;
            }

            _context.Set<AporteMeta>().RemoveRange(aportesMetas);
            _context.AcaoSimulacaoFinanceira.RemoveRange(acoesSimulacao);
            _context.RendaProjecao.RemoveRange(rendasProjecao);
            _context.RendaExtraProjecaoMensal.RemoveRange(rendasExtras);
            _context.DividaManualProjecaoMensal.RemoveRange(dividasProjecao);
            _context.LancamentoFixo.RemoveRange(lancamentosFixos);
            _context.LancamentoParcelado.RemoveRange(lancamentosParcelados);
            _context.PermanenciaBemMaterial.RemoveRange(permanenciasBens);
            _context.Set<PermanenciaPassivo>().RemoveRange(permanenciasPassivos);
            _context.ObjetivoPlanoEstrategico.RemoveRange(objetivosPlanos);
            _context.ConfiguracaoPerfilFinanceiro.RemoveRange(configuracoesPerfis);
            _context.AnaliseFinanceiraHistorica.RemoveRange(analises);
            _context.HistoricoMfScore.RemoveRange(historicosMfScore);
            _context.Meta.RemoveRange(metas);
            _context.BemPatrimonial.RemoveRange(bemPatrimonial);
            _context.Passivo.RemoveRange(passivos);
            _context.SnapshotPatrimonial.RemoveRange(snapshots);
            _context.CompromissoFinanceiro.RemoveRange(compromissos);
            _context.PlanoEstrategicoFinanceiro.RemoveRange(planos);
            _context.PerfilFinanceiro.RemoveRange(perfis);
            _context.Projecao.RemoveRange(projecoes);
            _context.SimulacaoFinanceira.RemoveRange(simulacoes);
            _context.Lancamento.RemoveRange(lancamentos);
            _context.Categoria.RemoveRange(categorias);
            _context.Cartao.RemoveRange(cartoes);
            _context.Conta.RemoveRange(contas);
            _context.Users.Remove(usuario);

            await _context.SaveChangesAsync();
        }
    }  
}
