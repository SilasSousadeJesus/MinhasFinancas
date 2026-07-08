# Changelog - Minhas Finanças

Registrar aqui apenas mudanças relevantes do sistema.

## 08/07/2026

- Consolidação da Fase 4.2.5 — IA Estratégica, com revisão do prompt oficial para conectar estado atual, evolução, plano estratégico, consistência e compromissos em uma narrativa consultiva.
- Atualização da documentação de produto, roadmap, visão do assistente e AI context para refletir a IA Estratégica como fase concluída.
- Criação do documento `docs/ASSISTANT_VISION.md`, com a visão humana e evolutiva do Assistente Financeiro.
- Referências ao novo documento adicionadas em `AI_CONTEXT.md`, `docs/PRODUCT_VISION.md` e `docs/MODULE_GUIDE.md`.
- Criação da base inicial do `Modelo de Decisão Financeira`, com `DecisaoFinanceiraIA`, `InterpretadorDecisaoFinanceira` e preparação do contexto do Assistente Financeiro para receber essa camada.

## 07/07/2026

- Criação da tela de gestão do Plano Estratégico Financeiro no frontend.
- Visualização do plano vigente, edição por nova versão e histórico simples de versões.
- Inclusão do acesso ao módulo na navegação lateral.
- Implementação da Consistência Estratégica como avaliação determinística entre decisão, situação atual e plano estratégico vigente.
- Integração da Consistência Estratégica ao contexto textual enviado para a IA.

## 05/07/2026

- Migração do banco oficial de SQL Server para MySQL.
- Criação do módulo de Patrimônio com ativos, passivos, snapshots e evolução patrimonial.
- Criação do módulo de Simulações Financeiras.
- Criação do Radar Financeiro no Dashboard.
- Criação da tela de Fluxo de Caixa Simples.
- Criação da infraestrutura global de loading no frontend.
- Implementação de exportação para Excel na tela de Lançamentos.
- Implementação de novo modelo de status dos lançamentos com efetivação rápida.

## 04/07/2026

- Integração real de autenticação entre frontend e backend.
- Integração do dashboard com dados reais.
- Criação do CRUD de contas e cartões.
- Criação do CRUD de categorias e subcategorias com seed inicial por usuário.
- Evolução do módulo de Projeções com persistência por usuário e múltiplos cenários.
- Implementação de lançamentos programados:
  - único
  - parcelado
  - fixo
  - por dia útil

## Histórico acumulado

- O projeto evoluiu de frontend majoritariamente visual para aplicação integrada de ponta a ponta.
- Os módulos centrais hoje já possuem fluxo real entre interface, API e persistência.
