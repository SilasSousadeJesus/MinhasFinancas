# Changelog - Minhas Financas

Registrar aqui apenas mudancas relevantes do sistema.

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





