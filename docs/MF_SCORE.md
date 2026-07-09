# MF Score

O MF Score, ou Minhas Finanças Score, é o modelo oficial de avaliação de risco financeiro pessoal do sistema.

Ele substitui conceitualmente a antiga noção de simples pontuação de saúde financeira e passa a responder uma pergunta mais precisa:

> Qual a probabilidade de o usuário perder estabilidade financeira se continuar seguindo a trajetória atual?

O MF Score não mede riqueza, felicidade financeira nem apenas disciplina de gastos.
Ele mede risco, proteção, pressão estrutural e maturidade financeira ao longo do tempo.

## Filosofia

- O score existe para interpretar risco financeiro pessoal.
- O sistema analisa fatos, indicadores e tendência antes de emitir a nota final.
- A IA apenas explica o que o sistema já calculou.
- Nenhuma regra crítica deve ficar escondida em texto livre.
- O score deve ser transparente, rastreável e evolutivo.

## Estrutura

O MF Score é construído em cinco pilares:

1. Fluxo de Caixa
2. Liquidez e Reserva
3. Endividamento e Obrigações
4. Patrimônio
5. Planejamento e Disciplina

Cada pilar gera sua própria nota.
Depois disso o sistema calcula um `MF Score Base`.

Em seguida, regras críticas podem reduzir a nota para refletir riscos que não devem ser suavizados por média simples.

## Pesos iniciais

- Fluxo de Caixa: 30%
- Liquidez e Reserva: 25%
- Endividamento e Obrigações: 20%
- Patrimônio: 15%
- Planejamento e Disciplina: 10%

## Indicadores por pilar

### Fluxo de Caixa

- Economia mensal
- Percentual de economia
- Comprometimento da renda
- Comprometimento financeiro futuro de 30 dias

### Liquidez e Reserva

- Reserva atual
- Cobertura da reserva
- Reserva ideal configurada

### Endividamento e Obrigações

- Endividamento patrimonial
- Pressão financeira acumulada de 90 dias
- Pressão financeira acumulada de 180 dias
- Pressão financeira acumulada de 12 meses
- Obrigações futuras

### Patrimônio

- Patrimônio líquido atual
- Patrimônio-alvo
- Percentual de avanço sobre o patrimônio-alvo

### Planejamento e Disciplina

- Perfil financeiro configurado
- Plano estratégico
- Compromissos financeiros
- Metas cadastradas
- Consistência estratégica

Na primeira versão, quando nem todos os dados estiverem disponíveis diretamente na mesma camada, o sistema usa os sinais estruturados já existentes como proxy para manter o score funcional e expansível.

## Score Base

O `MF Score Base` é a média ponderada das notas dos cinco pilares.

Fluxo de cálculo:

1. cada pilar recebe uma nota de 0 a 100;
2. cada pilar é multiplicado pelo seu peso;
3. o total ponderado é dividido pela soma dos pesos;
4. o resultado é arredondado para inteiro;
5. o score base é então ajustado por regras críticas.

## Regras críticas

As regras críticas representam situações que exigem penalização explícita.

Exemplos:

- reserva inexistente;
- comprometimento da renda muito elevado;
- pressão financeira futura muito elevada;
- endividamento patrimonial muito alto;
- patrimônio líquido negativo.

As penalizações são progressivas sempre que possível.

## Classificação oficial

- `90-100` - Excelente
- `80-89` - Muito Bom
- `70-79` - Bom
- `60-69` - Atenção
- `40-59` - Crítico
- `0-39` - Muito Crítico

## Tendência

A tendência existe para mostrar se a trajetória está melhorando, estabilizada ou piorando.

Na primeira versão:

- a tendência pode ser calculada de forma determinística com base nos sinais atuais;
- o histórico visual de evolução pode ser acrescentado depois;
- o modelo já nasce preparado para receber séries históricas futuras.

## Relação com outras camadas

- `AnaliseFinanceira` calcula os pilares e o MF Score.
- `Saúde Financeira` exibe o score, a classificação, o risco e a leitura dos pilares.
- `ResumoFinanceiroIA` leva o score consolidado para a IA e para interfaces executivas.
- `Assistente Financeiro` transforma esse score em narrativa consultiva.
- `Simulações`, `Projeções`, `Patrimônio` e `Plano Estratégico` poderão usar o MF Score como referência para avaliar qualidade de cenários e decisões.

## Exemplo resumido

Se os pilares forem:

- Fluxo de Caixa: 72
- Liquidez e Reserva: 34
- Endividamento e Obrigações: 59
- Patrimônio: 81
- Planejamento e Disciplina: 93

O sistema gera um MF Score Base e aplica as regras críticas antes de produzir a nota final.

## Regra de manutenção

Sempre que mudar um indicador, peso, fórmula, regra crítica, classificação ou tendência, este documento e `docs/INDICADORES_FINANCEIROS.md` devem ser atualizados na mesma entrega.
