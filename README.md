# Minhas Finanças

`Minhas Finanças` é uma plataforma de gestão financeira pessoal criada para transformar registros financeiros em decisões melhores ao longo do tempo.

Mais do que cadastrar receitas e despesas, o projeto busca organizar a operação financeira do usuário, dar contexto aos números e apoiar leitura, planejamento e acompanhamento patrimonial em um só lugar.

## Visão Geral

O sistema foi pensado para ir além do controle básico de movimentações. A proposta é ajudar o usuário a entender o presente, planejar o futuro e acompanhar sua evolução financeira com clareza.

Na prática, isso acontece por meio de módulos que combinam operação, histórico e análise, com foco em:

- planejamento financeiro
- controle patrimonial
- projeções e simulações
- indicadores e alertas
- histórico e rastreabilidade

## Principais funcionalidades

- Dashboard Financeiro
- Controle de Lançamentos
- Fluxo de Caixa Simples
- Patrimônio
- Metas
- Perfil Financeiro
- Simulações Financeiras
- Projeções
- Exportação para Excel

## Arquitetura

Visão de alto nível:

`Frontend`  
↓  
`API`  
↓  
`MySQL`

## Tecnologias

- .NET
- ASP.NET Core
- React
- TypeScript
- MySQL
- Entity Framework Core

## Como executar

### Backend

1. Acesse `minhas-financas-back-end/`.
2. Configure a connection string do banco MySQL em `appsettings.json` ou `appsettings.Development.json`.
3. Execute a API principal.

### Frontend

1. Acesse `minhas-financas-front-end/`.
2. Instale as dependências com `npm install`.
3. Execute `npm run dev`.

### Banco de dados

1. Garanta que o MySQL esteja disponível.
2. Utilize o banco `minhasfinancas`.
3. Aplique as migrations do backend antes de usar a aplicação.

## Documentação

A documentação detalhada do projeto está na pasta [`docs/`](docs).

- [PRODUCT_VISION.md](docs/PRODUCT_VISION.md): visão de produto e direção estratégica.
- [MODULE_GUIDE.md](docs/MODULE_GUIDE.md): finalidade de cada módulo e impacto no restante do sistema.
- [DOMAIN_GLOSSARY.md](docs/DOMAIN_GLOSSARY.md): conceitos oficiais do domínio financeiro usados no projeto.
- [ROADMAP.md](docs/ROADMAP.md): evolução planejada do produto.
- [CHANGELOG.md](docs/CHANGELOG.md): histórico das implementações já realizadas.

## Status do Projeto

O projeto está em desenvolvimento ativo.

Hoje a base principal já cobre autenticação, lançamentos, dashboard, fluxo de caixa, projeções, simulações, patrimônio, perfil financeiro, contas, cartões e categorias. As próximas evoluções concentram-se principalmente em metas, relatórios, orçamento e aprofundamento dos indicadores financeiros.

## Filosofia

O objetivo do Minhas Finanças não é apenas registrar movimentações financeiras, mas fornecer informações que ajudem o usuário a tomar melhores decisões ao longo do tempo.
