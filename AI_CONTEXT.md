# AI Context - Minhas FinanÃ§as

Este documento existe para dar a uma IA visÃ£o rÃ¡pida e confiÃ¡vel do estado tÃ©cnico atual do projeto.

Ele deve permanecer enxuto, arquitetural e orientado a implementaÃ§Ã£o.

InformaÃ§Ãµes de produto, roadmap, changelog e glossÃ¡rio de domÃ­nio ficam em documentos prÃ³prios dentro de `docs/`.

`docs/MODULE_GUIDE.md` contém a explicação funcional dos módulos e seus impactos no restante do sistema.

## Arquitetura da soluÃ§Ã£o

O backend segue uma arquitetura em camadas prÃ³xima de `Clean Architecture` / `Onion`, sem rigidez acadÃªmica absoluta:

- `API`: entrada HTTP, autenticaÃ§Ã£o, configuraÃ§Ã£o, DI e controllers
- `Application`: casos de uso, DTOs, interfaces, orquestraÃ§Ã£o e a maior parte das regras de negÃ³cio
- `Domain`: entidades persistidas e serviÃ§os de cÃ¡lculo
- `Infra`: EF Core, `ApplicationDbContext`, repositories e migrations
- `CrossCutting`: enums, utilitÃ¡rios e `RetornoGenerico`

ObservaÃ§Ã£o importante:

- No estado atual do projeto, a regra de negÃ³cio estÃ¡ majoritariamente em `Application/Services`
- `Controller` deve permanecer fino
- `Repository` deve cuidar de acesso a dados, nÃ£o de decisÃ£o de negÃ³cio

## Estrutura dos projetos

### Backend

- `minhas-financas-back-end/minhas-financas-back-end`
  - projeto ASP.NET Core Web API
- `MinhasFinancas.Application`
  - app services, DTOs, interfaces, recursos e mapeamentos
- `MinhasFinancas.Domain`
  - entidades e serviÃ§os de domÃ­nio
- `MinhasFinancas.Infra`
  - contexto EF Core, repositories e migrations
- `MinhasFinancas.CrossCutting`
  - enums, utilitÃ¡rios e tipos compartilhados
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
  - componentes visuais e telas por mÃ³dulo
- `minhas-financas-front-end/src/providers`
  - autenticaÃ§Ã£o, tema e infraestrutura global
- `minhas-financas-front-end/src/services/api`
  - cliente HTTP e serviÃ§os por domÃ­nio
- `minhas-financas-front-end/src/types`
  - contratos TypeScript
- `minhas-financas-front-end/src/lib`
  - helpers e utilitÃ¡rios transversais

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

## OrganizaÃ§Ã£o das camadas

### API

- expÃµe endpoints
- aplica autenticaÃ§Ã£o/autorizaÃ§Ã£o
- delega tudo para `Application`

### Application

- valida usuÃ¡rio
- valida consistÃªncia da operaÃ§Ã£o
- orquestra repositories
- chama serviÃ§os de domÃ­nio quando necessÃ¡rio
- devolve `RetornoGenerico`

### Domain

- modela entidades persistidas
- concentra cÃ¡lculos agregados e financeiros reutilizÃ¡veis

### Infra

- persistÃªncia com EF Core
- relaÃ§Ãµes entre entidades
- queries especÃ­ficas de leitura
- migrations

### CrossCutting

- enums compartilhados
- contratos utilitÃ¡rios

## Fluxo tÃ©cnico padrÃ£o

1. O frontend chama `apiRequest` em `src/services/api/http.ts`.
2. O request entra no controle global de loading.
3. O token JWT Ã© enviado quando existe sessÃ£o.
4. O controller recebe a rota.
5. O controller delega para um `AppService`.
6. O `AppService` valida usuÃ¡rio, DTO e estado de negÃ³cio.
7. O repository consulta ou persiste via `ApplicationDbContext`.
8. Se houver cÃ¡lculo agregado, `Application` usa `Domain`.
9. A resposta volta em `RetornoGenerico`.
10. O frontend interpreta `sucesso`, `mensagemUsuario` e `dados`.

## PadrÃµes adotados

- contrato padrÃ£o de resposta: `RetornoGenerico`
- controllers finos
- app services como eixo principal da regra de negÃ³cio
- repositories especÃ­ficos por mÃ³dulo quando a query exige comportamento extra
- DTOs organizados por mÃ³dulo
- validaÃ§Ãµes de formulÃ¡rio no front com `react-hook-form + zod`
- autenticaÃ§Ã£o no front persistida em `localStorage`
- `usuarioId` ainda Ã© passado em grande parte das rotas, mesmo com JWT

## Estrutura de pastas relevantes

### Backend

- `Controllers`
  - endpoints HTTP
- `Application/DTOs`
  - contratos de entrada e saÃ­da
- `Application/Interfaces`
  - contratos de app services
- `Application/Services`
  - regra de negÃ³cio
- `Application/Resources`
  - seeds e recursos auxiliares
- `Domain/Entities`
  - entidades persistidas
- `Domain/Services`
  - serviÃ§os de cÃ¡lculo
- `Infra/Data/Interfaces`
  - contratos de repository
- `Infra/Data/Repositories`
  - implementaÃ§Ãµes de acesso a dados
- `Infra/Migrations`
  - histÃ³rico do schema

### Frontend

- `src/app/(authenticated)`
  - Ã¡rea protegida
- `src/components/dashboard`
  - dashboard e radar financeiro
- `src/components/lancamentos`
  - listagem, filtros e modais de lanÃ§amentos
- `src/components/contas-cartoes`
  - CRUD de contas e cartÃµes
- `src/components/configuracoes`
  - categorias e subcategorias
- `src/components/projecao`
  - overview e detalhe de projeÃ§Ãµes
- `src/components/patrimonio`
  - visÃ£o patrimonial, modais e grÃ¡fico
- `src/components/simulacao-financeira`
  - overview, ediÃ§Ã£o e resultado das simulaÃ§Ãµes
- `src/services/api`
  - cliente HTTP, loading global e serviÃ§os de integraÃ§Ã£o

## Principais mÃ³dulos existentes

### AutenticaÃ§Ã£o

- login real com JWT
- cadastro real
- leitura do `usuarioId` a partir do token
- proteÃ§Ã£o de rotas autenticadas no frontend

### Categorias e subcategorias

- CRUD completo
- seed inicial por usuÃ¡rio no cadastro
- suporte ponta a ponta no cadastro de lanÃ§amentos

### Contas e cartÃµes

- CRUD completo
- seleÃ§Ã£o real no modal de lanÃ§amento
- modal rÃ¡pido de gerenciamento no dashboard

### LanÃ§amentos

- CRUD completo
- filtros
- ordenaÃ§Ã£o
- paginaÃ§Ã£o
- exportaÃ§Ã£o para Excel via endpoint backend e infraestrutura compartilhada de relatÃ³rios
- efetivaÃ§Ã£o rÃ¡pida de receitas e despesas
- suporte a lanÃ§amento Ãºnico, parcelado, fixo e por dia Ãºtil
- rastreabilidade com agrupadores de parcelamento e programaÃ§Ã£o
- parcelamentos podem ser gerenciados em grupo por `GrupoParcelamentoId`, com visualizaÃ§Ã£o das parcelas e ediÃ§Ã£o em lote dos campos comuns

### Dashboard

- agregados financeiros
- grÃ¡ficos principais
- radar financeiro

### Fluxo de Caixa Simples

- consolidaÃ§Ã£o mensal por `DataVencimento`
- resumo, comparativo e listas de receitas/despesas
- exportaÃ§Ã£o para Excel do mÃªs atual, intervalo de meses ou ano inteiro, com uma aba por mÃªs

### ProjeÃ§Ãµes

- mÃºltiplas projeÃ§Ãµes por usuÃ¡rio
- renda base
- renda extra mensal
- objetivo
- acumulado inicial
- modo atrelado a despesas ou manual

### PatrimÃ´nio

- ativos patrimoniais
- passivos
- snapshots manuais
- evoluÃ§Ã£o patrimonial

### SimulaÃ§Ãµes Financeiras

- cenÃ¡rios hipotÃ©ticos persistidos por usuÃ¡rio
- aÃ§Ãµes simuladas independentes dos dados reais
- cÃ¡lculo mensal usando lanÃ§amentos reais como base

### Perfil Financeiro

- parÃ¢metros financeiros pessoais por usuÃ¡rio
- histÃ³rico de configuraÃ§Ãµes vigentes e anteriores
- base futura para indicadores, dashboard, alertas e insights

## Infraestrutura e integraÃ§Ãµes existentes

### Banco de dados

- banco oficial atual: MySQL
- nome oficial da base: `minhasfinancas`
- migrations ficam em `MinhasFinancas.Infra/Migrations`

### Cliente HTTP

- `apiRequest` centraliza autenticaÃ§Ã£o, tratamento de erro e loading global
- toda requisiÃ§Ã£o integrada por esse cliente participa automaticamente do overlay global
- `downloadRequest` centraliza downloads binÃ¡rios autenticados, reaproveitando loading global e tratamento consistente de erro

### Infraestrutura de relatÃ³rios

- existe uma infraestrutura reutilizÃ¡vel de exportaÃ§Ã£o em `MinhasFinancas.Infra/Reports`
- a implementaÃ§Ã£o inicial cobre Excel em `Reports/Excel`
- `ExcelWorkbookFactory`, `ExcelStyleHelper`, `ExcelExtensions` e `ExcelReportBase` concentram criaÃ§Ã£o de workbook, estilos, formatos e layout compartilhado
- cada relatÃ³rio fornece apenas os dados e delega a apresentaÃ§Ã£o para a infraestrutura comum
- contratos de exportaÃ§Ã£o ficam em `MinhasFinancas.CrossCutting/Reports` para evitar acoplamento circular entre camadas

### Loading global

- implementado com provider/gerenciador compartilhado
- usa contador interno de requisiÃ§Ãµes ativas
- possui debounce e tempo mÃ­nimo de exibiÃ§Ã£o para evitar flicker

### Seed inicial

- ao cadastrar usuÃ¡rio, o sistema cria:
  - categorias e subcategorias iniciais
  - bens patrimoniais base

### Projetos auxiliares

- existe infraestrutura de Hangfire
- existe projeto SignalR
- hoje ambos ainda nÃ£o estÃ£o integrados ao fluxo principal do usuÃ¡rio

## DecisÃµes arquiteturais relevantes

### Por que `RetornoGenerico`

- padronizar o contrato da API
- simplificar consumo no frontend
- manter consistÃªncia de mensagens de negÃ³cio

### Por que Repository

- isolar acesso a dados da camada de aplicaÃ§Ã£o
- manter controllers e services sem EF Core direto
- permitir queries especÃ­ficas por mÃ³dulo

### Por que AutoMapper

- reduzir repetiÃ§Ã£o de mapeamento DTO -> entidade
- concentrar conversÃµes comuns no `MappingProfile`

### Onde fica a regra de negÃ³cio

- no projeto atual, a maior parte da regra mora em `Application/Services`
- `Domain` Ã© usado principalmente para entidades e cÃ¡lculos reutilizÃ¡veis

## ConvenÃ§Ãµes operacionais do cÃ³digo

- controllers com sufixo `Controller`
- services com sufixo `AppService`
- repositories com sufixo `Repository`
- DTOs separados por mÃ³dulo
- validaÃ§Ãµes crÃ­ticas no backend, mesmo quando jÃ¡ existem no frontend
- formulÃ¡rios do frontend preferem `react-hook-form + zod`

## Pontos de atenÃ§Ã£o

- preserve o contrato `RetornoGenerico`
- nÃ£o assuma que JWT elimina a necessidade de `usuarioId` na rota
- nÃ£o mova regra para controller
- parte da nomenclatura ainda Ã© legada e inconsistente
- `src/app` e `src/pages` coexistem no frontend
- o backend executa `app.MigrateDatabase()` no startup
- filtros de lanÃ§amentos ainda tÃªm parte da lÃ³gica aplicada apÃ³s o carregamento da lista
- o dashboard retorna alguns valores monetÃ¡rios jÃ¡ formatados em string
- projeÃ§Ãµes fazem preview local adicional no frontend

## DÃ­vida tÃ©cnica atual

- repetiÃ§Ã£o de validaÃ§Ã£o de usuÃ¡rio em vÃ¡rios app services
- repetiÃ§Ã£o de montagem manual de `RetornoGenerico`
- parte da lÃ³gica de lanÃ§amentos mistura persistÃªncia, saldo e patrimÃ´nio no mesmo service
- filtragem e paginaÃ§Ã£o de lanÃ§amentos ainda podem evoluir para consultas mais orientadas ao banco
- nomenclaturas legadas como `BancoId`, `faturamentoId` e `idPatrono`
- coexistÃªncia de `src/pages` com App Router
- warnings antigos de compatibilidade/vulnerabilidade em alguns pacotes do backend

## Estado Atual

### Data da Ãºltima atualizaÃ§Ã£o

- 05/07/2026

### MÃ³dulos concluÃ­dos

- autenticaÃ§Ã£o
- categorias e subcategorias
- contas e cartÃµes
- lanÃ§amentos
- dashboard
- fluxo de caixa simples
- projeÃ§Ãµes
- patrimÃ´nio
- simulaÃ§Ãµes financeiras
- perfil financeiro

### MÃ³dulos em desenvolvimento

- metas no frontend
- relatÃ³rios no frontend
- orÃ§amento
- integraÃ§Ãµes reais com Hangfire e SignalR

### PrÃ³xima implementaÃ§Ã£o prevista

- fechamento do mÃ³dulo de metas no frontend e evoluÃ§Ã£o dos relatÃ³rios
