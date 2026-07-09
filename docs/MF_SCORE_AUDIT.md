## Governança técnica do Motor Financeiro

A partir desta etapa, este documento deixa de ser apenas um resumo do estado do `MF Score`.

Ele passa a ser o documento oficial de governança técnica do Motor Financeiro.

Este arquivo passa a responder, de forma permanente, três perguntas:

1. Como o Motor Financeiro funciona?
2. Como validamos se ele continua correto?
3. Quais limitações conhecidas ainda existem?

### O que deve ser registrado aqui

- limitações conhecidas;
- lacunas conceituais;
- oportunidades de evolução;
- cobertura atual do modelo;
- achados encontrados durante implementações;
- dívida técnica do Motor Financeiro.

Nem toda limitação precisa ser corrigida imediatamente.

Mas toda limitação relevante deve ser registrada aqui.

## Auditoria Arquitetural do Motor Financeiro

### Cobertura atual dos pilares

| Pilar | Cobertura atual | Avaliação técnica |
| --- | --- | --- |
| Fluxo de Caixa | Excelente | O modelo mede economia mensal, percentual de economia, comprometimento da renda e pressão futura de curto prazo com boa clareza operacional. |
| Liquidez | Excelente | Reserva atual, cobertura e reserva ideal já formam uma leitura robusta de proteção imediata. |
| Endividamento | Boa | O modelo cobre endividamento patrimonial e pressão futura em múltiplos horizontes, mas ainda não representa toda a inadimplência real e alguns estados comportamentais de dívida. |
| Patrimônio | Boa | O patrimônio líquido atual e o patrimônio-alvo já entram no score, mas a leitura ainda não incorpora profundamente qualidade patrimonial, liquidez do patrimônio e evolução histórica no próprio cálculo. |
| Planejamento | Parcial | O pilar ainda depende majoritariamente de proxies de organização financeira e não usa de forma madura os elementos estratégicos completos do domínio. |

### Cobertura do domínio

| Conceito | Cobertura atual | Observação |
| --- | --- | --- |
| Fluxo de Caixa | Completo | Bem representado no curto prazo e no mês corrente. |
| Liquidez | Completo | Reserva atual e meta ideal já oferecem leitura sólida. |
| Reserva | Completo | O conceito está bem refletido na camada analítica atual. |
| Patrimônio | Parcial | Há boa leitura do patrimônio líquido atual, mas ainda com pouca profundidade qualitativa no score. |
| Endividamento | Parcial | Bem representado no patrimonial e no futuro previsto, mas ainda sem leitura plena de inadimplência e comportamento de dívida real. |
| Pressão Financeira | Completo | Os horizontes de 30, 90, 180 e 365 dias já dão boa cobertura. |
| Planejamento | Parcial | Ainda depende de sinais indiretos. |
| Consistência Estratégica | Não implementado no score | Existe no domínio e no contexto da IA, mas ainda não pesa diretamente no cálculo do `MF Score`. |
| Compromissos | Não implementado no score | Já existem no domínio, mas ainda não participam diretamente do pilar Planejamento. |
| Histórico Financeiro | Parcial | O projeto possui histórico, mas o score ainda não o utiliza de forma madura como comportamento temporal. |
| Evolução Temporal | Parcial | A tendência existe conceitualmente, porém o comportamento histórico ainda está em amadurecimento. |
| Comportamento Financeiro | Parcial | O modelo já infere parte do comportamento por sinais operacionais, mas ainda não mede execução estratégica de forma completa. |

## Limitações Conhecidas

- O pilar `Planejamento` ainda utiliza proxies de organização financeira e não reflete plenamente a execução estratégica do usuário.
- O `MF Score` ainda não incorpora diretamente o plano estratégico vigente no cálculo oficial.
- Os compromissos financeiros ativos ainda não alteram diretamente a nota do score.
- O histórico de cumprimento ou descumprimento de compromissos ainda não é usado como sinal analítico do motor.
- A consistência entre plano estratégico e comportamento financeiro ainda não participa diretamente da pontuação final.
- A leitura de inadimplência ainda é aproximada em alguns cenários de auditoria, pois o domínio atual não expõe toda a semântica de atraso real como componente analítico do score.
- O pilar Patrimônio ainda não diferencia com profundidade qualidade patrimonial, liquidez dos ativos e evolução patrimonial histórica no próprio cálculo.
- A tendência do score já existe como conceito, mas ainda não representa uma leitura histórica plenamente madura do comportamento financeiro ao longo do tempo.

## Achados da Auditoria

### Estrutura oficial dos achados

Cada achado do Motor Financeiro deve registrar:

- ID
- título
- descrição
- impacto
- prioridade
- status
- possível evolução futura

### MF-001 - Pilar Planejamento utiliza proxies

- **ID:** `MF-001`
- **Título:** Pilar Planejamento utiliza proxies
- **Descrição:** Hoje o pilar Planejamento e Disciplina ainda não utiliza diretamente os elementos estratégicos do domínio. Sua avaliação é feita principalmente através de proxies de organização financeira, como perfil financeiro configurado, metas preenchidas e objetivos financeiros configurados. Atualmente o cálculo do `MF Score` ainda não considera diretamente o plano estratégico vigente, os compromissos financeiros, o histórico de cumprimento dos compromissos, a consistência da execução do plano e a evolução do comportamento estratégico.
- **Impacto:** Usuários altamente organizados podem receber nota inferior ao que seria esperado em um modelo completamente maduro.
- **Prioridade:** Alta
- **Status:** Aberto
- **Possível evolução futura:** incorporar sinais estratégicos reais do domínio ao pilar Planejamento sem quebrar a rastreabilidade e a coerência do modelo.

## Dívida Técnica do Motor Financeiro

Esta seção passa a funcionar como backlog técnico permanente do `MF Score`.

### Itens atuais

- amadurecer o pilar `Planejamento` para consumir elementos estratégicos reais do domínio;
- incorporar compromissos financeiros como sinal determinístico do Motor Financeiro;
- avaliar uso do histórico de cumprimento de compromissos como medidor de disciplina financeira;
- evoluir a leitura de consistência estratégica para eventual participação direta no score;
- fortalecer a representação de inadimplência real dentro da camada analítica;
- amadurecer a leitura histórica de tendência e comportamento financeiro;
- avaliar enriquecimento do pilar Patrimônio com qualidade e liquidez patrimonial.

## Recomendações para a próxima calibração

Durante as próximas evoluções do `MF Score`, os próximos achados que merecem acompanhamento especial são:

1. como o motor deve representar inadimplência real de forma mais determinística;
2. como diferenciar melhor patrimônio alto com baixa liquidez patrimonial;
3. como incorporar compromissos e consistência estratégica sem transformar o pilar Planejamento em uma soma de sinais frágeis;
4. como usar histórico temporal sem criar ruído ou duplicação entre tendência, memória e score;
5. como evitar que bons números operacionais de curto prazo escondam fragilidades estruturais de disciplina e execução estratégica.

# Auditoria do MF Score

Este documento foi criado para ser a leitura única e consolidada do estado atual do `MF Score`.

O objetivo é permitir que uma pessoa, ou outra IA, entenda rapidamente:

- o que o MF Score é hoje;
- como ele funciona;
- onde ele está documentado;
- o que já foi consolidado no sistema;
- o que ainda está em calibração;
- qual é a próxima evolução oficial do projeto.

Este arquivo não substitui os documentos oficiais do projeto. Ele os organiza em uma visão única de auditoria.

## 1. Resumo executivo

O `MF Score` já é o modelo oficial de avaliação de risco financeiro pessoal do sistema.

Ele deixou de ser uma pontuação simples e passou a representar uma leitura estruturada de risco, proteção, pressão, maturidade e trajetória financeira.

Hoje o projeto está em um ponto em que:

- o modelo principal já existe;
- a Saúde Financeira já exibe o score;
- o Assistente Financeiro já consome o score;
- a suíte oficial de validação já foi criada;
- o roadmap já foi reposicionado para uma etapa de calibração antes do Simulador Inteligente.

Em resumo:

**o MF Score está consolidado como Motor Financeiro, mas ainda está em fase de amadurecimento e calibração contínua.**

## 2. O que o MF Score é

O MF Score responde à pergunta:

> Qual a probabilidade de o usuário perder estabilidade financeira se continuar seguindo a trajetória atual?

Ele não mede apenas riqueza.

Ele não mede apenas disciplina.

Ele mede risco financeiro pessoal a partir de pilares e regras críticas.

## 3. Estrutura oficial atual

O MF Score é composto por cinco pilares:

1. Fluxo de Caixa
2. Liquidez
3. Endividamento
4. Patrimônio
5. Planejamento

Os pesos oficiais documentados são:

- Fluxo de Caixa: 30%
- Liquidez: 25%
- Endividamento: 20%
- Patrimônio: 15%
- Planejamento: 10%

### Leitura conceitual dos pilares

- **Fluxo de Caixa**: mostra a capacidade operacional do mês e do curto prazo.
- **Liquidez**: mostra a proteção disponível para enfrentar imprevistos.
- **Endividamento**: mostra a pressão estrutural gerada por passivos e obrigações.
- **Patrimônio**: mostra a base patrimonial líquida do usuário.
- **Planejamento**: mostra a disciplina estratégica e a coerência de longo prazo.

## 4. Como o score é calculado

O cálculo oficial possui três blocos:

### 4.1 Nota de cada pilar

Cada pilar recebe uma nota de 0 a 100.

Essa nota é derivada dos indicadores associados àquele pilar.

### 4.2 MF Score Base

O `MF Score Base` é a média ponderada das notas dos cinco pilares.

Fluxo simplificado:

1. cada pilar recebe sua nota;
2. cada pilar é multiplicado por seu peso;
3. o sistema calcula a média ponderada;
4. o valor é arredondado;
5. o score passa para a etapa de penalizações críticas.

### 4.3 Penalizações críticas

Depois do score base, o sistema aplica regras críticas.

Exemplos documentados:

- reserva inexistente;
- comprometimento da renda muito elevado;
- pressão financeira futura muito elevada;
- endividamento patrimonial muito alto;
- patrimônio líquido negativo.

Essas regras existem para evitar que uma média simples suavize riscos que deveriam permanecer explícitos.

## 5. Classificação oficial

A classificação documentada hoje é:

- `90-100` - Excelente
- `80-89` - Muito Bom
- `70-79` - Bom
- `60-69` - Atenção
- `40-59` - Crítico
- `0-39` - Muito Crítico

## 6. Tendência e explicação

O modelo já nasce preparado para tendência.

Hoje a tendência é tratada como:

- direção geral do score;
- leitura qualitativa;
- base para histórico futuro.

Além disso, o sistema já está preparado para explicar por que o score subiu ou caiu, com base em mudanças de:

- compromissos;
- reserva;
- pressão financeira;
- endividamento;
- patrimônio.

Essa explicação é produzida pelo backend.

A IA apenas organiza o texto.

## 7. O que já foi consolidado no projeto

### Na análise financeira

- a camada `AnaliseFinanceira` já organiza indicadores em estrutura própria;
- a Saúde Financeira já deixou de ser uma pontuação simples e passou a exibir o MF Score;
- os indicadores temporais de curto, médio e longo prazo já estão documentados;
- a leitura da saúde financeira já usa referências estruturadas do perfil financeiro;
- a lógica analítica está centralizada e não deve ser recalculada por telas.

### Na experiência do usuário

- a tela de Saúde Financeira já exibe o score;
- o Assistente Financeiro já usa o MF Score como base executiva;
- o Dashboard consome apenas o resumo consolidado;
- a inteligência principal do produto já está separada da interface.

### Na documentação

Já existem documentos oficiais para:

- funcionamento do score;
- fórmulas dos indicadores;
- roadmap;
- visão do assistente;
- visão do produto;
- glossário do domínio;
- design da IA;
- validação do modelo;
- changelog;
- guia funcional dos módulos.

## 8. Suíte oficial de validação

Foi criada uma base de validação permanente para proteger o MF Score contra regressões.

O documento oficial dessa base é:

- `docs/MF_SCORE_VALIDATION.md`

Ele existe para responder perguntas como:

- o score continua coerente depois de uma mudança?
- o modelo ainda diferencia bem risco alto, médio e baixo?
- uma alteração numérica melhorou o modelo ou apenas mudou números?

### Cenários oficiais

Os cenários oficiais documentados cobrem, entre outros:

- vida financeira excelente;
- boa renda com liquidez inexistente;
- patrimônio alto com fluxo ruim;
- excelente fluxo com pouco patrimônio;
- inadimplência;
- comprometimento extremo;
- liquidez inexistente;
- planejamento financeiro excelente.

### Casos canônicos

Casos canônicos são cenários que nunca podem gerar resultado incoerente.

Exemplos:

- liquidez zero e comprometimento extremo nunca podem gerar score excelente;
- inadimplência nunca pode parecer situação saudável;
- endividamento extremo não pode ser mascarado apenas por patrimônio isolado.

### Matriz de sensibilidade

A documentação também prevê uma matriz de sensibilidade para observar o comportamento do score quando apenas uma variável muda.

Isso ajuda a calibrar:

- comprometimento;
- liquidez;
- pressão financeira;
- tendência de risco.

## 9. Estado atual do projeto

O estado atual pode ser resumido assim:

- `MF Score` está oficializado;
- `Saúde Financeira` e `Assistente Financeiro` já o consomem;
- a `Suite Oficial de Validação` já existe;
- a fase de `Evolução e Calibração do MF Score` está em andamento;
- o `Simulador Inteligente` foi reposicionado para depois dessa etapa.

Em outras palavras:

**o projeto não está mais tentando apenas criar um score.**

Ele já criou o score.

Agora está construindo a maturidade do modelo.

## 10. O que ainda não está concluído

Ainda faltam evoluções importantes, mas elas agora devem acontecer sobre a base já criada.

As principais pendências conceituais são:

- calibrar o score com a suíte oficial de validação;
- consolidar melhor a explicação de subida e queda do score;
- amadurecer a tendência histórica;
- evoluir o conceito de `MF Score Potencial`;
- criar uma base técnica de validação automatizada, se isso fizer sentido depois.

## 11. Como o MF Score aparece nas demais camadas

### `docs/MF_SCORE.md`

Explica como o modelo funciona.

### `docs/MF_SCORE_VALIDATION.md`

Explica como validamos se o modelo continua coerente.

### `docs/INDICADORES_FINANCEIROS.md`

Explica as fórmulas, intenções e pesos dos indicadores.

### `docs/ROADMAP.md`

Mostra a evolução oficial do produto e a posição atual da fase de calibração.

### `docs/AI_DESIGN.md`

Mostra como a IA usa o MF Score sem recalcular nem contradizer o Motor Financeiro.

### `docs/MODULE_GUIDE.md`

Mostra como Saúde Financeira, Assistente Financeiro e demais módulos usam o score na prática.

### `docs/DOMAIN_GLOSSARY.md`

Define os conceitos oficiais do domínio.

### `docs/PRODUCT_VISION.md`

Explica a filosofia do produto e o papel central do Motor Financeiro.

### `docs/ASSISTANT_VISION.md`

Explica a visão humana e evolutiva do Assistente Financeiro.

### `AI_CONTEXT.md`

Resume o contexto técnico e arquitetural para qualquer IA que precise trabalhar no projeto.

### `docs/CHANGELOG.md`

Registra a ordem histórica das evoluções relevantes.

## 12. Leitura recomendada para entender o estado atual

Se alguém quiser entender o MF Score em ordem lógica, a leitura recomendada é:

1. `docs/MF_SCORE.md`
2. `docs/MF_SCORE_VALIDATION.md`
3. `docs/INDICADORES_FINANCEIROS.md`
4. `docs/ROADMAP.md`
5. `docs/MODULE_GUIDE.md`
6. `docs/AI_DESIGN.md`
7. `docs/PRODUCT_VISION.md`
8. `docs/ASSISTANT_VISION.md`

## 13. Auditoria prática do que existe hoje

### Já consolidado

- modelo oficial de risco definido;
- pilares e pesos documentados;
- classificação oficial documentada;
- indicadores e fórmulas registrados;
- leitura detalhada em Saúde Financeira;
- uso executivo no Assistente Financeiro;
- suíte oficial de validação criada;
- roadmap reposicionado para calibração antes do Simulador Inteligente.

### Em evolução

- calibrar o modelo com mais rigor;
- amadurecer a explicação da variação do score;
- melhorar o uso histórico e a leitura de tendência;
- evoluir o conceito de `MF Score Potencial`.

### Próxima etapa oficial

- `Fase 4.2.7 — Evolução e Calibração do MF Score`

### Etapa seguinte depois dela

- `Fase 4.2.8 — Simulador Inteligente`

## 14. Síntese final

O MF Score hoje está assim:

- definido conceitualmente;
- implementado funcionalmente;
- exibido nas telas principais;
- integrado ao Assistente Financeiro;
- documentado como ativo central do Motor Financeiro;
- protegido por uma suíte oficial de validação;
- pronto para uma fase de calibração estruturada.

O próximo passo natural não é reinventar o score.

É calibrá-lo com rigor.
## 15. Auditoria operacional implementada

Para transformar a calibração em rotina prática, o projeto passou a ter uma auditoria operacional interna do `MF Score`.

### Endpoint interno

- `POST /api/MfScoreAuditoria/GerarPlanilha`

Regras:

- disponível apenas em ambiente de desenvolvimento;
- protegido por autenticação;
- sem tela própria e sem acesso por menu;
- pensado para uso técnico e auditoria de regressão.

### Motor utilizado

A auditoria não possui fórmula paralela.

Ela executa o fluxo oficial:

`ContextoAnaliseFinanceira -> IndicadoresFinanceirosService -> SaudeFinanceiraService`

Isso garante que a planilha reflita exatamente o comportamento real do Motor Financeiro.

### Personas oficiais da auditoria

Os cenários sintéticos iniciais implementados são:

1. Vida Financeira Excelente
2. Boa renda, reserva zero e cartão alto
3. Patrimônio alto com fluxo ruim
4. Excelente fluxo com pouco patrimônio
5. Inadimplência
6. Comprometimento extremo
7. Reserva inexistente sem dívidas
8. Planejamento excelente

### Estrutura da planilha

A planilha gerada possui as abas:

- `Resumo`
- `Cenarios`
- `Pilares`
- `Indicadores Criticos`
- `Dados de Entrada`

### Critério de aprovação

Cada cenário recebe status:

- `OK` quando `ScoreEsperadoMin <= ScoreObtido <= ScoreEsperadoMax`
- `FALHA` quando o score sai da faixa esperada

### Papel desta auditoria

Essa auditoria não substitui `docs/MF_SCORE.md` nem `docs/MF_SCORE_VALIDATION.md`.

Ela existe para:

- operacionalizar a calibração;
- reduzir risco de regressão silenciosa;
- documentar a resposta real do motor em cenários de referência;
- apoiar entregas futuras com evidência objetiva do comportamento do score.
