# Changelog - Minhas Financas

Registrar aqui apenas mudancas relevantes do sistema.

## 09/07/2026

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





