# AI Context - Minhas Financas

IMPORTANTE

O arquivo AI_CONTEXT.md será a principal documentação técnica deste projeto e deve ser tratado como um documento vivo.

Sempre que uma funcionalidade for implementada, removida ou alterada de forma relevante, este arquivo deve ser atualizado para refletir o estado atual do sistema.

Ao implementar qualquer feature, além das alterações de código, verifique se é necessário atualizar o AI_CONTEXT.md.

Nunca deixe o documento desatualizado em relação ao projeto.

## Visao Geral

### Objetivo do sistema
`Minhas Financas` e uma plataforma de controle financeiro pessoal com foco em consolidar receitas, despesas, investimentos, contas, cartoes, categorias, metas, patrimonio, passivos, relatorios e projecoes em um unico ecossistema. O sistema tenta responder duas perguntas principais:

- Como o usuario esta financeiramente hoje.
- Para onde a vida financeira dele esta caminhando.

### Publico-alvo

- Usuarios finais que querem organizar suas financas pessoais.
- Pessoas que controlam contas, cartoes, gastos por categoria e metas de poupanca.
- Usuarios que querem projetar quando atingirao um objetivo financeiro.

### Problema que resolve
O projeto centraliza dados financeiros dispersos e transforma movimentacoes em leitura operacional:

- classificacao por categoria/subcategoria
- consolidacao de receitas e despesas
- acompanhamento de metas
- visao patrimonial
- projecao de acumulacao futura
- relatorios por categoria e por ano

### Funcionalidades existentes

#### Backend

- autenticacao JWT
- cadastro, busca, edicao e exclusao de usuario
- CRUD de contas
- CRUD de cartoes
- CRUD de categorias
- CRUD de subcategorias
- CRUD de lancamentos
- suporte a lancamento pontual, fixo e parcelado com geracao imediata de registros reais
- filtros de lancamentos por tipo, periodo, categoria, conta, cartao, status realizado, texto, ordenacao e paginacao
- dashboard agregado
- CRUD de metas
- atualizacao do andamento de meta por aporte
- CRUD de bens patrimoniais
- CRUD de passivos
- relatorios por categoria
- relatorios por ano
- calculo de potencial de compra de imovel
- CRUD e calculo de projecoes
- seed inicial de categorias e bens patrimoniais quando o usuario e criado

#### Frontend

- login real com JWT
- cadastro real
- sessao persistida em `localStorage`
- protecao minima de rotas autenticadas
- dashboard integrado
- CRUD de categorias e subcategorias com boa UX
- CRUD de lancamentos com modal de criacao e edicao
- filtros, ordenacao e paginacao de lancamentos
- CRUD de contas e cartoes
- selecao real de conta/cartao/categoria/subcategoria no modal de lancamento
- tela de projecoes com overview em cards
- tela detalhada de projecao com renda base, renda extra por mes, modo atrelado a despesas ou manual

### Funcionalidades parcialmente implementadas ou ainda sem fechamento completo

- metas: backend pronto, frontend ainda nao aparece como modulo integrado final
- bens patrimoniais e passivos: backend pronto, frontend nao foi identificado como fluxo completo
- relatorios: backend pronto, frontend ainda esta em pagina placeholder
- orcamento: rota existe no front, ainda sem implementacao de negocio
- SignalR e Hangfire: projetos existem, mas nao aparecem integrados ao fluxo principal atual

## Arquitetura

### Arquitetura utilizada
O backend segue uma arquitetura em camadas muito proxima de `Clean Architecture` / `Onion`, embora sem rigor absoluto academico. A separacao principal e:

- `API`: entrada HTTP, configuracao, controllers
- `Application`: casos de uso, DTOs, interfaces, orquestracao e parte importante das regras de negocio
- `Domain`: entidades e calculos de dominio
- `Infra`: persistencia, `DbContext`, repositories, migrations
- `CrossCutting`: enums, utilitarios e tipos compartilhados

Observacao importante: na pratica, a regra de negocio esta dividida entre `Application` e `Domain`. Se voce for implementar algo novo, trate `Application` como a camada dominante de regra de negocio do projeto.

### Estrutura da solution

#### Backend

- `minhas-financas-back-end/minhas-financas-back-end`
  - projeto ASP.NET Core Web API
- `MinhasFinancas.Application`
  - app services, DTOs, interfaces, resources, AutoMapper
- `MinhasFinancas.Domain`
  - entidades e servicos de dominio para calculos
- `MinhasFinancas.Infra`
  - Entity Framework Core, `ApplicationDbContext`, repositories, migrations
- `MinhasFinancas.CrossCutting`
  - enums, utilitarios e `RetornoGenerico`
- `Minhas-Financas-Hangfire`
  - worker/app de jobs recorrentes
- `Minhas-Financas-hangfire.Infra`
  - infraestrutura auxiliar do Hangfire
- `Minhas-Financas-SignalR`
  - projeto dedicado a tempo real

#### Frontend

- `minhas-financas-front-end/src/app`
  - App Router do Next.js
- `src/pages`
  - legacy pages ainda existentes para login e cadastro
- `src/components`
  - UI e componentes de tela
- `src/providers`
  - providers de autenticacao e tema
- `src/services/api`
  - client HTTP e modulos por dominio
- `src/types`
  - contratos TS para API e dominio de tela
- `src/lib`
  - JWT helpers, vinculos de lancamento, utilitarios

### Responsabilidade de cada projeto

#### `minhas-financas-back-end/minhas-financas-back-end`

- bootstrap da API
- DI
- autenticacao
- CORS
- Swagger/Scalar
- registro de services e repositories
- chamada de `app.MigrateDatabase()`

#### `MinhasFinancas.Application`

- concentra o comportamento de negocio acessado pelos controllers
- valida existencia de usuario antes de quase todas as operacoes
- faz mapeamento DTO -> entidade
- orquestra repositorios e servicos auxiliares
- contem recursos de seed inicial

#### `MinhasFinancas.Domain`

- entidades persistidas
- servicos de calculo, como `Dashboard`, `RelatorioPorCategoria`, `RelatorioPorAno`, `PotencialCompraImovel`

#### `MinhasFinancas.Infra`

- contexto EF Core
- implementacao concreta de repositories
- migrations
- configuracao de relacionamento e cascata

#### `MinhasFinancas.CrossCutting`

- enums usados em varias camadas
- `RetornoGenerico`
- utilitarios transversais

### Fluxo de uma requisicao do inicio ao fim

Fluxo tipico:

1. O frontend chama `fetch` via `apiRequest` em `src/services/api/http.ts`.
2. Se houver token, ele envia `Authorization: Bearer {token}`.
3. O controller da API recebe a rota em `api/[Controller]`.
4. O controller delega para um `AppService`.
5. O `AppService` normalmente:
   - valida usuario
   - valida existencia do agregado principal
   - mapeia DTOs
   - chama repository
   - monta `RetornoGenerico`
6. O repository usa EF Core no `ApplicationDbContext`.
7. Em calculos agregados, o `AppService` chama servicos do `Domain`.
8. A resposta volta em `RetornoGenerico`.
9. O frontend interpreta `sucesso`, `mensagemUsuario` e `dados`.
10. Se `sucesso = false` ou status nao OK, `apiRequest` gera `ApiError`.

## Tecnologias

### Backend

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Microsoft.AspNetCore.Identity
- JWT Bearer Authentication
- AutoMapper
- Swagger / OpenAPI
- Scalar.AspNetCore
- Hangfire
- SignalR

### Frontend

- Next.js 14.2.5
- React 18
- TypeScript 5
- Tailwind CSS
- React Hook Form
- Zod
- Recharts
- next-themes
- next-auth
- lucide-react
- date-fns
- Radix UI

### Bibliotecas importantes

#### Backend

- `Microsoft.EntityFrameworkCore`
- `Microsoft.AspNetCore.Identity`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `AutoMapper`

#### Frontend

- `react-hook-form`
- `@hookform/resolvers`
- `zod`
- `recharts`
- `@radix-ui/*`
- `class-variance-authority`
- `clsx`
- `tailwind-merge`

### Banco de dados

- SQL Server
- string de conexao padrao em `appsettings.json`
- migrations em `MinhasFinancas.Infra/Migrations`

## Estrutura das Pastas

### Backend

- `Controllers`
  - endpoints HTTP
- `Extensions`
  - setup de autenticacao e extensoes de startup
- `Application/DTOs`
  - contratos de entrada/saida da API
- `Application/Services`
  - casos de uso
- `Application/Interfaces`
  - contratos dos app services
- `Application/Resources`
  - seeds e recursos auxiliares
- `Application/ViewModel`
  - modelos especificos de resposta, como token
- `Domain/Entities`
  - modelos persistidos
- `Domain/Services`
  - calculos financeiros de dominio
- `Infra/Data/Repositories`
  - implementacoes de acesso a dados
- `Infra/Data/Interfaces`
  - contratos de repositorio
- `Infra/Migrations`
  - historico de schema

### Frontend

- `src/app`
  - rotas Next App Router
- `src/app/(authenticated)`
  - area protegida por sessao
- `src/pages`
  - pages legacy para login/cadastro e compatibilidade de build
- `src/components/ui`
  - primitives Radix/Tailwind
- `src/components/dashboard`
  - tela do dashboard
- `src/components/lancamentos`
  - listagem, filtros e modais de lancamentos
- `src/components/contas-cartoes`
  - CRUD de contas e cartoes
- `src/components/configuracoes`
  - categorias e subcategorias
- `src/components/projecao`
  - overview e detalhe de projecoes
- `src/providers`
  - autenticacao e tema
- `src/services/api`
  - acesso a backend
- `src/types`
  - tipagens dos modulos
- `src/lib`
  - helpers transversais

## Entidades

### Usuario

- Finalidade: usuario autenticavel via Identity.
- Base: herda de `IdentityUser`.
- Relacionamentos:
  - contas
  - cartoes
  - lancamentos
  - categorias
  - bens patrimoniais
  - metas
  - projecoes
- Principais propriedades:
  - `Id`
  - `Nome`
  - `Email`
- Regras importantes:
  - ao cadastrar, recebe seed inicial de categorias e dois bens patrimoniais padrao

### Conta

- Finalidade: representar conta financeira do usuario.
- Relacionamentos:
  - vinculada a `Usuario`
  - pode ser referenciada por `Lancamento`
- Principais propriedades:
  - `NomeConta`
  - `Saldo`
  - `SaldoInvestimento`
  - `Instituicao`
  - `Tipo`
- Regras importantes:
  - lancamentos de saque/deposito/investimento podem alterar seu saldo via `LancamentoAppService`

### Cartao

- Finalidade: representar cartao de credito/debito.
- Relacionamentos:
  - vinculado a `Usuario`
  - pode ser referenciado por `Lancamento`
- Principais propriedades:
  - `NomeCartao`
  - `Saldo`
  - `Bandeira`
  - `Ultimos4Digitos`
  - `DiaFechamento`
  - `DiaVencimento`
  - `Tipo`
- Regras importantes:
  - hoje o backend faz CRUD simples; nao foi identificado calculo automatico de fatura

### Categoria

- Finalidade: classificar lancamentos.
- Relacionamentos:
  - pertence a `Usuario`
  - possui varias `SubCategoria`
- Principais propriedades:
  - `NomeCategoria`
  - `Icone`
  - `Tipo`
- Regras importantes:
  - existe validacao de nome unico por usuario
  - seed inicial e criada no cadastro
  - suporta tipos `Despesa`, `Receita`, `Investimento`, `Transferencia`

### SubCategoria

- Finalidade: detalhamento de uma categoria.
- Relacionamentos:
  - pertence a `Categoria`
- Principais propriedades:
  - `NomeSubCategoria`
  - `CategoriaId`
- Regras importantes:
  - validacao de nome unico dentro da categoria

### Lancamento

- Finalidade: registro central de movimentacao financeira.
- Relacionamentos:
  - pertence a `Usuario`
  - opcionalmente vincula `Conta`
  - opcionalmente vincula `Cartao`
  - opcionalmente vincula `Categoria`
  - opcionalmente vincula `SubCategoria`
  - pode ter `LancamentoFixo`
  - pode ter `LancamentoParcelado`
- Principais propriedades:
  - `Valor`
  - `Descricao`
  - `Observacao`
  - `DataPagamento`
  - `DataLancamento`
  - `GrupoParcelamentoId`
  - `NumeroParcela`
  - `TotalParcelas`
  - `Realizado`
  - `FrequenciaLancamento`
  - `Tipo`
  - `Vinculo`
- Regras importantes:
  - tipos: `Despesa`, `Receita`, `InvestimentoDeposito`, `InvestimentoSaque`, `Transferencia`, `Saque`, `Deposito`
  - frequencias: `Pontual`, `Fixo`, `Parcelado`
  - `Pontual` cria 1 lancamento
  - `Parcelado` cria imediatamente N lancamentos mensais reais e divide o valor total entre as parcelas
  - no parcelado, todas as parcelas compartilham o mesmo `GrupoParcelamentoId` e recebem `NumeroParcela` e `TotalParcelas`
  - `Pontual` e `Fixo` mantem `GrupoParcelamentoId`, `NumeroParcela` e `TotalParcelas` nulos
  - `Fixo` cria imediatamente 12 lancamentos mensais reais
  - filtros backend suportam texto, tipo, categoria, conta, cartao, status, periodo, ordenacao e paginacao
  - parte da movimentacao altera conta/bem patrimonial no cadastro

### LancamentoFixo

- Finalidade: apoiar recorrencia fixa.
- Observacao: a entidade existe, mas o fluxo operacional atual gera os registros reais diretamente na tabela `Lancamento`.

### LancamentoParcelado

- Finalidade: apoiar parcelamento.
- Observacao: a entidade existe, mas o fluxo operacional atual gera os registros reais diretamente na tabela `Lancamento`.

### Meta

- Finalidade: acompanhar objetivo financeiro acumulativo.
- Relacionamentos:
  - pertence a `Usuario`
  - possui varios `AporteMeta`
- Principais propriedades:
  - `NomeMeta`
  - `ValorFinal`
  - `ValorAtual`
  - `ValorParaChegarNaMeta`
  - `PorcentagemDaMeta`
  - `DataInicio`
  - `DataFim`
  - `MetaAlcancada`
- Regras importantes:
  - `CalcularDiferenca()` recalcula valor restante, percentual e se a meta foi alcancada
  - ao cadastrar meta, um primeiro aporte e criado com `ValorAtual`
  - existe endpoint para adicionar andamento via valor incremental

### AporteMeta

- Finalidade: historico de aportes feitos na meta.
- Principais propriedades:
  - `MetaId`
  - `DataAporte`
  - `Valor`

### BemPatrimonial

- Finalidade: registrar ativos/patrimonio.
- Relacionamentos:
  - pertence a `Usuario`
  - possui historico em `PermanenciaBemMaterial`
- Principais propriedades:
  - `NomeBemPatrimonial`
  - `Descricao`
  - `Permanencia`
  - `DataCadastro`
  - `Tipo`
- Regras importantes:
  - no cadastro de usuario sao criados ao menos:
    - `Dinheiro em Conta`
    - `Investimento em Conta`

### PermanenciaBemMaterial

- Finalidade: historico temporal do valor de um bem patrimonial.
- Regras importantes:
  - usada pelo sistema para consolidacoes patrimoniais e ajustes por lancamento

### Passivo

- Finalidade: registrar dividas/passivos.
- Relacionamentos:
  - pertence a `Usuario`
  - possui historico em `PermanenciaPassivo`
- Principais propriedades:
  - `NomePassivo`
  - `Descricao`
  - `Permanencia`
  - `DataCadastro`
  - `Tipo`

### PermanenciaPassivo

- Finalidade: historico temporal do valor de um passivo.

### Projecao

- Finalidade: simular acumulacao financeira ao longo de meses.
- Relacionamentos:
  - pertence a `Usuario`
  - possui varias `RendaProjecao`
  - possui varias `RendaExtraProjecaoMensal`
  - possui varias `DividaManualProjecaoMensal`
- Principais propriedades:
  - `Nome`
  - `DataInicial`
  - `ValorAcumuladoInicial`
  - `ValorObjetivo`
  - `MesesLimite`
  - `AtreladaADespesas`
- Regras importantes:
  - se `AtreladaADespesas = true`, usa despesas reais dos lancamentos
  - se `AtreladaADespesas = false`, usa dividas manuais por mes
  - exige pelo menos uma renda base maior que zero

### RendaProjecao

- Finalidade: renda base recorrente da projecao.
- Principais propriedades:
  - `Nome`
  - `ValorMensal`

### RendaExtraProjecaoMensal

- Finalidade: renda adicional manual por mes na projecao.
- Principais propriedades:
  - `MesReferencia`
  - `Valor`

### DividaManualProjecaoMensal

- Finalidade: divida manual por mes quando a projecao nao esta atrelada a despesas.
- Principais propriedades:
  - `MesReferencia`
  - `Valor`

### Tag

- Entidade existe no dominio.
- Nao foi identificado fluxo ativo nem endpoints relacionados no estado atual.

## Regras de Negocio

### Regra geral de acesso

- quase todos os modulos validam primeiro se o usuario existe
- a maioria dos endpoints protegidos recebe `usuarioId` na rota
- o frontend precisa obter esse ID do token JWT

### Como funciona um lancamento

- o frontend envia um DTO de criacao/edicao
- o backend mapeia para `Lancamento`
- no cadastro:
  - `Pontual` persiste um unico registro
  - `Parcelado` exige `QuantidadeParcelas > 1`, gera todos os meses imediatamente, acrescenta `X/Y` na descricao e compartilha um identificador de grupo entre as parcelas
  - `Fixo` gera 12 meses futuros imediatamente
- filtros e listagem acontecem em memoria a partir da lista carregada do repository
- se o lancamento tiver `ContaId`, alguns tipos ajustam saldo e patrimonio:
  - `InvestimentoDeposito`: soma em `SaldoInvestimento` e no bem `Investimento`
  - `InvestimentoSaque`: subtrai de `SaldoInvestimento` e no bem `Investimento`
  - `Saque`: subtrai de `Saldo` e do bem `Dinheiro em Conta`
  - `Deposito`: soma em `Saldo` e no bem `Dinheiro em Conta`
- `Transferencia` existe como enum, mas a regra automatica correspondente nao ficou evidente no service atual

### Como funciona uma meta

- `ValorAtual` representa o acumulado atual
- `ValorFinal` representa o alvo
- `ValorParaChegarNaMeta = ValorFinal - ValorAtual`
- `PorcentagemDaMeta = ValorAtual / ValorFinal * 100` se o valor final for maior que zero
- `MetaAlcancada = ValorAtual >= ValorFinal`
- a criacao da meta gera um `AporteMeta` inicial
- existe operacao separada para incrementar andamento por valor

### Como funciona uma projecao

- projecao possui:
  - renda base recorrente
  - acumulado inicial
  - objetivo final
  - horizonte em meses
  - opcionalmente renda extra por mes
  - opcionalmente divida manual por mes
- o calculo sempre parte do acumulado inicial
- para cada mes:
  - `ReceitaTotalMes = RendaBaseTotal + RendaExtraDoMes`
  - `SobraDoMes = ReceitaTotalMes - DividasTotais`
  - `AcumuladoProjetado += SobraDoMes`
- se o objetivo for atingido em algum mes, esse mes vira `MesObjetivo`
- `PercentualConcluido` considera o acumulado inicial em relacao ao objetivo
- `ValorRestanteParaObjetivo` no backend e calculado a partir do acumulado inicial quando o resultado e montado; no frontend a tabela detalhada tambem faz preview local por linha

### Como funciona o dashboard

- agrega lancamentos por ano, mes atual e mes anterior
- receitas usam `Tipo = Receita`
- despesas usam `Tipo = Despesa`
- investimentos usam `Tipo = InvestimentoDeposito`
- resultado percentual e `despesa / receita * 100` com tratamento para divisao por zero
- tambem agrupa:
  - receitas x despesas por mes
  - acumulo de investimentos
  - despesas por categoria
  - contas a pagar com `Realizado = false`

### Como sao calculados os totais

- dashboard usa somas por `DataPagamento`
- filtros de lancamento por periodo usam `DataLancamento`
- categorias de despesa no dashboard agrupam por `Categoria`
- graficos do front traduzem strings monetarias do dashboard para numero quando necessario

### Restricoes importantes

- nomes de categoria devem ser unicos por usuario
- nomes de subcategoria devem ser unicos por categoria
- projecao exige renda base > 0
- controllers retornam sempre `RetornoGenerico`
- parte dos repositorios e services ainda usa nomes historicos como `BancoId` para conta

## Fluxos

### Fluxo de autenticacao

1. Usuario envia email/senha.
2. `AutenticacaoController` chama `AutenticacaoAppService.Login`.
3. `SignInManager` valida credenciais.
4. Se sucesso, o service gera access token e refresh token.
5. O frontend decodifica JWT e monta sessao com:
   - `usuario.id = sub`
   - `usuario.nome = name`
   - `usuario.email = email`
6. Sessao fica em `localStorage`.

### Fluxo de cadastro de usuario

1. Front chama cadastro.
2. `UsuarioAppService.Cadastrar` cria `Usuario` via Identity.
3. Se sucesso:
   - desativa lockout
   - chama `InformacoesComplementares`
4. `InformacoesComplementares`:
   - carrega `categorias-iniciais.json`
   - cria categorias/subcategorias do usuario
   - cria dois bens patrimoniais base

### Fluxo de categorias/subcategorias

1. Front usa tela de configuracoes.
2. CRUD chama `CategoriaController`.
3. `CategoriaAppService` valida duplicidade e normaliza nome.
4. Repository persiste categoria/subcategoria.

### Fluxo de contas e cartoes

1. Front gerencia CRUD em tela/modais.
2. Endpoints chamam `ContaAppService` e `CartaoAppService`.
3. Services fazem validacao de usuario e CRUD direto.

### Fluxo de lancamentos

1. Front abre `NovoLancamentoModal` ou `EditarLancamentoModal`.
2. Usuario seleciona tipo, categoria, subcategoria e opcionalmente conta/cartao.
3. Backend persiste e pode refletir impacto em saldo/patrimonio.
4. Listagem suporta:
   - busca por descricao
   - filtro por tipo
   - filtro por periodo
   - filtro por categoria
   - filtro por conta
   - filtro por cartao
   - filtro por realizado
   - ordenacao por data/valor
   - paginacao

### Fluxo do dashboard

1. Front carrega `GET /api/Dashboard/{usuarioId}` com token.
2. Backend busca todos os lancamentos do usuario.
3. `Domain.Services.DashBoard.Dashboard` consolida agregados.
4. Front alimenta cards e graficos.

### Fluxo de projecoes

1. Front cria uma projecao no overview.
2. A tela detalhada permite:
   - definir nome
   - objetivo
   - acumulado inicial
   - rendas base
   - se esta atrelada a despesas
   - renda extra por mes
   - divida manual por mes se nao atrelada
3. `Salvar e gerar Projeção` salva e recalcula.
4. O backend retorna `ResultadoProjecaoDTO`.
5. O front tambem calcula preview local por linha para refletir edicoes imediatamente.

## API

### Padrao geral

- base URL local do front: `http://localhost:5242/api`
- autenticacao via Bearer token
- retorno padrao: `RetornoGenerico`

### Principais endpoints

#### Autenticacao

- `POST /api/Autenticacao/Login`
  - autentica e retorna token

#### Usuario

- `POST /api/Usuario/Cadastrar`
- `GET /api/Usuario/BuscarUmUsuario/{usuarioId}`
- `GET /api/Usuario/BuscarTodosOsUsuario`
- `DELETE /api/Usuario/DeletarUsuario/{usuarioId}`

#### Conta

- `POST /api/Conta/Cadastrar`
- `GET /api/Conta/BuscarTodosAsContas/{usuarioId}`
- `GET /api/Conta/BuscarUmaConta/{usuarioId}/{bancoId}`
- `PUT /api/Conta/EditarConta/{usuarioId}/{bancoId}`
- `DELETE /api/Conta/DeletarConta/{usuarioId}/{bancoId}`

#### Cartao

- `POST /api/Cartao/CadastrarCartao`
- `GET /api/Cartao/BuscarTodosOsCartoes/{usuarioId}`
- `GET /api/Cartao/BuscarUmCartao/{usuarioId}/{cartaoId}`
- `PUT /api/Cartao/EditarCartao/{usuarioId}/{cartaoId}`
- `DELETE /api/Cartao/DeletarCartao/{usuarioId}/{cartaoId}`

#### Categoria e subcategoria

- `POST /api/Categoria/CadastrarCategoria`
- `GET /api/Categoria/BuscarTodosAsCategorias/{usuarioId}`
- `GET /api/Categoria/BuscarUmaCategoria/{usuarioId}/{categoriaId}`
- `PUT /api/Categoria/EditarCategoria/{usuarioId}/{categoriaId}`
- `DELETE /api/Categoria/DeletarCategoria/{usuarioId}/{categoriaId}`
- `POST /api/Categoria/CadastrarSubCategoria/{usuarioId}/{categoriaId}`
- `GET /api/Categoria/BuscarTodosAsSubCategorias/{usuarioId}/{categoriaId}`
- `GET /api/Categoria/BuscarUmaSubCategoria/{categoriaId}/{subCategoriaId}`
- `PUT /api/Categoria/EditarSubCategoria/{usuarioId}/{categoriaId}/{subCategoriaId}`
- `DELETE /api/Categoria/DeletarSubCategoria/{usuarioId}/{categoriaId}/{subCategoriaId}`

#### Lancamento

- `POST /api/Lancamento/CadastrarLancamento`
- `GET /api/Lancamento/BuscarTodosOsLancamento/{usuarioId}`
- `GET /api/Lancamento/BuscarUmLancamento/{usuarioId}/{faturamentoId}`
- `PUT /api/Lancamento/EditarLancamento/{usuarioId}/{faturamentoId}`
- `DELETE /api/Lancamento/DeletarLancamento/{usuarioId}/{faturamentoId}`
- `GET /api/Lancamento/BuscarLancamentosPorCategoria/{usuarioId}`

#### Dashboard

- `GET /api/Dashboard/{usuarioId}`

#### Meta

- `POST /api/Meta/Cadastrar`
- `GET /api/Meta/BuscarTodosAsMetas/{usuarioId}`
- `GET /api/Meta/BuscarUmaMeta/{usuarioId}/{metaId}`
- `PUT /api/Meta/EditarMeta/{usuarioId}/{metaId}`
- `DELETE /api/Meta/DeletarMeta/{usuarioId}/{metaId}`
- `POST /api/Meta/AtualizarAndamentoMeta/{idPatrono}/{elementoId}/{valor}`

#### Passivo

- CRUD em `/api/Passivo/*`

#### Bem patrimonial

- CRUD em `/api/BemMaterial/*`

#### Relatorios

- `GET /api/Relatorios/PorCategoria/{usuarioId}`
- `GET /api/Relatorios/PorAno/{usuarioId}`

#### Potencial de compra

- `POST /api/PotecialCompra`

#### Projecao

- `GET /api/Projecao/BuscarTodas/{usuarioId}`
- `GET /api/Projecao/BuscarUma/{usuarioId}/{projecaoId}`
- `POST /api/Projecao/Cadastrar`
- `PUT /api/Projecao/Editar/{usuarioId}/{projecaoId}`
- `DELETE /api/Projecao/Deletar/{usuarioId}/{projecaoId}`
- `POST /api/Projecao/Calcular/{usuarioId}`
- `POST /api/Projecao/CalcularSalva/{usuarioId}/{projecaoId}`

## Banco

### Principais tabelas

- tabelas do Identity para usuarios e autenticacao
- `Conta`
- `Cartao`
- `Categoria`
- `SubCategoria`
- `Lancamento`
- `LancamentoFixo`
- `LancamentoParcelado`
- `BemPatrimonial`
- `PermanenciaBemMaterial`
- `Passivo`
- `PermanenciaPassivo`
- `Meta`
- `AporteMeta`
- `Projecao`
- `RendaProjecao`
- `RendaExtraProjecaoMensal`
- `DividaManualProjecaoMensal`

### Relacionamentos relevantes

- `Usuario 1:N Conta`
- `Usuario 1:N Cartao`
- `Usuario 1:N Categoria`
- `Categoria 1:N SubCategoria`
- `Usuario 1:N Lancamento`
- `Lancamento N:1 Categoria`
- `Lancamento N:1 SubCategoria`
- `Lancamento N:1 Conta`
- `Lancamento N:1 Cartao`
- `Usuario 1:N Meta`
- `Meta 1:N AporteMeta`
- `Usuario 1:N BemPatrimonial`
- `BemPatrimonial 1:N PermanenciaBemMaterial`
- `Usuario 1:N Projecao`
- `Projecao 1:N RendaProjecao`
- `Projecao 1:N RendaExtraProjecaoMensal`
- `Projecao 1:N DividaManualProjecaoMensal`

### Configuracoes importantes no `ApplicationDbContext`

- `SubCategoria` tem cascade delete a partir de `Categoria`
- `Projecao` tem cascade delete para:
  - `Rendas`
  - `RendasExtrasMensais`
  - `DividasManuaisMensais`
- `Lancamento -> Categoria` e `Lancamento -> SubCategoria` usam `DeleteBehavior.NoAction`

## Convencoes do Projeto

### Nomenclatura

- Controllers com sufixo `Controller`
- Services com sufixo `AppService`
- Repositories com sufixo `Repository`
- DTOs separados por modulo
- muitos nomes refletem historico do projeto:
  - `BancoId` em alguns fluxos de conta
  - `elementoId`, `idPatrono` como nomes genericos

### DTOs

- DTOs de entrada e saida vivem em `Application/DTOs`
- padrao comum:
  - `CadastrarXDTO`
  - `EditarXDTO`
  - `FiltroXDTO`
  - `ResultadoPaginadoDTO`

### Services

- app service e o ponto principal da regra de negocio
- controller deve ser fino
- validacao de usuario e muito repetida
- retorno e sempre embrulhado em `RetornoGenerico`

### Repositories

- CRUD base via `IRepository<T>`
- repositories especificos adicionam queries extras
- parte da filtragem ainda acontece no AppService apos buscar lista completa

### AutoMapper

- `MappingProfile` centralizado
- mapeia DTO -> entidade
- alguns DTOs exigem conversao manual, como datas de mes de projecao

### Validacoes

- validacoes de formulario no front usam `react-hook-form + zod`
- validacoes de negocio no back ficam majoritariamente em AppService
- autenticacao usa Identity e JWT

## Dependencias

### Backend

- API depende de `Application`, `Infra`, `Domain`, `CrossCutting`
- `Application` depende de `Domain`, `Infra.Data.Interfaces`, `CrossCutting`
- `Infra` depende de `Domain`
- `Domain` depende de `CrossCutting`

### Frontend

- `components` dependem de `types`, `services/api`, `providers`, `lib`, `ui`
- `services/api` dependem de `types` e `config`
- `auth-provider` depende de `services/api/auth` e `lib/jwt`
- rotas autenticadas dependem de `ProtectedRoute`

## Pontos de Atencao

- O projeto usa `RetornoGenerico` em praticamente tudo; preserve esse contrato.
- Muitos endpoints exigem `usuarioId` na rota, mesmo com JWT.
- Nao assuma que todas as regras estao no `Domain`; muito do comportamento mora em `Application`.
- Parte da nomenclatura e legada e inconsistente.
- `src/app` e `src/pages` coexistem no front; qualquer alteracao de login/cadastro precisa respeitar ambos.
- O build do front pode quebrar com problemas de casing/import por causa do Windows + Next.
- O backend executa migracao automatica no startup via `app.MigrateDatabase()`.
- Filtros de lancamento sao aplicados apos carregar a lista completa; cuidado com performance se o volume crescer.
- O dashboard trabalha com strings monetarias formatadas no backend, nao com decimais crus.
- O frontend de projecao faz preview local adicional alem do calculo do backend.

## Divida Tecnica

- Repeticao intensa de validacao de usuario nos AppServices.
- Repeticao intensa de montagem manual de `RetornoGenerico`.
- Falta um padrao mais forte de errors/result.
- Parte da regra de lancamento mistura persistencia, saldo e patrimonio no mesmo service.
- Filtragem e paginacao de lancamentos poderia ser feita no banco, nao em memoria.
- Inconsistencias de nomenclatura (`BancoId`, `faturamentoId`, `idPatrono`).
- Strings monetarias no dashboard dificultam reuso no front.
- `src/pages` legacy ainda convive com App Router.
- Ainda existem modulos com implementacao parcial ou sem tela integrada.
- Pacotes backend mostram warnings de compatibilidade e vulnerabilidade em build.

## Proximas Funcionalidades / Areas Parcialmente Implementadas

- orcamento ainda nao esta conectado a uma regra real
- metas carecem de tela integrada de uso final
- relatorios possuem backend, mas nao front completo
- bens patrimoniais e passivos nao aparecem como CRUD consolidado no front
- SignalR e Hangfire nao estao ligados ao fluxo principal do usuario
- existe controller de sorteios, mas nao foi identificado front correspondente

## Decisoes Arquiteturais

### Por que AutoMapper

- reduz repeticao de mapeamento DTO -> entidade
- centraliza conversoes em um `MappingProfile`
- o projeto ja adotou esse padrao em todos os modulos principais

### Por que Repository

- isolar acesso a dados da camada de aplicacao
- manter controllers e services sem EF Core direto
- permitir queries especificas por modulo

### Onde ficam as regras de negocio

- regras simples de estado e calculo localizado:
  - podem estar em entidade/servico de dominio
- regras operacionais do sistema:
  - hoje ficam majoritariamente em `Application/Services`

### O que nunca deve ficar no Controller

- logica de negocio
- validacao rica de dominio
- acesso direto ao `DbContext`
- calculos financeiros

## Como implementar novas funcionalidades

### Passo a passo recomendado

1. Identifique se a feature pertence a um modulo existente ou a um novo modulo.
2. Se for persistida, crie/ajuste:
   - entidade em `Domain/Entities`
   - `DbSet` e relacionamentos no `ApplicationDbContext`
   - migration
3. Crie DTOs em `Application/DTOs/{Modulo}`.
4. Adicione mapeamentos no `MappingProfile`.
5. Crie ou ajuste interface em `Application/Interfaces`.
6. Crie ou ajuste repository interface e implementacao.
7. Implemente a regra principal no `AppService`.
8. Exponha endpoints no controller.
9. Registre as dependencias no `Program.cs`.
10. No front:
   - crie tipos TS em `src/types`
   - crie service de API em `src/services/api`
   - integre em componente/tela
   - use `auth-provider` para token/usuario quando necessario

### Boas praticas especificas deste projeto

- siga o padrao `RetornoGenerico`
- valide o usuario antes de operar sobre os dados dele
- preserve o `usuarioId` nas rotas se o modulo atual seguir esse padrao
- reaproveite enums existentes
- se houver formulario, use `react-hook-form + zod`
- se houver CRUD visual, siga o estilo dos modulos de categorias, lancamentos, contas/cartoes e projecoes

## Historico das Features

### Autenticacao

- Objetivo: permitir login com JWT e sessao no frontend.
- Arquivos envolvidos:
  - `AutenticacaoController`
  - `AutenticacaoAppService`
  - `AuthenticationSetup`
  - `auth-provider.tsx`
  - `jwt.ts`
  - `formularioLogin.tsx`
- Entidades impactadas:
  - `Usuario`
- Decisoes tomadas:
  - token JWT como mecanismo principal
  - usuarioId lido do claim `sub`
  - sessao persistida em `localStorage`

### Categorias e Subcategorias

- Objetivo: dar classificacao financeira ao usuario.
- Arquivos envolvidos:
  - `CategoriaController`
  - `CategoriaAppService`
  - `ICategoriaRepository` / `CategoriaRepository`
  - `CategoriasSubCategorias.cs`
  - `categorias-iniciais.json`
  - `CategoriasManager.tsx`
- Entidades impactadas:
  - `Categoria`
  - `SubCategoria`
- Decisoes tomadas:
  - seed inicial por usuario no cadastro
  - validacao de nomes unicos
  - suporte de ponta a ponta no modal de lancamento

### Lancamentos

- Objetivo: registrar movimentacoes financeiras.
- Arquivos envolvidos:
  - `LancamentoController`
  - `LancamentoAppService`
  - `LancamentoRepository`
  - `LancamentosManager.tsx`
  - `NovoLancamentoModal.tsx`
  - `EditarLancamentoModal.tsx`
- Entidades impactadas:
  - `Lancamento`
  - `Conta`
  - `Cartao`
  - `Categoria`
  - `SubCategoria`
  - `BemPatrimonial`
- Decisoes tomadas:
  - filtros, ordenacao e paginacao no backend
  - selecao real de conta/cartao/categoria/subcategoria no front
  - parte dos tipos atualiza saldo/patrimonio
  - recorrencia nao usa tabela separada por enquanto; `Fixo` e `Parcelado` geram registros reais em `Lancamento`
  - parcelamentos possuem `GrupoParcelamentoId`, `NumeroParcela` e `TotalParcelas` para agrupamento futuro no frontend

### Dashboard

- Objetivo: apresentar agregados financeiros prontos.
- Arquivos envolvidos:
  - `DashboardController`
  - `DashboardAppService`
  - `Domain/Services/Dashboard`
  - `components/dashboard/dashboard.tsx`
- Entidades impactadas:
  - `Lancamento`
- Decisoes tomadas:
  - backend monta agregados prontos
  - frontend interpreta periodo (ano, mes atual, mes passado)
  - orcado ainda nao esta integrado ao modulo de orcamento

### Contas e Cartoes

- Objetivo: cadastrar meios financeiros reais e usa-los nos lancamentos.
- Arquivos envolvidos:
  - `ContaController`, `ContaAppService`
  - `CartaoController`, `CartaoAppService`
  - `ContasCartoesManager.tsx`
  - `GerenciarContasCartoesModal.tsx`
- Entidades impactadas:
  - `Conta`
  - `Cartao`
- Decisoes tomadas:
  - tela unica no front com alternancia entre contas e cartoes
  - modal no dashboard para gerenciar rapidamente

### Metas

- Objetivo: acompanhar poupanca orientada a objetivo.
- Arquivos envolvidos:
  - `MetaController`
  - `MetaAppService`
  - `Meta`
  - `AporteMeta`
- Entidades impactadas:
  - `Meta`
  - `AporteMeta`
- Decisoes tomadas:
  - historico de aportes
  - calculo de percentual e valor restante dentro da entidade

### Patrimonio

- Objetivo: registrar ativos e acompanhar permanencia historica.
- Arquivos envolvidos:
  - `BemMaterialController`
  - `BemPatrimonialAppService`
  - `BemPatrimonial`
  - `PermanenciaBemMaterial`
- Entidades impactadas:
  - `BemPatrimonial`
  - `PermanenciaBemMaterial`
- Decisoes tomadas:
  - criar ativos base automaticamente no cadastro
  - usar historico de permanencia para relatorios/evolucao

### Passivos

- Objetivo: registrar dividas e acompanhamentos de valor.
- Arquivos envolvidos:
  - `PassivoController`
  - `PassivoAppService`
- Entidades impactadas:
  - `Passivo`
  - `PermanenciaPassivo`

### Projecoes

- Objetivo: simular quando um objetivo financeiro sera alcancado.
- Arquivos envolvidos:
  - `ProjecaoController`
  - `ProjecaoAppService`
  - `ProjecaoRepository`
  - `ProjecaoManager.tsx`
  - `ProjecoesOverview.tsx`
- Entidades impactadas:
  - `Projecao`
  - `RendaProjecao`
  - `RendaExtraProjecaoMensal`
  - `DividaManualProjecaoMensal`
- Decisoes tomadas:
  - persistencia por usuario
  - multiplas projecoes independentes
  - renda extra por mes, nao global
  - flag `AtreladaADespesas`
  - modo manual de dividas mensais
  - preview local no front para refletir alteracoes imediatamente

### Relatorios

- Objetivo: consolidar leitura anual e por categoria.
- Arquivos envolvidos:
  - `RelatoriosController`
  - `RelatoriosAppService`
  - `Domain/Services/Relatorios`
- Entidades impactadas:
  - `Lancamento`
  - `Categoria`
  - `BemPatrimonial`
- Decisoes tomadas:
  - calculos ficam em servicos de dominio
  - API devolve relatorio pronto

## Resumo Executivo

`Minhas Financas` e um sistema de gestao financeira pessoal com backend em ASP.NET Core e frontend em Next.js. O backend esta organizado em camadas claras, com controllers finos, AppServices como eixo principal da regra de negocio, entities e calculos de dominio, alem de repositories EF Core para persistencia em SQL Server. O frontend evoluiu de um estado inicialmente visual para uma aplicacao funcional em modulos centrais como autenticacao, dashboard, categorias, lancamentos, contas/cartoes e projecoes.

O fluxo tecnico principal parte de login com JWT. O token e decodificado no frontend para extrair `usuarioId`, nome e email, persistindo a sessao em `localStorage`. Esse `usuarioId` e essencial porque a API, mesmo autenticada, ainda depende fortemente de rotas com identificador explicito do usuario. O contrato padrao da API e `RetornoGenerico`, que deve ser preservado em qualquer extensao do sistema.

No dominio financeiro, o modulo mais maduro e o de lancamentos: ele suporta classificacao por categoria e subcategoria, filtros ricos, relacao com conta/cartao e parte do ajuste automatico de saldo e patrimonio. O dashboard consome esses lancamentos para calcular receitas, despesas, investimentos, resultado percentual, contas a pagar, evolucao mensal e distribuicao por categoria. Metas, bens patrimoniais, passivos e relatorios tambem existem no backend com base funcional, embora parte dessas areas ainda nao esteja plenamente conectada no frontend.

O modulo de projecoes merece destaque especial. Ele foi modelado como um agregado proprio, com renda base recorrente, renda extra mensal, acumulado inicial, objetivo e duas estrategias de divida: usar despesas reais dos lancamentos ou valores manuais por mes. Essa feature combina persistencia por usuario com preview local no frontend, o que exige cuidado para manter coerencia entre calculo do backend e UX da tela.

Para uma nova IA trabalhar bem neste projeto, o mais importante e respeitar os padroes existentes: colocar a regra operacional em AppServices, manter o contrato `RetornoGenerico`, validar usuario explicitamente, criar DTOs dedicados, mapear com AutoMapper, usar repositories para persistencia e seguir no frontend o stack `react-hook-form + zod + services/api + types`. Os maiores riscos estao em nomenclaturas legadas, duplicacao de logica, coexistencia de `src/app` com `src/pages`, e em comportamentos transversais como migracao automatica no startup e dependencia do `usuarioId` na rota.
