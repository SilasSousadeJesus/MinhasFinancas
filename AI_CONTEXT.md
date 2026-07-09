# AI Context - Minhas Finanças

Este documento existe para dar a uma IA visão rápida e confiÃ¡vel do estado tÃ©cnico atual do projeto.

Ele deve permanecer enxuto, arquitetural e orientado a implementaÃ§Ã£o.

InformaÃ§Ãµes de produto, roadmap, changelog e glossÃ¡rio de domÃ­nio ficam em documentos prÃ³prios dentro de `docs/`.

`docs/MODULE_GUIDE.md` contém a explicação funcional dos módulos e seus impactos no restante do sistema.

`docs/ASSISTANT_VISION.md` reúne a visão humana e evolutiva do Assistente Financeiro.

A camada `AnaliseFinanceira` distingue o comprometimento financeiro futuro de curto prazo da pressão financeira acumulada em horizontes maiores e agora expõe explicitamente, nesses indicadores, obrigações previstas, receita prevista e percentual de comprometimento.

O modelo oficial de risco financeiro do sistema é o `MF Score`, que organiza os indicadores em cinco pilares e passa a orientar a leitura da Saúde Financeira, do Assistente Financeiro e das integrações futuras.

O `MF Score` agora também possui uma etapa oficial de calibração contínua, com cenários de validação, explicação de variação, tendência e documentação dedicada em `docs/MF_SCORE.md` e `docs/INDICADORES_FINANCEIROS.md`.

Existe também uma suíte oficial de validação documentada em `docs/MF_SCORE_VALIDATION.md`, usada para confirmar se alterações futuras continuam coerentes com os cenários canônicos do modelo.

Além da suíte conceitual, o projeto agora possui uma auditoria operacional interna do `MF Score`, exposta apenas em desenvolvimento por `POST /api/MfScoreAuditoria/GerarPlanilha`, que monta personas sintéticas em memória, executa o motor oficial (`ContextoAnaliseFinanceira -> IndicadoresFinanceirosService -> SaudeFinanceiraService`) e devolve uma planilha `.xlsx` de conferência.

O projeto passa a ter também uma segunda auditoria interna, `POST /api/MfScoreAuditoria/GerarPlanilhaAuditoriaHumana`, voltada para avaliação humana cega das personas. Ela não aprova nem reprova automaticamente o motor; serve para documentar a nota que um consultor daria e transformar essa leitura em futuros padrões oficiais.

O fluxo de calibração do `MF Score` agora também possui um CRUD persistido de `Personas de Calibração`, exposto por `api/MfScorePersonas` e pela tela autenticada `/mf-score-personas`. Essas personas não representam usuários reais; são cenários sintéticos internos usados para cadastrar, auditar, rodar o motor oficial e promover casos maduros a `casos canônicos`.

`docs/MF_SCORE_AUDIT.md` deixou de ser apenas um resumo e passou a ser o documento oficial de governança técnica do Motor Financeiro, registrando cobertura, limitações conhecidas, achados de auditoria e dívida técnica.

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
  - comprometimento financeiro futuro em múltiplos horizontes
  - endividamento patrimonial
  - patrimÃ´nio lÃ­quido atual
  - percentual do patrimÃ´nio alvo
- a camada consome apenas dados jÃ¡ existentes:
  - lanÃ§amentos
  - bens patrimoniais
  - passivos
- configuraÃ§Ã£o vigente do perfil financeiro
- `SaudeFinanceiraService` interpreta os indicadores e gera pontuaÃ§Ã£o ponderada, classificaÃ§Ã£o e pontos de atenÃ§Ã£o
- `InsightsFinanceirosService` transforma indicadores e saÃºde financeira em alertas, oportunidades, destaques positivos e orientaÃ§Ãµes acionÃ¡veis
- `ResumoFinanceiroIAService` consolida saÃºde financeira, indicadores e insights em um payload Ãºnico pronto para consumo por interfaces e futuras integraÃ§Ãµes com IA
- os indicadores temporais são serializados com os campos `ValorObrigacoesPrevistas`, `ValorReceitaPrevista` e `PercentualComprometimento`, mantendo a leitura explícita na UI e no contexto enviado à IA
- as fórmulas oficiais, pesos e regras de interpretação dos indicadores ficam documentadas em `docs/INDICADORES_FINANCEIROS.md`
- o dashboard consome essa camada e nÃ£o deve recalcular indicadores diretamente
- a inteligÃªncia financeira deve evoluir respeitando a cadeia `Dados -> Indicadores -> SaÃºde Financeira -> Insights -> ResumoFinanceiroIA`

### SaÃºde Financeira

- tela dedicada para leitura consolidada da situaÃ§Ã£o financeira do usuÃ¡rio
- consome o endpoint `api/SaudeFinanceira/{usuarioId}`
- mostra pontuação geral, classificação textual, resumo geral com pontos de atenção integrados e todos os indicadores
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
- `AssistenteFinanceiroService` orquestra a preparação de contexto, prompt e resposta
- `InterpretadorMemoriaFinanceira` transforma o histórico resumido em narrativa de evolução e continuidade consultiva antes da chamada à IA
- `AvaliadorConsistenciaEstrategica` calcula de forma determinística o nível de consistência entre a pergunta do usuário, a situação financeira atual e o plano estratégico vigente
- `ConstrutorContextoIA` transforma `ResumoFinanceiroIA` em contexto textual estruturado e seguro para consumo externo
- `ConstrutorPromptIA` monta a requisiÃ§Ã£o final a partir do contexto preparado e do prompt base versionado em arquivo
- `IProvedorIA` abstrai provedores externos para evitar acoplamento com uma implementaÃ§Ã£o especÃ­fica
- `OpenAIProvider` agora possui implementaÃ§Ã£o real via HTTP para a API da OpenAI
- a chamada externa usa apenas `ResumoFinanceiroIA -> Memória Financeira -> InterpretadorMemoriaFinanceira -> Continuidade Consultiva -> Plano Estratégico Financeiro -> InterpretadorEstrategico -> Consistência Estratégica -> CompromissosFinanceiros -> Pareceres dos Especialistas -> ConstrutorContextoIA -> ConstrutorPromptIA -> IProvedorIA`
- a Fase 4.2.5 consolidou a IA Estratégica, que passa a conectar estado atual, evolução, plano, consistência e compromissos em uma única narrativa consultiva
- a Fase 4.2.6 adicionou especialistas internos por domínio, sem criar uma segunda IA
- o provedor trata timeout, retry simples, respostas vazias, autenticaÃ§Ã£o invÃ¡lida e falhas transitÃ³rias
- logs tÃ©cnicos existem, mas nÃ£o devem registrar API Key, prompt completo nem resposta completa da IA
- a chave de API nÃ£o deve ser versionada
- a configuraÃ§Ã£o oficial fica na seÃ§Ã£o `OpenAI`, com placeholders em `appsettings` e valor real vindo de `user-secrets` ou variÃ¡vel de ambiente `OpenAI__ApiKey`
- exemplo de configuraÃ§Ã£o local segura:
  - `dotnet user-secrets set "OpenAI:ApiKey" "sua-chave"` no projeto `minhas-financas-back-end/minhas-financas-back-end`
  - ou variÃ¡vel de ambiente `OpenAI__ApiKey`
- o roadmap passou a separar a evoluÃ§Ã£o da IA em fases técnicas e de experiência
- a Fase 4.2.5 passa a representar a IA Estratégica já consolidada no produto

### Assistente Financeiro

- tela dedicada no frontend em `src/app/(authenticated)/assistente-financeiro`
- consome exclusivamente o endpoint `api/ResumoFinanceiroIA/{usuarioId}`
- o backend tambÃ©m expÃµe `POST api/AssistenteFinanceiro/GerarAnalise/{usuarioId}` para execuÃ§Ã£o tÃ©cnica da anÃ¡lise via IA
- exibe um Ãºnico card principal de resumo executivo
- recebe o apoio dos especialistas internos por domínio antes da resposta final da IA
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
- a análise aprofundada já utiliza a Fase 4.2.5 para produzir leitura estratégica conectando estado atual, evolução, plano, consistência e compromissos
- os `Insights Financeiros` permanecem como bloco próprio de risco, oportunidade, configuração ou destaque positivo, sem exercer o mesmo papel do resumo ou da conclusão
- a seção `Consistência Estratégica` é montada pelo backend como avaliação determinística do alinhamento entre a decisão do usuário e o plano vigente
- usa links para levar o usuÃ¡rio para a anÃ¡lise completa em `SaÃºde Financeira`
- nÃ£o recalcula indicadores no frontend
- o resumo executivo pode ser minimizado localmente para priorizar a leitura da anÃ¡lise aprofundada
- possui card funcional de anÃ¡lise aprofundada com IA
- a tela permite pergunta opcional e usa uma pergunta padrÃ£o quando o usuÃ¡rio sÃ³ clica no botÃ£o
- o frontend chama o endpoint existente `POST api/AssistenteFinanceiro/GerarAnalise/{usuarioId}`
- o frontend tambÃ©m consome `GET api/AnalisesFinanceirasHistoricas/{usuarioId}` e `GET api/AnalisesFinanceirasHistoricas/{usuarioId}/{analiseId}` para carregar o histÃ³rico visual da MemÃ³ria Financeira
- o endpoint `DELETE api/AnalisesFinanceirasHistoricas/{usuarioId}/{analiseId}` realiza exclusÃ£o lÃ³gica do histÃ³rico ao marcar a anÃ¡lise como inativa
- a resposta da IA Ã© exibida em Markdown na prÃ³pria tela, com estilo de parecer executivo
- a tela permite copiar a anÃ¡lise, gerar novamente, abrir anÃ¡lises anteriores e excluir anÃ¡lises do histÃ³rico visual
- o histÃ³rico e a anÃ¡lise aprofundada ficam em cards recolhÃ­veis, iniciando minimizados para deixar a tela mais compacta
- cada interaÃ§Ã£o usa a prÃ³pria pergunta do usuÃ¡rio como identificador principal no histÃ³rico, com truncamento visual quando necessÃ¡rio
- o card principal da anÃ¡lise aprofundada reutiliza o mesmo espaÃ§o para respostas novas e anÃ¡lises histÃ³ricas selecionadas
- erros amigÃ¡veis retornados pela API sÃ£o mostrados sem apagar a anÃ¡lise anterior
- os especialistas internos jÃ¡ alimentam o contexto consolidado com pareceres por domÃ­nio financeiro
- nÃ£o existe chat nem conversa contÃ­nua nesta fase
- a MemÃ³ria Financeira continua sendo responsabilidade do backend; o frontend apenas solicita a geraÃ§Ã£o e exibe o resultado
- a IA passou a receber a seção `Evolução Financeira` como interpretação oficial da continuidade histórica, e não apenas uma lista cronológica
- a IA também recebe a seção `Consistência Estratégica` como avaliação oficial e determinística do alinhamento com o plano vigente

### Personas de Calibração do MF Score

- tela interna autenticada em `/mf-score-personas`
- CRUD persistido de cenários sintéticos para calibrar o Motor Financeiro
- permite registrar dados simulados, avaliação humana, faixa esperada e justificativa
- permite rodar o motor oficial do `MF Score` sem duplicar fórmulas
- permite marcar a persona como `Auditada` ou `Caso Canônico`
- nesta V1, a persona persiste sinais de planejamento adicionais, mas o cálculo continua refletindo apenas a cobertura atual do motor oficial

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

- 07/07/2026

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
- plano estratÃ©gico financeiro

### MÃ³dulos em desenvolvimento

- metas no frontend
- relatÃ³rios no frontend
- orÃ§amento
- integraÃ§Ãµes reais com Hangfire e SignalR

### Próxima implementação prevista

- Fase 4.2.8 — Simulador Inteligente no roadmap de Inteligência Financeira
- a Fase 4.2.4 — Compromissos Financeiros foi concluída e passou a integrar a cadeia do Assistente Financeiro
- a etapa de continuidade conversacional foi retirada da visão oficial, mantendo o Assistente Financeiro como consultor e não como chat contínuo
- a intenção do usuário já pode ser interpretada como `DecisaoFinanceiraIA` antes da montagem do contexto estratégico
- evolução da Base de Conhecimento Financeira para registrar direção estratégica do usuário ao longo do tempo
- consolidação da tela de gestão do Plano Estratégico Financeiro no frontend, com edição por nova versão e histórico simples

## Atualização da documentação de IA

- `docs/AI_DESIGN.md` passou a ser o documento oficial de design da camada de IA
- a Fase 4.2 foi iniciada com a formalização do fluxo de análise executiva com IA
- o primeiro prompt oficial da Fase 4.2.5 foi implementado em `MinhasFinancas.Infra/IA/Prompts/PromptAnaliseFinanceira.md`
A cadeia oficial continua sendo `ResumoFinanceiroIA -> Memória Financeira -> InterpretadorMemoriaFinanceira -> InterpretadorDecisaoFinanceira -> Plano Estratégico Financeiro -> InterpretadorEstrategico -> Consistência Estratégica -> ConstrutorContextoIA -> ConstrutorPromptIA -> IProvedorIA`

## Atualização técnica — Base de Conhecimento Financeira

As subfases 4.2.1, 4.2.2.1, 4.2.3.2 e 4.2.3.3 adicionaram a primeira versão da Base de Conhecimento Financeira do Assistente Financeiro, sua interpretação histórica, sua leitura estratégica e sua consistência determinística.

### Novos elementos estruturais
- `TipoDecisaoFinanceira` em `MinhasFinancas.Infra/IA/Enums`
- `DecisaoFinanceiraIA` em `MinhasFinancas.Infra/IA/Modelos`
- `InterpretadorDecisaoFinanceira` em `MinhasFinancas.Infra/IA/Interpretadores`

- entidade `AnaliseFinanceiraHistorica`
- `DbSet<AnaliseFinanceiraHistorica>` no `ApplicationDbContext`
- `IAnaliseFinanceiraHistoricaRepository` e `AnaliseFinanceiraHistoricaRepository`
- `IAnaliseFinanceiraHistoricaAppService` e `AnaliseFinanceiraHistoricaAppService`
- modelo `MemoriaFinanceiraResumidaIA` em `MinhasFinancas.Infra/IA/Modelos`
- modelo `InterpretacaoMemoriaFinanceiraIA` em `MinhasFinancas.Infra/IA/Modelos`
- modelo `InterpretacaoPlanoEstrategicoIA` em `MinhasFinancas.Infra/IA/Modelos`
- `InterpretadorMemoriaFinanceira` em `MinhasFinancas.Infra/IA/Interpretadores`
- `InterpretadorEstrategico` em `MinhasFinancas.Infra/IA/Interpretadores`
- `ContextoAssistenteFinanceiro.DecisaoFinanceira` para armazenar a decisão interpretada na montagem do contexto
- `AvaliadorConsistenciaEstrategica` passou a aceitar `DecisaoFinanceiraIA` como entrada opcional para futura evolução do raciocínio
- endpoints `GET /api/AnalisesFinanceirasHistoricas/{usuarioId}` e `GET /api/AnalisesFinanceirasHistoricas/{usuarioId}/{analiseId}`

### Conceitos da base

- **Memória Financeira**
  - implementada
  - registra fotografias históricas completas da situação analisada
- **Plano Estratégico Financeiro**
  - já implementado na fase 4.2.3.1
  - registra a direção estratégica escolhida pelo usuário ao longo do tempo em versões históricas
- **Compromissos Financeiros**
  - implementados
  - registram ações e decisões combinadas com o Assistente

### Fluxo atualizado do Assistente Financeiro

1. O sistema monta o `ResumoFinanceiroIA`.
2. O backend consulta a Memória Financeira e recupera um resumo das últimas análises.
3. O `InterpretadorMemoriaFinanceira` transforma esse histórico em uma narrativa estruturada de continuidade.
4. O `InterpretadorEstrategico` transforma o plano vigente em leitura estratégica textual para a IA.
5. O `ConstrutorContextoIA` gera o contexto textual, incluindo as seções `## Evolução Financeira`, `## Memória Financeira`, `## Compromissos Financeiros` e `## Plano Estrategico Financeiro`.
6. O `ConstrutorPromptIA` gera a requisição final e informa a versão do prompt.
7. O provedor retorna a resposta real ou a falha tratada.
8. O backend salva uma `AnaliseFinanceiraHistorica` com contexto, resumos, resposta e métricas técnicas.
9. O retorno ao cliente pode incluir `AnaliseFinanceiraHistoricaId`.

### Regra de contexto

- a IA nunca recebe todas as análises históricas
- o contexto usa apenas um resumo recente das últimas análises
- a continuidade histórica passa a ser enviada principalmente como narrativa interpretada em `## Evolução Financeira`
- as seções `## Memória Financeira` e `## Compromissos Financeiros` permanecem como apoio resumido, e não como blocos principais de interpretação
- quando não existe histórico, o contexto informa explicitamente que ainda não existem análises suficientes para avaliar evolução



