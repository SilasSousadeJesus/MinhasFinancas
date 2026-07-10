using MinhasFinancas.Application.DTOs.MfScorePersona;
using MinhasFinancas.Application.Interfaces;
using MinhasFinancas.CrossCutting.Util.Enum;
using MinhasFinancas.Domain.Entities;
using MinhasFinancas.Domain.Services.AnaliseFinanceira;
using MinhasFinancas.Domain.Services.AnaliseFinanceira.Modelos;
using MinhasFinancas.Infra.Data.Interfaces;
using System.Net;

namespace MinhasFinancas.Application.Services
{
    public class MfScorePersonaAppService : IMfScorePersonaAppService
    {
        private static readonly DateTime DataReferenciaCalibracao = new(2026, 7, 1);

        private readonly IMfScorePersonaRepository _repository;
        private readonly IIndicadoresFinanceirosService _indicadoresFinanceirosService;
        private readonly ISaudeFinanceiraService _saudeFinanceiraService;

        public MfScorePersonaAppService(
            IMfScorePersonaRepository repository,
            IIndicadoresFinanceirosService indicadoresFinanceirosService,
            ISaudeFinanceiraService saudeFinanceiraService)
        {
            _repository = repository;
            _indicadoresFinanceirosService = indicadoresFinanceirosService;
            _saudeFinanceiraService = saudeFinanceiraService;
        }

        public async Task<RetornoGenerico> BuscarTodasAsync()
        {
            try
            {
                var personas = await _repository.BuscarTodasAsync();
                var dados = personas.Select(Mapear).ToList();

                return new RetornoGenerico(
                    true,
                    $"{dados.Count} persona(s) encontrada(s).",
                    $"{dados.Count} persona(s) carregada(s) com sucesso.",
                    HttpStatusCode.OK,
                    dados);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível listar as personas do MF Score.");
            }
        }

        public async Task<RetornoGenerico> BuscarUmaAsync(Guid personaId)
        {
            try
            {
                var persona = await _repository.BuscarUmaAsync(personaId);
                if (persona == null)
                {
                    return CriarNaoEncontrado("Persona do MF Score não encontrada.");
                }

                return new RetornoGenerico(
                    true,
                    "Persona do MF Score carregada com sucesso.",
                    "Persona do MF Score carregada com sucesso.",
                    HttpStatusCode.OK,
                    Mapear(persona));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível carregar a persona do MF Score.");
            }
        }

        public async Task<RetornoGenerico> CadastrarAsync(SalvarMfScorePersonaDTO dto)
        {
            try
            {
                var validacao = Validar(dto);
                if (validacao != null)
                {
                    return validacao;
                }

                var agora = DateTime.UtcNow;
                var persona = new PersonaMfScore
                {
                    Id = Guid.NewGuid(),
                    DataCriacao = agora,
                    DataAtualizacao = agora
                };

                AplicarDados(persona, dto, agora);

                await _repository.AdicionarAsync(persona);
                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(
                    true,
                    "Persona do MF Score criada com sucesso.",
                    "Persona do MF Score criada com sucesso.",
                    HttpStatusCode.OK,
                    Mapear(persona));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível criar a persona do MF Score.");
            }
        }

        public async Task<RetornoGenerico> EditarAsync(Guid personaId, SalvarMfScorePersonaDTO dto)
        {
            try
            {
                var validacao = Validar(dto);
                if (validacao != null)
                {
                    return validacao;
                }

                var persona = await _repository.BuscarUmaGerenciadaAsync(personaId);
                if (persona == null)
                {
                    return CriarNaoEncontrado("Persona do MF Score não encontrada.");
                }

                AplicarDados(persona, dto, DateTime.UtcNow);
                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(
                    true,
                    "Persona do MF Score atualizada com sucesso.",
                    "Persona do MF Score atualizada com sucesso.",
                    HttpStatusCode.OK,
                    Mapear(persona));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível editar a persona do MF Score.");
            }
        }

        public async Task<RetornoGenerico> InativarAsync(Guid personaId)
        {
            try
            {
                var persona = await _repository.BuscarUmaGerenciadaAsync(personaId);
                if (persona == null)
                {
                    return CriarNaoEncontrado("Persona do MF Score não encontrada.");
                }

                persona.Status = EnumStatusPersonaMfScore.Inativa;
                persona.EhCasoCanonico = false;
                persona.DataAtualizacao = DateTime.UtcNow;

                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(
                    true,
                    "Persona do MF Score inativada com sucesso.",
                    "Persona do MF Score inativada com sucesso.",
                    HttpStatusCode.OK,
                    null);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível inativar a persona do MF Score.");
            }
        }

        public async Task<RetornoGenerico> RodarScoreAsync(Guid personaId)
        {
            try
            {
                var persona = await _repository.BuscarUmaAsync(personaId);
                if (persona == null)
                {
                    return CriarNaoEncontrado("Persona do MF Score não encontrada.");
                }

                if (persona.Status == EnumStatusPersonaMfScore.Inativa)
                {
                    return CriarErroNegocio("Personas inativas não podem rodar o MF Score.");
                }

                var contexto = CriarContexto(persona);
                var indicadores = _indicadoresFinanceirosService.Calcular(contexto);
                var contextoComplementar = ConstrutorContextoComplementarMfScoreFinanceiro.Construir(contexto);
                var painel = _saudeFinanceiraService.GerarPainel(indicadores, contextoComplementar);
                var mfScore = painel.Resumo.MfScore;

                var dto = new ResultadoRodarMfScorePersonaDTO
                {
                    PersonaId = persona.Id,
                    Persona = persona.Nome,
                    Descricao = persona.Descricao,
                    MfScoreBase = mfScore.PontuacaoBase,
                    MfScoreFinal = mfScore.PontuacaoFinal,
                    Classificacao = mfScore.Classificacao,
                    Risco = mfScore.Risco,
                    PenalidadeTotal = mfScore.PenalidadeTotal,
                    ScoreHumanoSugerido = persona.ScoreHumanoSugerido,
                    FaixaEsperadaMin = persona.FaixaEsperadaMin,
                    FaixaEsperadaMax = persona.FaixaEsperadaMax,
                    DiferencaScoreHumano = persona.ScoreHumanoSugerido.HasValue
                        ? mfScore.PontuacaoFinal - persona.ScoreHumanoSugerido.Value
                        : null,
                    DentroDaFaixaEsperada = persona.FaixaEsperadaMin.HasValue && persona.FaixaEsperadaMax.HasValue
                        ? mfScore.PontuacaoFinal >= persona.FaixaEsperadaMin.Value && mfScore.PontuacaoFinal <= persona.FaixaEsperadaMax.Value
                        : null,
                    ObservacaoComparativa = MontarObservacaoComparativa(persona, mfScore.PontuacaoFinal),
                    Pilares = mfScore.Pilares
                        .Select(pilar => new ResultadoPilarMfScorePersonaDTO
                        {
                            Pilar = pilar.Nome,
                            Nota = pilar.Nota,
                            Peso = pilar.Peso,
                            Descricao = pilar.Descricao
                        })
                        .ToList(),
                    IndicadoresCriticos = mfScore.IndicadoresCriticos
                        .Select(indicador => new ResultadoIndicadorCriticoMfScorePersonaDTO
                        {
                            Indicador = indicador.Nome,
                            PilarRelacionado = indicador.PilarRelacionado,
                            Penalidade = indicador.Penalidade,
                            Motivo = indicador.Motivo
                        })
                        .ToList(),
                    PenalizacoesAplicadas = mfScore.RegrasCriticasAplicadas
                };

                return new RetornoGenerico(
                    true,
                    "MF Score da persona calculado com sucesso.",
                    "MF Score da persona calculado com sucesso.",
                    HttpStatusCode.OK,
                    dto);
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível rodar o MF Score da persona.");
            }
        }

        public async Task<RetornoGenerico> MarcarAuditadaAsync(Guid personaId)
        {
            try
            {
                var persona = await _repository.BuscarUmaGerenciadaAsync(personaId);
                if (persona == null)
                {
                    return CriarNaoEncontrado("Persona do MF Score não encontrada.");
                }

                if (persona.Status == EnumStatusPersonaMfScore.Inativa)
                {
                    return CriarErroNegocio("Personas inativas não podem ser auditadas.");
                }

                var validacaoAuditoria = ValidarAuditoriaHumana(persona);
                if (validacaoAuditoria != null)
                {
                    return validacaoAuditoria;
                }

                persona.Status = EnumStatusPersonaMfScore.Auditada;
                persona.EhCasoCanonico = false;
                persona.DataAtualizacao = DateTime.UtcNow;

                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(
                    true,
                    "Persona marcada como auditada com sucesso.",
                    "Persona marcada como auditada com sucesso.",
                    HttpStatusCode.OK,
                    Mapear(persona));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível marcar a persona como auditada.");
            }
        }

        public async Task<RetornoGenerico> MarcarCasoCanonicoAsync(Guid personaId)
        {
            try
            {
                var persona = await _repository.BuscarUmaGerenciadaAsync(personaId);
                if (persona == null)
                {
                    return CriarNaoEncontrado("Persona do MF Score não encontrada.");
                }

                if (persona.Status == EnumStatusPersonaMfScore.Inativa)
                {
                    return CriarErroNegocio("Personas inativas não podem virar caso canônico.");
                }

                if (persona.Status != EnumStatusPersonaMfScore.Auditada)
                {
                    return CriarErroNegocio("A persona precisa estar marcada como auditada antes de virar caso canônico.");
                }

                var validacaoAuditoria = ValidarAuditoriaHumana(persona);
                if (validacaoAuditoria != null)
                {
                    return validacaoAuditoria;
                }

                persona.Status = EnumStatusPersonaMfScore.CasoCanonico;
                persona.EhCasoCanonico = true;
                persona.DataAtualizacao = DateTime.UtcNow;

                await _repository.SalvarAlteracoesAsync();

                return new RetornoGenerico(
                    true,
                    "Persona marcada como caso canônico com sucesso.",
                    "Persona marcada como caso canônico com sucesso.",
                    HttpStatusCode.OK,
                    Mapear(persona));
            }
            catch (Exception ex)
            {
                return CriarErro(ex, "Não foi possível marcar a persona como caso canônico.");
            }
        }

        private static RetornoGenerico? Validar(SalvarMfScorePersonaDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return CriarErroNegocio("Informe o nome da persona.");
            }

            if (string.IsNullOrWhiteSpace(dto.ObjetivoDaPersona))
            {
                return CriarErroNegocio("Informe o objetivo da persona.");
            }

            if (dto.CompromissosCumpridos < 0)
            {
                return CriarErroNegocio("Compromissos cumpridos não pode ser negativo.");
            }

            if (dto.FaixaEsperadaMin.HasValue && dto.FaixaEsperadaMax.HasValue && dto.FaixaEsperadaMin.Value > dto.FaixaEsperadaMax.Value)
            {
                return CriarErroNegocio("A faixa esperada mínima não pode ser maior que a máxima.");
            }

            if (!HorizonteEhCrescente(dto.ReceitasPrevistas30Dias, dto.ReceitasPrevistas90Dias, dto.ReceitasPrevistas180Dias, dto.ReceitasPrevistas12Meses))
            {
                return CriarErroNegocio("As receitas previstas por horizonte devem ser acumuladas e não decrescentes.");
            }

            if (!HorizonteEhCrescente(dto.Obrigacoes30Dias, dto.Obrigacoes90Dias, dto.Obrigacoes180Dias, dto.Obrigacoes12Meses))
            {
                return CriarErroNegocio("As obrigações por horizonte devem ser acumuladas e não decrescentes.");
            }

            return null;
        }

        private static bool HorizonteEhCrescente(decimal dias30, decimal dias90, decimal dias180, decimal meses12)
        {
            return dias30 <= dias90 && dias90 <= dias180 && dias180 <= meses12;
        }

        private static RetornoGenerico? ValidarAuditoriaHumana(PersonaMfScore persona)
        {
            if (!persona.ScoreHumanoSugerido.HasValue)
            {
                return CriarErroNegocio("Preencha o score humano sugerido antes de concluir a auditoria.");
            }

            if (!persona.FaixaEsperadaMin.HasValue || !persona.FaixaEsperadaMax.HasValue)
            {
                return CriarErroNegocio("Preencha a faixa esperada mínima e máxima antes de concluir a auditoria.");
            }

            if (persona.FaixaEsperadaMin.Value > persona.FaixaEsperadaMax.Value)
            {
                return CriarErroNegocio("A faixa esperada mínima não pode ser maior que a máxima.");
            }

            if (string.IsNullOrWhiteSpace(persona.JustificativaNotaHumana))
            {
                return CriarErroNegocio("Informe a justificativa humana antes de concluir a auditoria.");
            }

            return null;
        }

        private static void AplicarDados(PersonaMfScore persona, SalvarMfScorePersonaDTO dto, DateTime dataAtualizacao)
        {
            persona.Nome = dto.Nome.Trim();
            persona.Descricao = dto.Descricao.Trim();
            persona.ObjetivoDaPersona = dto.ObjetivoDaPersona.Trim();
            persona.RendaMensal = dto.RendaMensal;
            persona.ReceitasPrevistas30Dias = dto.ReceitasPrevistas30Dias;
            persona.ReceitasPrevistas90Dias = dto.ReceitasPrevistas90Dias;
            persona.ReceitasPrevistas180Dias = dto.ReceitasPrevistas180Dias;
            persona.ReceitasPrevistas12Meses = dto.ReceitasPrevistas12Meses;
            persona.DespesasMensais = dto.DespesasMensais;
            persona.Obrigacoes30Dias = dto.Obrigacoes30Dias;
            persona.Obrigacoes90Dias = dto.Obrigacoes90Dias;
            persona.Obrigacoes180Dias = dto.Obrigacoes180Dias;
            persona.Obrigacoes12Meses = dto.Obrigacoes12Meses;
            persona.ReservaEmergencia = dto.ReservaEmergencia;
            persona.PatrimonioBruto = dto.PatrimonioBruto;
            persona.Passivos = dto.Passivos;
            persona.PatrimonioLiquido = dto.PatrimonioLiquido;
            persona.PossuiPerfilFinanceiroConfigurado = dto.PossuiPerfilFinanceiroConfigurado;
            persona.PossuiPlanoEstrategico = dto.PossuiPlanoEstrategico;
            persona.PossuiMetas = dto.PossuiMetas;
            persona.PossuiCompromissos = dto.PossuiCompromissos;
            persona.CompromissosCumpridos = dto.CompromissosCumpridos;
            persona.PossuiInadimplencia = dto.PossuiInadimplencia;
            persona.ScoreHumanoSugerido = dto.ScoreHumanoSugerido;
            persona.FaixaEsperadaMin = dto.FaixaEsperadaMin;
            persona.FaixaEsperadaMax = dto.FaixaEsperadaMax;
            persona.JustificativaNotaHumana = LimparTextoOpcional(dto.JustificativaNotaHumana);
            persona.Observacoes = LimparTextoOpcional(dto.Observacoes);
            persona.DataAtualizacao = dataAtualizacao;

            if (persona.Status == EnumStatusPersonaMfScore.Inativa)
            {
                persona.Status = EnumStatusPersonaMfScore.Rascunho;
            }
        }

        private static string? LimparTextoOpcional(string? texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
        }

        private static MfScorePersonaDTO Mapear(PersonaMfScore persona)
        {
            return new MfScorePersonaDTO
            {
                Id = persona.Id,
                Nome = persona.Nome,
                Descricao = persona.Descricao,
                ObjetivoDaPersona = persona.ObjetivoDaPersona,
                RendaMensal = persona.RendaMensal,
                ReceitasPrevistas30Dias = persona.ReceitasPrevistas30Dias,
                ReceitasPrevistas90Dias = persona.ReceitasPrevistas90Dias,
                ReceitasPrevistas180Dias = persona.ReceitasPrevistas180Dias,
                ReceitasPrevistas12Meses = persona.ReceitasPrevistas12Meses,
                DespesasMensais = persona.DespesasMensais,
                Obrigacoes30Dias = persona.Obrigacoes30Dias,
                Obrigacoes90Dias = persona.Obrigacoes90Dias,
                Obrigacoes180Dias = persona.Obrigacoes180Dias,
                Obrigacoes12Meses = persona.Obrigacoes12Meses,
                ReservaEmergencia = persona.ReservaEmergencia,
                PatrimonioBruto = persona.PatrimonioBruto,
                Passivos = persona.Passivos,
                PatrimonioLiquido = persona.PatrimonioLiquido,
                PossuiPerfilFinanceiroConfigurado = persona.PossuiPerfilFinanceiroConfigurado,
                PossuiPlanoEstrategico = persona.PossuiPlanoEstrategico,
                PossuiMetas = persona.PossuiMetas,
                PossuiCompromissos = persona.PossuiCompromissos,
                CompromissosCumpridos = persona.CompromissosCumpridos,
                PossuiInadimplencia = persona.PossuiInadimplencia,
                ScoreHumanoSugerido = persona.ScoreHumanoSugerido,
                FaixaEsperadaMin = persona.FaixaEsperadaMin,
                FaixaEsperadaMax = persona.FaixaEsperadaMax,
                JustificativaNotaHumana = persona.JustificativaNotaHumana,
                Status = persona.Status,
                EhCasoCanonico = persona.EhCasoCanonico,
                Observacoes = persona.Observacoes,
                DataCriacao = persona.DataCriacao,
                DataAtualizacao = persona.DataAtualizacao
            };
        }

        private static ContextoAnaliseFinanceira CriarContexto(PersonaMfScore persona)
        {
            var lancamentos = new List<Lancamento>();

            lancamentos.AddRange(CriarLancamentosBaseMensais(persona));
            lancamentos.AddRange(CriarLancamentosHorizonte(
                EnumTipoLancamento.Receita,
                persona.ReceitasPrevistas30Dias,
                persona.ReceitasPrevistas90Dias,
                persona.ReceitasPrevistas180Dias,
                persona.ReceitasPrevistas12Meses,
                "Receita prevista"));
            lancamentos.AddRange(CriarLancamentosHorizonte(
                EnumTipoLancamento.Despesa,
                persona.Obrigacoes30Dias,
                persona.Obrigacoes90Dias,
                persona.Obrigacoes180Dias,
                persona.Obrigacoes12Meses,
                "Obrigação prevista"));

            if (persona.PossuiInadimplencia)
            {
                var valorInadimplente = persona.RendaMensal > 0m
                    ? Math.Round(persona.RendaMensal * 0.30m, 2)
                    : 1000m;

                lancamentos.Add(CriarLancamento(
                    EnumTipoLancamento.Despesa,
                    valorInadimplente,
                    DataReferenciaCalibracao.AddDays(-35),
                    "Despesa vencida simulada"));
            }

            var ativos = CriarAtivos(persona);
            var passivos = CriarPassivos(persona);

            return new ContextoAnaliseFinanceira
            {
                DataReferencia = DataReferenciaCalibracao,
                Lancamentos = lancamentos,
                Ativos = ativos,
                Passivos = passivos,
                ConfiguracaoPerfilFinanceiro = persona.PossuiPerfilFinanceiroConfigurado
                    ? CriarConfiguracaoPadrao(persona)
                    : null,
                PlanoEstrategicoFinanceiroVigente = persona.PossuiPlanoEstrategico
                    ? CriarPlanoEstrategicoSimulado(persona)
                    : null,
                CompromissosFinanceiros = persona.PossuiCompromissos
                    ? CriarCompromissosSimulados(persona)
                    : [],
                Metas = []
            };
        }

        private static List<Lancamento> CriarLancamentosBaseMensais(PersonaMfScore persona)
        {
            var lancamentos = new List<Lancamento>();

            if (persona.RendaMensal > 0)
            {
                lancamentos.Add(CriarLancamento(EnumTipoLancamento.Receita, persona.RendaMensal, DataReferenciaCalibracao.AddDays(5), "Renda mensal"));
            }

            if (persona.DespesasMensais > 0)
            {
                lancamentos.Add(CriarLancamento(EnumTipoLancamento.Despesa, persona.DespesasMensais, DataReferenciaCalibracao.AddDays(10), "Despesas mensais"));
            }

            return lancamentos;
        }

        private static List<Lancamento> CriarLancamentosHorizonte(
            EnumTipoLancamento tipo,
            decimal total30Dias,
            decimal total90Dias,
            decimal total180Dias,
            decimal total12Meses,
            string descricaoBase)
        {
            var lancamentos = new List<Lancamento>();
            var incrementais = new[]
            {
                new { Valor = total30Dias, Data = DataReferenciaCalibracao.AddDays(20), Sufixo = "30 dias" },
                new { Valor = total90Dias - total30Dias, Data = DataReferenciaCalibracao.AddDays(60), Sufixo = "90 dias" },
                new { Valor = total180Dias - total90Dias, Data = DataReferenciaCalibracao.AddDays(135), Sufixo = "180 dias" },
                new { Valor = total12Meses - total180Dias, Data = DataReferenciaCalibracao.AddDays(270), Sufixo = "12 meses" }
            };

            foreach (var item in incrementais.Where(x => x.Valor > 0))
            {
                lancamentos.Add(CriarLancamento(tipo, item.Valor, item.Data, $"{descricaoBase} - {item.Sufixo}"));
            }

            return lancamentos;
        }

        private static Lancamento CriarLancamento(EnumTipoLancamento tipo, decimal valor, DateTime dataVencimento, string descricao)
        {
            return new Lancamento
            {
                Id = Guid.NewGuid(),
                Valor = valor,
                Descricao = descricao,
                Observacao = descricao,
                DataVencimento = dataVencimento,
                DataLancamento = DataReferenciaCalibracao,
                StatusLancamento = EnumStatusLancamento.Pendente,
                FrequenciaLancamento = EnumTipoFrequenciaLancamento.Pontual,
                Tipo = tipo,
                Vinculo = EnumVinculoLancamento.Avulso
            };
        }

        private static List<BemPatrimonial> CriarAtivos(PersonaMfScore persona)
        {
            var ativos = new List<BemPatrimonial>();

            if (persona.ReservaEmergencia > 0)
            {
                ativos.Add(CriarAtivo("Reserva de emergência", EnumBemPatrimonial.DinheiroEmConta, persona.ReservaEmergencia));
            }

            var patrimonioResidual = Math.Max(persona.PatrimonioBruto - persona.ReservaEmergencia, 0m);
            if (patrimonioResidual > 0)
            {
                ativos.Add(CriarAtivo("Patrimônio bruto", EnumBemPatrimonial.Outro, patrimonioResidual));
            }

            return ativos;
        }

        private static List<Passivo> CriarPassivos(PersonaMfScore persona)
        {
            if (persona.Passivos <= 0)
            {
                return [];
            }

            return
            [
                CriarPassivo("Passivos simulados", EnumPassivo.ObrigacaoFinanceira, persona.Passivos)
            ];
        }

        private static BemPatrimonial CriarAtivo(string nome, EnumBemPatrimonial tipo, decimal valor)
        {
            var id = Guid.NewGuid();

            return new BemPatrimonial
            {
                Id = id,
                NomeBemPatrimonial = nome,
                Descricao = nome,
                Ativo = true,
                Permanencia = true,
                DataCadastro = DataReferenciaCalibracao.AddMonths(-1),
                DataAquisicao = DataReferenciaCalibracao.AddMonths(-1),
                Tipo = tipo,
                DataPermanencia =
                [
                    new PermanenciaBemMaterial
                    {
                        Id = Guid.NewGuid(),
                        BemPatrimonialId = id,
                        DataPermanencia = DataReferenciaCalibracao,
                        Valor = valor
                    }
                ]
            };
        }

        private static Passivo CriarPassivo(string nome, EnumPassivo tipo, decimal valor)
        {
            var id = Guid.NewGuid();

            return new Passivo
            {
                Id = id,
                NomePassivo = nome,
                Descricao = nome,
                Ativo = true,
                Permanencia = true,
                DataCadastro = DataReferenciaCalibracao.AddMonths(-1),
                DataInicio = DataReferenciaCalibracao.AddMonths(-1),
                Tipo = tipo,
                DataPermanencia =
                [
                    new PermanenciaPassivo
                    {
                        Id = Guid.NewGuid(),
                        PassivoId = id,
                        DataPermanencia = DataReferenciaCalibracao,
                        Valor = valor
                    }
                ]
            };
        }

        private static ConfiguracaoPerfilFinanceiro CriarConfiguracaoPadrao(PersonaMfScore persona)
        {
            var patrimonioAlvo = persona.PatrimonioLiquido > 0
                ? persona.PatrimonioLiquido * 2
                : Math.Max(persona.RendaMensal * 12, 10000m);

            return new ConfiguracaoPerfilFinanceiro
            {
                Id = Guid.NewGuid(),
                PerfilFinanceiroId = Guid.NewGuid(),
                DataInicioVigencia = DataReferenciaCalibracao.AddMonths(-1),
                DataCriacao = DataReferenciaCalibracao.AddMonths(-1),
                PercentualEconomiaMensalDesejado = 20m,
                PercentualReservaEmergenciaDesejado = 100m,
                MesesReservaEmergenciaDesejados = 6,
                PercentualMaximoComprometimentoRenda = 50m,
                PercentualMaximoEndividamento = 50m,
                PercentualMinimoInvestimento = 10m,
                PatrimonioLiquidoAlvo = patrimonioAlvo
            };
        }

        private static PlanoEstrategicoFinanceiro CriarPlanoEstrategicoSimulado(PersonaMfScore persona)
        {
            var planoId = Guid.NewGuid();
            var objetivos = new List<ObjetivoPlanoEstrategico>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    PlanoEstrategicoFinanceiroId = planoId,
                    Titulo = "Fortalecer reserva de emergência",
                    Descricao = "Construir proteção para imprevistos no curto prazo.",
                    Prioridade = EnumPrioridadeObjetivoPlanoEstrategico.Critica,
                    Status = persona.ReservaEmergencia > 0m
                        ? EnumStatusObjetivoPlanoEstrategico.EmAndamento
                        : EnumStatusObjetivoPlanoEstrategico.Planejado,
                    Ordem = 1,
                    DataAlvo = DataReferenciaCalibracao.AddMonths(6),
                    ValorAlvo = Math.Max(persona.DespesasMensais * 6m, persona.RendaMensal * 3m),
                    ValorAtual = persona.ReservaEmergencia,
                    DataCriacao = DataReferenciaCalibracao.AddMonths(-2)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    PlanoEstrategicoFinanceiroId = planoId,
                    Titulo = "Melhorar patrimônio líquido",
                    Descricao = "Aumentar o patrimônio líquido com consistência.",
                    Prioridade = EnumPrioridadeObjetivoPlanoEstrategico.Alta,
                    Status = persona.CompromissosCumpridos > 0
                        ? EnumStatusObjetivoPlanoEstrategico.EmAndamento
                        : EnumStatusObjetivoPlanoEstrategico.Planejado,
                    Ordem = 2,
                    DataAlvo = DataReferenciaCalibracao.AddMonths(12),
                    ValorAlvo = Math.Max(persona.PatrimonioLiquido * 1.5m, persona.RendaMensal * 12m),
                    ValorAtual = Math.Max(persona.PatrimonioLiquido, 0m),
                    DataCriacao = DataReferenciaCalibracao.AddMonths(-2)
                }
            };

            if (persona.PossuiMetas)
            {
                objetivos.Add(new ObjetivoPlanoEstrategico
                {
                    Id = Guid.NewGuid(),
                    PlanoEstrategicoFinanceiroId = planoId,
                    Titulo = "Concluir meta financeira prioritária",
                    Descricao = "Transformar metas cadastradas em entregas concluídas.",
                    Prioridade = EnumPrioridadeObjetivoPlanoEstrategico.Media,
                    Status = persona.CompromissosCumpridos >= 2
                        ? EnumStatusObjetivoPlanoEstrategico.Concluido
                        : EnumStatusObjetivoPlanoEstrategico.EmAndamento,
                    Ordem = 3,
                    DataAlvo = DataReferenciaCalibracao.AddMonths(9),
                    ValorAlvo = persona.RendaMensal,
                    ValorAtual = persona.CompromissosCumpridos >= 2 ? persona.RendaMensal : persona.RendaMensal * 0.5m,
                    DataCriacao = DataReferenciaCalibracao.AddMonths(-2)
                });
            }

            return new PlanoEstrategicoFinanceiro
            {
                Id = planoId,
                PlanoRaizId = planoId,
                UsuarioId = "persona-mf-score",
                Nome = $"Plano estratégico simulado - {persona.Nome}",
                Descricao = persona.ObjetivoDaPersona,
                Observacao = "Plano sintético usado para calibração do MF Score.",
                NumeroVersao = 1,
                DataInicioVigencia = DataReferenciaCalibracao.AddMonths(-2),
                DataCadastro = DataReferenciaCalibracao.AddMonths(-2),
                DataAtualizacao = DataReferenciaCalibracao.AddMonths(-1),
                Ativo = true,
                Objetivos = objetivos
            };
        }

        private static IReadOnlyCollection<CompromissoFinanceiro> CriarCompromissosSimulados(PersonaMfScore persona)
        {
            var quantidadeConcluidos = Math.Max(0, persona.CompromissosCumpridos);
            var quantidadeTotal = Math.Max(1, quantidadeConcluidos + 1);
            var compromissos = new List<CompromissoFinanceiro>();

            for (var indice = 0; indice < quantidadeTotal; indice++)
            {
                var concluido = indice < quantidadeConcluidos;

                compromissos.Add(new CompromissoFinanceiro
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = "persona-mf-score",
                    Descricao = concluido
                        ? $"Compromisso concluído {indice + 1}"
                        : $"Compromisso em andamento {indice + 1}",
                    Origem = EnumOrigemCompromissoFinanceiro.Manual,
                    Status = concluido
                        ? EnumStatusCompromissoFinanceiro.Concluido
                        : EnumStatusCompromissoFinanceiro.EmAndamento,
                    DataCriacao = DataReferenciaCalibracao.AddMonths(-2).AddDays(indice * 5),
                    DataConclusao = concluido
                        ? DataReferenciaCalibracao.AddMonths(-1).AddDays(indice)
                        : null,
                    Observacoes = "Compromisso sintético usado para calibração do MF Score.",
                    Ativo = true
                });
            }

            return compromissos;
        }

        private static string? MontarObservacaoComparativa(PersonaMfScore persona, int scoreCalculado)
        {
            if (persona.ScoreHumanoSugerido.HasValue)
            {
                var diferenca = scoreCalculado - persona.ScoreHumanoSugerido.Value;
                if (diferenca == 0)
                {
                    return "O score calculado coincide exatamente com a avaliação humana sugerida.";
                }

                return diferenca > 0
                    ? $"O motor ficou {diferenca} ponto(s) acima da avaliação humana sugerida."
                    : $"O motor ficou {Math.Abs(diferenca)} ponto(s) abaixo da avaliação humana sugerida.";
            }

            if (persona.FaixaEsperadaMin.HasValue && persona.FaixaEsperadaMax.HasValue)
            {
                var dentro = scoreCalculado >= persona.FaixaEsperadaMin.Value && scoreCalculado <= persona.FaixaEsperadaMax.Value;
                return dentro
                    ? "O score calculado ficou dentro da faixa esperada informada."
                    : "O score calculado ficou fora da faixa esperada informada.";
            }

            return null;
        }

        private static RetornoGenerico CriarErro(Exception ex, string mensagemUsuario)
        {
            return new RetornoGenerico(false, ex.ToString(), mensagemUsuario, HttpStatusCode.InternalServerError, null);
        }

        private static RetornoGenerico CriarErroNegocio(string mensagem)
        {
            return new RetornoGenerico(false, mensagem, mensagem, HttpStatusCode.BadRequest, null);
        }

        private static RetornoGenerico CriarNaoEncontrado(string mensagem)
        {
            return new RetornoGenerico(false, mensagem, mensagem, HttpStatusCode.NotFound, null);
        }
    }
}
