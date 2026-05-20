# Minhas Finanças

## 1. Visão geral narrativa

O projeto "Minhas Finanças" é uma solução completa para controle financeiro pessoal e familiar. Ele nasceu para ajudar o usuário a acompanhar contas, cartões, lançamentos, categorias, metas, bens patrimoniais e passivos em um único lugar, com suporte a login seguro e dashboards analíticos.

Na prática, o aplicativo funciona como uma plataforma de gestão financeira onde cada usuário pode se cadastrar, fazer login, cadastrar suas contas bancárias, cartões, despesas e receitas, monitorar metas e visualizar relatórios e gráficos. O objetivo é transformar dados financeiros em informações úteis, mostrando não apenas valores isolados, mas também o comportamento das despesas por categoria, o total de bens, passivos e possibilidades de compra.

O fluxo natural do projeto foca em três camadas:

- Interface do usuário no navegador, feita em Next.js e com componentes modernos.
- Uma API REST em ASP.NET Core que centraliza a lógica de autenticação, validação, persistência e retorno de resultados.
- Uma camada de dados baseada em Entity Framework Core e SQL Server para gravar informações financeiras estruturadas.

Esse conjunto permite que o aplicativo evolua para uma solução híbrida onde o usuário entra no sistema, registra dados e visualiza indicadores relevantes sem sair do fluxo.

## 2. Detalhes técnicos

### 2.1 Estrutura do repositório

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

### 2.2 Backend

#### 2.2.1 API e endpoints

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

#### 2.2.2 Autenticação e segurança

- O backend usa `Microsoft.AspNetCore.Identity` para usuários e roles.
- A autenticação é baseada em JWT, configurada em `Extensions/AuthenticationSetup.cs`.
- O JWT é validado com issuer, audience e chave simétrica definidas em `appsettings.json`.
- Endpoints críticos usam `[Authorize]` para exigir token válido.

#### 2.2.3 Persistência de dados

- `ApplicationDbContext` extende `IdentityDbContext<Usuario>` e expõe `DbSet` para as entidades principais:
  - Conta, Cartao, Categoria, SubCategoria, Lancamento
  - LancamentoFixo, LancamentoParcelado, BemPatrimonial, Meta, Passivo
- O mapeamento de relacionamentos e regras de exclusão está em `OnModelCreating`.
- A conexão do SQL Server está configurada em `appsettings.json`.

#### 2.2.4 Camada de aplicação

- A camada `MinhasFinancas.Application` define interfaces como `IContaAppService`, `ILancamentoAppService`, `IDashboardAppService`, etc.
- Os serviços da aplicação são registrados no `Program.cs` e fazem a ponte entre controllers e repositórios.
- O AutoMapper é configurado para mapear DTOs entre a camada de aplicação e as entidades de domínio.

### 2.3 Frontend

#### 2.3.1 Stack e ferramentas

- Next.js 14 com App Router
- React 18
- Tailwind CSS para estilos
- Radix UI para componentes acessíveis
- React Hook Form + Zod para formulários e validação
- Recharts para visualização de gráficos
- Next Auth como base de autenticação (a rota de auth existe, mas está sem implementação no momento)
- `ThemeProvider` para modo claro/escuro

#### 2.3.2 Fluxo do frontend

- A página inicial (`src/app/page.tsx`) carrega o componente de login.
- Assim que o usuário faz login, o frontend deve consumir a API de autenticacao e receber o JWT.
- Com o token, o app passa a acessar rotas protegidas para buscar dados de usuário, lançamentos, dashboard e demais módulos.
- Os componentes de sidebar e layout controlam a navegação entre telas internas (como relatórios, metas, contas e cartões).

### 2.4 Fluxo de dados geral

1. O usuário entra no frontend e faz login.
2. O frontend chama `POST api/Autenticacao/Login` com credenciais.
3. O backend valida o login e retorna um token JWT.
4. O frontend armazena o token e envia em `Authorization: Bearer {token}` para chamadas subsequentes.
5. O backend recebe o token e permite acesso a endpoints protegidos como `api/Lancamento`, `api/Usuario`, `api/Dashboard`.
6. Os endpoints usam services e repositórios para consultar e gravar no banco SQL Server.
7. O resultado é retornado ao frontend, que atualiza a interface e exibe gráficos, tabelas e valores.

### 2.5 Como rodar o projeto

#### Backend

1. Abra a solução em Visual Studio ou VS Code.
2. Atualize `ConnectionStrings:ConnectionMinhasFinancas` em `minhas-financas-back-end/minhas-financas-back-end/appsettings.json` para apontar ao seu SQL Server.
3. Execute `minhas-financas-back-end/minhas-financas-back-end`.
4. A API expõe Swagger em modo Development em `/swagger`.

#### Frontend

1. Navegue até `minhas-financas-front-end`.
2. Execute `npm install`.
3. Execute `npm run dev`.
4. Abra `http://localhost:3000`.

### 2.6 Observações importantes

- O backend possui projetos auxiliares de Hangfire e SignalR, indicando potencial para agendamentos e comunicação em tempo real.
- A rota de autenticação no frontend (`src/services/api/auth/[...nextauth].js`) está presente porém vazia, sugerindo que a integração do Next Auth ainda precisa ser finalizada.
- O projeto está preparado para evoluir com mais dashboards, filtros por categoria e controle de entradas/saídas financeiras.
