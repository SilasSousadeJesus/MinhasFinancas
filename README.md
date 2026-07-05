# Minhas Finanças

## 1. Tecnologias do projeto

### 1.1 Backend

- ASP.NET Core Web API
- .NET
- Entity Framework Core
- MySQL
- Microsoft.AspNetCore.Identity
- JWT Bearer Authentication
- AutoMapper
- Swagger / OpenAPI
- Scalar.AspNetCore
- Hangfire
- SignalR

### 1.2 Frontend

- Next.js 14
- React 18
- TypeScript
- Tailwind CSS
- PostCSS
- next-themes
- next-auth
- react-hook-form
- zod
- @hookform/resolvers
- Recharts
- date-fns
- lucide-react
- class-variance-authority
- clsx
- tailwind-merge
- tailwindcss-animate
- tailwindcss-debug-screens
- Bibliotecas Radix UI:
- `@radix-ui/react-accordion`
- `@radix-ui/react-alert-dialog`
- `@radix-ui/react-aspect-ratio`
- `@radix-ui/react-avatar`
- `@radix-ui/react-checkbox`
- `@radix-ui/react-collapsible`
- `@radix-ui/react-context-menu`
- `@radix-ui/react-dialog`
- `@radix-ui/react-dropdown-menu`
- `@radix-ui/react-hover-card`
- `@radix-ui/react-icons`
- `@radix-ui/react-label`
- `@radix-ui/react-menubar`
- `@radix-ui/react-navigation-menu`
- `@radix-ui/react-popover`
- `@radix-ui/react-progress`
- `@radix-ui/react-radio-group`
- `@radix-ui/react-scroll-area`
- `@radix-ui/react-select`
- `@radix-ui/react-separator`
- `@radix-ui/react-slider`
- `@radix-ui/react-slot`
- `@radix-ui/react-switch`
- `@radix-ui/react-tabs`
- `@radix-ui/react-toast`
- `@radix-ui/react-toggle`
- `@radix-ui/react-toggle-group`
- `@radix-ui/react-tooltip`
- Outras bibliotecas de UI e suporte:
- `cmdk`
- `embla-carousel-react`
- `input-otp`
- `react-day-picker`
- `react-resizable-panels`
- `sonner`
- `vaul`
- Ferramentas de desenvolvimento:
- `eslint`
- `eslint-config-next`
- `@types/node`
- `@types/react`
- `@types/react-dom`

## 2. Entendimento atual do projeto

### 2.1 Estrutura geral

- O projeto é dividido em dois blocos principais: `minhas-financas-front-end/` e `minhas-financas-back-end/`.
- O backend segue arquitetura em camadas, com separação entre API, aplicação, domínio e infraestrutura.
- O frontend usa App Router do Next.js e organiza a interface entre `src/app`, `src/pages`, `src/components`, `src/providers` e `src/services`.

### 2.2 Como o backend funciona hoje

- A API já possui controllers para autenticação, usuários, contas, cartões, categorias, lançamentos, dashboard, metas, passivos, bens patrimoniais, relatórios, potencial de compra e sorteios.
- A autenticação é feita via JWT e usa `Microsoft.AspNetCore.Identity`.
- O login retorna um `RetornoGenerico` contendo um `TokenViewModel` com token e refresh token.
- O token carrega informações importantes do usuário, como `sub`, `email` e `name`.
- Grande parte dos endpoints protegidos exige o `usuarioId` na rota, então o frontend precisará armazenar o token e recuperar o identificador do usuário para operar nos módulos autenticados.
- A lógica de negócio principal está concentrada nos `AppService`, enquanto a persistência está em repositórios da camada `Infra`.

### 2.3 Como o frontend funciona hoje

- A página inicial carrega a tela de login.
- Existe uma tela de cadastro separada.
- A sidebar, o tema, o layout base e parte do dashboard visual já estão montados.
- O dashboard já tem estrutura visual pronta, com cards, gráficos e navegação, mas os dados exibidos ainda são fixos.
- As telas de `contas-e-cartoes`, `lancamentos`, `metas`, `orcamento`, `configuracoes` e relatórios ainda estão em estado inicial, muitas delas apenas com placeholders.
- Não encontrei uma camada de integração real com a API no frontend.
- Não encontrei uso efetivo de `fetch`, `axios`, `Authorization: Bearer`, `useSession`, `signIn` ou `signOut`.

### 2.4 O que está só como view hoje

- O formulário de login valida os campos com `zod` e `react-hook-form`, mas no submit apenas executa um `alert`.
- O formulário de cadastro segue o mesmo padrão: valida visualmente, mas ainda não envia dados ao backend.
- A rota de autenticação do NextAuth existe em `src/services/api/auth/[...nextauth].js`, porém o arquivo está vazio.
- O dashboard exibe nome fixo de usuário, valores fixos como `R$ 0,00` e gráficos estáticos.

### 2.5 O que parece pronto para integração

- O backend já oferece endpoints suficientes para iniciar a vida real do frontend pelos fluxos de autenticação, cadastro de usuário e dashboard.
- O frontend já possui boa base visual para receber integração incremental sem precisar reconstruir a interface do zero.
- O primeiro caminho natural de integração é:

1. Login
2. Cadastro
3. Sessão/autenticação no frontend
4. Dashboard
5. Contas e cartões
6. Lançamentos
7. Metas
8. Relatórios

### 2.6 Riscos e pontos de atenção encontrados

- O backend executa `app.MigrateDatabase()` no startup. Isso significa que apenas subir a API já tenta rodar migrações automaticamente.
- A ordem do pipeline de autenticação no `Program.cs` está invertida, com `UseAuthorization()` antes de `UseAuthentication()`.
- O fluxo de login no `AutenticacaoAppService` pode quebrar em caso de falha de autenticação, porque o código cria `TokenViewModel` mesmo quando as credenciais podem estar nulas.
- Não encontrei configuração de CORS no `Program.cs`, o que pode bloquear chamadas do frontend para a API no navegador.
- A parte de subcategorias está exposta no controller, mas no service ainda possui métodos com `NotImplementedException`.
- Há inconsistências de nomenclatura em alguns endpoints do backend, como uso misto de `bancoId` e `contaId`, o que merece revisão antes da integração de formulários e telas CRUD.

## 3. Visão geral narrativa

O projeto "Minhas Finanças" é uma solução completa para controle financeiro pessoal e familiar. Ele nasceu para ajudar o usuário a acompanhar contas, cartões, lançamentos, categorias, metas, bens patrimoniais e passivos em um único lugar, com suporte a login seguro e dashboards analíticos.

Na prática, o aplicativo funciona como uma plataforma de gestão financeira onde cada usuário pode se cadastrar, fazer login, cadastrar suas contas bancárias, cartões, despesas e receitas, monitorar metas e visualizar relatórios e gráficos. O objetivo é transformar dados financeiros em informações úteis, mostrando não apenas valores isolados, mas também o comportamento das despesas por categoria, o total de bens, passivos e possibilidades de compra.

O fluxo natural do projeto foca em três camadas:

- Interface do usuário no navegador, feita em Next.js e com componentes modernos.
- Uma API REST em ASP.NET Core que centraliza a lógica de autenticação, validação, persistência e retorno de resultados.
- Uma camada de dados baseada em Entity Framework Core e MySQL para gravar informações financeiras estruturadas.

Esse conjunto permite que o aplicativo evolua para uma solução híbrida onde o usuário entra no sistema, registra dados e visualiza indicadores relevantes sem sair do fluxo.

## 4. Detalhes técnicos

### 4.1 Estrutura do repositório

- `minhas-financas-back-end/`
  - `minhas-financas-back-end/`: projeto principal da API ASP.NET Core
  - `MinhasFinancas.Application/`: camada de aplicação com serviços, DTOs, interfaces e configurações
  - `MinhasFinancas.Domain/`: entidades de domínio e regras de negócio
  - `MinhasFinancas.Infra/`: contexto de dados, repositórios e acesso ao banco de dados
  - `Minhas-Financas-Hangfire/`: projeto de jobs agendados (Hangfire)
  - `Minhas-Financas-hangfire.Infra/`: infraestrutura de Hangfire
  - `Minhas-Financas-SignalR/`: projeto para comunicação em tempo real

- `minhas-financas-front-end/`
  - `src/app/`: páginas e roteamento do Next.js
  - `src/components/`: componentes de interface reutilizáveis
  - `src/lib/`: utilitários e funções auxiliares
  - `src/providers/`: providers de tema e contexto
  - `src/services/`: serviços de API e autenticação

### 4.2 Backend

#### 4.2.1 API e endpoints

A API disponibiliza controladores para os principais módulos do sistema:

- `AutenticacaoController`: login de usuário com JWT
- `UsuarioController`: cadastro, edição, busca e exclusão de usuários
- `ContaController`: controle de contas bancárias
- `CartaoController`: gestão de cartões
- `CategoriaController`: cadastro e consulta de categorias e subcategorias
- `LancamentoController`: CRUD de lançamentos financeiros e filtros por categoria
- `DashboardController`: dados resumidos para painel financeiro
- `MetaController`: metas e objetivos financeiros
- `PassivoController`: registro de dívidas/passivos
- `BemMaterialController`: bens patrimoniais
- `PotecialCompraController`: potencial de compra de imóveis
- `RelatoriosController`: geração de relatórios financeiros
- `SorteiosController`: cadastro e gestão de sorteios

#### 4.2.2 Autenticação e segurança

- O backend usa `Microsoft.AspNetCore.Identity` para usuários e roles.
- A autenticação é baseada em JWT, configurada em `Extensions/AuthenticationSetup.cs`.
- O JWT é validado com issuer, audience e chave simétrica definidas em `appsettings.json`.
- Endpoints críticos usam `[Authorize]` para exigir token válido.

#### 4.2.3 Persistência de dados

- `ApplicationDbContext` extende `IdentityDbContext<Usuario>` e expõe `DbSet` para as entidades principais:
  - Conta, Cartao, Categoria, SubCategoria, Lancamento
  - LancamentoFixo, LancamentoParcelado, BemPatrimonial, Meta, Passivo
- O mapeamento de relacionamentos e regras de exclusão está em `OnModelCreating`.
- A conexão do MySQL está configurada em `appsettings.json`.

#### 4.2.4 Camada de aplicação

- A camada `MinhasFinancas.Application` define interfaces como `IContaAppService`, `ILancamentoAppService`, `IDashboardAppService`, etc.
- Os serviços da aplicação são registrados no `Program.cs` e fazem a ponte entre controllers e repositórios.
- O AutoMapper é configurado para mapear DTOs entre a camada de aplicação e as entidades de domínio.

### 4.3 Frontend

#### 4.3.1 Stack e ferramentas

- Next.js 14 com App Router
- React 18
- Tailwind CSS para estilos
- Radix UI para componentes acessíveis
- React Hook Form + Zod para formulários e validação
- Recharts para visualização de gráficos
- Next Auth como base de autenticação (a rota de auth existe, mas está sem implementação no momento)
- `ThemeProvider` para modo claro/escuro

#### 4.3.2 Fluxo do frontend

- A página inicial (`src/app/page.tsx`) carrega o componente de login.
- Assim que o usuário faz login, o frontend deve consumir a API de autenticacao e receber o JWT.
- Com o token, o app passa a acessar rotas protegidas para buscar dados de usuário, lançamentos, dashboard e demais módulos.
- Os componentes de sidebar e layout controlam a navegação entre telas internas (como relatórios, metas, contas e cartões).

### 4.4 Fluxo de dados geral

1. O usuário entra no frontend e faz login.
2. O frontend chama `POST api/Autenticacao/Login` com credenciais.
3. O backend valida o login e retorna um token JWT.
4. O frontend armazena o token e envia em `Authorization: Bearer {token}` para chamadas subsequentes.
5. O backend recebe o token e permite acesso a endpoints protegidos como `api/Lancamento`, `api/Usuario`, `api/Dashboard`.
6. Os endpoints usam services e repositórios para consultar e gravar no banco MySQL.
7. O resultado é retornado ao frontend, que atualiza a interface e exibe gráficos, tabelas e valores.

### 4.5 Como rodar o projeto

#### Backend

1. Abra a solução em Visual Studio ou VS Code.
2. Suba um MySQL local ou ajuste `ConnectionStrings:ConnectionMinhasFinancas` em `minhas-financas-back-end/minhas-financas-back-end/appsettings.json`.
3. Gere a migration inicial MySQL.
4. Aplique a migration no banco.
5. Execute `minhas-financas-back-end/minhas-financas-back-end`.
6. A API expõe Swagger em modo Development em `/swagger`.

### 4.5.1 Setup do MySQL

Connection string base usada no projeto:

`Server=localhost;Port=3306;Database=minhasfinancas;User=root;Password=senha;Allow User Variables=True`

Se quiser subir localmente com Docker:

```bash
docker compose up -d
```

Comandos para gerar e aplicar migrations MySQL:

```bash
dotnet ef migrations add InitialMySql --project minhas-financas-back-end/MinhasFinancas.Infra/MinhasFinancas.Infra.csproj --startup-project minhas-financas-back-end/minhas-financas-back-end/MinhasFinancas.API.csproj
dotnet ef database update --project minhas-financas-back-end/MinhasFinancas.Infra/MinhasFinancas.Infra.csproj --startup-project minhas-financas-back-end/minhas-financas-back-end/MinhasFinancas.API.csproj
```

Observação:

- O histórico antigo de migrations legadas foi removido.
- O projeto agora espera uma nova migration inicial compatível com MySQL.

#### Frontend

1. Navegue até `minhas-financas-front-end`.
2. Execute `npm install`.
3. Execute `npm run dev`.
4. Abra `http://localhost:3000`.

### 4.6 Observações importantes

- O backend possui projetos auxiliares de Hangfire e SignalR, indicando potencial para agendamentos e comunicação em tempo real.
- A rota de autenticação no frontend (`src/services/api/auth/[...nextauth].js`) está presente porém vazia, sugerindo que a integração do Next Auth ainda precisa ser finalizada.
- O projeto está preparado para evoluir com mais dashboards, filtros por categoria e controle de entradas/saídas financeiras.
