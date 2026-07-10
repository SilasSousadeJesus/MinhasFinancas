using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra;

namespace MinhasFinancas.API.Jobs
{
    public class BemPatrimonialJobs : IBemPatrimonialJobs
    {
        private readonly ApplicationDbContext _context;

        public BemPatrimonialJobs(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task FilaJobs()
        {
            await AtualizacaoAnualDePermanencia();
        }

        public async Task AtualizacaoAnualDePermanencia()
        {
            var listaBemMaterial = await _context.Set<BemPatrimonial>()
                .Include(p => p.DataPermanencia)
                .ToListAsync();

            if (!listaBemMaterial.Any())
            {
                return;
            }

            foreach (var bem in listaBemMaterial)
            {
                if (!bem.DataPermanencia.Any())
                {
                    continue;
                }

                var ultimaDataPermanencia = bem.DataPermanencia
                    .OrderByDescending(x => x.DataPermanencia)
                    .ToList();

                var novoAnoPermanencia = ultimaDataPermanencia[0].DataPermanencia.Year + 1;
                var novaData = new DateTime(novoAnoPermanencia, 1, 1);

                var novaDataPermanencia = new PermanenciaBemMaterial
                {
                    DataPermanencia = novaData,
                    BemPatrimonialId = bem.Id,
                    Valor = ultimaDataPermanencia[0].Valor,
                    Id = Guid.NewGuid()
                };

                await _context.Set<PermanenciaBemMaterial>().AddAsync(novaDataPermanencia);
            }

            await _context.SaveChangesAsync();
        }
    }
}
