using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Application.DTOs.MfScoreLaboratorio;
using MinhasFinancas.Application.DTOs.Usuario;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Infra;
using MinhasFinancas.Infra.Data.Interfaces;

namespace MinhasFinancas.Application.Services
{
    public class GeradorBaseSimulacaoMfScoreService : IGeradorBaseSimulacaoMfScoreService
    {
        public const string OrigemBaseSimulacao = "MF_SCORE_SIMULACAO";
        public const string VersaoBaseSimulacao = "1.0";

        private const string SenhaPadraoUsuariosSinteticos = "MfScore2026";

        private readonly ApplicationDbContext _context;
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<Usuario> _userManager;

        public GeradorBaseSimulacaoMfScoreService(
            ApplicationDbContext context,
            IUsuarioAppService usuarioAppService,
            IUsuarioRepository usuarioRepository,
            UserManager<Usuario> userManager)
        {
            _context = context;
            _usuarioAppService = usuarioAppService;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
        }

        public async Task<ResultadoGeracaoBaseSimulacaoMfScoreDTO> GerarAsync()
        {
            var usuariosSinteticosExistentes = await _usuarioRepository.BuscarUsuariosSinteticosAsync();
            if (usuariosSinteticosExistentes.Count > 0)
            {
                throw new InvalidOperationException(
                    $"A Base Oficial de Simulação já existe com {usuariosSinteticosExistentes.Count} usuário(s) sintético(s). Limpe a base antes de gerar novamente.");
            }

            var agora = DateTime.UtcNow;
            var cenarios = CriarCenarios(agora);
            var usuariosGerados = new List<UsuarioMfScoreLaboratorioDTO>();

            foreach (var cenario in cenarios)
            {
                Usuario? usuario = null;

                try
                {
                    usuario = await CriarUsuarioSinteticoAsync(cenario, agora);
                    _context.ChangeTracker.Clear();
                    await PopularCenarioAsync(usuario, cenario, agora);
                    usuariosGerados.Add(MapearUsuarioSintetico(usuario));
                }
                catch
                {
                    if (usuario != null)
                    {
                        await RemoverUsuarioSinteticoParcialAsync(usuario.Id);
                    }

                    throw;
                }
            }

            return new ResultadoGeracaoBaseSimulacaoMfScoreDTO
            {
                VersaoBase = VersaoBaseSimulacao,
                QuantidadeCenarios = cenarios.Count,
                QuantidadeUsuariosGerados = usuariosGerados.Count,
                DataGeracao = agora,
                UsuariosGerados = usuariosGerados
            };
        }

        public async Task<ResultadoLimpezaBaseSimulacaoMfScoreDTO> LimparAsync()
        {
            var usuariosSinteticos = await _usuarioRepository.BuscarUsuariosSinteticosAsync();
            var codigos = usuariosSinteticos
                .Select(x => x.CodigoCenarioSimulacao ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (var usuario in usuariosSinteticos)
            {
                await _usuarioRepository.DeletarUsuarioESeusDados(usuario);
            }

            return new ResultadoLimpezaBaseSimulacaoMfScoreDTO
            {
                QuantidadeUsuariosRemovidos = usuariosSinteticos.Count,
                CodigosCenariosRemovidos = codigos
            };
        }

        private async Task<Usuario> CriarUsuarioSinteticoAsync(CenarioBaseSimulacao cenario, DateTime agora)
        {
            var cadastro = await _usuarioAppService.Cadastrar(new CadastrarUsuarioDTO
            {
                Nome = cenario.NomeUsuario,
                Email = cenario.Email,
                Senha = SenhaPadraoUsuariosSinteticos,
                ConfirmacaoSenha = SenhaPadraoUsuariosSinteticos
            });

            if (!cadastro.Sucesso)
            {
                throw new InvalidOperationException($"Não foi possível criar o usuário sintético do cenário {cenario.Codigo}. {cadastro.MensagemSistema}");
            }

            var usuario = await _usuarioRepository.BuscarPorEmailAsync(cenario.Email);
            if (usuario == null)
            {
                throw new InvalidOperationException($"O usuário sintético do cenário {cenario.Codigo} foi criado, mas não pôde ser localizado.");
            }

            usuario.EhUsuarioSintetico = true;
            usuario.OrigemUsuario = OrigemBaseSimulacao;
            usuario.CodigoCenarioSimulacao = cenario.Codigo;
            usuario.VersaoBaseSimulacao = VersaoBaseSimulacao;
            usuario.DataGeracaoBaseSimulacao = agora;
            usuario.DescricaoCenarioSimulacao = cenario.Descricao;
            usuario.ObjetivoCenarioSimulacao = cenario.Objetivo;

            var resultadoAtualizacao = await _userManager.UpdateAsync(usuario);
            if (!resultadoAtualizacao.Succeeded)
            {
                throw new InvalidOperationException($"Não foi possível marcar o usuário sintético do cenário {cenario.Codigo}.");
            }

            return usuario;
        }

        private async Task PopularCenarioAsync(Usuario usuario, CenarioBaseSimulacao cenario, DateTime agora)
        {
            var categorias = await _context.Categoria
                .Include(x => x.SubCategorias)
                .Where(x => x.UsuarioId == usuario.Id)
                .ToListAsync();

            var referencias = new ReferenciasCenario(
                BuscarCategoria(categorias, "Salario"),
                BuscarCategoria(categorias, "Renda Extra"),
                BuscarCategoria(categorias, "Pro labore"),
                BuscarCategoria(categorias, "Dividendos"),
                BuscarCategoria(categorias, "Casa"),
                BuscarCategoria(categorias, "Alimentacao"),
                BuscarCategoria(categorias, "Transporte"),
                BuscarCategoria(categorias, "Saude"),
                BuscarCategoria(categorias, "Educacao"),
                BuscarCategoria(categorias, "Lazer"),
                BuscarCategoria(categorias, "Assinaturas"),
                BuscarCategoria(categorias, "Outras Despesas"));

            var contaPrincipal = new Conta
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                NomeConta = "Conta principal",
                Descricao = "Conta corrente principal da base sintética",
                Instituicao = cenario.InstituicaoConta,
                Tipo = EnumTipoConta.Corrente,
                Saldo = cenario.SaldoConta,
                SaldoInvestimento = cenario.SaldoInvestimento
            };

            var contaReserva = new Conta
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                NomeConta = "Conta reserva",
                Descricao = "Conta de apoio para reserva e investimentos",
                Instituicao = cenario.InstituicaoConta,
                Tipo = EnumTipoConta.Investimento,
                Saldo = 0m,
                SaldoInvestimento = cenario.SaldoInvestimento
            };

            _context.Conta.AddRange(contaPrincipal, contaReserva);

            Cartao? cartao = null;
            if (cenario.PossuiCartao)
            {
                cartao = new Cartao
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    NomeCartao = "Cartão principal",
                    Descricao = "Cartão da base sintética",
                    Instituicao = cenario.InstituicaoCartao,
                    Bandeira = "Visa",
                    Ultimos4Digitos = cenario.Ultimos4DigitosCartao,
                    DiaFechamento = "10",
                    DiaVencimento = "15",
                    ContaPadraoPagamento = contaPrincipal.NomeConta,
                    Tipo = EnumTipoCartao.Credito,
                    Saldo = cenario.SaldoCartaoAtual
                };

                _context.Cartao.Add(cartao);
            }

            await ConfigurarPatrimonioBaseAsync(usuario.Id, cenario, agora);
            await CriarMetasAsync(usuario.Id, cenario, agora);
            await CriarPlanoECompromissosAsync(usuario.Id, cenario, agora);
            await PersonalizarPerfilFinanceiroAsync(usuario.Id, cenario, agora);

            var lancamentos = GerarLancamentos(usuario.Id, cenario, referencias, contaPrincipal, cartao, agora);
            _context.Lancamento.AddRange(lancamentos);

            await _context.SaveChangesAsync();
        }

        private async Task RemoverUsuarioSinteticoParcialAsync(string usuarioId)
        {
            _context.ChangeTracker.Clear();

            var usuarioParcial = await _context.Users.FirstOrDefaultAsync(x => x.Id == usuarioId);
            if (usuarioParcial == null)
            {
                return;
            }

            await _usuarioRepository.DeletarUsuarioESeusDados(usuarioParcial);
            _context.ChangeTracker.Clear();
        }

        private async Task ConfigurarPatrimonioBaseAsync(string usuarioId, CenarioBaseSimulacao cenario, DateTime agora)
        {
            var bensBase = await _context.BemPatrimonial
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId)
                .ToListAsync();

            var dinheiroEmConta = bensBase.FirstOrDefault(x => x.Tipo == EnumBemPatrimonial.DinheiroEmConta);
            var investimento = bensBase.FirstOrDefault(x => x.Tipo == EnumBemPatrimonial.Investimento);

            if (dinheiroEmConta != null)
            {
                _context.PermanenciaBemMaterial.Add(new PermanenciaBemMaterial
                {
                    Id = Guid.NewGuid(),
                    BemPatrimonialId = dinheiroEmConta.Id,
                    DataPermanencia = agora,
                    Valor = cenario.SaldoConta
                });
            }

            if (investimento != null)
            {
                _context.PermanenciaBemMaterial.Add(new PermanenciaBemMaterial
                {
                    Id = Guid.NewGuid(),
                    BemPatrimonialId = investimento.Id,
                    DataPermanencia = agora,
                    Valor = cenario.SaldoInvestimento
                });
            }

            foreach (var ativo in cenario.AtivosAdicionais)
            {
                var bemId = Guid.NewGuid();
                _context.BemPatrimonial.Add(new BemPatrimonial
                {
                    Id = bemId,
                    UsuarioId = usuarioId,
                    NomeBemPatrimonial = ativo.Nome,
                    Descricao = ativo.Descricao,
                    Tipo = ativo.Tipo,
                    DataCadastro = agora,
                    DataAquisicao = ativo.DataAquisicao,
                    Permanencia = true,
                    Ativo = true,
                    DataPermanencia =
                    [
                        new PermanenciaBemMaterial
                        {
                            Id = Guid.NewGuid(),
                            BemPatrimonialId = bemId,
                            DataPermanencia = agora,
                            Valor = ativo.ValorAtual
                        }
                    ]
                });
            }

            foreach (var passivo in cenario.Passivos)
            {
                var passivoId = Guid.NewGuid();
                _context.Passivo.Add(new Passivo
                {
                    Id = passivoId,
                    UsuarioId = usuarioId,
                    NomePassivo = passivo.Nome,
                    Descricao = passivo.Descricao,
                    Tipo = passivo.Tipo,
                    DataCadastro = agora,
                    DataInicio = passivo.DataInicio,
                    DataFim = passivo.DataFim,
                    Permanencia = true,
                    Ativo = true,
                    DataPermanencia =
                    [
                        new PermanenciaPassivo
                        {
                            Id = Guid.NewGuid(),
                            PassivoId = passivoId,
                            DataPermanencia = agora,
                            Valor = passivo.ValorAtual
                        }
                    ]
                });
            }
        }

        private async Task CriarMetasAsync(string usuarioId, CenarioBaseSimulacao cenario, DateTime agora)
        {
            foreach (var meta in cenario.Metas)
            {
                var entidade = new Meta
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    NomeMeta = meta.Nome,
                    ValorFinal = meta.ValorFinal,
                    ValorAtual = meta.ValorAtual,
                    DataInicio = agora.AddMonths(-6),
                    DataFim = agora.AddMonths(meta.PrazoMeses),
                };
                entidade.CalcularDiferenca();
                _context.Meta.Add(entidade);
            }

            await Task.CompletedTask;
        }

        private async Task CriarPlanoECompromissosAsync(string usuarioId, CenarioBaseSimulacao cenario, DateTime agora)
        {
            if (cenario.ObjetivosPlano.Count > 0)
            {
                var planoId = Guid.NewGuid();
                var plano = new PlanoEstrategicoFinanceiro
                {
                    Id = planoId,
                    PlanoRaizId = planoId,
                    UsuarioId = usuarioId,
                    Nome = $"Plano {cenario.Codigo}",
                    Descricao = cenario.Objetivo,
                    Observacao = cenario.Descricao,
                    NumeroVersao = 1,
                    DataInicioVigencia = agora.AddMonths(-2),
                    DataCadastro = agora.AddMonths(-2),
                    DataAtualizacao = agora.AddMonths(-2),
                    Ativo = true,
                    Objetivos = cenario.ObjetivosPlano.Select((objetivo, indice) => new ObjetivoPlanoEstrategico
                    {
                        Id = Guid.NewGuid(),
                        PlanoEstrategicoFinanceiroId = planoId,
                        Titulo = objetivo.Titulo,
                        Descricao = objetivo.Descricao,
                        Ordem = indice + 1,
                        Prioridade = objetivo.Prioridade,
                        Status = objetivo.Status,
                        ValorAlvo = objetivo.ValorObjetivo,
                        DataAlvo = objetivo.DataAlvo
                    }).ToList()
                };

                _context.PlanoEstrategicoFinanceiro.Add(plano);
            }

            foreach (var compromisso in cenario.Compromissos)
            {
                _context.CompromissoFinanceiro.Add(new CompromissoFinanceiro
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Descricao = compromisso.Descricao,
                    Observacoes = compromisso.Observacoes,
                    Origem = EnumOrigemCompromissoFinanceiro.Manual,
                    Status = compromisso.Status,
                    DataCriacao = agora.AddMonths(-1),
                    DataConclusao = compromisso.Status == EnumStatusCompromissoFinanceiro.Concluido ? agora.AddDays(-15) : null,
                    Ativo = true
                });
            }

            await Task.CompletedTask;
        }

        private async Task PersonalizarPerfilFinanceiroAsync(string usuarioId, CenarioBaseSimulacao cenario, DateTime agora)
        {
            if (!cenario.PersonalizarPerfilFinanceiro)
            {
                return;
            }

            var perfil = await _context.PerfilFinanceiro
                .AsNoTracking()
                .Include(x => x.Configuracoes)
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioId);

            var configuracaoAtual = perfil?.Configuracoes
                .Where(x => x.DataFimVigencia == null)
                .OrderByDescending(x => x.DataInicioVigencia)
                .FirstOrDefault();

            if (perfil == null || configuracaoAtual == null)
            {
                return;
            }

            await _context.ConfiguracaoPerfilFinanceiro
                .Where(x => x.Id == configuracaoAtual.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.DataFimVigencia, agora.AddDays(-1)));

            _context.ConfiguracaoPerfilFinanceiro.Add(new ConfiguracaoPerfilFinanceiro
            {
                Id = Guid.NewGuid(),
                PerfilFinanceiroId = perfil.Id,
                DataCriacao = agora,
                DataInicioVigencia = agora,
                PercentualEconomiaMensalDesejado = cenario.PercentualEconomiaDesejado,
                PercentualReservaEmergenciaDesejado = cenario.PercentualReservaDesejado,
                MesesReservaEmergenciaDesejados = cenario.MesesReservaDesejados,
                PercentualMaximoComprometimentoRenda = cenario.PercentualMaximoComprometimento,
                PercentualMaximoEndividamento = cenario.PercentualMaximoEndividamento,
                PercentualMinimoInvestimento = cenario.PercentualMinimoInvestimento,
                PatrimonioLiquidoAlvo = cenario.PatrimonioAlvo,
                Observacao = "Perfil sintético personalizado para calibração do MF Score.",
                OrigemPerfilFinanceiro = EnumOrigemPerfilFinanceiro.PersonalizadoPeloUsuario
            });
        }

        private List<Lancamento> GerarLancamentos(
            string usuarioId,
            CenarioBaseSimulacao cenario,
            ReferenciasCenario referencias,
            Conta contaPrincipal,
            Cartao? cartao,
            DateTime agora)
        {
            var lancamentos = new List<Lancamento>();
            var inicioMesAtual = new DateTime(agora.Year, agora.Month, 1);

            for (var offset = -11; offset <= 3; offset++)
            {
                var competencia = inicioMesAtual.AddMonths(offset);
                var indiceMes = offset + 11;

                AdicionarReceita(
                    lancamentos,
                    usuarioId,
                    cenario,
                    referencias.Salario,
                    referencias.ObterSubcategoria(referencias.Salario, cenario.SubcategoriaReceitaPrincipal),
                    "Receita principal",
                    cenario.CalcularReceitaPrincipal(indiceMes),
                    competencia,
                    5,
                    contaPrincipal,
                    agora);

                if (cenario.CalcularReceitaSecundaria(indiceMes) > 0)
                {
                    var categoriaSecundaria = cenario.UsaProLabore ? referencias.ProLabore : referencias.RendaExtra;
                    AdicionarReceita(
                        lancamentos,
                        usuarioId,
                        cenario,
                        categoriaSecundaria,
                        referencias.ObterSubcategoria(categoriaSecundaria, cenario.SubcategoriaReceitaSecundaria),
                        "Receita secundária",
                        cenario.CalcularReceitaSecundaria(indiceMes),
                        competencia,
                        20,
                        contaPrincipal,
                        agora);
                }

                if (cenario.CalcularDividendos(indiceMes) > 0)
                {
                    AdicionarReceita(
                        lancamentos,
                        usuarioId,
                        cenario,
                        referencias.Dividendos,
                        referencias.ObterSubcategoria(referencias.Dividendos, "FIIs"),
                        "Dividendos",
                        cenario.CalcularDividendos(indiceMes),
                        competencia,
                        12,
                        contaPrincipal,
                        agora);
                }

                AdicionarDespesaCasa(lancamentos, usuarioId, cenario, referencias, competencia, agora, contaPrincipal);
                AdicionarMercadoSemanal(lancamentos, usuarioId, cenario, referencias, competencia, agora, contaPrincipal);
                AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Transporte, "Combustivel", "Transporte", cenario.CalcularTransporte(indiceMes), competencia, 11, contaPrincipal, cartao, cenario.DespesasNoCartao, agora);
                AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Saude, "Farmacia", "Saúde", cenario.CalcularSaude(indiceMes), competencia, 18, contaPrincipal, null, false, agora);
                AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Educacao, cenario.SubcategoriaEducacao, "Educação", cenario.CalcularEducacao(indiceMes), competencia, 7, contaPrincipal, null, false, agora);
                AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Lazer, cenario.SubcategoriaLazer, "Lazer", cenario.CalcularLazer(indiceMes), competencia, 22, contaPrincipal, cartao, cenario.DespesasNoCartao, agora);
                AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Assinaturas, "Software", "Assinaturas", cenario.AssinaturasMensais, competencia, 9, contaPrincipal, null, false, agora);
                AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.OutrasDespesas, null, "Outras despesas", cenario.CalcularOutrasDespesas(indiceMes), competencia, 26, contaPrincipal, cartao, cenario.DespesasNoCartao, agora);

                if (cenario.ParcelaMensalCartao > 0)
                {
                    AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.OutrasDespesas, null, "Fatura do cartão", cenario.ParcelaMensalCartao, competencia, 15, contaPrincipal, null, false, agora);
                }
            }

            foreach (var parcelamento in cenario.Parcelamentos)
            {
                AdicionarParcelamento(lancamentos, usuarioId, cenario, referencias, contaPrincipal, cartao, parcelamento, agora);
            }

            cenario.CustomizarLancamentos?.Invoke(lancamentos, referencias, contaPrincipal, cartao, agora);

            return lancamentos;
        }

        private static void AdicionarReceita(
            List<Lancamento> lancamentos,
            string usuarioId,
            CenarioBaseSimulacao cenario,
            Categoria categoria,
            SubCategoria? subCategoria,
            string descricaoBase,
            decimal valor,
            DateTime competencia,
            int dia,
            Conta contaPrincipal,
            DateTime agora)
        {
            if (valor <= 0)
            {
                return;
            }

            var data = CriarDataSegura(competencia, dia);
            var (status, efetivacao) = ResolverStatusLancamento(data, EnumTipoLancamento.Receita, agora, false);

            lancamentos.Add(new Lancamento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Tipo = EnumTipoLancamento.Receita,
                FrequenciaLancamento = EnumTipoFrequenciaLancamento.Pontual,
                StatusLancamento = status,
                DataLancamento = data,
                DataVencimento = data,
                DataEfetivacao = efetivacao,
                Valor = decimal.Round(valor, 2),
                Descricao = $"{descricaoBase} {cenario.NomeUsuario}",
                Observacao = cenario.Codigo,
                CategoriaId = categoria.Id,
                SubCategoriaId = subCategoria?.Id,
                ContaId = contaPrincipal.Id,
                Vinculo = EnumVinculoLancamento.Conta
            });
        }

        private static void AdicionarDespesaCasa(
            List<Lancamento> lancamentos,
            string usuarioId,
            CenarioBaseSimulacao cenario,
            ReferenciasCenario referencias,
            DateTime competencia,
            DateTime agora,
            Conta contaPrincipal)
        {
            AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Casa, "Aluguel", "Aluguel", cenario.CalcularAluguel(competencia), competencia, 5, contaPrincipal, null, false, agora);
            AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Casa, "Energia", "Energia", cenario.CalcularEnergia(competencia), competencia, 8, contaPrincipal, null, false, agora);
            AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Casa, "Internet", "Internet", cenario.CalcularInternet(competencia), competencia, 10, contaPrincipal, null, false, agora);
        }

        private static void AdicionarMercadoSemanal(
            List<Lancamento> lancamentos,
            string usuarioId,
            CenarioBaseSimulacao cenario,
            ReferenciasCenario referencias,
            DateTime competencia,
            DateTime agora,
            Conta contaPrincipal)
        {
            if (cenario.MercadoSemanal <= 0)
            {
                return;
            }

            foreach (var dia in new[] { 6, 13, 20, 27 })
            {
                AdicionarDespesaPontual(lancamentos, usuarioId, cenario, referencias.Alimentacao, "Mercado", "Mercado semanal", cenario.MercadoSemanal, competencia, dia, contaPrincipal, null, false, agora);
            }
        }

        private static void AdicionarDespesaPontual(
            List<Lancamento> lancamentos,
            string usuarioId,
            CenarioBaseSimulacao cenario,
            Categoria categoria,
            string? nomeSubcategoria,
            string descricaoBase,
            decimal valor,
            DateTime competencia,
            int dia,
            Conta contaPrincipal,
            Cartao? cartao,
            bool usarCartao,
            DateTime agora)
        {
            if (valor <= 0)
            {
                return;
            }

            var data = CriarDataSegura(competencia, dia);
            var (status, efetivacao) = ResolverStatusLancamento(data, EnumTipoLancamento.Despesa, agora, false);
            var subcategoria = nomeSubcategoria == null ? null : categoria.SubCategorias?.FirstOrDefault(x => x.NomeSubCategoria == nomeSubcategoria);

            lancamentos.Add(new Lancamento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Tipo = EnumTipoLancamento.Despesa,
                FrequenciaLancamento = EnumTipoFrequenciaLancamento.Pontual,
                StatusLancamento = status,
                DataLancamento = data,
                DataVencimento = data,
                DataEfetivacao = efetivacao,
                Valor = decimal.Round(valor, 2),
                Descricao = $"{descricaoBase} {cenario.NomeUsuario}",
                Observacao = cenario.Codigo,
                CategoriaId = categoria.Id,
                SubCategoriaId = subcategoria?.Id,
                ContaId = usarCartao ? null : contaPrincipal.Id,
                CartaoId = usarCartao ? cartao?.Id : null,
                Vinculo = usarCartao && cartao != null ? EnumVinculoLancamento.CartaoCredito : EnumVinculoLancamento.Conta
            });
        }

        private static void AdicionarParcelamento(
            List<Lancamento> lancamentos,
            string usuarioId,
            CenarioBaseSimulacao cenario,
            ReferenciasCenario referencias,
            Conta contaPrincipal,
            Cartao? cartao,
            ParcelamentoSimulado parcelamento,
            DateTime agora)
        {
            var grupoId = Guid.NewGuid();
            for (var indice = 0; indice < parcelamento.TotalParcelas; indice++)
            {
                var competencia = new DateTime(parcelamento.DataInicial.Year, parcelamento.DataInicial.Month, 1).AddMonths(indice);
                var data = CriarDataSegura(competencia, parcelamento.DiaVencimento);
                var manterPendente = parcelamento.ParcelasEmAberto.Contains(indice + 1) && data < agora.Date;
                var (status, efetivacao) = ResolverStatusLancamento(data, EnumTipoLancamento.Despesa, agora, manterPendente);
                var categoria = parcelamento.Categoria == CategoriaParcelamento.Educacao ? referencias.Educacao : referencias.OutrasDespesas;
                var subCategoria = parcelamento.Categoria == CategoriaParcelamento.Educacao
                    ? referencias.ObterSubcategoria(referencias.Educacao, "Cursos")
                    : null;

                lancamentos.Add(new Lancamento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Tipo = EnumTipoLancamento.Despesa,
                    FrequenciaLancamento = EnumTipoFrequenciaLancamento.Parcelado,
                    StatusLancamento = status,
                    DataLancamento = data,
                    DataVencimento = data,
                    DataEfetivacao = efetivacao,
                    Valor = decimal.Round(parcelamento.ValorParcela, 2),
                    Descricao = $"{parcelamento.DescricaoBase} {indice + 1}/{parcelamento.TotalParcelas}",
                    Observacao = cenario.Codigo,
                    CategoriaId = categoria.Id,
                    SubCategoriaId = subCategoria?.Id,
                    CartaoId = cartao?.Id,
                    ContaId = cartao == null ? contaPrincipal.Id : null,
                    Vinculo = cartao == null ? EnumVinculoLancamento.Conta : EnumVinculoLancamento.CartaoCredito,
                    GrupoParcelamentoId = grupoId,
                    NumeroParcela = indice + 1,
                    TotalParcelas = parcelamento.TotalParcelas
                });
            }
        }

        private static (EnumStatusLancamento status, DateTime? dataEfetivacao) ResolverStatusLancamento(
            DateTime dataVencimento,
            EnumTipoLancamento tipo,
            DateTime agora,
            bool manterPendente)
        {
            if (manterPendente || dataVencimento.Date > agora.Date)
            {
                return (EnumStatusLancamento.Pendente, null);
            }

            return tipo == EnumTipoLancamento.Receita
                ? (EnumStatusLancamento.Recebido, dataVencimento)
                : (EnumStatusLancamento.Pago, dataVencimento);
        }

        private static DateTime CriarDataSegura(DateTime competencia, int dia)
        {
            var ultimoDia = DateTime.DaysInMonth(competencia.Year, competencia.Month);
            return new DateTime(competencia.Year, competencia.Month, Math.Min(dia, ultimoDia), 12, 0, 0, DateTimeKind.Utc);
        }

        private static Categoria BuscarCategoria(List<Categoria> categorias, string nome)
        {
            return categorias.First(x => x.NomeCategoria == nome);
        }

        private static UsuarioMfScoreLaboratorioDTO MapearUsuarioSintetico(Usuario usuario)
        {
            return new UsuarioMfScoreLaboratorioDTO
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome ?? "Usuário sintético",
                Email = usuario.Email ?? string.Empty,
                EhUsuarioSintetico = usuario.EhUsuarioSintetico,
                OrigemUsuario = usuario.OrigemUsuario ?? string.Empty,
                CodigoCenario = usuario.CodigoCenarioSimulacao ?? string.Empty,
                VersaoBase = usuario.VersaoBaseSimulacao ?? string.Empty,
                DataGeracaoBase = usuario.DataGeracaoBaseSimulacao,
                DescricaoCenario = usuario.DescricaoCenarioSimulacao ?? string.Empty,
                ObjetivoCenario = usuario.ObjetivoCenarioSimulacao ?? string.Empty
            };
        }

        private static List<CenarioBaseSimulacao> CriarCenarios(DateTime agora)
        {
            var hoje = agora.Date;
            return
            [
                new CenarioBaseSimulacao("MF-CENARIO-01", "Estudante Base", "Estudante que mora com os pais, tem renda parcial e poucos compromissos fixos.", "Validar perfil inicial com baixa renda, baixo patrimônio e despesas controladas sem ruptura.", 1600m, 150m, 0m, 0m, 55m, 0m, 0m, 140m, 60m, 320m, 40m, 0m, 80m, 0m, 900m, 0m, 0m, false, false, true, "Banco Base", "0000")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaReceitaSecundaria = "Freelance",
                    SubcategoriaEducacao = "Mensalidade",
                    SubcategoriaLazer = "Streaming",
                    PersonalizarPerfilFinanceiro = true,
                    PercentualEconomiaDesejado = 10m,
                    PercentualReservaDesejado = 100m,
                    MesesReservaDesejados = 3,
                    PercentualMaximoComprometimento = 45m,
                    PercentualMaximoEndividamento = 20m,
                    PercentualMinimoInvestimento = 5m,
                    PatrimonioAlvo = 15000m,
                    Metas =
                    [
                        new MetaSimulada("Notebook", 3500m, 900m, 8)
                    ],
                    ObjetivosPlano =
                    [
                        new ObjetivoPlanoSimulado("Montar reserva inicial", "Construir a primeira reserva para imprevistos.", EnumPrioridadeObjetivoPlanoEstrategico.Alta, EnumStatusObjetivoPlanoEstrategico.EmAndamento, 3000m, hoje.AddMonths(10))
                    ]
                },
                new CenarioBaseSimulacao("MF-CENARIO-02", "Primeiro Emprego", "Primeiro emprego formal, salário fixo e início de organização financeira.", "Validar transição entre vida financeira inicial e começo de estabilidade operacional.", 3100m, 220m, 980m, 105m, 90m, 150m, 100m, 215m, 70m, 145m, 55m, 95m, 105m, 700m, 0m, 300m, 0m, true, true, false, "Banco Base", "1111")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaReceitaSecundaria = "Bico",
                    SubcategoriaEducacao = "Cursos",
                    SubcategoriaLazer = "Passeios",
                    PersonalizarPerfilFinanceiro = true,
                    PercentualEconomiaDesejado = 15m,
                    PercentualReservaDesejado = 100m,
                    MesesReservaDesejados = 4,
                    PercentualMaximoComprometimento = 50m,
                    PercentualMaximoEndividamento = 25m,
                    PercentualMinimoInvestimento = 5m,
                    PatrimonioAlvo = 25000m,
                    Metas =
                    [
                        new MetaSimulada("Reserva inicial", 6000m, 700m, 12)
                    ],
                    Parcelamentos =
                    [
                        new ParcelamentoSimulado("Notebook profissional", 7, 90m, hoje.AddMonths(-2), 12, CategoriaParcelamento.OutrasDespesas, [])
                    ],
                    Compromissos =
                    [
                        new CompromissoSimulado("Guardar parte do décimo terceiro e manter a fatura do cartão sempre em dia.", "Compromisso de disciplina no início da vida financeira.", EnumStatusCompromissoFinanceiro.EmAndamento)
                    ]
                },
                new CenarioBaseSimulacao("MF-CENARIO-03", "CLT Organizado", "Profissional CLT organizado, com fluxo estável, reserva crescente e gastos sob controle.", "Validar perfil saudável de média renda com disciplina consistente.", 5200m, 0m, 1350m, 180m, 120m, 220m, 120m, 520m, 130m, 320m, 65m, 60m, 140m, 0m, 9500m, 4500m, 0m, true, true, true, "Banco Prime", "2222")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaEducacao = "Cursos",
                    SubcategoriaLazer = "Passeios",
                    DespesasNoCartao = true,
                    PersonalizarPerfilFinanceiro = true,
                    PercentualEconomiaDesejado = 20m,
                    PercentualReservaDesejado = 100m,
                    MesesReservaDesejados = 6,
                    PercentualMaximoComprometimento = 45m,
                    PercentualMaximoEndividamento = 25m,
                    PercentualMinimoInvestimento = 12m,
                    PatrimonioAlvo = 120000m,
                    Metas =
                    [
                        new MetaSimulada("Viagem internacional", 12000m, 4500m, 14)
                    ],
                    ObjetivosPlano =
                    [
                        new ObjetivoPlanoSimulado("Completar reserva de emergência", "Chegar a seis meses de despesas protegidas.", EnumPrioridadeObjetivoPlanoEstrategico.Alta, EnumStatusObjetivoPlanoEstrategico.EmAndamento, 18000m, hoje.AddMonths(9)),
                        new ObjetivoPlanoSimulado("Aumentar aportes mensais", "Elevar o investimento mensal após completar a reserva.", EnumPrioridadeObjetivoPlanoEstrategico.Media, EnumStatusObjetivoPlanoEstrategico.Planejado, 0m, hoje.AddMonths(16))
                    ],
                    Compromissos =
                    [
                        new CompromissoSimulado("Manter aporte automático no investimento todo início do mês.", null, EnumStatusCompromissoFinanceiro.EmAndamento)
                    ]
                },
                new CenarioBaseSimulacao("MF-CENARIO-04", "Alta Renda Forte", "Alta renda com disciplina elevada, boa reserva, investimentos e patrimônio em expansão.", "Validar cenário de baixo risco e alta maturidade financeira.", 14000m, 1800m, 2800m, 260m, 180m, 320m, 160m, 680m, 200m, 420m, 95m, 120m, 260m, 0m, 22000m, 38000m, 0m, true, true, true, "Banco Premium", "3333")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaReceitaSecundaria = "Consultoria",
                    SubcategoriaLazer = "Viagem",
                    DespesasNoCartao = true,
                    ParcelaMensalCartao = 950m,
                    PersonalizarPerfilFinanceiro = true,
                    PercentualEconomiaDesejado = 25m,
                    PercentualReservaDesejado = 100m,
                    MesesReservaDesejados = 6,
                    PercentualMaximoComprometimento = 35m,
                    PercentualMaximoEndividamento = 20m,
                    PercentualMinimoInvestimento = 20m,
                    PatrimonioAlvo = 450000m,
                    AtivosAdicionais =
                    [
                        new AtivoSimulado("Apartamento", "Imóvel quitado utilizado como patrimônio principal.", EnumBemPatrimonial.Imovel, 420000m, hoje.AddYears(-4)),
                        new AtivoSimulado("Veículo", "Veículo próprio de uso familiar.", EnumBemPatrimonial.Automovel, 78000m, hoje.AddYears(-2))
                    ],
                    ObjetivosPlano =
                    [
                        new ObjetivoPlanoSimulado("Elevar patrimônio investido", "Aumentar a parcela direcionada para investimentos recorrentes.", EnumPrioridadeObjetivoPlanoEstrategico.Alta, EnumStatusObjetivoPlanoEstrategico.EmAndamento, 500000m, hoje.AddMonths(24))
                    ],
                    Compromissos =
                    [
                        new CompromissoSimulado("Revisar gastos discricionários de viagem antes de cada trimestre.", null, EnumStatusCompromissoFinanceiro.EmAndamento)
                    ]
                },
                new CenarioBaseSimulacao("MF-CENARIO-05", "Alta Renda Caos", "Alta renda com padrão de consumo elevado, compras parceladas e pouca organização.", "Validar risco moderado-alto em perfil de renda alta com disciplina fraca.", 14500m, 600m, 4200m, 320m, 220m, 760m, 220m, 920m, 280m, 900m, 120m, 260m, 640m, 2500m, 6000m, 3500m, 18000m, true, false, false, "Banco Premium", "4444")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaReceitaSecundaria = "Comissao",
                    SubcategoriaLazer = "Viagem",
                    DespesasNoCartao = true,
                    ParcelaMensalCartao = 3800m,
                    PercentualEconomiaDesejado = 20m,
                    PercentualReservaDesejado = 100m,
                    MesesReservaDesejados = 6,
                    PercentualMaximoComprometimento = 45m,
                    PercentualMaximoEndividamento = 35m,
                    PercentualMinimoInvestimento = 10m,
                    PatrimonioAlvo = 200000m,
                    Parcelamentos =
                    [
                        new ParcelamentoSimulado("Home theater", 10, 720m, hoje.AddMonths(-3), 12, CategoriaParcelamento.OutrasDespesas, [])
                    ],
                    Passivos =
                    [
                        new PassivoSimulado("Empréstimo pessoal", "Empréstimo usado para cobrir excessos de consumo.", EnumPassivo.Emprestimo, 18000m, hoje.AddMonths(-5), hoje.AddMonths(18))
                    ]
                },
                new CenarioBaseSimulacao("MF-CENARIO-06", "Divida Organizada", "Usuário endividado, porém adimplente, com fluxo ajustado e plano de quitação.", "Validar cenário de dívida relevante sem inadimplência ativa.", 5200m, 0m, 1600m, 180m, 120m, 260m, 90m, 460m, 75m, 180m, 65m, 35m, 80m, 650m, 2400m, 300m, 26000m, true, true, true, "Banco Base", "5555")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaLazer = "Streaming",
                    DespesasNoCartao = true,
                    PersonalizarPerfilFinanceiro = true,
                    PercentualEconomiaDesejado = 12m,
                    PercentualReservaDesejado = 100m,
                    MesesReservaDesejados = 6,
                    PercentualMaximoComprometimento = 55m,
                    PercentualMaximoEndividamento = 50m,
                    PercentualMinimoInvestimento = 5m,
                    PatrimonioAlvo = 50000m,
                    Passivos =
                    [
                        new PassivoSimulado("Financiamento de veículo", "Financiamento em andamento, pago em dia.", EnumPassivo.Financiamento, 26000m, hoje.AddYears(-1), hoje.AddYears(2))
                    ],
                    ObjetivosPlano =
                    [
                        new ObjetivoPlanoSimulado("Quitar dívida principal", "Reduzir o financiamento antes de ampliar gastos discricionários.", EnumPrioridadeObjetivoPlanoEstrategico.Critica, EnumStatusObjetivoPlanoEstrategico.EmAndamento, 0m, hoje.AddMonths(20))
                    ],
                    Compromissos =
                    [
                        new CompromissoSimulado("Não contratar novas parcelas enquanto o financiamento atual não recuar.", null, EnumStatusCompromissoFinanceiro.EmAndamento)
                    ]
                },
                new CenarioBaseSimulacao("MF-CENARIO-07", "Atraso Leve", "Usuário com orçamento apertado e um atraso leve recente, mas ainda recuperável.", "Validar inadimplência leve sem colapso completo do score.", 4350m, 80m, 1540m, 150m, 110m, 190m, 95m, 260m, 70m, 130m, 55m, 125m, 290m, 1600m, 0m, 450m, 900m, true, false, false, "Banco Base", "6666")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaLazer = "Passeios",
                    Passivos =
                    [
                        new PassivoSimulado("Parcelamento emergencial", "Parcelamento ativo por gasto inesperado, mas já em redução.", EnumPassivo.Parcelamento, 900m, hoje.AddMonths(-4), hoje.AddMonths(5))
                    ],
                    CustomizarLancamentos = (lancamentos, referencias, conta, cartao, dataAtual) =>
                    {
                        var alvoEnergia = lancamentos
                            .Where(x => x.Tipo == EnumTipoLancamento.Despesa && x.Descricao.StartsWith("Energia"))
                            .OrderByDescending(x => x.DataVencimento)
                            .FirstOrDefault(x => x.DataVencimento < dataAtual);

                        if (alvoEnergia != null)
                        {
                            alvoEnergia.StatusLancamento = EnumStatusLancamento.Pendente;
                            alvoEnergia.DataEfetivacao = null;
                            alvoEnergia.DataVencimento = dataAtual.AddDays(-6);
                            alvoEnergia.DataLancamento = alvoEnergia.DataVencimento;
                            alvoEnergia.Valor = 180m;
                        }

                        var alvoOutras = lancamentos
                            .Where(x => x.Tipo == EnumTipoLancamento.Despesa && x.Descricao.StartsWith("Outras despesas"))
                            .OrderByDescending(x => x.DataVencimento)
                            .FirstOrDefault(x => x.DataVencimento < dataAtual);

                        if (alvoOutras != null)
                        {
                            alvoOutras.StatusLancamento = EnumStatusLancamento.Pendente;
                            alvoOutras.DataEfetivacao = null;
                            alvoOutras.DataVencimento = dataAtual.AddDays(-3);
                            alvoOutras.DataLancamento = alvoOutras.DataVencimento;
                            alvoOutras.Valor = 90m;
                        }
                    }
                },
                new CenarioBaseSimulacao("MF-CENARIO-08", "Atraso Grave", "Usuário com renda instável, atrasos relevantes e inadimplência materializada.", "Validar cenário de risco alto por inadimplência grave e pouca proteção.", 3900m, 250m, 1700m, 140m, 95m, 220m, 120m, 340m, 80m, 120m, 55m, 30m, 70m, 680m, 450m, 0m, 14500m, true, false, false, "Banco Base", "7777")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaReceitaSecundaria = "Bico",
                    DespesasNoCartao = true,
                    ParcelaMensalCartao = 1400m,
                    Passivos =
                    [
                        new PassivoSimulado("Empréstimo emergencial", "Empréstimo tomado após perda de fôlego financeiro.", EnumPassivo.Emprestimo, 14500m, hoje.AddMonths(-8), hoje.AddMonths(20))
                    ],
                    CustomizarLancamentos = (lancamentos, referencias, conta, cartao, dataAtual) =>
                    {
                        var alvos = lancamentos
                            .Where(x => x.Tipo == EnumTipoLancamento.Despesa && x.Descricao.StartsWith("Fatura do cartão"))
                            .Where(x => x.DataVencimento < dataAtual)
                            .OrderByDescending(x => x.DataVencimento)
                            .Take(2)
                            .ToList();

                        foreach (var alvo in alvos)
                        {
                            alvo.StatusLancamento = EnumStatusLancamento.Pendente;
                            alvo.DataEfetivacao = null;
                        }

                        if (alvos.Count > 0)
                        {
                            alvos[0].DataVencimento = dataAtual.AddDays(-75);
                            alvos[0].DataLancamento = alvos[0].DataVencimento;
                            alvos[0].Valor = 2800m;
                        }

                        if (alvos.Count > 1)
                        {
                            alvos[1].DataVencimento = dataAtual.AddDays(-38);
                            alvos[1].DataLancamento = alvos[1].DataVencimento;
                            alvos[1].Valor = 1200m;
                        }
                    }
                },
                new CenarioBaseSimulacao("MF-CENARIO-09", "Autonomo Reserva", "Autônomo com receitas variáveis, boa reserva e disciplina de proteção.", "Validar perfil volátil com proteção financeira madura.", 3400m, 2550m, 1180m, 145m, 105m, 215m, 115m, 300m, 55m, 170m, 50m, 115m, 0m, 6500m, 0m, 0m, 0m, true, true, false, "Banco Negócios", "8888")
                {
                    UsaProLabore = true,
                    SubcategoriaReceitaPrincipal = "Mensal",
                    SubcategoriaReceitaSecundaria = "Consultoria",
                    SubcategoriaLazer = "Passeios",
                    PersonalizarPerfilFinanceiro = true,
                    PercentualEconomiaDesejado = 25m,
                    PercentualReservaDesejado = 100m,
                    MesesReservaDesejados = 8,
                    PercentualMaximoComprometimento = 45m,
                    PercentualMaximoEndividamento = 20m,
                    PercentualMinimoInvestimento = 15m,
                    PatrimonioAlvo = 180000m,
                    AtivosAdicionais =
                    [
                        new AtivoSimulado("Reserva de liquidez diária", "Aplicação conservadora usada como colchão para meses fracos.", EnumBemPatrimonial.Investimento, 7500m, hoje.AddMonths(-14)),
                        new AtivoSimulado("Equipamentos de trabalho", "Computador e câmera usados para gerar renda como autônomo.", EnumBemPatrimonial.Equipamento, 9000m, hoje.AddYears(-1))
                    ],
                    ObjetivosPlano =
                    [
                        new ObjetivoPlanoSimulado("Manter reserva robusta", "Preservar caixa para absorver meses ruins do trabalho autônomo.", EnumPrioridadeObjetivoPlanoEstrategico.Alta, EnumStatusObjetivoPlanoEstrategico.EmAndamento, 30000m, hoje.AddMonths(6))
                    ],
                    Compromissos =
                    [
                        new CompromissoSimulado("Separar automaticamente parte das entradas fortes para reforçar a reserva dos meses fracos.", null, EnumStatusCompromissoFinanceiro.EmAndamento)
                    ],
                    CustomizarLancamentos = (lancamentos, referencias, conta, cartao, dataAtual) =>
                    {
                        var inicioAtual = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                        var multiplicadoresReceita = new Dictionary<int, decimal>
                        {
                            [-11] = 0.82m,
                            [-10] = 1.18m,
                            [-9] = 0.74m,
                            [-8] = 1.24m,
                            [-7] = 0.88m,
                            [-6] = 1.12m,
                            [-5] = 0.79m,
                            [-4] = 1.28m,
                            [-3] = 0.86m,
                            [-2] = 1.21m,
                            [-1] = 1.00m,
                            [0] = 1.04m,
                            [1] = 1.00m,
                            [2] = 1.08m,
                            [3] = 0.98m
                        };

                        foreach (var lancamento in lancamentos.Where(x => x.Tipo == EnumTipoLancamento.Receita))
                        {
                            var competencia = new DateTime(lancamento.DataVencimento.Year, lancamento.DataVencimento.Month, 1);
                            var offset = ((competencia.Year - inicioAtual.Year) * 12) + (competencia.Month - inicioAtual.Month);
                            if (multiplicadoresReceita.TryGetValue(offset, out var multiplicador))
                            {
                                lancamento.Valor = decimal.Round(lancamento.Valor * multiplicador, 2);
                            }
                        }

                        foreach (var lancamento in lancamentos.Where(x => x.Tipo == EnumTipoLancamento.Despesa && x.Descricao.StartsWith("Saúde")))
                        {
                            var competencia = new DateTime(lancamento.DataVencimento.Year, lancamento.DataVencimento.Month, 1);
                            var offset = ((competencia.Year - inicioAtual.Year) * 12) + (competencia.Month - inicioAtual.Month);
                            if (offset is -9 or -4 or -1)
                            {
                                lancamento.Valor = decimal.Round(lancamento.Valor * 1.35m, 2);
                            }
                        }
                    }
                },
                new CenarioBaseSimulacao("MF-CENARIO-10", "Autonomo Sem Res", "Autônomo sem reserva, com renda volátil e forte vulnerabilidade no curto prazo.", "Validar risco elevado em perfil volátil sem colchão financeiro.", 4600m, 1400m, 1650m, 150m, 110m, 240m, 130m, 420m, 100m, 260m, 75m, 70m, 130m, 300m, 800m, 0m, 3500m, true, false, false, "Banco Negócios", "9999")
                {
                    UsaProLabore = true,
                    SubcategoriaReceitaPrincipal = "Mensal",
                    SubcategoriaReceitaSecundaria = "Freelance",
                    DespesasNoCartao = true,
                    ParcelaMensalCartao = 780m,
                    Passivos =
                    [
                        new PassivoSimulado("Cheque especial recorrente", "Uso recorrente de limite para compensar variação de receita.", EnumPassivo.Divida, 3500m, hoje.AddMonths(-3), null)
                    ]
                },
                new CenarioBaseSimulacao("MF-CENARIO-11", "Familia Financia", "Família com financiamento saudável, escola, saúde e fluxo ainda equilibrado.", "Validar cenário familiar com obrigações elevadas, mas sob controle.", 9300m, 0m, 2800m, 220m, 140m, 420m, 220m, 850m, 180m, 420m, 110m, 180m, 220m, 1350m, 5200m, 6000m, 185000m, true, true, true, "Banco Familia", "1212")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaEducacao = "Mensalidade",
                    SubcategoriaLazer = "Passeios",
                    DespesasNoCartao = true,
                    PersonalizarPerfilFinanceiro = true,
                    PercentualEconomiaDesejado = 15m,
                    PercentualReservaDesejado = 100m,
                    MesesReservaDesejados = 6,
                    PercentualMaximoComprometimento = 55m,
                    PercentualMaximoEndividamento = 45m,
                    PercentualMinimoInvestimento = 8m,
                    PatrimonioAlvo = 350000m,
                    AtivosAdicionais =
                    [
                        new AtivoSimulado("Imóvel financiado", "Imóvel residencial da família.", EnumBemPatrimonial.Imovel, 320000m, hoje.AddYears(-2))
                    ],
                    Passivos =
                    [
                        new PassivoSimulado("Financiamento imobiliário", "Financiamento saudável, pago em dia.", EnumPassivo.Financiamento, 185000m, hoje.AddYears(-2), hoje.AddYears(18))
                    ],
                    Metas =
                    [
                        new MetaSimulada("Faculdade dos filhos", 40000m, 9000m, 30)
                    ],
                    ObjetivosPlano =
                    [
                        new ObjetivoPlanoSimulado("Ampliar reserva familiar", "Manter segurança para emergências da família.", EnumPrioridadeObjetivoPlanoEstrategico.Alta, EnumStatusObjetivoPlanoEstrategico.EmAndamento, 25000m, hoje.AddMonths(18))
                    ]
                },
                new CenarioBaseSimulacao("MF-CENARIO-12", "Patrimonio Fluxo", "Usuário com patrimônio alto, mas fluxo de caixa ruim e pouca folga operacional.", "Validar cenário em que riqueza acumulada não mascara deterioração operacional.", 7200m, 0m, 2400m, 220m, 150m, 650m, 180m, 840m, 160m, 520m, 95m, 190m, 280m, 900m, 3500m, 22000m, 25000m, true, true, true, "Banco Premium", "3434")
                {
                    SubcategoriaReceitaPrincipal = "CLT",
                    SubcategoriaLazer = "Viagem",
                    DespesasNoCartao = true,
                    ParcelaMensalCartao = 1800m,
                    AtivosAdicionais =
                    [
                        new AtivoSimulado("Apartamento alugado", "Imóvel como principal fonte de patrimônio.", EnumBemPatrimonial.Imovel, 520000m, hoje.AddYears(-6))
                    ],
                    Passivos =
                    [
                        new PassivoSimulado("Empréstimo de reorganização", "Empréstimo contratado para reorganizar o caixa.", EnumPassivo.Emprestimo, 25000m, hoje.AddMonths(-10), hoje.AddMonths(26))
                    ],
                    ObjetivosPlano =
                    [
                        new ObjetivoPlanoSimulado("Reequilibrar fluxo mensal", "Reduzir padrão de consumo para proteger o patrimônio acumulado.", EnumPrioridadeObjetivoPlanoEstrategico.Critica, EnumStatusObjetivoPlanoEstrategico.EmAndamento, 0m, hoje.AddMonths(8))
                    ],
                    Compromissos =
                    [
                        new CompromissoSimulado("Cortar despesas discricionárias até o caixa voltar ao azul com folga.", null, EnumStatusCompromissoFinanceiro.EmAndamento)
                    ]
                }
            ];
        }

        private sealed class ReferenciasCenario
        {
            public ReferenciasCenario(
                Categoria salario,
                Categoria rendaExtra,
                Categoria proLabore,
                Categoria dividendos,
                Categoria casa,
                Categoria alimentacao,
                Categoria transporte,
                Categoria saude,
                Categoria educacao,
                Categoria lazer,
                Categoria assinaturas,
                Categoria outrasDespesas)
            {
                Salario = salario;
                RendaExtra = rendaExtra;
                ProLabore = proLabore;
                Dividendos = dividendos;
                Casa = casa;
                Alimentacao = alimentacao;
                Transporte = transporte;
                Saude = saude;
                Educacao = educacao;
                Lazer = lazer;
                Assinaturas = assinaturas;
                OutrasDespesas = outrasDespesas;
            }

            public Categoria Salario { get; }
            public Categoria RendaExtra { get; }
            public Categoria ProLabore { get; }
            public Categoria Dividendos { get; }
            public Categoria Casa { get; }
            public Categoria Alimentacao { get; }
            public Categoria Transporte { get; }
            public Categoria Saude { get; }
            public Categoria Educacao { get; }
            public Categoria Lazer { get; }
            public Categoria Assinaturas { get; }
            public Categoria OutrasDespesas { get; }

            public SubCategoria? ObterSubcategoria(Categoria categoria, string? nomeSubCategoria)
            {
                if (string.IsNullOrWhiteSpace(nomeSubCategoria))
                {
                    return null;
                }

                return categoria.SubCategorias?.FirstOrDefault(x => x.NomeSubCategoria == nomeSubCategoria);
            }
        }

        private sealed class CenarioBaseSimulacao
        {
            public CenarioBaseSimulacao(
                string codigo,
                string nomeUsuario,
                string descricao,
                string objetivo,
                decimal receitaPrincipal,
                decimal receitaSecundaria,
                decimal aluguel,
                decimal energia,
                decimal internet,
                decimal transporte,
                decimal saude,
                decimal mercadoSemanal,
                decimal educacao,
                decimal lazer,
                decimal assinaturasMensais,
                decimal outrasDespesas,
                decimal parcelaMensalCartao,
                decimal saldoConta,
                decimal saldoInvestimento,
                decimal saldoCartaoAtual,
                decimal valorPassivoBase,
                bool possuiCartao,
                bool personalizarPerfilFinanceiro,
                bool despesasNoCartao,
                string instituicaoConta,
                string ultimos4DigitosCartao)
            {
                Codigo = codigo;
                NomeUsuario = nomeUsuario;
                Descricao = descricao;
                Objetivo = objetivo;
                ReceitaPrincipal = receitaPrincipal;
                ReceitaSecundaria = receitaSecundaria;
                Aluguel = aluguel;
                Energia = energia;
                Internet = internet;
                Transporte = transporte;
                Saude = saude;
                MercadoSemanal = mercadoSemanal;
                Educacao = educacao;
                Lazer = lazer;
                AssinaturasMensais = assinaturasMensais;
                OutrasDespesas = outrasDespesas;
                ParcelaMensalCartao = parcelaMensalCartao;
                SaldoConta = saldoConta;
                SaldoInvestimento = saldoInvestimento;
                SaldoCartaoAtual = saldoCartaoAtual;
                ValorPassivoBase = valorPassivoBase;
                PossuiCartao = possuiCartao;
                PersonalizarPerfilFinanceiro = personalizarPerfilFinanceiro;
                DespesasNoCartao = despesasNoCartao;
                InstituicaoConta = instituicaoConta;
                Ultimos4DigitosCartao = ultimos4DigitosCartao;
            }

            public string Codigo { get; }
            public string NomeUsuario { get; }
            public string Email => $"{Codigo.ToLowerInvariant()}@mfscore.local";
            public string Descricao { get; }
            public string Objetivo { get; }
            public decimal ReceitaPrincipal { get; }
            public decimal ReceitaSecundaria { get; }
            public decimal Aluguel { get; }
            public decimal Energia { get; }
            public decimal Internet { get; }
            public decimal Transporte { get; }
            public decimal Saude { get; }
            public decimal MercadoSemanal { get; }
            public decimal Educacao { get; }
            public decimal Lazer { get; }
            public decimal AssinaturasMensais { get; }
            public decimal OutrasDespesas { get; }
            public decimal ParcelaMensalCartao { get; set; }
            public decimal SaldoConta { get; }
            public decimal SaldoInvestimento { get; }
            public decimal SaldoCartaoAtual { get; }
            public decimal ValorPassivoBase { get; }
            public bool PossuiCartao { get; }
            public bool PersonalizarPerfilFinanceiro { get; set; }
            public bool DespesasNoCartao { get; set; }
            public string InstituicaoConta { get; }
            public string InstituicaoCartao => InstituicaoConta;
            public string Ultimos4DigitosCartao { get; }
            public bool UsaProLabore { get; set; }
            public string SubcategoriaReceitaPrincipal { get; set; } = "CLT";
            public string? SubcategoriaReceitaSecundaria { get; set; }
            public string SubcategoriaEducacao { get; set; } = "Cursos";
            public string SubcategoriaLazer { get; set; } = "Passeios";
            public decimal PercentualEconomiaDesejado { get; set; } = 20m;
            public decimal PercentualReservaDesejado { get; set; } = 100m;
            public int MesesReservaDesejados { get; set; } = 6;
            public decimal PercentualMaximoComprometimento { get; set; } = 50m;
            public decimal PercentualMaximoEndividamento { get; set; } = 50m;
            public decimal PercentualMinimoInvestimento { get; set; } = 10m;
            public decimal PatrimonioAlvo { get; set; }
            public List<AtivoSimulado> AtivosAdicionais { get; set; } = [];
            public List<PassivoSimulado> Passivos { get; set; } = [];
            public List<MetaSimulada> Metas { get; set; } = [];
            public List<ObjetivoPlanoSimulado> ObjetivosPlano { get; set; } = [];
            public List<CompromissoSimulado> Compromissos { get; set; } = [];
            public List<ParcelamentoSimulado> Parcelamentos { get; set; } = [];
            public Action<List<Lancamento>, ReferenciasCenario, Conta, Cartao?, DateTime>? CustomizarLancamentos { get; set; }

            public decimal CalcularReceitaPrincipal(int indiceMes)
            {
                return ReceitaPrincipal;
            }

            public decimal CalcularReceitaSecundaria(int indiceMes)
            {
                if (ReceitaSecundaria <= 0)
                {
                    return 0m;
                }

                return indiceMes % 3 switch
                {
                    0 => ReceitaSecundaria,
                    1 => decimal.Round(ReceitaSecundaria * 0.75m, 2),
                    _ => decimal.Round(ReceitaSecundaria * 1.15m, 2)
                };
            }

            public decimal CalcularDividendos(int indiceMes)
            {
                return SaldoInvestimento > 10000m ? decimal.Round(SaldoInvestimento * 0.004m, 2) : 0m;
            }

            public decimal CalcularAluguel(DateTime competencia) => Aluguel;
            public decimal CalcularEnergia(DateTime competencia) => Energia;
            public decimal CalcularInternet(DateTime competencia) => Internet;

            public decimal CalcularTransporte(int indiceMes)
            {
                return indiceMes % 4 == 0 ? decimal.Round(Transporte * 1.1m, 2) : Transporte;
            }

            public decimal CalcularSaude(int indiceMes)
            {
                return indiceMes % 5 == 0 ? decimal.Round(Saude * 1.35m, 2) : Saude;
            }

            public decimal CalcularEducacao(int indiceMes) => Educacao;
            public decimal CalcularLazer(int indiceMes) => indiceMes % 2 == 0 ? Lazer : decimal.Round(Lazer * 0.8m, 2);

            public decimal CalcularOutrasDespesas(int indiceMes)
            {
                return indiceMes % 6 == 0 ? decimal.Round(OutrasDespesas * 1.6m, 2) : OutrasDespesas;
            }
        }

        private sealed record AtivoSimulado(
            string Nome,
            string Descricao,
            EnumBemPatrimonial Tipo,
            decimal ValorAtual,
            DateTime? DataAquisicao);

        private sealed record PassivoSimulado(
            string Nome,
            string Descricao,
            EnumPassivo Tipo,
            decimal ValorAtual,
            DateTime? DataInicio,
            DateTime? DataFim);

        private sealed record MetaSimulada(
            string Nome,
            decimal ValorFinal,
            decimal ValorAtual,
            int PrazoMeses);

        private sealed record ObjetivoPlanoSimulado(
            string Titulo,
            string Descricao,
            EnumPrioridadeObjetivoPlanoEstrategico Prioridade,
            EnumStatusObjetivoPlanoEstrategico Status,
            decimal ValorObjetivo,
            DateTime? DataAlvo);

        private sealed record CompromissoSimulado(
            string Descricao,
            string? Observacoes,
            EnumStatusCompromissoFinanceiro Status);

        private sealed record ParcelamentoSimulado(
            string DescricaoBase,
            int TotalParcelas,
            decimal ValorParcela,
            DateTime DataInicial,
            int DiaVencimento,
            CategoriaParcelamento Categoria,
            int[] ParcelasEmAberto);

        private enum CategoriaParcelamento
        {
            Educacao = 0,
            OutrasDespesas = 1
        }
    }
}
