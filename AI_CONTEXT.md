# AI Context - Minhas Finanças

Este documento existe para dar a uma IA visão rápida e confiÃ¡vel do estado tÃ©cnico atual do projeto.

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
  - inclui `AnaliseFinanceira`, responsÃ¡vel pela camada analÃ­tica reutilizÃ¡vel do sistema
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
- o dashboard deixou de concentrar a leitura analÃ­tica detalhada da saÃºde financeira para evitar duplicidade com as telas especializadas

### Camada AnalÃ­tica

- localizada em `MinhasFinancas.Domain/Services/AnaliseFinanceira`
- `IndicadoresFinanceirosService` orquestra os cÃ¡lculos e devolve um painel reutilizÃ¡vel
- cada indicador possui responsabilidade Ãºnica e implementaÃ§Ã£o isolada
- V1 implementada:
  - economia mensal
  - percentual de economia
  - reserva de emergÃªncia atual
  - reserva de emergÃªncia ideal
  - comprometimento da renda
  - endividamento
  - patrimÃ´nio lÃ­quido atual
  - percentual do patrimÃ´nio alvo
- a camada consome apenas dados jÃ¡ existentes:
  - lanÃ§amentos
  - bens patrimoniais
  - passivos
- configuraÃ§Ã£o vigente do perfil financeiro
- `SaudeFinanceiraService` interpreta os indicadores e gera pontuaÃ§Ã£o, classificaÃ§Ã£o e pontos de atenÃ§Ã£o
- `InsightsFinanceirosService` transforma indicadores e saÃºde financeira em alertas, oportunidades, destaques positivos e orientaÃ§Ãµes acionÃ¡veis
- `ResumoFinanceiroIAService` consolida saÃºde financeira, indicadores e insights em um payload Ãºnico pronto para consumo por interfaces e futuras integraÃ§Ãµes com IA
- o dashboard consome essa camada e nÃ£o deve recalcular indicadores diretamente
- a inteligÃªncia financeira deve evoluir respeitando a cadeia `Dados -> Indicadores -> SaÃºde Financeira -> Insights -> ResumoFinanceiroIA`

### SaÃºde Financeira

- tela dedicada para leitura consolidada da situaÃ§Ã£o financeira do usuÃ¡rio
- consome o endpoint `api/SaudeFinanceira/{usuarioId}`
- mostra pontuaÃ§Ã£o geral, classificaÃ§Ã£o textual, pontos de atenÃ§Ã£o e todos os indicadores
- centraliza os insights financeiros do `ResumoFinanceiroIA`
- possui Ã¡rea reservada para grÃ¡ficos analÃ­ticos futuros, como evoluÃ§Ã£o patrimonial, economia mensal e reserva de emergÃªncia
- usa a mesma base analÃ­tica do dashboard

### InteligÃªncia Financeira

- `InteligenciaFinanceiraAppService` reaproveita os indicadores e a saÃºde financeira jÃ¡ existentes para montar as camadas seguintes, sem duplicar cÃ¡lculos
- o endpoint `api/InsightsFinanceiros/{usuarioId}` devolve a primeira versÃ£o dos insights financeiros
- o endpoint `api/ResumoFinanceiroIA/{usuarioId}` devolve um resumo consolidado com prioridades imediatas, destaques positivos, indicadores, saÃºde financeira e texto executivo
- `ResumoExecutivo` Ã© gerado por regras no backend e explica o significado da situaÃ§Ã£o atual, sem repetir apenas pontuaÃ§Ã£o e classificaÃ§Ã£o
- `PrioridadesImediatas` sÃ£o geradas como aÃ§Ãµes curtas e diretas, sem copiar literalmente os insights

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

### Infraestrutura de IA

- o contexto atual da IA passou a ser montado por seções, com resumo executivo, saúde financeira, pontos de atenção, prioridades, destaques, indicadores e insights
- `RespostaIA` agora devolve métricas de observabilidade:
  - tokens de entrada estimados
  - tokens de entrada utilizados
  - tokens de saída utilizados
  - tokens de raciocínio quando disponíveis
  - total de tokens
  - tempo total da requisição
  - custo estimado em USD
- o custo estimado usa preços configuráveis na seção `OpenAI`
- a aplicação superior continua conhecendo apenas `IProvedorIA`; a classificação genérica de falhas usa `CategoriaErroIA`, reduzindo acoplamento específico com OpenAI
- radar financeiro detalhado, histórico, projeções, simulações e detalhamento patrimonial ainda não entram como blocos próprios do contexto e seguem como oportunidade de evolução para a fase seguinte

- localizada em `MinhasFinancas.Infra/IA`
- `AssistenteFinanceiroService` orquestra a preparaÃ§Ã£o de contexto, prompt e resposta
- `ConstrutorContextoIA` transforma `ResumoFinanceiroIA` em contexto textual estruturado e seguro para consumo externo
- `ConstrutorPromptIA` monta a requisiÃ§Ã£o final a partir do contexto preparado e do prompt base versionado em arquivo
- `IProvedorIA` abstrai provedores externos para evitar acoplamento com uma implementaÃ§Ã£o especÃ­fica
- `OpenAIProvider` agora possui implementaÃ§Ã£o real via HTTP para a API da OpenAI
- a chamada externa usa apenas `ResumoFinanceiroIA -> ConstrutorContextoIA -> ConstrutorPromptIA -> IProvedorIA`
- o provedor trata timeout, retry simples, respostas vazias, autenticaÃ§Ã£o invÃ¡lida e falhas transitÃ³rias
- logs tÃ©cnicos existem, mas nÃ£o devem registrar API Key, prompt completo nem resposta completa da IA
- a chave de API nÃ£o deve ser versionada
- a configuraÃ§Ã£o oficial fica na seÃ§Ã£o `OpenAI`, com placeholders em `appsettings` e valor real vindo de `user-secrets` ou variÃ¡vel de ambiente `OpenAI__ApiKey`
- exemplo de configuraÃ§Ã£o local segura:
  - `dotnet user-secrets set "OpenAI:ApiKey" "sua-chave"` no projeto `minhas-financas-back-end/minhas-financas-back-end`
  - ou variÃ¡vel de ambiente `OpenAI__ApiKey`
- a cadeia prevista para integraÃ§Ã£o futura Ã© `ResumoFinanceiroIA -> ConstrutorContextoIA -> ConstrutorPromptIA -> IProvedorIA`
- o roadmap passou a separar a evoluÃ§Ã£o futura em duas subfases:
  - `Fase 4.1`, agora implementada para ativaÃ§Ã£o tÃ©cnica real com o provedor
  - `Fase 4.2`, focada na primeira experiÃªncia real de anÃ¡lise financeira com IA

### Assistente Financeiro

- tela dedicada no frontend em `src/app/(authenticated)/assistente-financeiro`
- consome exclusivamente o endpoint `api/ResumoFinanceiroIA/{usuarioId}`
- o backend tambÃ©m expÃµe `POST api/AssistenteFinanceiro/GerarAnalise/{usuarioId}` para execuÃ§Ã£o tÃ©cnica da anÃ¡lise via IA
- exibe um Ãºnico card principal de resumo executivo
- dentro desse resumo, organiza a leitura em seÃ§Ãµes com tÃ­tulos:
  - resumo
  - prioridades
  - principais indicadores
  - leitura estratÃ©gica
  - conclusÃ£o
- cada seÃ§Ã£o textual responde a uma pergunta diferente:
  - resumo executivo explica a situaÃ§Ã£o atual
  - prioridades indicam aÃ§Ãµes curtas
  - principais indicadores sustentam a leitura com nÃºmeros
  - leitura estratÃ©gica destaca forÃ§as e riscos
  - conclusÃ£o sintetiza o parecer final do perÃ­odo
- `principais indicadores` e `leitura estratÃ©gica` sÃ£o exibidos em formato textual
- os textos de `principais indicadores` e `pontos de atenÃ§Ã£o` do assistente sÃ£o interpretativos e gerados por regras prÃ³prias no frontend, sem reutilizar literalmente `descricao` e `observacao` tÃ©cnicas dos indicadores
- a seÃ§Ã£o `conclusÃ£o` Ã© gerada por regras determinÃ­sticas no frontend via `ConclusaoFinanceiraBuilder`, sem uso de IA generativa
- a conclusÃ£o nÃ£o reutiliza descriÃ§Ãµes tÃ©cnicas nem repete literalmente prioridades; ela usa frases interpretativas prÃ³prias por indicador para formar um parecer executivo
- a abertura da conclusÃ£o varia conforme pontuaÃ§Ã£o/classificaÃ§Ã£o, para soar menos automÃ¡tica
- os `Insights Financeiros` permanecem como bloco prÃ³prio de risco, oportunidade, configuraÃ§Ã£o ou destaque positivo, sem exercer o mesmo papel do resumo ou da conclusÃ£o
- usa links para levar o usuÃ¡rio para a anÃ¡lise completa em `SaÃºde Financeira`
- nÃ£o recalcula indicadores no frontend
- possui card final de anÃ¡lise aprofundada com IA com botÃ£o desabilitado nesta fase
- continua sem qualquer chamada real para IA
- a integraÃ§Ã£o tÃ©cnica real do backend jÃ¡ existe, mas a qualidade final da experiÃªncia e do relatÃ³rio continua reservada para a `Fase 4.2`

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

- 06/07/2026

### MÃ³dulos concluÃ­dos

- autenticaÃ§Ã£o
- assistente financeiro
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

- Fase 4.2 â€” Primeira AnÃ¡lise Financeira com IA do roadmap de InteligÃªncia Financeira
- evoluÃ§Ã£o do prompt oficial, do formato do relatÃ³rio e da experiÃªncia final do assistente, mantendo `ResumoFinanceiroIA` como Ãºnica fonte de contexto
