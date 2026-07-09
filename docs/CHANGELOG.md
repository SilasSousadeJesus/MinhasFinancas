# Changelog - Minhas Financas

Registrar aqui apenas mudancas relevantes do sistema.

## 08/07/2026

- Refinamento da Memória Financeira e da IA Estratégica para reforçar continuidade consultiva entre análises relacionadas, destacando recorrência, evolução de entendimento e comparação com leituras anteriores.
- Rollback da Fase 4.2.7 - Conversa Contínua, removendo a experiência de chat contínuo para manter o Assistente Financeiro como consultor financeiro e não como conversa persistida.
- Atualização do roadmap e das visões de produto para retirar a Conversa Contínua da linha oficial do produto e registrar a próxima etapa como Simulador Inteligente.
- Atualização da documentação funcional e conceitual para refletir a remoção da Conversa Contínua do fluxo do Assistente Financeiro.
- Consolidacao da Fase 4.2.5 - IA Estrategica, com revisao do prompt oficial para conectar estado atual, evolucao, plano estrategico, consistencia e compromissos em uma narrativa consultiva.
- Atualizacao da documentacao de produto, roadmap, visao do assistente e AI context para refletir a IA Estrategica como fase concluida.
- Criacao do documento `docs/ASSISTANT_VISION.md`, com a visao humana e evolutiva do Assistente Financeiro.
- Referencias ao novo documento adicionadas em `AI_CONTEXT.md`, `docs/PRODUCT_VISION.md` e `docs/MODULE_GUIDE.md`.
- Criacao da base inicial do `Modelo de Decisao Financeira`, com `DecisaoFinanceiraIA`, `InterpretadorDecisaoFinanceira` e preparacao do contexto do Assistente Financeiro para receber essa camada.
- Consolidacao da Fase 4.2.6 - Especialistas Financeiros, com pareceres internos por dominio integrados ao contexto consolidado do Assistente Financeiro.
- Criacao dos documentos de apoio dos especialistas internos em `MinhasFinancas.Infra/IA/Prompts/Especialistas/`, registrando objetivos, regras, prioridades e limitacoes de cada dominio.

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
