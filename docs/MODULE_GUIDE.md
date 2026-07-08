# Guia dos MÃ³dulos - Minhas FinanÃ§as

Este documento explica o papel funcional de cada mÃ³dulo do sistema.

Ele existe para responder, de forma simples:

- para que a tela serve
- o que o usuÃ¡rio faz nela
- quais dados ela produz
- como esses dados impactam o restante do sistema

Este arquivo nÃ£o substitui:

- `AI_CONTEXT.md`, que documenta arquitetura e fluxo tÃ©cnico
- `docs/DOMAIN_GLOSSARY.md`, que documenta conceitos oficiais do domÃ­nio
- `docs/ROADMAP.md`, que documenta evoluÃ§Ã£o futura do produto

## Dashboard

### Finalidade
Ã‰ a visÃ£o geral da situaÃ§Ã£o financeira do usuÃ¡rio. O dashboard existe para transformar dados espalhados em uma leitura rÃ¡pida do momento atual, dos riscos prÃ³ximos e dos principais nÃºmeros de acompanhamento.

### O que o usuÃ¡rio faz aqui
- visualiza os principais agregados financeiros
- acompanha grÃ¡ficos e comparativos
- identifica prÃ³ximos vencimentos e contas atrasadas
- acessa atalhos para operaÃ§Ãµes importantes, como gerenciamento de contas e cartÃµes

### Dados gerados ou mantidos
- nÃ£o Ã© um mÃ³dulo de cadastro principal
- consome dados agregados de lanÃ§amentos, contas, cartÃµes e categorias
- consome indicadores calculados pela camada analÃ­tica
- consome parÃ¢metros do perfil financeiro para contextualizar indicadores

### Impacto no restante do sistema
- depende fortemente de lanÃ§amentos, contas, cartÃµes e categorias
- deverÃ¡ consumir dados de patrimÃ´nio para mostrar patrimÃ´nio lÃ­quido e evoluÃ§Ã£o patrimonial
- jÃ¡ consome dados do perfil financeiro para calcular indicadores e deverÃ¡ ampliar esse uso em alertas e leituras de saÃºde financeira
- poderÃ¡ consumir metas, projeÃ§Ãµes e simulaÃ§Ãµes para mostrar risco, progresso e cenÃ¡rio futuro

### O que jÃ¡ estÃ¡ funcional
- agregados principais
- grÃ¡ficos do dashboard
- radar financeiro com prÃ³ximos vencimentos, contas atrasadas, alertas e fluxo de caixa prÃ³ximo
- o dashboard deixou de repetir a leitura analÃ­tica detalhada da saÃºde financeira
- atalhos operacionais

### EvoluÃ§Ãµes futuras
- patrimÃ´nio lÃ­quido e evoluÃ§Ã£o patrimonial no dashboard
- saldo projetado
- metas em risco
- resumo financeiro inteligente

## SaÃºde Financeira

### Finalidade
Ã‰ a tela analÃ­tica da saÃºde financeira. Seu papel Ã© traduzir os indicadores da anÃ¡lise financeira em uma leitura detalhada da situaÃ§Ã£o atual do usuÃ¡rio.

### O que o usuÃ¡rio faz aqui
- visualiza a pontuaÃ§Ã£o geral da saÃºde financeira
- entende a classificaÃ§Ã£o atual
- identifica os principais pontos de atenÃ§Ã£o
- acompanha todos os indicadores com valor atual, valor ideal, status e observaÃ§Ã£o
- consulta insights financeiros centralizados na prÃ³pria tela
- enxerga a Ã¡rea preparada para grÃ¡ficos analÃ­ticos futuros

### Dados gerados ou mantidos
- nÃ£o cria dados prÃ³prios
- consome o painel de indicadores financeiros
- consome o resumo interpretativo da saÃºde financeira

### Impacto no restante do sistema
- reutiliza a mesma camada analÃ­tica do dashboard
- reforÃ§a o uso do perfil financeiro como rÃ©gua pessoal
- jÃ¡ serve de base para a primeira versÃ£o de insights financeiros e do `ResumoFinanceiroIA`
- continuarÃ¡ servindo de base para relatÃ³rios, alertas e leituras futuras de saÃºde financeira

### O que jÃ¡ estÃ¡ funcional
- tela dedicada de saÃºde financeira
- pontuaÃ§Ã£o geral de 0 a 100
- classificaÃ§Ã£o textual
- pontos de atenÃ§Ã£o
- cards com todos os indicadores calculados
- valor atual, valor ideal, status e observaÃ§Ã£o por indicador
- insights financeiros centralizados nesta tela
- Ã¡rea reservada para grÃ¡ficos de evoluÃ§Ã£o patrimonial, economia mensal e reserva de emergÃªncia
- geraÃ§Ã£o backend de insights financeiros e de resumo consolidado para futuras interfaces e consumo por IA

### EvoluÃ§Ãµes futuras
- explicaÃ§Ãµes mais profundas por indicador
- recomendaÃ§Ãµes automÃ¡ticas
- integraÃ§Ã£o com relatÃ³rios e insights inteligentes

## Assistente Financeiro

ObservaÃ§Ãµes adicionais da fase tÃ©cnica:

- a infraestrutura backend jÃ¡ monta um contexto estruturado por seÃ§Ãµes antes de qualquer chamada externa para IA
- a integraÃ§Ã£o tÃ©cnica jÃ¡ registra tempo, tokens e custo estimado por chamada sem expor prompt completo nem dados financeiros pessoais em logs

### Finalidade
Ã‰ a tela executiva da inteligÃªncia financeira. Seu papel Ã© organizar a situaÃ§Ã£o atual do usuÃ¡rio em formato de prioridades, destaques e direcionamento prÃ¡tico para tomada de decisÃ£o.

### O que o usuÃ¡rio faz aqui
- acompanha a saÃºde financeira consolidada em formato executivo
- lÃª um resumo corrido com prioridades, principais indicadores, leitura estratÃ©gica e conclusÃ£o
- usa links para aprofundar a anÃ¡lise completa em SaÃºde Financeira
- pode solicitar uma anÃ¡lise aprofundada com IA diretamente na prÃ³pria tela
- pode escrever uma pergunta opcional para orientar a anÃ¡lise
- pode copiar o parecer gerado e gerar novamente sem sair da pÃ¡gina

### Dados gerados ou mantidos
- nÃ£o criarÃ¡ dados financeiros brutos
- consumirÃ¡ `ResumoFinanceiroIA` como fonte oficial de contexto consolidado
- jÃ¡ registra histÃ³rico analÃ­tico na MemÃ³ria Financeira quando uma anÃ¡lise com IA Ã© solicitada
- futuramente poderÃ¡ produzir relatÃ³rios executivos gerados a partir desse resumo e da Base de Conhecimento Financeira

### Impacto no restante do sistema
- reutiliza a cadeia `Dados -> Indicadores -> SaÃºde Financeira -> Insights -> ResumoFinanceiroIA`
- dependerÃ¡ da camada analÃ­tica e do resumo consolidado, sem recalcular nada na interface
- passa a consultar a MemÃ³ria Financeira para dar continuidade Ã  conversa analÃ­tica
- servirÃ¡ como ponto central para futuras integraÃ§Ãµes com IA, especialistas temÃ¡ticos, EstratÃ©gia Financeira e Compromissos Financeiros

### O que jÃ¡ estÃ¡ funcional
- tela executiva do Assistente Financeiro jÃ¡ implementada
- consumo exclusivo de `ResumoFinanceiroIA`
- um Ãºnico bloco principal de resumo executivo
- saÃºde financeira consolidada com pontuaÃ§Ã£o e classificaÃ§Ã£o em destaque
- seÃ§Ãµes internas por tÃ­tulo: resumo, prioridades, principais indicadores, leitura estratÃ©gica e conclusÃ£o
- cada seÃ§Ã£o possui responsabilidade textual prÃ³pria:
  - resumo explica a situaÃ§Ã£o
  - prioridades mostram aÃ§Ãµes curtas
  - principais indicadores sustentam a leitura com nÃºmeros
  - leitura estratÃ©gica destaca forÃ§as e riscos
  - conclusÃ£o fecha o parecer do perÃ­odo
- os blocos textuais nÃ£o reutilizam literalmente as descriÃ§Ãµes tÃ©cnicas dos indicadores; o assistente usa frases interpretativas prÃ³prias para transformar os dados em linguagem executiva
- os insights permanecem como camada separada de alertas, oportunidades, configuraÃ§Ã£o e destaques positivos
- principais indicadores em formato textual
- leitura estratÃ©gica com pontos fortes e pontos de atenÃ§Ã£o em formato textual
- conclusÃ£o dinÃ¢mica construÃ­da por regras com base na saÃºde financeira atual, sem IA generativa
- a conclusÃ£o usa interpretaÃ§Ãµes executivas prÃ³prias por indicador, em vez de repetir literalmente descriÃ§Ãµes tÃ©cnicas ou prioridades
- a abertura da conclusÃ£o varia conforme a classificaÃ§Ã£o da saÃºde financeira
- link para a anÃ¡lise completa em `SaÃºde Financeira`
- prÃ³ximas prioridades
- experiÃªncia visual da anÃ¡lise aprofundada com IA implementada na prÃ³pria tela
- resumo executivo com opÃ§Ã£o de minimizar e mostrar novamente, para liberar espaÃ§o quando o foco estiver na anÃ¡lise da IA
- histÃ³rico de anÃ¡lises acima da anÃ¡lise aprofundada, com opÃ§Ã£o de minimizar e mostrar novamente
- botÃ£o funcional para gerar anÃ¡lise aprofundada
- renderizaÃ§Ã£o da resposta da IA em Markdown
- loading amigÃ¡vel durante a geraÃ§Ã£o
- aÃ§Ãµes de copiar anÃ¡lise e gerar novamente
- histÃ³rico visual das Ãºltimas anÃ¡lises dentro da prÃ³pria tela
- cada interaÃ§Ã£o usa a prÃ³pria pergunta do usuÃ¡rio como identificador principal no histÃ³rico, com truncamento visual quando necessÃ¡rio
- abertura de anÃ¡lises anteriores no mesmo card principal da anÃ¡lise aprofundada
- exclusÃ£o lÃ³gica de anÃ¡lises diretamente pelo histÃ³rico visual
- os dados tÃ©cnicos da geraÃ§Ã£o permanecem apenas no backend e nÃ£o sÃ£o exibidos ao usuÃ¡rio
- a infraestrutura backend da Fase 4.1 continua responsÃ¡vel pela integraÃ§Ã£o tÃ©cnica com o provedor
- a MemÃ³ria Financeira continua sendo persistida pelo backend e apresentada visualmente pelo frontend
- antes de chegar Ã  IA, a MemÃ³ria Financeira agora Ã© interpretada pelo backend para criar continuidade entre anÃ¡lises
- nÃ£o existe chat nem conversa contÃ­nua nesta etapa

### EvoluÃ§Ãµes futuras
- futura camada de `Interpretador Financeiro` entre `SaÃºde Financeira` e `Insights Financeiros` para qualificar ainda mais a linguagem natural baseada em regras
- especialistas temÃ¡ticos reutilizando a mesma infraestrutura
- conversa contÃ­nua com contexto financeiro consolidado

## LanÃ§amentos

### Finalidade
Ã‰ o nÃºcleo operacional do sistema. Aqui o usuÃ¡rio registra receitas e despesas, organiza o passado e o futuro financeiro e constrÃ³i a base de quase todas as leituras do produto.

### O que o usuÃ¡rio faz aqui
- cria lanÃ§amentos
- edita lanÃ§amentos
- exclui lanÃ§amentos
- efetiva rapidamente pagamentos e recebimentos
- filtra, ordena e pagina a listagem
- exporta os resultados filtrados para Excel
- gerencia parcelamentos em lote a partir de qualquer parcela vinculada ao grupo

### Dados gerados ou mantidos
- receitas
- despesas
- status do lanÃ§amento
- datas de lanÃ§amento, vencimento e efetivaÃ§Ã£o
- categoria e subcategoria
- conta ou cartÃ£o vinculado
- agrupamentos de parcelamento e programaÃ§Ã£o

### Impacto no restante do sistema
- alimenta dashboard e radar financeiro
- alimenta fluxo de caixa simples
- alimenta projeÃ§Ãµes atreladas a despesas
- alimenta simulaÃ§Ãµes financeiras como base real
- impacta relatÃ³rios
- influencia leituras de patrimÃ´nio e metas em versÃµes futuras

### O que jÃ¡ estÃ¡ funcional
- CRUD completo
- filtros
- paginaÃ§Ã£o
- ordenaÃ§Ã£o
- exportaÃ§Ã£o para Excel respeitando os filtros aplicados e usando a infraestrutura compartilhada de relatÃ³rios
- efetivaÃ§Ã£o rÃ¡pida
- lanÃ§amentos Ãºnicos, parcelados, fixos e por dia Ãºtil
- gestÃ£o de parcelamentos por grupo, com visualizaÃ§Ã£o das parcelas e recÃ¡lculo dos vencimentos das parcelas elegÃ­veis

### EvoluÃ§Ãµes futuras
- visualizaÃ§Ã£o agrupada de parcelas
- ediÃ§Ã£o em lote de parcelamentos
- exclusÃ£o parcial ou total de parcelamentos
- histÃ³rico de alteraÃ§Ãµes

## Fluxo de Caixa Simples

### Finalidade
Ã‰ uma tela de conferÃªncia rÃ¡pida do mÃªs. Serve para responder, de forma objetiva, como estÃ¡ o fluxo financeiro do perÃ­odo sem exigir navegaÃ§Ã£o por muitos grÃ¡ficos ou filtros.

### O que o usuÃ¡rio faz aqui
- navega entre meses
- visualiza receitas do mÃªs
- visualiza despesas do mÃªs
- acompanha o saldo do mÃªs
- confere listas separadas de receitas e despesas
- exporta o mÃªs atual, um intervalo de meses ou um ano inteiro para Excel

### Dados gerados ou mantidos
- nÃ£o cria dados prÃ³prios
- consolida dados dos lanÃ§amentos com base na data de vencimento

### Impacto no restante do sistema
- depende dos lanÃ§amentos jÃ¡ cadastrados
- pode consumir perfil financeiro no futuro para comparar saldo real com economia desejada
- pode servir como base de leitura para relatÃ³rios mensais

### O que jÃ¡ estÃ¡ funcional
- navegaÃ§Ã£o mensal
- resumo com receitas, despesas e saldo
- grÃ¡fico comparativo simples
- listagem de receitas
- listagem de despesas
- exportaÃ§Ã£o para Excel com uma aba por mÃªs exportado

### EvoluÃ§Ãµes futuras
- comparaÃ§Ã£o com metas mensais
- leitura comparativa com meses anteriores
- alertas simples do mÃªs
- consumo dos parÃ¢metros do perfil financeiro

## ProjeÃ§Ãµes

### Finalidade
Permite planejar quando um objetivo financeiro poderÃ¡ ser alcanÃ§ado com base em renda, dÃ­vidas, acumulado inicial e horizonte de tempo.

### O que o usuÃ¡rio faz aqui
- cria vÃ¡rias projeÃ§Ãµes independentes
- define objetivo financeiro
- informa acumulado inicial
- cadastra rendas base
- ajusta rendas extras por mÃªs
- escolhe se a projeÃ§Ã£o serÃ¡ atrelada Ã s despesas reais ou manual
- acompanha a evoluÃ§Ã£o mÃªs a mÃªs

### Dados gerados ou mantidos
- projeÃ§Ãµes por usuÃ¡rio
- rendas da projeÃ§Ã£o
- rendas extras mensais
- dÃ­vidas manuais mensais
- configuraÃ§Ã£o de vÃ­nculo com despesas reais

### Impacto no restante do sistema
- pode consumir lanÃ§amentos reais para montar as despesas da projeÃ§Ã£o
- futuramente pode dialogar com metas e perfil financeiro
- pode ser usada no dashboard como visÃ£o de alcance de objetivos

### O que jÃ¡ estÃ¡ funcional
- mÃºltiplas projeÃ§Ãµes
- cards de resumo
- detalhamento da projeÃ§Ã£o
- tabela mensal
- grÃ¡fico visual
- modo atrelado a despesas ou manual

### EvoluÃ§Ãµes futuras
- integraÃ§Ã£o mais forte com metas
- leitura comparativa com perfil financeiro
- indicadores de viabilidade
- cenÃ¡rios mais avanÃ§ados

## PatrimÃ´nio

### Finalidade
Ã‰ o mÃ³dulo que mostra a fotografia patrimonial do usuÃ¡rio e sua evoluÃ§Ã£o ao longo do tempo. Ele existe para responder quanto o usuÃ¡rio tem em ativos, quanto deve e qual Ã© seu patrimÃ´nio lÃ­quido.

### O que o usuÃ¡rio faz aqui
- cadastra ativos
- cadastra passivos
- edita registros patrimoniais
- inativa itens
- gera snapshots manuais
- acompanha o histÃ³rico patrimonial

### Dados gerados ou mantidos
- ativos patrimoniais
- passivos
- snapshots patrimoniais
- histÃ³rico de evoluÃ§Ã£o do patrimÃ´nio lÃ­quido

### Impacto no restante do sistema
- deverÃ¡ alimentar o dashboard com patrimÃ´nio lÃ­quido e evoluÃ§Ã£o patrimonial
- pode ser comparado com patrimÃ´nio lÃ­quido alvo do perfil financeiro
- poderÃ¡ ser usado em relatÃ³rios e planejamento financeiro

### O que jÃ¡ estÃ¡ funcional
- CRUD de ativos
- CRUD de passivos
- cÃ¡lculo do patrimÃ´nio lÃ­quido atual
- geraÃ§Ã£o manual de snapshot
- histÃ³rico de snapshots
- grÃ¡fico de evoluÃ§Ã£o patrimonial

### EvoluÃ§Ãµes futuras
- integraÃ§Ã£o automÃ¡tica com contas, cartÃµes e lanÃ§amentos
- snapshots automÃ¡ticos por perÃ­odo
- relatÃ³rios patrimoniais dedicados

## SimulaÃ§Ãµes Financeiras

### Finalidade
Serve para testar cenÃ¡rios hipotÃ©ticos sem alterar os dados reais do usuÃ¡rio. Ã‰ o espaÃ§o para comparar decisÃµes antes de colocÃ¡-las em prÃ¡tica.

### O que o usuÃ¡rio faz aqui
- cria simulaÃ§Ãµes
- define perÃ­odo da simulaÃ§Ã£o
- adiciona aÃ§Ãµes hipotÃ©ticas
- compara fluxo real e fluxo simulado
- acompanha o impacto mÃªs a mÃªs

### Dados gerados ou mantidos
- simulaÃ§Ãµes financeiras
- aÃ§Ãµes de simulaÃ§Ã£o
- resultado consolidado por perÃ­odo

### Impacto no restante do sistema
- consome lanÃ§amentos reais como base de comparaÃ§Ã£o
- pode consumir perfil financeiro para avaliar qualidade do cenÃ¡rio
- poderÃ¡ conversar com patrimÃ´nio, metas e projeÃ§Ãµes em versÃµes futuras

### O que jÃ¡ estÃ¡ funcional
- overview de simulaÃ§Ãµes
- tela de ediÃ§Ã£o detalhada
- aÃ§Ãµes simuladas persistidas
- cÃ¡lculo comparativo mensal entre real e simulado

### EvoluÃ§Ãµes futuras
- comparaÃ§Ã£o entre mÃºltiplas simulaÃ§Ãµes
- duplicaÃ§Ã£o rÃ¡pida de cenÃ¡rios
- impacto em patrimÃ´nio lÃ­quido
- impacto em metas
- importaÃ§Ã£o de aÃ§Ãµes a partir de lanÃ§amentos reais

## Perfil Financeiro

### Finalidade
Ã‰ a rÃ©gua pessoal de avaliaÃ§Ã£o da saÃºde financeira do usuÃ¡rio. Esse mÃ³dulo nÃ£o registra movimentaÃ§Ãµes; ele define os parÃ¢metros que dizem o que Ã© saudÃ¡vel, aceitÃ¡vel ou desejado para aquela pessoa.

### O que o usuÃ¡rio faz aqui
- define percentual desejado de economia mensal
- define percentual desejado de reserva de emergÃªncia
- define quantidade ideal de meses de reserva
- define limite de comprometimento da renda
- define limite de endividamento
- define investimento mÃ­nimo desejado
- define patrimÃ´nio lÃ­quido alvo
- registra observaÃ§Ãµes e contexto

### Dados gerados ou mantidos
- perfil financeiro do usuÃ¡rio
- histÃ³rico de configuraÃ§Ãµes
- configuraÃ§Ã£o vigente
- parÃ¢metros pessoais de leitura financeira

### Impacto no restante do sistema
- jÃ¡ Ã© consumido pela camada analÃ­tica para calcular indicadores financeiros
- jÃ¡ Ã© consumido pelo dashboard para contextualizar indicadores
- serÃ¡ consumido pelo radar financeiro para alertas personalizados
- poderÃ¡ ser consumido por patrimÃ´nio para comparar patrimÃ´nio atual com patrimÃ´nio alvo
- poderÃ¡ ser consumido por fluxo de caixa simples para comparar saldo do mÃªs com a economia desejada
- poderÃ¡ ser consumido por projeÃ§Ãµes para avaliar se a trajetÃ³ria planejada estÃ¡ alinhada ao padrÃ£o desejado
- poderÃ¡ ser consumido por simulaÃ§Ãµes financeiras para medir qualidade de cenÃ¡rios hipotÃ©ticos
- poderÃ¡ ser consumido por metas como referÃªncia estratÃ©gica
- poderÃ¡ ser consumido por relatÃ³rios e indicadores futuros

### O que jÃ¡ estÃ¡ funcional
- cadastro dos parÃ¢metros financeiros principais
- atualizaÃ§Ã£o do perfil
- histÃ³rico de vigÃªncia
- leitura da configuraÃ§Ã£o vigente
- consumo inicial pelos indicadores financeiros do dashboard
- consumo real pela tela de SaÃºde Financeira
- consumo pelos insights financeiros e pelo resumo consolidado da inteligÃªncia do sistema

### EvoluÃ§Ãµes futuras
- integraÃ§Ã£o real com dashboard
- integraÃ§Ã£o com alertas do radar financeiro
- score de saÃºde financeira
- insights automÃ¡ticos com base no histÃ³rico

## Plano Estratégico Financeiro

### Finalidade
É o módulo que registra a direção financeira deliberadamente escolhida pelo usuário para o longo prazo. Ele complementa o `Perfil Financeiro`, que define a régua de avaliação, com uma camada de intenção, prioridade e estratégia.

### Diferença para o Perfil Financeiro
- o `Perfil Financeiro` define limites, parâmetros e metas numéricas de referência
- o `Plano Estratégico Financeiro` define prioridades, objetivos, princípios e direção de longo prazo
- os dois módulos são complementares e não devem ser confundidos

### O que o usuário faz aqui
- registra objetivos estratégicos
- define prioridades de longo prazo
- descreve princípios importantes para sua jornada financeira
- atualiza a direção escolhida ao longo do tempo

### Dados gerados ou mantidos
- plano estratégico do usuário
- objetivos estratégicos
- prioridades e princípios
- histórico de direção escolhida

### Impacto no restante do sistema
- será consumido pela IA para contextualizar recomendações
- poderá ser consumido por insights e leituras estratégicas futuras
- permitirá avaliar se decisões atuais estão alinhadas com a direção escolhida

### O que já está funcional
- backend funcional com CRUD de versões
- cada atualização cria uma nova versão preservando o histórico
- listagem de versões e carregamento da versão vigente
- a tela frontend ainda não foi entregue nesta fase

### Evoluções futuras
- interpretador estratégico
- consistência estratégica
- IA estratégica

## Metas

### Finalidade
Ã‰ o mÃ³dulo voltado a objetivos financeiros acumulativos. Ele existe para transformar desejos financeiros em acompanhamento estruturado.

### O que o usuÃ¡rio faz aqui
- cadastra metas
- acompanha progresso
- consulta andamento das metas

### Dados gerados ou mantidos
- metas financeiras
- valores objetivos
- progresso associado

### Impacto no restante do sistema
- poderÃ¡ ser consumido por dashboard
- poderÃ¡ conversar com projeÃ§Ãµes e simulaÃ§Ãµes
- poderÃ¡ usar patrimÃ´nio e perfil financeiro como contexto de prioridade e viabilidade

### O que jÃ¡ estÃ¡ funcional
- existe base no backend
- existe estrutura de dados para o mÃ³dulo

### EvoluÃ§Ãµes futuras
- fechamento completo do frontend
- integraÃ§Ã£o com dashboard
- leitura de risco e progresso

## Contas e CartÃµes

### Finalidade
Organiza os meios financeiros utilizados pelo usuÃ¡rio. Esse mÃ³dulo dÃ¡ estrutura para que os lanÃ§amentos sejam vinculados a contas e cartÃµes reais.

### O que o usuÃ¡rio faz aqui
- cadastra contas
- cadastra cartÃµes
- edita dados
- remove ou inativa registros
- usa contas e cartÃµes nos lanÃ§amentos

### Dados gerados ou mantidos
- contas
- cartÃµes
- saldos e dados cadastrais associados

### Impacto no restante do sistema
- Ã© consumido diretamente pelos lanÃ§amentos
- pode ser explorado no dashboard em anÃ¡lises por conta e cartÃ£o
- poderÃ¡ influenciar patrimÃ´nio e relatÃ³rios futuros

### O que jÃ¡ estÃ¡ funcional
- CRUD de contas
- CRUD de cartÃµes
- uso real no modal de lanÃ§amentos
- gerenciamento rÃ¡pido via dashboard

### EvoluÃ§Ãµes futuras
- anÃ¡lises mais fortes por conta e cartÃ£o
- melhor integraÃ§Ã£o com patrimÃ´nio
- relatÃ³rios especÃ­ficos por meio financeiro

## Categorias e Subcategorias

### Finalidade
Classifica financeiramente os lanÃ§amentos. Esse mÃ³dulo existe para dar significado aos nÃºmeros e permitir leitura analÃ­tica por tipo de gasto ou receita.

### O que o usuÃ¡rio faz aqui
- cria categorias
- cria subcategorias
- edita classificaÃ§Ãµes
- gerencia a estrutura usada nos lanÃ§amentos

### Dados gerados ou mantidos
- categorias por usuÃ¡rio
- subcategorias por usuÃ¡rio
- relacionamento entre classificaÃ§Ã£o e lanÃ§amentos

### Impacto no restante do sistema
- Ã© consumido pelos lanÃ§amentos
- alimenta grÃ¡ficos e anÃ¡lises do dashboard
- poderÃ¡ alimentar relatÃ³rios por categoria e subcategoria
- ajuda projeÃ§Ãµes e simulaÃ§Ãµes a ganharem contexto analÃ­tico

### O que jÃ¡ estÃ¡ funcional
- CRUD completo
- seed inicial no cadastro do usuÃ¡rio
- integraÃ§Ã£o ponta a ponta com lanÃ§amentos

### EvoluÃ§Ãµes futuras
- refinamento da taxonomia inicial
- anÃ¡lises comparativas mais profundas
- relatÃ³rios mais detalhados por classificaÃ§Ã£o

## RelatÃ³rios

### Finalidade
Ã‰ o mÃ³dulo voltado Ã  leitura consolidada e histÃ³rica do sistema. Deve transformar os dados operacionais em visÃµes mais analÃ­ticas e Ãºteis para decisÃ£o.

### O que o usuÃ¡rio faz aqui
- consulta consolidaÃ§Ãµes financeiras
- compara perÃ­odos
- analisa comportamento financeiro

### Dados gerados ou mantidos
- nÃ£o deve ser um mÃ³dulo de cadastro principal
- consome dados de lanÃ§amentos, patrimÃ´nio, perfil financeiro, metas e demais mÃ³dulos analÃ­ticos

### Impacto no restante do sistema
- depende de quase todos os mÃ³dulos financeiros
- deve sintetizar leituras do sistema em formatos de anÃ¡lise

### O que jÃ¡ estÃ¡ funcional
- existem serviÃ§os e base no backend

### EvoluÃ§Ãµes futuras
- fechamento do frontend
- relatÃ³rios por categoria, conta, cartÃ£o e perÃ­odo
- relatÃ³rios patrimoniais
- relatÃ³rios comparativos com perfil financeiro e metas

## OrÃ§amento

### Finalidade
SerÃ¡ o mÃ³dulo responsÃ¡vel por definir limites planejados de gasto e acompanhar aderÃªncia ao plano financeiro do usuÃ¡rio.

### O que o usuÃ¡rio faz aqui
- definirÃ¡ valores orÃ§ados
- organizarÃ¡ limites por contexto financeiro
- acompanharÃ¡ desvio entre orÃ§ado e realizado

### Dados gerados ou mantidos
- valores planejados de orÃ§amento
- limites e referÃªncias para comparaÃ§Ã£o com despesas reais

### Impacto no restante do sistema
- deverÃ¡ impactar o dashboard
- deverÃ¡ impactar leituras de lanÃ§amentos
- poderÃ¡ dialogar com perfil financeiro, metas e relatÃ³rios

### O que jÃ¡ estÃ¡ funcional
- ainda nÃ£o existe integraÃ§Ã£o funcional concluÃ­da

### EvoluÃ§Ãµes futuras
- definiÃ§Ã£o formal da regra de negÃ³cio
- integraÃ§Ã£o com dashboard e lanÃ§amentos
- comparativo entre planejado e realizado

## Atualização do módulo Assistente Financeiro

A partir da Fase 4.2, o módulo passa a ter também uma camada oficial de design documentada em `docs/AI_DESIGN.md`.

### Complemento funcional

- a infraestrutura técnica da Fase 4.1 permanece a mesma
- a Fase 4.2 iniciou a formalização do comportamento textual da IA
- o primeiro prompt oficial foi estruturado para gerar uma análise consultiva, prudente, educativa e organizada em: diagnóstico, principais riscos, pontos positivos, recomendações, plano de ação e conclusão
- a IA continua consumindo exclusivamente o contexto consolidado derivado de `ResumoFinanceiroIA`

## Atualização do módulo Assistente Financeiro — Base de Conhecimento Financeira

### O que já está funcional no backend

- cada análise real gerada pelo endpoint do Assistente Financeiro passa a ser registrada na **Memória Financeira**
- o sistema preserva o `ResumoFinanceiroIA` usado na geração
- o sistema preserva indicadores resumidos, insights resumidos e perfil financeiro vigente
- o sistema preserva a resposta da IA, sucesso ou falha, e métricas técnicas da chamada
- existem endpoints de listagem e detalhe do histórico
- antes de cada nova análise, o backend recupera um resumo das últimas análises e o envia ao contexto da IA como **Memória Consultiva**
- antes da montagem final do contexto, esse histórico resumido passa pelo `InterpretadorMemoriaFinanceira`, que gera a seção **Evolução Financeira**
- nenhuma tela nova foi criada nesta etapa

### Evolução futura imediata

- a próxima etapa visual será a exibição da análise aprofundada usando essa memória como base histórica
- a evolução posterior da Base de Conhecimento incluirá **Interpretador Estratégico**, **Consistência Estratégica**, **IA Estratégica** e **Compromissos Financeiros**

