# MF Score

O `MF Score`, ou `Minhas Finanças Score`, é o modelo oficial de avaliação de risco financeiro pessoal do sistema.

Ele responde à pergunta:

> Qual é o risco financeiro pessoal do usuário se ele continuar seguindo a trajetória atual?

O `MF Score` não mede apenas riqueza, nem apenas disciplina. Ele mede risco, proteção, pressão estrutural, maturidade e persistência de comportamento ao longo do tempo.

Na versão atual do motor (`mf-score-v2.1-1000`), o modelo também passa a:

- usar meta monetária coerente para `Economia Mensal`;
- adotar faixas explícitas de status nos indicadores centrais;
- tratar inadimplência de forma gradual, e não mais binária;
- recalibrar penalizações temporais de fluxo negativo para refletir severidade progressiva;
- tratar o pilar `Planejamento` com base mínima explícita no `Perfil Financeiro`;
- manter os horizontes `30/90/180/365`, mas com influência decrescente conforme o prazo aumenta.

## Filosofia oficial

- O `MF Score` deve funcionar como score de risco financeiro pessoal.
- Indicadores ruins reduzem a nota dos pilares.
- Penalizações críticas existem apenas para eventos graves, risco materializado ou persistência temporal.
- Um mesmo fato econômico não deve ser punido duas vezes.
- A IA apenas comunica e interpreta o que o sistema já calculou.
- O modelo precisa ser transparente, rastreável, auditável e evolutivo.

## Escalas oficiais

### Pilares

- Cada pilar permanece em escala `0 a 100`.

### MF Score final

- O `MF Score Base` e o `MF Score Final` usam escala `0 a 1000`.

Exemplo:

- score base `66/100` no cálculo interno dos pilares
- score final exibido ao usuário: `660/1000`

## Estrutura do modelo

O `MF Score` é construído em cinco pilares:

1. Fluxo de Caixa
2. Liquidez
3. Endividamento
4. Patrimônio
5. Planejamento

### Regra vigente do pilar Planejamento

O pilar `Planejamento` continua com peso de `10%`, mas agora usa uma régua mínima explícita:

- o usuário precisa configurar os cinco parâmetros básicos do `Perfil Financeiro` para alcançar nota realmente alta;
- sem essa base, o pilar permanece limitado mesmo quando existem sinais operacionais positivos.

Parâmetros básicos exigidos:

- percentual de economia mensal desejado
- percentual de reserva de emergência desejado
- meses de reserva desejados
- comprometimento máximo da renda
- endividamento máximo

## Pesos oficiais

- Fluxo de Caixa: `30%`
- Liquidez: `25%`
- Endividamento: `20%`
- Patrimônio: `15%`
- Planejamento: `10%`

## Horizontes futuros

O motor continua trabalhando com quatro horizontes futuros:

- `30 dias`
- `90 dias`
- `180 dias`
- `365 dias`

Decisão oficial desta rodada:

- manter os quatro horizontes;
- não colapsar tudo em um único índice ainda;
- priorizar o curto prazo como pressão operacional principal;
- reduzir progressivamente o peso dos horizontes mais longos para evitar superinfluência estrutural no score corrente.

## Como o score é calculado

O cálculo oficial possui quatro camadas:

1. nota dos pilares
2. score base
3. penalizações críticas
4. persistência temporal do risco

### 1. Nota dos pilares

Cada pilar recebe nota de `0 a 100`, a partir dos indicadores associados.

### 2. Score base

O `MF Score Base` é a média ponderada dos cinco pilares e depois é convertido para a escala `0 a 1000`.

Fluxo simplificado:

1. cada pilar recebe sua nota de `0 a 100`
2. cada pilar é multiplicado pelo seu peso
3. o sistema calcula a média ponderada
4. o resultado normalizado é convertido para `0 a 1000`

### 3. Penalizações críticas

Depois do score base, o sistema aplica apenas penalizações que representam risco grave, risco já materializado ou persistência temporal de deterioração.

### 4. Persistência temporal do risco

O modelo já considera recorrência histórica como agravante de risco.

Na versão atual:

- `1 mês` negativo gera alerta e eventual penalização leve
- `2 meses consecutivos` negativos elevam a penalização
- `3 ou mais meses consecutivos` negativos elevam a penalização de forma forte

Penalizações oficiais atuais no score final:

- `1 mês negativo`: `40 pontos`
- `2 meses consecutivos negativos`: `90 pontos`
- `3 ou mais meses consecutivos negativos`: `140 pontos`

## Regra oficial contra dupla penalização

Um mesmo fato econômico não deve ser punido duas vezes.

Exemplos oficiais:

- `reserva zero` deve reduzir o pilar `Liquidez`, mas não gerar penalização crítica automática só por existir
- `comprometimento alto da renda` deve reduzir o pilar `Fluxo de Caixa`, mas não gerar penalização crítica automática se o usuário ainda mantém fluxo positivo e não está inadimplente
- `pressão financeira futura` deve reduzir `Fluxo de Caixa` e `Endividamento`, mas não gerar penalização crítica automática sem evidência de incapacidade de pagamento, inadimplência ou persistência negativa

### Posição conceitual oficial do comprometimento da renda

O indicador `Comprometimento da Renda` permanece oficialmente posicionado como leitura principal de `Fluxo de Caixa`.

Isso significa:

- ele mede pressão operacional da renda no mês corrente;
- ele não representa, por si só, qualidade de planejamento;
- ele pode influenciar a percepção global de risco, mas não deve ser usado como base principal do pilar `Planejamento`.

## Penalizações críticas oficiais

As penalizações críticas da versão atual devem se concentrar em eventos como:

1. inadimplência
2. fluxo de caixa mensal negativo
3. recorrência de meses consecutivos no vermelho
4. patrimônio líquido negativo
5. ausência de dados essenciais que comprometa a confiabilidade mínima da análise

### Regra oficial atual de inadimplência

A inadimplência agora é graduada por severidade.

Fatores considerados:

- dias máximos de atraso;
- percentual do valor em atraso sobre a renda mensal atual.

Níveis atuais:

- `Nível 1`: `30 pontos` de penalidade final
- `Nível 2`: `90 pontos`
- `Nível 3`: `170 pontos`
- `Nível 4`: `250 pontos`

O nível aplicado é sempre o mais grave entre:

- faixa de tempo;
- faixa de materialidade do valor vencido.

### Regra oficial atual de fluxo negativo recorrente

O fluxo negativo passou a ser penalizado de forma mais proporcional:

- `1 mês negativo`: alerta com punição leve;
- `2 meses consecutivos negativos`: agravamento moderado;
- `3 ou mais meses consecutivos negativos`: agravamento forte.

Essa recalibragem existe para:

- respeitar apetite de risco `moderado`;
- evitar colapso artificial do score por um único mês ruim;
- manter resposta severa quando o desequilíbrio vira padrão.

### O que não deve ser penalização crítica automática

Os itens abaixo devem afetar prioritariamente os pilares e não a camada de penalização crítica automática:

- reserva de emergência baixa ou inexistente
- comprometimento alto da renda
- pressão futura de 30 dias
- pressão financeira acumulada
- endividamento patrimonial alto, quando ainda não houver evidência de ruptura operacional

## Classificação oficial do MF Score

- `900-1000` - Excelente - Risco Muito Baixo
- `800-899` - Muito Bom - Risco Baixo
- `700-799` - Bom - Risco Moderado
- `600-699` - Atenção - Risco Moderado-Alto
- `400-599` - Crítico - Risco Alto
- `0-399` - Muito Crítico - Risco Muito Alto

## Tendência

A tendência mostra se a trajetória está:

- melhorando
- estável
- piorando

Na versão atual, ela já pode usar histórico mensal real quando disponível. Quando o histórico ainda é insuficiente, o sistema usa fallback determinístico baseado no equilíbrio dos indicadores.

## Histórico mensal do MF Score

O projeto agora possui persistência mensal oficial do score por meio da entidade `HistoricoMfScore`.

Objetivo:

- preservar a evolução mensal do score
- permitir leitura histórica real
- dar base para tendência, auditoria e futuras análises comparativas

Campos principais:

- `UsuarioId`
- `CompetenciaAno`
- `CompetenciaMes`
- `MfScoreBase`
- `MfScoreFinal`
- `Classificacao`
- `Risco`
- `PenalidadeTotal`
- `DataCalculo`
- `VersaoModelo`
- `JsonPilares`
- `JsonIndicadoresCriticos`
- `JsonResumo`
- `CriadoEm`

Regra:

- o histórico preserva o resultado da competência
- não deve sobrescrever competências antigas sem regra explícita

## Hangfire

O projeto agora possui um job recorrente mensal para persistir o histórico do `MF Score`.

Configuração atual:

- execução: `dia 01 de cada mês`
- cron: `0 2 1 * *`
- comportamento adotado: calcula a `competência anterior`

Exemplo:

- no dia `01/08/2026`, o job calcula e persiste a competência de `07/2026`

Objetivo:

- buscar usuários ativos
- calcular o score oficial
- salvar no histórico
- registrar logs
- tratar falhas por usuário sem interromper toda a execução

## Relação com outras camadas

- `AnaliseFinanceira` calcula pilares, score base, penalizações e tendência
- `Saúde Financeira` exibe o `MF Score`, classificação, risco e leitura detalhada
- `Assistente Financeiro` usa o score como base executiva
- `ResumoFinanceiroIA` leva o score consolidado para IA e interfaces estratégicas
- `Personas de Calibração` e auditorias usam o mesmo motor oficial

## Personas, validação e auditoria

Toda evolução do `MF Score` deve ser confrontada com:

- `docs/MF_SCORE_VALIDATION.md`
- `docs/MF_SCORE_AUDIT.md`

Além disso:

- a auditoria operacional continua obrigatória em mudanças relevantes
- a auditoria humana continua sendo a base para amadurecer faixas oficiais e casos canônicos
- personas persistidas continuam sendo cenários sintéticos, nunca usuários reais

## MF Score Potencial

O conceito de `MF Score Potencial` permanece como evolução futura.

Ele representa para onde o usuário poderia evoluir se corrigisse os principais pontos de pressão financeira sem alterar negativamente sua base atual.

## Regra de manutenção

Sempre que houver alteração em:

- escala
- fórmula
- pesos
- pilares
- penalizações
- classificações
- histórico
- tendência

devem ser atualizados, na mesma entrega:

- `docs/MF_SCORE.md`
- `docs/INDICADORES_FINANCEIROS.md`
- `docs/MF_SCORE_AUDIT.md`
- `docs/CHANGELOG.md`
