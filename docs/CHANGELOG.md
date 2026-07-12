# Changelog - Minhas Financas

Registrar aqui apenas mudancas relevantes do sistema.

## 12/07/2026

- Reconstrução completa das massas sintéticas de `MF-CENARIO-02`, `MF-CENARIO-07` e `MF-CENARIO-09`, mantendo o motor `mf-score-v2.5-1000` congelado.
- Ajuste exclusivo de dados sintéticos de receitas, despesas, saldos, reserva, passivos leves, parcelamentos, compromissos e histórico mensal desses três cenários, sem alterar fórmulas, pesos, indicadores, pilares ou penalizações.
- Nova rodada oficial da Base de Simulação após a reconstrução, trazendo os três cenários antes inválidos para dentro das respectivas faixas humanas do benchmark: `MF-CENARIO-02` (`640`), `MF-CENARIO-07` (`380`) e `MF-CENARIO-09` (`750`).
- Confirmação documental de que a sprint atuou apenas sobre a coerência da massa sintética do benchmark, preservando integralmente a arquitetura do `MF Score`.

- Consolidação documental da auditoria comparativa da `mf-score-v2.5-1000`, sem alteração de código, fórmulas, pesos, indicadores, pilares ou penalizações.
- Registro oficial da leitura residual da `v2.5` com métricas calculadas apenas sobre os 9 cenários válidos do benchmark.
- Confirmação documental de melhora de `1/9` para `3/9` cenários válidos dentro da faixa e redução da diferença média absoluta de `178,89` para `92,22` pontos.
- Registro formal de `MF-CENARIO-12` como regressão residual da sprint e de `Liquidez e Reserva` como principal prioridade remanescente antes de qualquer futura `v2.6`.

- Conclusão da sprint `mf-score-v2.5-1000`, mantendo a arquitetura da `v2.4` congelada e atuando apenas em calibração numérica.
- Recalibração do pilar `Liquidez e Reserva` para reconhecer melhor reserva parcialmente formada e capacidade real de construção.
- Recalibração do pilar `Fluxo de Caixa`, reduzindo severidade excessiva em perfis organizados com pouca folga operacional.
- Recalibração do pilar `Endividamento e Obrigações`, distinguindo melhor dívida organizada, exposição patrimonial e pressão futura controlável.
- Redução da severidade das penalizações já existentes (`persistência de fluxo negativo`, `patrimônio líquido negativo`, `inadimplência` e `cura recente`), sem criar novas penalizações.
- Rerrodada oficial dos 12 cenários do benchmark ao final de cada bloco, com evolução de `1/12` para `4/12` cenários dentro da faixa aceitável e queda da diferença média absoluta de `205` para `102,5` pontos.
- Atualização da versão oficial do modelo para `mf-score-v2.5-1000`.

- Formalização da diretriz oficial da sprint `mf-score-v2.5`, deixando a `v2.4` congelada conceitualmente e restringindo a próxima rodada a calibração fina numérica.
- Registro documental da ordem oficial de atuação na `v2.5`: `Liquidez e Reserva`, `Fluxo de Caixa`, `Endividamento e Obrigações` e compressão das penalizações.
- Sincronização de `docs/MF_SCORE.md`, `docs/MF_SCORE_AUDIT.md`, `docs/ROADMAP.md` e `AI_CONTEXT.md` para refletir que a próxima sprint do MF Score não deve criar novos indicadores, pilares, arquitetura ou penalizações.

- Evolução do `Laboratório do MF Score` para funcionar como ferramenta oficial de calibração da sprint da `v2.4`, sem alterar pesos, fórmulas, indicadores, penalizações ou regras do motor.
- Criação da nova seção `Análise de Calibração` dentro da própria tela `/mf-score-laboratorio`.
- Integração automática do laboratório com `docs/MF_SCORE_BENCHMARK.md`, usando o código do cenário sintético aberto para carregar nota humana, faixa aceitável, diferença registrada, status e contexto oficial da auditoria.
- Inclusão de leitura qualitativa por pilar, lista ordenada de indicadores que puxaram a nota para baixo, principais pontos positivos, diagnóstico final e recomendação textual para a próxima rodada de calibração.

- Consolidação da Primeira Rodada de Referência Humana do Benchmark do MF Score em `docs/MF_SCORE_BENCHMARK.md`.
- Registro oficial, por cenário, de nota humana de referência, faixa aceitável, diferença entre score atual e expectativa humana, status, justificativa e decisão de auditoria.
- Formalização dos achados consolidados da auditoria humana da `v2.4`, incluindo aprovação arquitetural do motor, severidade excessiva em perfis saudáveis ou recuperáveis e perda de granularidade na base da escala.
- Classificação de `MF-CENARIO-02`, `MF-CENARIO-07` e `MF-CENARIO-09` como cenários inválidos para calibração definitiva até reconstrução de suas massas sintéticas.
- Sincronização curta de `AI_CONTEXT.md`, `docs/MF_SCORE.md`, `docs/MF_SCORE_AUDIT.md` e `docs/MF_SCORE_VALIDATION.md` para apontar o benchmark como fonte oficial da primeira rodada humana.

## 10/07/2026

- Consolidação oficial da auditoria humana da versão `mf-score-v2.4-1000`, aprovando a arquitetura atual do Motor Financeiro.
- Promoção do `docs/MF_SCORE_BENCHMARK.md` a benchmark oficial obrigatório para validação de regressão e calibração do MF Score.
- Registro formal de que a próxima etapa do MF Score deixa de ser revisão estrutural e passa a ser calibração fina numérica baseada nos 12 cenários oficiais.

- Refatoração conceitual do `MF Score` para a versão `mf-score-v2.4-1000`, preservando a arquitetura oficial do Motor Financeiro.
- Reposicionamento do pilar `Fluxo de Caixa` para medir principalmente a capacidade operacional do mês, com menor redundância entre `Economia Mensal`, `Percentual de Economia` e `Comprometimento da Renda`.
- Reestruturação do pilar `Endividamento e Obrigações`, separando dívida de consumo, financiamento patrimonial, obrigações recorrentes futuras e inadimplência.
- Reposicionamento do pilar `Patrimônio` para priorizar ativos, passivos e patrimônio líquido real, deixando `Patrimônio-alvo` como leitura secundária de evolução.
- Evolução do pilar `Planejamento e Disciplina`, reduzindo peso de configuração pura e aumentando a influência de execução real, consistência e sinais comportamentais observáveis.
- Substituição da antiga soma de penalizações temporais de fluxo negativo por um modelo progressivo de nível único, aplicando apenas a persistência mais grave encontrada.
- Correção da projeção de receitas recorrentes nos horizontes de `180` e `365` dias, reduzindo distorções na pressão futura acumulada.
- Recalibragem qualitativa das pressões financeiras acumuladas, impedindo que percentuais acima de `100%` permaneçam apenas como `Atenção`.
- Melhoria da apresentação analítica das interfaces, substituindo leituras técnicas como `999 meses` por mensagens compreensíveis ao usuário.
- Atualização da documentação oficial do Motor Financeiro para registrar os motivos arquiteturais da rodada `v2.4`.

- Criação da Base Oficial de Simulação do MF Score, com aproximadamente 12 cenários sintéticos persistidos e histórico financeiro coerente para desenvolvimento, auditoria e calibração.
- Criação dos endpoints `POST /api/MfScoreLaboratorio/GerarBaseSimulacao` e `DELETE /api/MfScoreLaboratorio/LimparBaseSimulacao`.
- Ampliação da entidade `Usuario` com marcação explícita de usuário sintético, origem, código de cenário, versão da base, data de geração, descrição e objetivo do cenário.
- Expansão do Laboratório do MF Score para listar usuários reais e sintéticos, filtrar por origem e exibir metadados dos cenários oficiais.
- Criação da migration `20260711001125_base_simulacao_mf_score`.
- Sincronização da documentação do MF Score para registrar a Base Oficial de Simulação como patrimônio permanente do projeto.

- Calibragem do `MF Score` para reduzir falso positivo de risco em usuarios iniciantes com fluxo de caixa muito forte.
- Evolucao oficial do motor para `mf-score-v2.3-1000`.
- Criacao do indicador auxiliar `Capacidade de Formacao de Reserva`, com exposicao no painel analitico e no Laboratorio do MF Score.
- Recalibragem do pilar `Liquidez e Reserva`, que agora considera a velocidade estimada para completar a reserva ideal sem transformar reserva zero automaticamente em risco critico extremo.
- Formalizacao da regra de `ponto de partida patrimonial neutro`, evitando tratar patrimonio zerado sem passivos como insolvencia.
- Implementação oficial do conceito de Perfil Financeiro Inicial, com criação automática para novos usuários e migração automática de usuários antigos sem perfil.
- Inclusão da origem da configuração do perfil (`PerfilInicialSistema` ou `PersonalizadoPeloUsuario`) para dar transparência ao Motor Financeiro e ao frontend.
- Atualização da tela Perfil Financeiro para informar quando o usuário ainda utiliza os parâmetros padrão do sistema.
- Garantia de que a camada analítica e o MF Score sempre encontrem uma configuração vigente de Perfil Financeiro sem alterar fórmulas, pesos, pilares ou penalizações.
- Criação da migration `20260710141450_perfilFinanceiroInicial`.

## 09/07/2026

- Consolidação do Hangfire dentro do projeto principal da API, removendo o projeto separado de agendamentos.
- Migração da configuração do Hangfire Server, storage MySQL, dashboard e registro dos jobs recorrentes para a API.
- Migração dos jobs `atualizacao-anual-bens-patrimoniais` e `historico-mensal-mf-score` para a API, preservando a mesma lógica de negócio.
- Remoção do projeto `Minhas-Financas-Hangfire` e da infraestrutura auxiliar obsoleta da solution.
- Criação do documento `docs/MF_SCORE_AI_QUESTION_FLOW.md` para organizar a sequência oficial de perguntas de calibração do `MF Score` com apoio de outra IA.

- Reformulação oficial do `MF Score` para escala final de `0 a 1000`, mantendo os pilares em `0 a 100`.
- Revisão da filosofia do Motor Financeiro para separar nota dos pilares, penalizações críticas, persistência temporal do risco e histórico mensal.
- Aplicação formal da regra de não dupla penalização, removendo penalizações críticas automáticas redundantes para reserva zero, comprometimento elevado e pressão futura sem risco materializado.
- Reclassificação das penalizações críticas para foco em inadimplência, fluxo mensal negativo, recorrência de meses negativos, patrimônio líquido negativo e ausência de dados essenciais.
- Criação da entidade `HistoricoMfScore` para persistir a evolução mensal do score por competência e versão de modelo.
- Criação da migration `20260709235851_historicoMfScore`.
- Criação do job Hangfire mensal do `MF Score`, executado no dia 01 para registrar a competência anterior.
- Atualização das telas de Saúde Financeira, Assistente Financeiro, conclusão executiva e Personas de Calibração para a nova escala de `0 a 1000`.
- Atualização da auditoria do MF Score e das faixas esperadas das personas para a nova escala.
- Sincronização da documentação oficial do Motor Financeiro, incluindo score, indicadores, validação, auditoria, módulos, glossário, AI context e regras permanentes.

- Criação da tela interna `/mf-score-personas` para CRUD de Personas de Calibração do MF Score.
- Criação da entidade persistida `PersonaMfScore`, com status de auditoria, faixa esperada, justificativa humana e promoção para caso canônico.
- Criação dos endpoints `GET/POST/PUT/DELETE /api/MfScorePersonas` e das ações `RodarScore`, `MarcarAuditada` e `MarcarCasoCanonico`.
- Início do fluxo estruturado de calibração humana persistida do MF Score, sem alterar pesos, fórmulas ou regras críticas do motor.

- Mudança oficial de prioridade do roadmap para amadurecer o MF Score antes do Simulador Inteligente.
- Criação da etapa contínua de Evolução e Calibração do MF Score como fase intermediária oficial.
- Consolidação do MF Score como ativo central do Motor Financeiro, com referência explícita ao risco financeiro pessoal.
- Atualização da documentação de produto, regras, visão de IA, módulos, glossário e referência oficial de indicadores.
- Registro do conceito de MF Score Potencial como evolução futura.
- Criação da auditoria operacional interna do MF Score com endpoint de desenvolvimento para geração de planilha `.xlsx`.
- Implementação de personas sintéticas oficiais para validar cenários canônicos do Motor Financeiro sem duplicar fórmulas.
- Reuso da infraestrutura existente de exportação Excel para produzir abas de resumo, cenários, pilares, indicadores críticos e dados de entrada.
- Sincronização da documentação para registrar a auditoria como evidência obrigatória em mudanças relevantes do Motor Financeiro.
- Expansão do `docs/MF_SCORE_AUDIT.md` para funcionar como documento oficial de governança técnica do Motor Financeiro.
- Criação da Auditoria Arquitetural do Motor Financeiro, com cobertura dos pilares, cobertura do domínio, limitações conhecidas, achados formais e dívida técnica.
- Registro oficial do achado `MF-001`, documentando que o pilar Planejamento ainda utiliza proxies e não mede diretamente toda a execução estratégica do usuário.
- Criação do endpoint interno `POST /api/MfScoreAuditoria/GerarPlanilhaAuditoriaHumana` para gerar a planilha de auditoria humana das personas.
- Separação explícita entre validação automática do `MF Score` e auditoria humana cega para calibragem futura.
- Preparação do processo para que personas auditadas manualmente possam evoluir para padrões oficiais do sistema.

## 10/07/2026

- Refatoração da antiga tela de Personas de Calibração para o novo `Laboratório do MF Score`.
- Criação da rota `/mf-score-laboratorio` e manutenção de compatibilidade em `/mf-score-personas`.
- Criação dos endpoints `GET /api/MfScoreLaboratorio/Usuarios` e `GET /api/MfScoreLaboratorio/Usuarios/{usuarioId}/Score`.
- Remoção visual das ações de CRUD, auditoria e promoção de personas da tela autenticada.
- Substituição do foco em cenários sintéticos por inspeção somente leitura de usuários reais, com score, pilares, indicadores, penalizações, regras críticas, dados de entrada e limitações do cálculo.
- Sincronização da documentação para registrar o laboratório como ferramenta interna de leitura do Motor Financeiro, sem alteração de fórmulas do `MF Score`.

- Evolução do Motor Financeiro para `mf-score-v2.2-1000`.
- Incorporação determinística de `Plano Estratégico Financeiro` e `Compromissos Financeiros` no pilar `Planejamento e Disciplina`, sem penalizar usuários que ainda não possuem esses elementos.
- Implementação de leitura de reincidência e cura recente da inadimplência como agravantes/resíduos distintos da inadimplência atual.
- Atualização do contexto complementar do `MF Score` para unificar cálculo real, auditoria e personas com os mesmos sinais de planejamento e atraso recente.
- Reforço da governança das personas: promoção para `Caso Canônico` agora exige persona previamente `Auditada`.
- Ampliação das personas persistidas para gerar plano estratégico e compromissos sintéticos quando o cenário declarar esses elementos.
- Regeneração da auditoria humana com descrição explícita de plano estratégico e compromissos no cenário avaliado.
- Auditoria operacional rerrodada com sucesso na versão `mf-score-v2.2-1000`, mantendo `8 de 8` cenários oficiais dentro da faixa esperada.
- Atualização sincronizada de `AI_CONTEXT.md`, `docs/MF_SCORE.md`, `docs/INDICADORES_FINANCEIROS.md`, `docs/MF_SCORE_VALIDATION.md` e `docs/MF_SCORE_AUDIT.md`.

- Recalibração da rodada seguinte do `mf-score-v2.1-1000`, com auditoria operacional rerrodada e `8 de 8` cenários oficiais aprovados dentro da faixa esperada.
- Revisão das personas sintéticas e dos casos canônicos `Boa renda, reserva zero e cartão alto`, `Excelente fluxo com pouco patrimônio` e `Planejamento excelente`, ajustando suas faixas esperadas para refletir melhor a severidade real do motor.
- Recalibragem das penalizações temporais de fluxo negativo para um modelo mais proporcional entre alerta pontual e deterioração recorrente.
- Evolução do pilar `Planejamento e Disciplina`, que passou a exigir a configuração básica dos cinco parâmetros essenciais do `Perfil Financeiro` como condição para nota realmente saudável.
- Consolidação da posição conceitual de `Comprometimento da Renda` como indicador primário de `Fluxo de Caixa`, e não como base de `Planejamento`.
- Decisão oficial de manter os horizontes futuros `30/90/180/365`, mas com peso decrescente nos prazos mais longos.
- Atualização sincronizada da documentação oficial do Motor Financeiro em `docs/MF_SCORE.md`, `docs/INDICADORES_FINANCEIROS.md`, `docs/MF_SCORE_VALIDATION.md`, `docs/MF_SCORE_AUDIT.md` e `AI_CONTEXT.md`.

- Implementação do pacote oficial aprovado para a próxima rodada do `MF Score`, preservando a arquitetura atual do motor.
- Consolidação da versão `mf-score-v2.1-1000`.
- Regularização semântica do indicador `Economia Mensal`, com meta monetária derivada da renda do próprio mês.
- Introdução de faixas explícitas de status para `Percentual de Economia`, `Reserva de Emergência Atual`, `Comprometimento da Renda`, `Comprometimento Financeiro Futuro - 30 dias` e `Endividamento Patrimonial`.
- Substituição da inadimplência binária por matriz gradual baseada em dias de atraso e materialidade do valor vencido sobre a renda.
- Centralização do cálculo de `ContextoComplementarMfScoreFinanceiro` para eliminar divergência entre cálculo real, auditoria e personas.
- Atualização da documentação oficial do Motor Financeiro em `docs/MF_SCORE.md`, `docs/INDICADORES_FINANCEIROS.md` e `docs/MF_SCORE_AUDIT.md`.

- Realização de auditoria arquitetural completa sobre o framework proposto em `docs/MF_Score_Framework_Definitivo.md`.
- Criação do documento oficial `docs/MF_SCORE_REVIEW.md`, consolidando a posição do projeto sobre cada proposta, seu alinhamento com a implementação atual e sua prioridade futura.
- Registro formal das propostas aceitas, aceitas com adaptações, adiadas e rejeitadas, sem alterar código, fórmulas, pesos ou penalizações do Motor Financeiro.

## 08/07/2026

- Revisão do motor financeiro para adicionar compromissos futuros em múltiplos horizontes, tornar o endividamento mais claro como endividamento patrimonial e integrar os pontos de atenção ao resumo geral da Saúde Financeira.
- Refinamento dos indicadores de horizonte temporal para separar o comprometimento financeiro futuro de curto prazo da pressão financeira acumulada em horizontes maiores, revisando fórmulas, pesos e interpretação em toda a inteligência financeira.
- Refinamento da camada `AnaliseFinanceira` com revisão dos pesos da saúde financeira, correção da leitura de compromissos futuros e criação do documento oficial `docs/INDICADORES_FINANCEIROS.md`.

- Refinamento da MemÃ³ria Financeira e da IA EstratÃ©gica para reforÃ§ar continuidade consultiva entre anÃ¡lises relacionadas, destacando recorrÃªncia, evoluÃ§Ã£o de entendimento e comparaÃ§Ã£o com leituras anteriores.
- Rollback da Fase 4.2.7 - Conversa ContÃ­nua, removendo a experiÃªncia de chat contÃ­nuo para manter o Assistente Financeiro como consultor financeiro e nÃ£o como conversa persistida.
- AtualizaÃ§Ã£o do roadmap e das visÃµes de produto para retirar a Conversa ContÃ­nua da linha oficial do produto e registrar a prÃ³xima etapa como Simulador Inteligente.
- AtualizaÃ§Ã£o da documentaÃ§Ã£o funcional e conceitual para refletir a remoÃ§Ã£o da Conversa ContÃ­nua do fluxo do Assistente Financeiro.
- Consolidacao da Fase 4.2.5 - IA Estrategica, com revisao do prompt oficial para conectar estado atual, evolucao, plano estrategico, consistencia e compromissos em uma narrativa consultiva.
- Atualizacao da documentacao de produto, roadmap, visao do assistente e AI context para refletir a IA Estrategica como fase concluida.
- Criacao do documento `docs/ASSISTANT_VISION.md`, com a visao humana e evolutiva do Assistente Financeiro.
- Referencias ao novo documento adicionadas em `AI_CONTEXT.md`, `docs/PRODUCT_VISION.md` e `docs/MODULE_GUIDE.md`.
- Criacao da base inicial do `Modelo de Decisao Financeira`, com `DecisaoFinanceiraIA`, `InterpretadorDecisaoFinanceira` e preparacao do contexto do Assistente Financeiro para receber essa camada.
- Consolidacao da Fase 4.2.6 - Especialistas Financeiros, com pareceres internos por dominio integrados ao contexto consolidado do Assistente Financeiro.
- Criacao dos documentos de apoio dos especialistas internos em `MinhasFinancas.Infra/IA/Prompts/Especialistas/`, registrando objetivos, regras, prioridades e limitacoes de cada dominio.

## 08/07/2026

- Transparência dos indicadores temporais na Saúde Financeira e no contexto de IA.
- Exibição explícita de obrigações previstas, receita prevista e percentual de comprometimento nos horizontes de 30, 90, 180 e 365 dias.
- Atualização da documentação técnica e funcional para refletir a leitura temporal detalhada.

## 07/07/2026

- Criacao da tela de gestao do Plano Estrategico Financeiro no frontend.
- Visualizacao do plano vigente, edicao por nova versao e historico simples de versoes.
- Inclusao do acesso ao modulo na navegacao lateral.
- Implementacao da Consistencia Estrategica como avaliacao deterministica entre decisao, situacao atual e plano estrategico vigente.
- Integracao da Consistencia Estrategica ao contexto textual enviado para a IA.

## 05/07/2026

- Migracao do banco oficial de SQL Server para MySQL.
- Criacao do modulo de Patrimonio com ativos, passivos, snapshots e evolucao patrimonial.
- Criacao do modulo de Simulacoes Financeiras.
- Criacao do Radar Financeiro no Dashboard.
- Criacao da tela de Fluxo de Caixa Simples.
- Criacao da infraestrutura global de loading no frontend.
- Implementacao de exportacao para Excel na tela de Lancamentos.
- Implementacao de novo modelo de status dos lancamentos com efetivacao rapida.

## 04/07/2026

- Integracao real de autenticacao entre frontend e backend.
- Integracao do dashboard com dados reais.
- Criacao do CRUD de contas e cartoes.
- Criacao do CRUD de categorias e subcategorias com seed inicial por usuario.
- Evolucao do modulo de Projecoes com persistencia por usuario e multiplos cenarios.
- Implementacao de lancamentos programados:
  - unico
  - parcelado
  - fixo
  - por dia util

## Historico acumulado

- O projeto evoluiu de frontend majoritariamente visual para aplicacao integrada de ponta a ponta.
- Os modulos centrais hoje ja possuem fluxo real entre interface, API e persistencia.





