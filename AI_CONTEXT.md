# AI Context - Minhas Finanças

Este documento existe para dar a uma IA visão rápida e confiável do estado técnico atual do projeto.

Ele deve permanecer enxuto, arquitetural e orientado a implementação.

Informações de produto, roadmap, changelog e glossário de domínio ficam em documentos próprios dentro de `docs/`.

## Arquitetura da solução

O backend segue uma arquitetura em camadas próxima de `Clean Architecture` / `Onion`, sem rigidez acadêmica absoluta:

- `API`: entrada HTTP, autenticação, configuração, DI e controllers
- `Application`: casos de uso, DTOs, interfaces, orquestração e a maior parte das regras de negócio
- `Domain`: entidades persistidas e serviços de cálculo
- `Infra`: EF Core, `ApplicationDbContext`, repositories e migrations
- `CrossCutting`: enums, utilitários e `RetornoGenerico`

Observação importante:

- No estado atual do projeto, a regra de negócio está majoritariamente em `Application/Services`
- `Controller` deve permanecer fino
- `Repository` deve cuidar de acesso a dados, não de decisão de negócio

## Estrutura dos projetos

### Backend

- `minhas-financas-back-end/minhas-financas-back-end`
  - projeto ASP.NET Core Web API
- `MinhasFinancas.Application`
  - app services, DTOs, interfaces, recursos e mapeamentos
- `MinhasFinancas.Domain`
  - entidades e serviços de domínio
- `MinhasFinancas.Infra`
  - contexto EF Core, repositories e migrations
- `MinhasFinancas.CrossCutting`
  - enums, utilitários e tipos compartilhados
- `Minhas-Financas-Hangfire`
  - projeto auxiliar de jobs
- `Minhas-Financas-hangfire.Infra`
  - infraestrutura auxiliar do Hangfire
- `Minhas-Financas-SignalR`
  - projeto dedicado a tempo real

### Frontend

- `minhas-financas-front-end/src/app`
  - App Router do Next.js
- `minhas-financas-front-end/src/pages`
  - rotas legadas de login e cadastro
- `minhas-financas-front-end/src/components`
  - componentes visuais e telas por módulo
- `minhas-financas-front-end/src/providers`
  - autenticação, tema e infraestrutura global
- `minhas-financas-front-end/src/services/api`
  - cliente HTTP e serviços por domínio
- `minhas-financas-front-end/src/types`
  - contratos TypeScript
- `minhas-financas-front-end/src/lib`
  - helpers e utilitários transversais

## Tecnologias utilizadas

### Backend

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- Pomelo.EntityFrameworkCore.MySql
- ASP.NET Core Identity
- JWT Bearer Authentication
- AutoMapper
- Swagger / OpenAPI
- Scalar.AspNetCore
- Hangfire
- SignalR

### Frontend

- Next.js 14
- React 18
- TypeScript 5
- Tailwind CSS
- React Hook Form
- Zod
- Recharts
- Radix UI
- next-themes
- lucide-react
- date-fns

## Organização das camadas

### API

- expõe endpoints
- aplica autenticação/autorização
- delega tudo para `Application`

### Application

- valida usuário
- valida consistência da operação
- orquestra repositories
- chama serviços de domínio quando necessário
- devolve `RetornoGenerico`

### Domain

- modela entidades persistidas
- concentra cálculos agregados e financeiros reutilizáveis

### Infra

- persistência com EF Core
- relações entre entidades
- queries específicas de leitura
- migrations

### CrossCutting

- enums compartilhados
- contratos utilitários

## Fluxo técnico padrão

1. O frontend chama `apiRequest` em `src/services/api/http.ts`.
2. O request entra no controle global de loading.
3. O token JWT é enviado quando existe sessão.
4. O controller recebe a rota.
5. O controller delega para um `AppService`.
6. O `AppService` valida usuário, DTO e estado de negócio.
7. O repository consulta ou persiste via `ApplicationDbContext`.
8. Se houver cálculo agregado, `Application` usa `Domain`.
9. A resposta volta em `RetornoGenerico`.
10. O frontend interpreta `sucesso`, `mensagemUsuario` e `dados`.

## Padrões adotados

- contrato padrão de resposta: `RetornoGenerico`
- controllers finos
- app services como eixo principal da regra de negócio
- repositories específicos por módulo quando a query exige comportamento extra
- DTOs organizados por módulo
- validações de formulário no front com `react-hook-form + zod`
- autenticação no front persistida em `localStorage`
- `usuarioId` ainda é passado em grande parte das rotas, mesmo com JWT

## Estrutura de pastas relevantes

### Backend

- `Controllers`
  - endpoints HTTP
- `Application/DTOs`
  - contratos de entrada e saída
- `Application/Interfaces`
  - contratos de app services
- `Application/Services`
  - regra de negócio
- `Application/Resources`
  - seeds e recursos auxiliares
- `Domain/Entities`
  - entidades persistidas
- `Domain/Services`
  - serviços de cálculo
- `Infra/Data/Interfaces`
  - contratos de repository
- `Infra/Data/Repositories`
  - implementações de acesso a dados
- `Infra/Migrations`
  - histórico do schema

### Frontend

- `src/app/(authenticated)`
  - área protegida
- `src/components/dashboard`
  - dashboard e radar financeiro
- `src/components/lancamentos`
  - listagem, filtros e modais de lançamentos
- `src/components/contas-cartoes`
  - CRUD de contas e cartões
- `src/components/configuracoes`
  - categorias e subcategorias
- `src/components/projecao`
  - overview e detalhe de projeções
- `src/components/patrimonio`
  - visão patrimonial, modais e gráfico
- `src/components/simulacao-financeira`
  - overview, edição e resultado das simulações
- `src/services/api`
  - cliente HTTP, loading global e serviços de integração

## Principais módulos existentes

### Autenticação

- login real com JWT
- cadastro real
- leitura do `usuarioId` a partir do token
- proteção de rotas autenticadas no frontend

### Categorias e subcategorias

- CRUD completo
- seed inicial por usuário no cadastro
- suporte ponta a ponta no cadastro de lançamentos

### Contas e cartões

- CRUD completo
- seleção real no modal de lançamento
- modal rápido de gerenciamento no dashboard

### Lançamentos

- CRUD completo
- filtros
- ordenação
- paginação
- exportação para Excel
- efetivação rápida de receitas e despesas
- suporte a lançamento único, parcelado, fixo e por dia útil
- rastreabilidade com agrupadores de parcelamento e programação

### Dashboard

- agregados financeiros
- gráficos principais
- radar financeiro

### Fluxo de Caixa Simples

- consolidação mensal por `DataVencimento`
- resumo, comparativo e listas de receitas/despesas

### Projeções

- múltiplas projeções por usuário
- renda base
- renda extra mensal
- objetivo
- acumulado inicial
- modo atrelado a despesas ou manual

### Patrimônio

- ativos patrimoniais
- passivos
- snapshots manuais
- evolução patrimonial

### Simulações Financeiras

- cenários hipotéticos persistidos por usuário
- ações simuladas independentes dos dados reais
- cálculo mensal usando lançamentos reais como base

### Perfil Financeiro

- parâmetros financeiros pessoais por usuário
- histórico de configurações vigentes e anteriores
- base futura para indicadores, dashboard, alertas e insights

## Infraestrutura e integrações existentes

### Banco de dados

- banco oficial atual: MySQL
- nome oficial da base: `minhasfinancas`
- migrations ficam em `MinhasFinancas.Infra/Migrations`

### Cliente HTTP

- `apiRequest` centraliza autenticação, tratamento de erro e loading global
- toda requisição integrada por esse cliente participa automaticamente do overlay global

### Loading global

- implementado com provider/gerenciador compartilhado
- usa contador interno de requisições ativas
- possui debounce e tempo mínimo de exibição para evitar flicker

### Seed inicial

- ao cadastrar usuário, o sistema cria:
  - categorias e subcategorias iniciais
  - bens patrimoniais base

### Projetos auxiliares

- existe infraestrutura de Hangfire
- existe projeto SignalR
- hoje ambos ainda não estão integrados ao fluxo principal do usuário

## Decisões arquiteturais relevantes

### Por que `RetornoGenerico`

- padronizar o contrato da API
- simplificar consumo no frontend
- manter consistência de mensagens de negócio

### Por que Repository

- isolar acesso a dados da camada de aplicação
- manter controllers e services sem EF Core direto
- permitir queries específicas por módulo

### Por que AutoMapper

- reduzir repetição de mapeamento DTO -> entidade
- concentrar conversões comuns no `MappingProfile`

### Onde fica a regra de negócio

- no projeto atual, a maior parte da regra mora em `Application/Services`
- `Domain` é usado principalmente para entidades e cálculos reutilizáveis

## Convenções operacionais do código

- controllers com sufixo `Controller`
- services com sufixo `AppService`
- repositories com sufixo `Repository`
- DTOs separados por módulo
- validações críticas no backend, mesmo quando já existem no frontend
- formulários do frontend preferem `react-hook-form + zod`

## Pontos de atenção

- preserve o contrato `RetornoGenerico`
- não assuma que JWT elimina a necessidade de `usuarioId` na rota
- não mova regra para controller
- parte da nomenclatura ainda é legada e inconsistente
- `src/app` e `src/pages` coexistem no frontend
- o backend executa `app.MigrateDatabase()` no startup
- filtros de lançamentos ainda têm parte da lógica aplicada após o carregamento da lista
- o dashboard retorna alguns valores monetários já formatados em string
- projeções fazem preview local adicional no frontend

## Dívida técnica atual

- repetição de validação de usuário em vários app services
- repetição de montagem manual de `RetornoGenerico`
- parte da lógica de lançamentos mistura persistência, saldo e patrimônio no mesmo service
- filtragem e paginação de lançamentos ainda podem evoluir para consultas mais orientadas ao banco
- nomenclaturas legadas como `BancoId`, `faturamentoId` e `idPatrono`
- coexistência de `src/pages` com App Router
- warnings antigos de compatibilidade/vulnerabilidade em alguns pacotes do backend

## Estado Atual

### Data da última atualização

- 05/07/2026

### Módulos concluídos

- autenticação
- categorias e subcategorias
- contas e cartões
- lançamentos
- dashboard
- fluxo de caixa simples
- projeções
- patrimônio
- simulações financeiras
- perfil financeiro

### Módulos em desenvolvimento

- metas no frontend
- relatórios no frontend
- orçamento
- integrações reais com Hangfire e SignalR

### Próxima implementação prevista

- fechamento do módulo de metas no frontend e evolução dos relatórios
