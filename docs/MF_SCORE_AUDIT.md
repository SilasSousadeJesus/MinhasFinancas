# Auditoria do MF Score

Este documento foi criado para servir como leitura única e consolidada do estado atual do `MF Score`.

O objetivo é ajudar uma pessoa ou outra IA a entender rapidamente:

- o que o MF Score é hoje;
- como ele funciona;
- como ele é validado;
- em que ponto o projeto está;
- o que já foi consolidado;
- o que ainda depende de calibração e evolução futura.

Este documento não substitui os demais documentos oficiais.

Ele os organiza em uma visão única de auditoria.

## 1. Resumo executivo

O `MF Score` já é o modelo oficial de avaliação de risco financeiro pessoal do projeto.

Ele deixou de ser apenas uma pontuação simples e passou a representar uma leitura estruturada de risco, proteção, pressão, maturidade e trajetória financeira.

Hoje o projeto está em um ponto em que:

- o modelo principal existe;
- a Saúde Financeira já exibe o score;
- o Assistente Financeiro já consome o score;
- a suíte oficial de validação já foi criada;
- o roadmap já foi reposicionado para uma etapa de calibração antes do Simulador Inteligente.

Ou seja:

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

Os pesos iniciais documentados são:

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
- a leitura da saúde financeira já usa referências estruturadas do perfil financeiro.

### Na experiência do usuário

- a tela de Saúde Financeira já exibe o score;
- o Assistente Financeiro já usa o MF Score como base executiva;
- o Dashboard consome apenas resumo consolidado;
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

A documentação também já prevê uma matriz de sensibilidade para observar o comportamento do score quando apenas uma variável muda.

Isso ajuda a calibrar:

- comprometimento;
- liquidez;
- pressão financeira;
- tendência de risco.

## 9. Em que ponto o projeto está hoje

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

## 11. Relação com os documentos oficiais

### `docs/MF_SCORE.md`

Explica **como o modelo funciona**.

### `docs/MF_SCORE_VALIDATION.md`

Explica **como validamos se o modelo continua coerente**.

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

## 13. Síntese final

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
