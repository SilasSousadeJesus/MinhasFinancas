# Roadmap - Minhas FinanÃ§as

Este documento contÃ©m apenas evoluÃ§Ã£o do produto.

Ele nÃ£o representa a arquitetura atual nem o histÃ³rico tÃ©cnico detalhado.

## Estado atual do roadmap

O projeto se encontra atualmente na **Fase 4.2.5 concluída** do roadmap de Inteligência Financeira / Assistente Financeiro.

A subfase **Fase 4.2.3.1.1 — Gestão do Plano Estratégico Financeiro** também foi concluída no frontend.

A próxima etapa oficial é a **Fase 4.2.6 — Especialistas Financeiros**.

## Roadmap da InteligÃªncia Financeira / Assistente Financeiro

### Fase 1 â€” InteligÃªncia do Sistema (sem IA)

**Status:** concluÃ­da

**Objetivo:**
Fazer o sistema entender a situaÃ§Ã£o financeira do usuÃ¡rio sozinho.

**Entregas:**

- Indicadores Financeiros
- SaÃºde Financeira
- Insights Financeiros
- ResumoFinanceiroIA

### Fase 2 â€” Infraestrutura de IA

**Status:** concluÃ­da

**Objetivo:**
Criar a infraestrutura de integraÃ§Ã£o com IA sem consumir nenhum provedor.

**Escopo entregue:**

- `IProvedorIA`
- `OpenAIProvider`
- `ConstrutorPromptIA`
- `ConstrutorContextoIA`
- `AssistenteFinanceiroService`

**Resultado desta fase:**

- a infraestrutura jÃ¡ prepara contexto e prompt a partir de `ResumoFinanceiroIA`
- o provedor permanece simulado
- nenhum token Ã© consumido
- nenhuma IA real Ã© chamada

### Fase 3 â€” Assistente Financeiro

**Status:** concluÃ­da

**Objetivo:**
Criar a tela do Assistente Financeiro.

Essa tela deve funcionar completamente mesmo sem IA.

Ela deve consumir apenas o `ResumoFinanceiroIA`.

**Exibir:**

- SaÃºde Financeira
- Indicadores
- Insights
- GrÃ¡ficos
- TendÃªncias
- HistÃ³rico
- RecomendaÃ§Ãµes produzidas pelo sistema

**ObservaÃ§Ã£o:**

Ao final da tela existirÃ¡ um botÃ£o:

`Gerar anÃ¡lise aprofundada com IA`

Esse botÃ£o pode permanecer desabilitado atÃ© a Fase 4.1.

### Fase 4.1 â€” IntegraÃ§Ã£o TÃ©cnica com IA

**Status:** concluÃ­da

**Objetivo:**
Implementar apenas a comunicaÃ§Ã£o tÃ©cnica com o primeiro provedor de IA.

**Escopo:**

- configuraÃ§Ã£o segura da API Key
- implementaÃ§Ã£o definitiva do `OpenAIProvider`
- comunicaÃ§Ã£o HTTP com a API
- tratamento de erros
- timeout
- retry
- configuraÃ§Ã£o do modelo
- controle de tokens
- configuraÃ§Ã£o por ambiente
- logs tÃ©cnicos

**Importante:**

- esta fase nÃ£o altera a experiÃªncia do usuÃ¡rio
- esta fase nÃ£o melhora os textos
- esta fase nÃ£o cria inteligÃªncia nova
- ela apenas faz a infraestrutura preparada na Fase 2 funcionar de verdade

**Resultado esperado:**

Ao final desta fase, o backend jÃ¡ consegue chamar a IA e obter uma resposta tÃ©cnica quando a chave estiver configurada corretamente no ambiente.

### Fase 4.2 — Primeira Análise Financeira com IA

**Status:** em evolução

**Objetivo:**
Criar a primeira experiência real de análise financeira utilizando IA.

**Fluxo oficial:**

`ResumoFinanceiroIA`
↓
`ConstrutorContextoIA`
↓
`ConstrutorPromptIA`
↓
`OpenAIProvider`
↓
`Relatório Executivo`

**Nesta fase serão definidos:**

- prompt oficial
- formato do relatório
- estilo de escrita
- tom do assistente
- regras de segurança
- contexto enviado
- comportamento esperado da IA

**Regra central:**

A IA nunca consulta diretamente o banco.

Ela recebe exclusivamente o `ResumoFinanceiroIA`.

**Resultado esperado:**

Transformar o `ResumoFinanceiroIA` em um parecer financeiro muito mais rico do que o produzido apenas pelas regras do sistema.

#### Fase 4.2.1 — Base de Conhecimento Financeira

**Status:** concluída no backend

**Escopo desta subfase:**

- Memória Financeira
- Memória Consultiva

**Entregas:**

- persistência histórica das análises do Assistente Financeiro
- fotografia completa do estado financeiro analisado em cada geração
- registro de resumo financeiro, indicadores resumidos, insights resumidos e perfil financeiro vigente
- preservação de pergunta do usuário, resposta da IA, provedor, modelo, versão do prompt, versão do sistema, tokens, tempo, custo, sucesso e erros
- recuperação resumida das últimas análises para enriquecer o contexto da IA com economia de tokens

#### Fase 4.2.2 — Experiência Visual da Análise IA

**Status:** concluída

**Objetivo:**

Transformar a análise aprofundada com IA em uma experiência visual clara, comparável e útil dentro do Assistente Financeiro.

**Entregas:**

- geração da análise aprofundada diretamente na tela do Assistente Financeiro
- visualização da resposta em Markdown no mesmo fluxo da tela
- ação de copiar análise
- ação de gerar novamente
- histórico visual das últimas análises
- carregamento de análises anteriores no mesmo card principal
- exclusão lógica de análises pelo histórico visual
- resumo executivo com opção de minimizar e mostrar novamente
- histórico e análise aprofundada iniciando recolhidos para reduzir poluição visual
- a pergunta do usuário passa a ser o identificador principal de cada análise, com truncamento visual quando necessário
- dados técnicos da geração não expostos na interface do usuário

#### Fase 4.2.2.1 — Interpretador da Memória Financeira

**Status:** concluída

**Objetivo:**

Transformar a memória histórica do usuário em uma narrativa de evolução produzida pelo próprio sistema antes do envio para a IA.

**Entregas:**

- criação do `InterpretadorMemoriaFinanceira`
- criação da seção `Evolução Financeira` no contexto enviado à IA
- redução da dependência de listagens cronológicas brutas
- orientação explícita do prompt para continuidade entre análises

#### Fase 4.2.3 — Plano Estratégico Financeiro

**Status:** em andamento

**Objetivo:**

Registrar a direção estratégica do usuário ao longo do tempo, como construir reserva, reduzir endividamento, iniciar investimentos ou planejar grandes compras.

**Subfases oficiais:**

- **Fase 4.2.3.1 — Plano Estratégico Financeiro**
  - concluída
  - armazenar objetivos, prioridades, princípios e direção estratégica formal do usuário
- **Fase 4.2.3.1.1 — Gestão do Plano Estratégico Financeiro**
  - concluída
  - tela para criar a primeira versão, editar a versão vigente por nova versão e consultar versões históricas de forma simples
- **Fase 4.2.3.2 — Interpretador Estratégico**
  - concluída
  - transformar o Plano Estratégico em narrativa compreensível para a IA
- **Fase 4.2.3.3 — Consistência Estratégica**
  - concluída
  - avaliar o quanto uma decisão está alinhada com o Plano Estratégico
- **Fase 4.2.5 — IA Estratégica**
  - concluída
  - usar Estado Atual + Evolução + Plano Estratégico para produzir recomendações contextualizadas

#### Fase 4.2.4 — Compromissos Financeiros

**Status:** concluída

**Objetivo:**

Registrar ações e decisões combinadas entre usuário e Assistente Financeiro, permitindo acompanhamento de execução, adiamento e conclusão.

**Entregas concluídas:**

- entidade de compromisso financeiro por usuário
- cadastro, edição, conclusão, cancelamento e exclusão lógica
- origem do compromisso como `Manual` ou `IA`
- integração do Assistente Financeiro com compromissos ativos
- sugestão de compromisso gerada pela análise aprofundada com IA
- tela de gerenciamento de compromissos financeiros

##### Fase 4.2.4.1 — Compromissos Financeiros

**Status:** concluída

**Objetivo:**

Registrar e gerenciar compromissos financeiros criados pelo usuário ou sugeridos pela IA, preservando rastreabilidade de status e evolução.

**Escopo concluído:**

- cadastro de compromissos financeiros
- edição de compromissos já existentes
- conclusão, cancelamento e exclusão lógica
- integração com o Assistente Financeiro para transformar sugestões em registros reais
- base de rastreabilidade para intenções e decisões do usuário

#### Fase 4.2.5 — IA Estratégica

**Status:** concluída

**Objetivo:**

Usar o contexto consolidado do usuário para produzir recomendações estratégicas mais profundas, sempre a partir da base de conhecimento já consolidada pelo sistema.

**Entregas concluídas:**

- prompt oficial revisado para conectar estado atual, evolução, plano, consistência e compromissos
- respostas com tom consultivo e narrativa estratégica
- recomendações priorizadas por impacto crítico, estratégico e de longo prazo
- conclusão executiva mais coerente com a direção escolhida pelo usuário

#### Fase 4.2.6 — Especialistas Financeiros

**Status:** futura

**Objetivo:**

Criar especialistas temáticos utilizando a mesma base técnica da IA estratégica, alterando principalmente contexto e foco de análise.

### Fase 5 — Especialistas

**Status:** futura

Criar especialistas utilizando a mesma infraestrutura.

**Exemplos:**

- Especialista em DÃ­vidas
- Especialista em PatrimÃ´nio
- Especialista em Metas
- Especialista em Fluxo de Caixa
- Especialista em SimulaÃ§Ãµes

**Nesta fase muda apenas:**

- prompt
- contexto enviado

Toda a infraestrutura permanece a mesma.

### Fase 6 â€” Conversa ContÃ­nua

**Status:** futura

Permitir que o usuÃ¡rio converse com o Assistente Financeiro utilizando todo o contexto financeiro jÃ¡ consolidado.

**Exemplos:**

- Vale a pena quitar meu emprÃ©stimo?
- E se eu comprar uma casa?
- Quanto posso gastar em um carro?
- Essa simulaÃ§Ã£o Ã© saudÃ¡vel?

## ConcluÃ­do

### FundaÃ§Ã£o

- autenticaÃ§Ã£o com JWT
- cliente HTTP padronizado
- loading global de requisiÃ§Ãµes
- migraÃ§Ã£o do banco oficial para MySQL

### OperaÃ§Ã£o financeira

- categorias e subcategorias
- contas e cartÃµes
- lanÃ§amentos com CRUD
- filtros, ordenaÃ§Ã£o, paginaÃ§Ã£o e exportaÃ§Ã£o Excel
- efetivaÃ§Ã£o rÃ¡pida de receitas e despesas
- lanÃ§amentos Ãºnicos, parcelados, fixos e por dia Ãºtil

### Leitura financeira

- dashboard
- radar financeiro
- fluxo de caixa simples
- indicadores financeiros
- saÃºde financeira
- insights financeiros bÃ¡sicos
- ResumoFinanceiroIA
- infraestrutura de IA preparada sem integraÃ§Ã£o real com provedor
- assistente financeiro consumindo apenas o resumo consolidado do backend

### Planejamento

- projeÃ§Ãµes financeiras
- simulaÃ§Ãµes financeiras
- perfil financeiro

### PatrimÃ´nio

- ativos patrimoniais
- passivos
- snapshots patrimoniais
- evoluÃ§Ã£o patrimonial

## Em desenvolvimento

### Metas

- integraÃ§Ã£o visual completa do mÃ³dulo no frontend

### RelatÃ³rios

- fechamento do frontend para consumo dos relatÃ³rios jÃ¡ existentes no backend

### OrÃ§amento

- definiÃ§Ã£o da regra de negÃ³cio e integraÃ§Ã£o real com a aplicaÃ§Ã£o

## PrÃ³ximas implementaÃ§Ãµes

### Inteligência Financeira / Assistente Financeiro

- Fase 4.2.6 — Especialistas Financeiros

### Dashboard

- patrimÃ´nio lÃ­quido no dashboard
- evoluÃ§Ã£o patrimonial no dashboard
- saldo projetado
- metas em risco
- resumo financeiro inteligente na interface, consumindo `ResumoFinanceiroIA`

### LanÃ§amentos

- modal para visualizaÃ§Ã£o de parcelas agrupadas
- ediÃ§Ã£o em lote de parcelamentos
- exclusÃ£o individual ou total de parcelamentos
- histÃ³rico de alteraÃ§Ãµes

### PatrimÃ´nio

- integraÃ§Ã£o automÃ¡tica com contas, cartÃµes e lanÃ§amentos
- geraÃ§Ã£o automÃ¡tica de snapshots por perÃ­odo
- relatÃ³rio patrimonial dedicado

### Metas

- experiÃªncia completa de uso no frontend
- leitura consolidada junto ao dashboard e ao planejamento financeiro

### Perfil Financeiro

- evoluÃ§Ã£o dos insights com base no histÃ³rico

## Ideias futuras

### SimulaÃ§Ãµes Financeiras

- comparaÃ§Ã£o entre mÃºltiplas simulaÃ§Ãµes
- aplicaÃ§Ã£o parcial de simulaÃ§Ã£o aos dados reais com confirmaÃ§Ã£o explÃ­cita
- impacto em patrimÃ´nio lÃ­quido
- impacto em metas
- importaÃ§Ã£o de aÃ§Ãµes a partir de lanÃ§amentos reais
- duplicaÃ§Ã£o rÃ¡pida de cenÃ¡rios

### Infraestrutura

- melhorias globais de performance
- melhorias de seguranÃ§a
- melhorias de observabilidade
- melhorias de auditoria
- melhor aproveitamento de Hangfire e SignalR

## Atualização da Fase 4.2

Esta seção registra o estado oficial mais recente da evolução do Assistente Financeiro.

- a **Fase 4.1 — Integração Técnica com IA** permanece **concluída**
- a **Fase 4.2 — Primeira Análise Financeira com IA** está **em evolução**
- a **Fase 4.2.1 — Base de Conhecimento Financeira** está **concluída no backend**
- a **Fase 4.2.2 — Experiência Visual da Análise IA** está **concluída**
- a **Fase 4.2.2.1 — Interpretador da Memória Financeira** está **concluída**
- a **Fase 4.2.3.2 — Interpretador Estratégico** está **concluída**
- a **Fase 4.2.3.3 — Consistência Estratégica** está **concluída**
- a **Fase 4.2.5 — IA Estratégica** está **concluída**
- a **próxima etapa oficial** é a **Fase 4.2.6 — Especialistas Financeiros**

### Entregas já consolidadas

- criação do documento oficial `docs/AI_DESIGN.md`
- formalização da filosofia da IA do projeto
- formalização do fluxo oficial `ResumoFinanceiroIA -> ConstrutorContextoIA -> ConstrutorPromptIA -> OpenAIProvider -> Relatório Executivo`
- implementação do primeiro prompt oficial consultivo da Fase 4.2
- criação da primeira versão da **Base de Conhecimento Financeira**
- integração visual da análise aprofundada diretamente na tela do Assistente Financeiro
- histórico visual da Memória Financeira com reabertura e exclusão de análises anteriores
- resumo executivo minimizável para foco na leitura da análise aprofundada
- interpretação da Memória Financeira em narrativa de evolução antes da chamada à IA
- avaliação determinística da Consistência Estratégica antes da montagem do contexto final
- integração da seção `Consistência Estrategica` no contexto textual enviado à IA
- IA Estratégica conectando estado atual, evolução, plano, consistência e compromissos em uma única narrativa consultiva

### Base de Conhecimento Financeira

Ela passa a ser formada por três conceitos:

- **Memória Financeira**
  - implementada nesta etapa
  - registra fotografias históricas da situação financeira analisada
- **Plano Estratégico Financeiro**
  - implementado na fase 4.2.3.1
  - registra a direção estratégica do usuário ao longo do tempo em versões históricas
- **Compromissos Financeiros**
  - implementados
  - registram decisões e ações combinadas entre usuário e Assistente

### Regra arquitetural

A Base de Conhecimento Financeira pertence ao domínio do sistema.

A IA pode consultar essa base para enriquecer respostas, mas não é dona dela.




