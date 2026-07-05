using AutoMapper;
using MinhasFinancas.Application.DTOs.Patrimonio;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class PatrimonioAppService : IPatrimonioAppService
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IBemMaterialRepository _bemMaterialRepository;
        private readonly IPassivoRepository _passivoRepository;
        private readonly IPatrimonioRepository _patrimonioRepository;
        private readonly IMapper _mapper;

        public PatrimonioAppService(
            IUsuarioAppService usuarioAppService,
            IBemMaterialRepository bemMaterialRepository,
            IPassivoRepository passivoRepository,
            IPatrimonioRepository patrimonioRepository,
            IMapper mapper)
        {
            _usuarioAppService = usuarioAppService;
            _bemMaterialRepository = bemMaterialRepository;
            _passivoRepository = passivoRepository;
            _patrimonioRepository = patrimonioRepository;
            _mapper = mapper;
        }

        public async Task<RetornoGenerico> BuscarVisaoGeralAsync(string usuarioId)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var validacaoUsuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);

                if (!validacaoUsuario.Sucesso)
                {
                    retorno.Sucesso = validacaoUsuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = validacaoUsuario.MensagemSistema;
                    retorno.MensagemUsuario = validacaoUsuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var visaoGeral = await MontarVisaoGeralAsync(usuarioId);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Visão patrimonial carregada com sucesso";
                retorno.MensagemUsuario = "Patrimônio carregado com sucesso";
                retorno.Dados = visaoGeral;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível carregar o patrimônio";
                retorno.Dados = null;
                return retorno;
            }
        }

        public async Task<RetornoGenerico> GerarSnapshotAsync(string usuarioId, CadastrarSnapshotPatrimonialDTO snapshotDTO)
        {
            var retorno = new RetornoGenerico();

            try
            {
                var validacaoUsuario = await _usuarioAppService.BuscarUmUsuario(usuarioId);

                if (!validacaoUsuario.Sucesso)
                {
                    retorno.Sucesso = validacaoUsuario.Sucesso;
                    retorno.HttpStatusCode = HttpStatusCode.NotFound;
                    retorno.MensagemSistema = validacaoUsuario.MensagemSistema;
                    retorno.MensagemUsuario = validacaoUsuario.MensagemUsuario;
                    retorno.Dados = null;
                    return retorno;
                }

                var visaoGeral = await MontarVisaoGeralAsync(usuarioId);
                var snapshot = _mapper.Map<SnapshotPatrimonial>(snapshotDTO);

                snapshot.Id = Guid.NewGuid();
                snapshot.UsuarioId = usuarioId;
                snapshot.TotalAtivos = visaoGeral.Resumo.TotalAtivos;
                snapshot.TotalPassivos = visaoGeral.Resumo.TotalPassivos;
                snapshot.PatrimonioLiquido = visaoGeral.Resumo.PatrimonioLiquido;
                snapshot.DataCriacao = DateTime.Now;

                await _patrimonioRepository.CadastrarSnapshotAsync(snapshot);

                retorno.Sucesso = true;
                retorno.HttpStatusCode = HttpStatusCode.OK;
                retorno.MensagemSistema = "Snapshot patrimonial gerado com sucesso";
                retorno.MensagemUsuario = "Snapshot patrimonial gerado com sucesso";
                retorno.Dados = null;
                return retorno;
            }
            catch (Exception ex)
            {
                retorno.Sucesso = false;
                retorno.HttpStatusCode = HttpStatusCode.InternalServerError;
                retorno.MensagemSistema = $"{ex}";
                retorno.MensagemUsuario = "Não foi possível gerar o snapshot patrimonial";
                retorno.Dados = null;
                return retorno;
            }
        }

        private async Task<VisaoGeralPatrimonioDTO> MontarVisaoGeralAsync(string usuarioId)
        {
            var ativos = await _bemMaterialRepository.BuscarTodosOsElementosAsync(usuarioId);
            var passivos = await _passivoRepository.BuscarTodosOsElementosAsync(usuarioId);
            var snapshots = await _patrimonioRepository.BuscarSnapshotsAsync(usuarioId);

            var ativosDTO = ativos
                .Select(MapearAtivo)
                .OrderBy(x => x.Nome)
                .ToList();

            var passivosDTO = passivos
                .Select(MapearPassivo)
                .OrderBy(x => x.Nome)
                .ToList();

            var resumo = new ResumoPatrimonialDTO
            {
                QuantidadeAtivos = ativosDTO.Count,
                QuantidadePassivos = passivosDTO.Count,
                TotalAtivos = ativosDTO.Sum(x => x.ValorAtual),
                TotalPassivos = passivosDTO.Sum(x => x.ValorAtual),
            };

            resumo.PatrimonioLiquido = resumo.TotalAtivos - resumo.TotalPassivos;

            return new VisaoGeralPatrimonioDTO
            {
                Resumo = resumo,
                Ativos = ativosDTO,
                Passivos = passivosDTO,
                Snapshots = snapshots
                    .Select(x => new SnapshotPatrimonialDTO
                    {
                        Id = x.Id,
                        DataReferencia = x.DataReferencia,
                        TotalAtivos = x.TotalAtivos,
                        TotalPassivos = x.TotalPassivos,
                        PatrimonioLiquido = x.PatrimonioLiquido,
                        Observacao = x.Observacao,
                        DataCriacao = x.DataCriacao,
                    })
                    .ToList(),
                Evolucao = snapshots
                    .OrderBy(x => x.DataReferencia)
                    .ThenBy(x => x.DataCriacao)
                    .Select(x => new LinhaEvolucaoPatrimonialDTO
                    {
                        DataReferencia = x.DataReferencia,
                        TotalAtivos = x.TotalAtivos,
                        TotalPassivos = x.TotalPassivos,
                        PatrimonioLiquido = x.PatrimonioLiquido,
                    })
                    .ToList(),
            };
        }

        private static ItemAtivoPatrimonialDTO MapearAtivo(BemPatrimonial ativo)
        {
            var ultimaPermanencia = ativo.DataPermanencia
                ?.OrderByDescending(x => x.DataPermanencia)
                .FirstOrDefault();

            return new ItemAtivoPatrimonialDTO
            {
                Id = ativo.Id,
                Nome = ativo.NomeBemPatrimonial,
                Descricao = ativo.Descricao,
                Tipo = (int)ativo.Tipo,
                ValorAtual = ultimaPermanencia?.Valor ?? decimal.Zero,
                DataReferenciaValor = ultimaPermanencia?.DataPermanencia,
                DataAquisicao = ativo.DataAquisicao,
                Ativo = ativo.Ativo,
            };
        }

        private static ItemPassivoPatrimonialDTO MapearPassivo(Passivo passivo)
        {
            var ultimaPermanencia = passivo.DataPermanencia
                ?.OrderByDescending(x => x.DataPermanencia)
                .FirstOrDefault();

            return new ItemPassivoPatrimonialDTO
            {
                Id = passivo.Id,
                Nome = passivo.NomePassivo,
                Descricao = passivo.Descricao,
                Tipo = (int)passivo.Tipo,
                ValorAtual = ultimaPermanencia?.Valor ?? decimal.Zero,
                DataReferenciaValor = ultimaPermanencia?.DataPermanencia,
                DataInicio = passivo.DataInicio,
                DataFim = passivo.DataFim,
                Ativo = passivo.Ativo,
            };
        }
    }
}
