# AI Design

Este documento registra as decisÃµes de design da camada de IA do projeto.

Ele complementa:

- `AI_CONTEXT.md`, que documenta arquitetura e fluxo tÃ©cnico
- `docs/MODULE_GUIDE.md`, que documenta o papel funcional do Assistente Financeiro
- `docs/ROADMAP.md`, que documenta a evoluÃ§Ã£o futura da IA no produto

## VisÃ£o geral

A IA do projeto nÃ£o consulta diretamente o banco de dados nem opera sobre entidades brutas.

Ela recebe um contexto jÃ¡ consolidado pelo sistema, preparado a partir da inteligÃªncia financeira existente.

O objetivo Ã© transformar esse contexto em uma anÃ¡lise executiva mais rica, clara e educativa para o usuÃ¡rio.

## Filosofia

PrincÃ­pios obrigatÃ³rios:

- a IA deve explicar antes de recomendar
- a IA deve ensinar antes de aconselhar
- a IA deve atuar como consultora financeira prudente, e nÃ£o como promotora de decisÃµes impulsivas
- a IA deve respeitar os limites do contexto recebido
- a IA deve complementar a inteligÃªncia do sistema, nunca substituÃ­-la

O Assistente Financeiro deve contribuir para educaÃ§Ã£o financeira.

Sempre que possÃ­vel, a resposta deve mostrar por que uma recomendaÃ§Ã£o importa e qual impacto ela tende a gerar.

## Fluxo oficial

O fluxo oficial da anÃ¡lise com IA Ã©:

`Dados -> Indicadores Financeiros -> SaÃºde Financeira -> Insights Financeiros -> ResumoFinanceiroIA -> MemÃ³ria Financeira -> InterpretadorMemoriaFinanceira -> ConstrutorContextoIA -> ConstrutorPromptIA -> IA -> RelatÃ³rio Executivo`

Esse fluxo garante separaÃ§Ã£o entre:

- dados persistidos
- cÃ¡lculos analÃ­ticos
- interpretaÃ§Ã£o baseada em regras
- geraÃ§Ã£o textual por IA

## Responsabilidades

### Backend

O backend Ã© responsÃ¡vel por:

- consolidar dados financeiros
- calcular indicadores
- montar a saÃºde financeira
- gerar insights baseados em regras
- produzir o `ResumoFinanceiroIA`
- transformar esse resumo em contexto seguro para uso externo
- construir o prompt final
- chamar o provedor de IA

### IA

A IA Ã© responsÃ¡vel por:

- interpretar o contexto consolidado recebido
- produzir uma anÃ¡lise executiva em linguagem natural
- conectar causas, riscos, oportunidades e prioridades
- organizar a resposta no formato esperado pelo sistema

A IA nÃ£o deve:

- inventar dados
- recalcular indicadores
- contradizer o contexto preparado pelo backend
- prometer resultados financeiros

### Frontend

O frontend Ã© responsÃ¡vel por:

- exibir o relatÃ³rio executivo
- preservar a separaÃ§Ã£o entre conteÃºdo tÃ©cnico e leitura executiva
- apresentar a anÃ¡lise sem recalcular nada localmente

## Contexto enviado para a IA

O contexto enviado para a IA nasce exclusivamente do `ResumoFinanceiroIA`.

Ele Ã© estruturado pelo `ConstrutorContextoIA` em blocos organizados, incluindo:

- data de referÃªncia
- pontuaÃ§Ã£o da saÃºde financeira
- classificaÃ§Ã£o
- resumo executivo do sistema
- evoluÃ§Ã£o financeira interpretada a partir da memÃ³ria
- prioridades imediatas
- destaques positivos
- insights prioritÃ¡rios

O contexto deve ser suficiente para gerar uma boa resposta sem expor a base inteira do sistema.

## Engenharia de prompt

O prompt oficial da Fase 4.2 deve orientar a IA a agir como consultora financeira experiente, com as seguintes caracterÃ­sticas:

- linguagem clara
- tom respeitoso
- postura prudente
- foco educativo
- orientaÃ§Ã£o prÃ¡tica

Estrutura obrigatÃ³ria da resposta:

1. DiagnÃ³stico
2. Principais riscos
3. Pontos positivos
4. RecomendaÃ§Ãµes
5. Plano de aÃ§Ã£o
6. ConclusÃ£o

O prompt tambÃ©m deve forÃ§ar:

- separaÃ§Ã£o entre explicaÃ§Ã£o e recomendaÃ§Ã£o
- plano de aÃ§Ã£o com no mÃ¡ximo 5 prioridades
- ausÃªncia de listas excessivas
- ausÃªncia de repetiÃ§Ã£o do contexto recebido
- linguagem natural e nÃ£o robÃ³tica

## SeguranÃ§a

Diretrizes obrigatÃ³rias:

- a IA nunca consulta diretamente o banco
- a IA nunca recebe credenciais
- logs tÃ©cnicos nÃ£o devem registrar prompt completo nem resposta completa
- a chave da API deve ficar fora do repositÃ³rio
- o contexto deve ser sempre preparado pelo backend antes da chamada externa

## Especialistas futuros

A infraestrutura foi desenhada para permitir especialistas futuros com a mesma base tÃ©cnica.

Exemplos:

- especialista em dÃ­vidas
- especialista em patrimÃ´nio
- especialista em metas
- especialista em fluxo de caixa
- especialista em simulaÃ§Ãµes

Nesses casos, a infraestrutura permanece a mesma e muda principalmente:

- o contexto enviado
- o prompt utilizado
- o objetivo da anÃ¡lise

## EvoluÃ§Ã£o futura

### Plano Estratégico Financeiro

Camada já implementada na primeira versão da fase 4.2.3.1.

Ela armazena objetivos, prioridades, princípios e direção de longo prazo do usuário em versões historicamente preservadas.

Fluxo arquitetural atual e futuro:

`Dados -> Indicadores Financeiros -> Saúde Financeira -> Insights Financeiros -> ResumoFinanceiroIA -> Memória Financeira -> InterpretadorMemoriaFinanceira -> Plano Estratégico Financeiro -> Interpretador Estratégico -> Consistência Estratégica -> ConstrutorContextoIA -> ConstrutorPromptIA -> IA`

Essa camada já serve para:

- registrar a direção escolhida pelo usuário
- servir como referência explícita para decisões futuras
- alimentar interpretações e recomendações contextualizadas

### Interpretador Estratégico

Melhoria futura ligada ao Plano Estratégico Financeiro.

Responsabilidades:

- transformar o plano em narrativa compreensível para IA e interfaces
- traduzir direção e prioridades em linguagem natural
- servir como apoio para contextualização executiva

### Consistência Estratégica

Melhoria futura ligada ao Plano Estratégico Financeiro.

Responsabilidades:

- avaliar alinhamento entre decisões e plano vigente
- apoiar recomendações contextualizadas
- evitar recomendações desalinhadas com objetivos de longo prazo

### Princípio do Plano Estratégico

O `Plano Estratégico Financeiro` representa a intenção deliberada do usuário.

Ele deve ser tratado como um ativo de longo prazo, versionado e historicamente preservado.

A IA nunca cria ou modifica estratégias.

Ela apenas interpreta decisões à luz do plano vigente, da evolução histórica e da situação financeira atual.

### InterpretadorMemoriaFinanceira

Camada implementada na infraestrutura de IA para interpretar a MemÃ³ria Financeira antes da montagem do contexto.

Responsabilidades:

- receber apenas memÃ³rias resumidas jÃ¡ existentes
- identificar melhora, piora ou estabilidade da pontuaÃ§Ã£o
- reconhecer classificaÃ§Ãµes recorrentes
- detectar prioridades, riscos e recomendaÃ§Ãµes repetidas
- produzir a seÃ§Ã£o `EvoluÃ§Ã£o Financeira` em linguagem narrativa

Essa camada nÃ£o consulta banco e nÃ£o substitui indicadores ou insights.

## Estado atual

SituaÃ§Ã£o atual da evoluÃ§Ã£o:

- Fase 4.1 concluÃ­da: integraÃ§Ã£o tÃ©cnica com IA jÃ¡ existe
- Fase 4.2 iniciada: primeiro prompt oficial de anÃ¡lise financeira com IA implementado
- Fase 4.2.2 implementada no frontend: experiÃªncia visual da anÃ¡lise aprofundada integrada Ã  tela do Assistente Financeiro
- Fase 4.2.2.1 implementada no backend: interpretaÃ§Ã£o da memÃ³ria histÃ³rica antes da chamada Ã  IA

Nesta etapa, o foco Ã© melhorar:

- qualidade do prompt
- formato do relatÃ³rio executivo
- consistÃªncia do tom do assistente
- valor consultivo das respostas

## Atualização — Experiência Visual da Análise IA

A subfase 4.2.2 tornou a análise aprofundada acessível diretamente na tela do Assistente Financeiro.

### O que foi implementado

- botão funcional para gerar a análise aprofundada
- uso do endpoint existente `POST /api/AssistenteFinanceiro/GerarAnalise/{usuarioId}`
- renderização da resposta da IA em Markdown
- loading amigável durante a geração
- ações de copiar análise e gerar novamente
- resumo executivo com opção de minimizar e mostrar novamente
- histórico de análises acima da análise aprofundada, com capacidade de recolher e expandir
- a pergunta do usuário é o identificador principal de cada análise histórica, com truncamento visual quando o texto é longo
- histórico visual das últimas análises dentro da própria tela
- reabertura de análises anteriores no mesmo card principal
- exclusão lógica de análises pelo histórico visual
- a interface não expõe dados técnicos como modelo, tempo, tokens, custo ou horário

### Limites desta fase

- não existe chat
- não existe conversa contínua
- não existem especialistas
- a persistência da Memória Financeira continua sendo responsabilidade do backend
- o frontend apenas consulta, apresenta e aciona a exclusão lógica do histórico existente

## Atualização — Base de Conhecimento Financeira

A partir da subfase 4.2.1, o sistema passa a tratar o histórico analítico como parte da **Base de Conhecimento Financeira** do usuário.

### Estrutura conceitual

A Base de Conhecimento Financeira possui três conceitos:

- **Memória Financeira**
  - implementada nesta etapa
  - preserva fotografias históricas da situação financeira analisada
- **Plano Estratégico Financeiro**
  - implementado na primeira versão da fase 4.2.3.1
  - registra a direção estratégica do usuário ao longo do tempo em versões preservadas
- **Compromissos Financeiros**
  - evolução futura
  - registrará decisões e ações combinadas entre usuário e Assistente

### Diretriz oficial

- a Base de Conhecimento Financeira pertence ao domínio do sistema
- a IA não é dona dessa base
- o provedor externo apenas consulta contexto consolidado preparado pelo backend

### Memória Financeira

Cada análise histórica preserva:

- data de geração
- período de referência
- pontuação e classificação da saúde financeira
- resumo executivo do sistema
- `ResumoFinanceiroIA` serializado em JSON
- indicadores resumidos
- insights resumidos
- perfil financeiro vigente
- pergunta do usuário
- resposta textual da IA
- provedor, modelo, versão do prompt e versão do sistema
- tokens, custo estimado, tempo total, sucesso e erro

### Memória Consultiva

Antes de solicitar uma nova análise, o backend consulta a Memória Financeira, passa esse histórico resumido pelo `InterpretadorMemoriaFinanceira` e então envia ao `ConstrutorContextoIA` uma narrativa estruturada de evolução junto com uma memória compacta de apoio.

Regras:

- nunca enviar todas as análises
- enviar apenas um conjunto resumido e recente
- priorizar economia de tokens
- informar explicitamente quando não existirem análises anteriores

Objetivo:

Permitir continuidade consultiva, para que a IA consiga perceber evolução, mudança de prioridades e histórico recente do usuário.

