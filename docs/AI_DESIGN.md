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

O fluxo oficial da análise com IA é:

`Dados -> Indicadores Financeiros -> Saude Financeira -> Insights Financeiros -> ResumoFinanceiroIA -> Memoria Financeira -> InterpretadorMemoriaFinanceira -> Plano Estrategico Financeiro -> InterpretadorEstrategico -> Consistencia Estrategica -> ConstrutorContextoIA -> ConstrutorPromptIA -> IA -> Relatorio Executivo`

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

Ele é estruturado pelo `ConstrutorContextoIA` em blocos organizados, incluindo:

- data de referência
- pontuação da saúde financeira
- classificação
- resumo executivo do sistema
- evolução financeira interpretada a partir da memória
- plano estratégico financeiro interpretado a partir da direção vigente do usuário
- prioridades imediatas
- destaques positivos
- insights prioritários

O contexto deve ser suficiente para gerar uma boa resposta sem expor a base inteira do sistema.

## Engenharia de prompt

O prompt oficial da Fase 4.2.5 deve orientar a IA a agir como consultora financeira experiente, com as seguintes caracterÃ­sticas:

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

## Especialistas Financeiros

A infraestrutura dos especialistas jÃ¡ existe no backend e foi criada para aprofundar a leitura financeira por domÃ­nio sem criar uma segunda IA.

Os especialistas internos atuais cobrem:

- dÃ­vidas
- reserva de emergÃªncia
- fluxo de caixa
- patrimÃ´nio
- plano estratÃ©gico
- compromissos financeiros

Esses especialistas:

- analisam sinais do contexto consolidado
- retornam pareceres estruturados
- nÃ£o chamam IA generativa
- nÃ£o substituem o Assistente Financeiro principal
- nÃ£o alteram regras de negÃ³cio

O valor dessa camada Ã© permitir profundidade sem fragmentar a experiÃªncia do usuÃ¡rio.

## EvoluÃ§Ã£o futura

### Plano Estratégico Financeiro

Camada já implementada na primeira versão da fase 4.2.3.1.

Ela armazena objetivos, prioridades, princípios e direção de longo prazo do usuário em versões historicamente preservadas.

Fluxo arquitetural atual e futuro:

`Dados -> Indicadores Financeiros -> Saude Financeira -> Insights Financeiros -> ResumoFinanceiroIA -> Memoria Financeira -> InterpretadorMemoriaFinanceira -> Plano Estrategico Financeiro -> InterpretadorEstrategico -> Consistencia Estrategica -> ConstrutorContextoIA -> ConstrutorPromptIA -> IA`

Essa camada já serve para:

- registrar a direção escolhida pelo usuário
- servir como referência explícita para decisões futuras
- alimentar interpretações e recomendações contextualizadas

### Interpretador Estratégico

Camada já implementada para transformar o Plano Estratégico Financeiro em narrativa compreensível para a IA.

Responsabilidades:

- transformar o plano em narrativa compreensível para IA e interfaces
- traduzir direção e prioridades em linguagem natural
- servir como apoio para contextualização executiva

### Consistência Estratégica

Camada implementada para avaliar, de forma determinística, se uma decisão está alinhada ao Plano Estratégico Financeiro vigente.

Responsabilidades:

- avaliar alinhamento entre decisão, pergunta do usuário e plano vigente
- apoiar recomendações contextualizadas
- evitar recomendações desalinhadas com objetivos de longo prazo
- fornecer ao contexto da IA uma leitura oficial de consistência, sem recalcular nada no modelo

### Modelo de Decisão Financeira

Camada arquitetural preparada para transformar a intenção do usuário em uma decisão financeira estruturada antes da leitura estratégica.

Responsabilidades:

- interpretar a pergunta do usuário como uma decisão financeira tipada
- organizar a intenção em uma forma rastreável para o restante do fluxo
- servir de ponte entre a linguagem livre do usuário e o raciocínio estratégico
- evitar que o sistema trate apenas texto solto como entrada principal

Fluxo desejado:

`Pergunta do usuário -> Interpretador da Decisão -> DecisaoFinanceiraIA -> Plano Estratégico Financeiro -> Consistência Estratégica -> ConstrutorContextoIA -> ConstrutorPromptIA -> IA`

Essa camada não substitui o plano estratégico nem a consistência.
Ela apenas organiza a intenção para que as próximas fases consigam raciocinar com mais clareza.

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

- Fase 4.1 concluída: integração técnica com IA já existe
- Fase 4.2 iniciada: primeiro prompt oficial de análise financeira com IA implementado
- Fase 4.2.2 implementada no frontend: experiência visual da análise aprofundada integrada à tela do Assistente Financeiro
- Fase 4.2.2.1 implementada no backend: interpretação da memória histórica antes da chamada à IA
- Fase 4.2.3.3 implementada no backend: Consistência Estratégica calculada pelo sistema e enviada ao contexto da IA
- Fase 4.2.5 concluída: IA Estratégica consolidada com leitura consultiva baseada em estado atual, evolução, plano, consistência e compromissos
- Fase 4.2.6 concluída: especialistas internos analisam domínios específicos antes da consolidação do contexto final
- a memória financeira agora reforça continuidade consultiva entre análises relacionadas, identificando recorrência, mudança de entendimento e evolução de prioridades
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
- não existe diálogo persistido
- não existem especialistas
- a persistência da Memória Financeira continua sendo responsabilidade do backend
- o frontend apenas consulta, apresenta e aciona a exclusão lógica do histórico existente
## Atualização — IA Estratégica

A Fase 4.2.5 consolidou a IA Estratégica como a camada responsável por transformar o contexto consolidado em um parecer mais profundo e consultivo.

### O que foi refinado

- o prompt passou a conectar estado atual, evolução, plano, consistência e compromissos em uma única narrativa
- as recomendações passaram a ser priorizadas por impacto crítico, estratégico e de longo prazo
- a conclusão passou a soar mais próxima de um consultor financeiro prudente
- a IA continua sem recalcular indicadores ou criar regras de negócio novas

### Diretriz oficial

- a IA interpreta o contexto preparado pelo backend
- o backend continua responsável por cálculos, memória, plano, consistência e compromissos
- a IA comunica com mais profundidade aquilo que o sistema já sabe

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


## MF Score na IA

A IA deve tratar o MF Score como a métrica oficial de risco financeiro do sistema.

Ela nunca recalcula o score por conta própria. Sempre interpreta o resultado preparado pelo backend, os pilares, os indicadores críticos e a tendência oficial.
