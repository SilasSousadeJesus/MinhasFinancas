# Glossário do Domínio - Minhas Finanças

Este documento registra os conceitos oficiais do domínio financeiro do sistema.

Sempre que surgir um novo conceito relevante, ele deve ser documentado aqui.

## Lançamento

Registro de uma movimentação financeira do usuário.

## Data de lançamento

Data em que o registro foi criado no sistema.

Não representa vencimento, pagamento nem recebimento.

## Data de vencimento

Data prevista para a movimentação ocorrer.

- em despesas, representa o vencimento da obrigação
- em receitas, representa a data prevista de entrada do valor

## Data de efetivação

Data em que o dinheiro realmente entrou ou saiu.

- em despesas, representa a data do pagamento
- em receitas, representa a data do recebimento

## Status do lançamento

Estado atual do ciclo de vida do lançamento.

## Pendente

Lançamento previsto, mas ainda não efetivado.

## Pago

Status final permitido para despesas efetivadas.

## Recebido

Status final permitido para receitas efetivadas.

## Cancelado

Lançamento que não deve mais produzir efeito financeiro previsto nem realizado.

## Receita

Movimentação de entrada de recursos.

Pode assumir apenas:

- `Pendente`
- `Recebido`
- `Cancelado`

## Despesa

Movimentação de saída de recursos.

Pode assumir apenas:

- `Pendente`
- `Pago`
- `Cancelado`

## Efetivação

Ato de registrar que a movimentação realmente ocorreu.

- receita: `Pendente -> Recebido`
- despesa: `Pendente -> Pago`

## Conta

Meio financeiro do usuário usado para organizar saldo e lançamentos.

## Cartão

Meio de pagamento vinculado ao usuário, utilizado nos lançamentos e no controle financeiro.

## Categoria

Classificação principal de um lançamento.

## Subcategoria

Detalhamento de uma categoria.

## Fluxo de Caixa Previsto

Leitura baseada em datas previstas, especialmente `DataVencimento`.

## Fluxo de Caixa Realizado

Leitura baseada apenas em movimentações efetivadas, usando `DataEfetivacao`.

## Patrimônio

Conjunto de ativos e passivos do usuário.

## Ativo patrimonial

Bem ou recurso que compõe positivamente o patrimônio.

## Passivo patrimonial

Obrigação financeira que reduz o patrimônio.

## Patrimônio líquido

Resultado de:

`Total de Ativos - Total de Passivos`

## Snapshot patrimonial

Fotografia congelada do patrimônio em uma data de referência.

## Evolução patrimonial

Leitura histórica construída a partir dos snapshots salvos.

## Meta

Objetivo financeiro acumulativo do usuário.

## Projeção

Simulação orientada a objetivo financeiro, com acumulado inicial, renda base, renda extra e horizonte temporal.

## Simulação Financeira

Cenário hipotético persistido por usuário para testar decisões futuras sem alterar dados reais.

## Perfil Financeiro

Conjunto de parâmetros pessoais usados para definir como o usuário deseja medir e avaliar sua própria saúde financeira.

## Parâmetro Financeiro

Valor configurado pelo usuário para servir de referência em análises, indicadores, alertas e comparações futuras.

## Configuração vigente

Registro atualmente ativo dentro de um histórico de configurações.

Em módulos históricos, representa a configuração que deve ser considerada como referência atual do usuário.

## Histórico do Perfil Financeiro

Conjunto temporal de configurações anteriores do perfil financeiro do usuário.

Permite saber quais critérios estavam válidos em cada período.

## Ação de Simulação

Evento hipotético dentro de uma simulação financeira.

Na V1 pode representar:

- receita única
- despesa única
- receita recorrente mensal
- despesa recorrente mensal
- despesa parcelada

## Resultado simulado

Consolidação mensal que combina base real de lançamentos com ações hipotéticas.

## Saldo real

Saldo mensal obtido apenas dos lançamentos reais.

## Saldo simulado

Saldo mensal obtido ao aplicar as ações simuladas sobre a base real.
