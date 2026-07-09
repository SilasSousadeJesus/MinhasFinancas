# Product Vision - Minhas Financas

## Objetivo do sistema

`Minhas Financas` e um sistema de gestao financeira pessoal voltado para transformar dados financeiros dispersos em decisoes praticas.

O produto deve ajudar o usuario a entender:

- como esta sua saude financeira hoje
- o que vence em seguida
- como seu patrimonio evolui
- quando alcancara objetivos financeiros
- como diferentes decisoes alterariam seu futuro financeiro

## Problema que o sistema resolve

Grande parte dos usuarios registra receitas e despesas, mas continua sem visao clara de:

- fluxo de caixa real
- compromissos futuros
- evolucao patrimonial
- progresso em metas
- impacto de novas decisoes financeiras

O sistema existe para reduzir essa cegueira operacional e transformar registro em inteligencia financeira.

## Tipo de produto que estamos construindo

Nao queremos apenas um cadastro de movimentacoes.

Queremos construir uma plataforma de inteligencia financeira pessoal com foco em:

- clareza
- rastreabilidade
- historico
- planejamento
- apoio a decisao

## Publico-alvo

- pessoas fisicas que desejam organizar sua vida financeira
- usuarios que controlam contas, cartoes, categorias, metas e patrimonio
- usuarios que desejam planejar objetivos e comparar cenarios futuros

## Principios do produto

- Toda funcionalidade deve gerar informacao util para tomada de decisao.
- Priorizar clareza antes de sofisticacao visual.
- Priorizar rastreabilidade antes de automacoes frageis.
- Priorizar historico antes de sobrescrever estados antigos.
- Priorizar planejamento financeiro, nao apenas registro do passado.
- Evitar funcionalidades que apenas armazenem dados sem produzir leitura util.
- Sempre que possivel, conectar numeros a contexto e consequencia pratica.

## Progressao de valor entre camadas

Cada camada do sistema deve agregar valor a anterior.

- Os dados registram fatos.
- Os indicadores resumem esses fatos.
- A saude financeira interpreta os indicadores.
- Os insights destacam oportunidades, riscos e prioridades.
- O `ResumoFinanceiroIA` organiza esse conhecimento para consumo por interfaces e por futuros modelos de IA.
- A Memoria Financeira preserva a evolucao analitica ao longo do tempo.
- O `InterpretadorMemoriaFinanceira` transforma esse historico em continuidade narrativa antes do consumo por IA.
- O `Plano Estratégico Financeiro` registra a direção escolhida pelo usuário para orientar decisões futuras.
- O `Interpretador Estratégico` transforma esse plano em narrativa compreensível para interfaces e IA.
- A `Consistência Estratégica` avalia de forma determinística se uma decisão está alinhada ao plano vigente antes de qualquer interpretação da IA.
- A intenção do usuário deve ser estruturada como `Decisão Financeira` antes de chegar às camadas estratégicas, para preservar rastreabilidade e clareza de domínio.
- A `IA Estratégica` transforma esse contexto consolidado em um parecer consultivo mais profundo, sem criar novas regras de negócio.
- Os `Especialistas Financeiros` aprofundam a leitura por domínio antes da consolidação final da resposta.

Nenhuma camada deve substituir a anterior.

Cada uma existe para enriquecer a compreensao da situacao financeira do usuario.

Quando o Assistente Financeiro identifica uma intenção relevante, ela pode ser convertida em um compromisso rastreável sem perder o contexto original.

A conversa do usuário também pode permanecer aberta por mais tempo, para que o sistema acompanhe o raciocínio em andamento sem obrigá-lo a repetir tudo a cada nova pergunta.

Para uma leitura mais humana e inspiradora da jornada do Assistente, consulte `docs/ASSISTANT_VISION.md`.

## Integracao com IA como evolucao em duas etapas

A evolucao do Assistente Financeiro com IA foi oficialmente separada em duas responsabilidades diferentes:

- primeiro, fazer a infraestrutura tecnica de integracao funcionar de verdade
- depois, construir a primeira experiencia real de analise financeira com IA

Essa separacao existe para evitar confusoes entre:

- conectar com um provedor externo
- melhorar a qualidade analitica da experiencia entregue ao usuario

O sistema so deve considerar a IA como evolucao de experiencia quando a base tecnica e de contexto estiver madura, segura e controlada.

## Direcao do produto

O produto caminha para se tornar um centro pessoal de inteligencia financeira com quatro pilares:

1. Operacao
   - lancamentos, contas, cartoes, categorias e efetivacao
2. Leitura
   - dashboard, radar financeiro, fluxo de caixa e relatorios
3. Planejamento
   - metas, projecoes e simulacoes financeiras
4. Patrimonio
   - ativos, passivos, snapshots e evolucao patrimonial

## Distinção entre régua e direção

O `Perfil Financeiro` define a régua de avaliação.

O `Plano Estratégico Financeiro` define a direção escolhida.

Os dois módulos são complementares:

- o perfil diz o que é desejável, aceitável ou saudável
- o plano diz para onde o usuário decidiu caminhar

Essa separação evita confundir parâmetros numéricos com intenção estratégica de longo prazo.

## Natureza do plano estratégico

O `Plano Estratégico Financeiro` representa a intenção deliberada do usuário.

Ele deve ser tratado como um ativo de longo prazo, versionado e historicamente preservado.

A primeira versão dessa camada já está disponível no backend como a fase 4.2.3.1.

A IA nunca cria ou modifica estratégias.

Ela apenas interpreta decisões à luz do plano vigente, da evolução histórica e da situação financeira atual.

Na Fase 4.2.5, ela passou a considerar também compromissos ativos e consistência estratégica para produzir uma leitura mais humana e mais responsável.

## Criterio para novas funcionalidades

Antes de criar qualquer nova feature, responder:

- Que decisao melhor o usuario conseguira tomar depois de ver ou usar isso?
- A funcionalidade melhora operacao, leitura, planejamento ou patrimonio?
- Existe rastreabilidade suficiente?
- A informacao produz valor recorrente ou apenas ocupa espaco?

