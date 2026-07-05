# Changelog - Minhas Finanças

Registrar aqui apenas mudanças relevantes do sistema.

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
