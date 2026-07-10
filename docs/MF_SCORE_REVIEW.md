# Revisão Arquitetural do Framework Proposto para Evolução do MF Score

Este documento registra a posição oficial do projeto sobre a proposta contida em `docs/MF_Score_Framework_Definitivo.md`.

## Escopo da auditoria

Esta revisão comparou a proposta com:

- implementação atual do motor, com foco em:
  - `MinhasFinancas.Domain/Services/AnaliseFinanceira/SaudeFinanceiraService.cs`
  - `MinhasFinancas.Application/Services/MfScoreCalculoAppService.cs`
  - indicadores e contexto complementar atuais
- documentação oficial vigente:
  - `AI_CONTEXT.md`
  - `PROJECT_RULES.md`
  - `docs/MF_SCORE.md`
  - `docs/MF_SCORE_AUDIT.md`
  - `docs/MF_SCORE_VALIDATION.md`
  - `docs/INDICADORES_FINANCEIROS.md`
  - `docs/MODULE_GUIDE.md`
  - `docs/DOMAIN_GLOSSARY.md`
  - `docs/AI_DESIGN.md`
  - `docs/PRODUCT_VISION.md`
  - `docs/ASSISTANT_VISION.md`
  - `docs/ROADMAP.md`
  - `docs/CHANGELOG.md`

## Resumo executivo da revisão

- O documento proposto tem boa qualidade conceitual e está fortemente alinhado à filosofia oficial do projeto.
- O maior acerto da proposta é reforçar o `MF Score` como modelo de risco financeiro pessoal, e não como score de riqueza ou disciplina isolada.
- O maior risco da proposta é misturar boas direções conceituais com números já “fechados” antes da validação oficial por auditoria, personas e suíte de cenários.
- A proposta é especialmente forte em:
  - reforço da regra de não dupla penalização
  - amadurecimento da inadimplência
  - revisão das personas
  - preocupação com persistência temporal
- Os principais pontos de atenção são:
  - introdução de complexidade alta demais cedo demais
  - potencial opacidade do modelo
  - mudanças fortes sem `shadow run`, auditoria ou telemetria histórica suficiente
  - conflito parcial com a modelagem atual do pilar `Planejamento`
  - conflito parcial com a distribuição atual do indicador `Comprometimento da Renda`

## Matriz oficial de revisão

| Proposta | Situação Atual | Concordância | Decisão | Justificativa | Prioridade |
|---|---|---|---|---|---|
| 1. Posicionar oficialmente o MF Score como score de risco financeiro pessoal | Implementado | Concordo integralmente | Aceita | Já é a filosofia oficial em `docs/MF_SCORE.md`, `AI_CONTEXT.md` e `docs/PRODUCT_VISION.md`. A proposta reforça corretamente a direção do produto. | Alta |
| 2. Organizar o modelo em quatro camadas: pilares, penalizações críticas, persistência temporal e histórico mensal | Implementado | Concordo integralmente | Aceita | A arquitetura atual já adota essa separação conceitual. A proposta está alinhada ao desenho vigente do motor. | Alta |
| 3. Tratar a regra de não dupla penalização como princípio central | Implementado | Concordo integralmente | Aceita | Esse é um princípio oficial do projeto e já está documentado nas regras permanentes, no score e na auditoria. | Alta |
| 4. Manter pilares em 0 a 100 e score final em 0 a 1000 | Implementado | Concordo integralmente | Aceita | Já está implantado no backend, nas telas e na documentação oficial. | Alta |
| 5. Manter os horizontes 30, 90, 180 e 365 dias com pesos decrescentes | Implementado | Concordo integralmente | Aceita | Já existe na implementação e nos documentos oficiais, além de combinar com a distinção entre curto prazo e pressão acumulada. | Alta |
| 6. Tratar reserva baixa ou zerada prioritariamente como deterioração do pilar, sem crítica automática | Implementado | Concordo integralmente | Aceita | Já é regra oficial do motor e está correta do ponto de vista de risco estrutural versus risco materializado. | Alta |
| 7. Tratar comprometimento alto da renda prioritariamente no pilar, sem crítica automática | Implementado | Concordo integralmente | Aceita | Já está alinhado à filosofia oficial. Penalização crítica só deve existir quando houver ruptura ou persistência negativa. | Alta |
| 8. Tratar endividamento patrimonial alto sem crítica automática quando ainda não há ruptura materializada | Implementado | Concordo integralmente | Aceita | A proposta está coerente com a documentação e com a modelagem atual do motor. | Alta |
| 9. Reposicionar o indicador Comprometimento da Renda como eixo principal de Endividamento | Parcial | Concordo parcialmente | Aceita com adaptações | Hoje o projeto o trata principalmente como pressão operacional de `Fluxo de Caixa`, e não como dívida pura. Faz sentido discutir sua presença em `Endividamento`, mas não mover integralmente sem recalibrar pilares e evitar duplicação com os compromissos futuros. | Média |
| 10. Adotar faixas operacionais por indicador com nível adicional de “risco grave = 0” | Parcial | Concordo parcialmente | Aceita com adaptações | Conceitualmente útil para calibração, mas conflita com a escala oficial atual dos status (`100/80/55/25`). Se adotado, deve entrar como camada de análise interna ou regra de faixa, e não necessariamente como novo status oficial. | Média |
| 11. Criar gatilho combinado de liquidez crítica + fluxo crítico para reconhecer colapso estrutural avançado | Não existe | Concordo parcialmente | Aceita com adaptações | A ideia é forte, mas zerar automaticamente o pilar `Planejamento` por combinação de outros pilares mistura domínios. O melhor caminho é estudar uma regra crítica própria de colapso estrutural sem desfigurar a semântica do pilar de planejamento. | Média |
| 12. Substituir ausência genérica de dados por regra de 45 dias sem atualização relevante | Parcial | Concordo parcialmente | Aceita com adaptações | Hoje existe penalização por dados essenciais insuficientes, mas não por desatualização temporal. A proposta faz sentido, porém depende de definir tecnicamente o que conta como “atualização relevante” sem gerar falsos positivos. | Média |
| 13. Fixar penalidade crítica de fluxo mensal negativo em -50 | Parcial | Concordo parcialmente | Aceita com adaptações | O conceito já existe, mas o valor exato ainda deve nascer da auditoria oficial, e não de decisão isolada. O número pode ser bom, mas ainda não é oficial. | Alta |
| 14. Fixar penalidade crítica de patrimônio líquido negativo em -200 | Parcial | Concordo parcialmente | Aceita com adaptações | A direção é correta: patrimônio líquido negativo merece severidade alta. O ponto em aberto é o tamanho exato da punição na escala de 0 a 1000 e seu efeito nas personas. | Alta |
| 15. Evoluir a inadimplência de regra binária para matriz gradual por dias de atraso e valor vencido | Não existe | Concordo integralmente | Aceita com adaptações | É uma das melhores propostas do documento. Está totalmente alinhada à auditoria atual, mas precisa de desenho cuidadoso para não romper simplicidade, rastreabilidade e comparabilidade histórica. | Alta |
| 16. Tornar a persistência temporal bem mais severa, com faixas como 2 meses = -150 e 3+ meses = -300 | Parcial | Concordo parcialmente | Aceita com adaptações | O motor já possui persistência temporal, mas os novos valores precisam ser auditados. O risco aqui é punir em excesso um comportamento já parcialmente refletido no fluxo e nas críticas. | Alta |
| 17. Aplicar multiplicador de 1,5x para reincidência de inadimplência em até 90 dias | Não existe | Concordo parcialmente | Adiada | A ideia é plausível, mas depende de histórico de eventos de inadimplência mais rico e de política clara de “cura” e “reincidência”. Hoje ainda não temos base oficial suficiente para estabilizar essa regra. | Futuro |
| 18. Criar curva de cura com amortização temporal residual de 50% e 25% por 90 dias | Não existe | Concordo parcialmente | Adiada | É uma boa evolução para evitar efeito ioiô, mas adiciona bastante opacidade ao motor. Antes disso, o projeto precisa amadurecer melhor a camada temporal e sua explicabilidade. | Futuro |
| 19. Revisar oficialmente as faixas esperadas das personas sintéticas | Parcial | Concordo integralmente | Aceita com adaptações | As personas já existem e a proposta de recalibragem é útil. O que não pode ocorrer é substituir as faixas atuais sem passar pela auditoria humana e pela suíte oficial. | Alta |
| 20. Aumentar o peso do Comprometimento Financeiro Futuro de 30 dias para 1.75 | Não existe | Concordo parcialmente | Adiada | Faz sentido como hipótese de calibração, mas ainda é uma mudança numérica específica demais para entrar sem rodada de auditoria. Deve ser testada, não adotada de imediato. | Média |
| 21. Evoluir o pilar Planejamento para sinais reais de execução, como Orçado vs. Realizado | Parcial | Concordo integralmente | Aceita | Está totalmente alinhado ao achado `MF-001` da auditoria oficial. Hoje o pilar ainda é proxy, e essa é uma das lacunas mais claras do motor. | Alta |
| 22. Realizar shadow run paralelo por 30 dias antes de mudanças fortes no score | Não existe | Concordo parcialmente | Aceita com adaptações | A recomendação é boa, mas não precisa necessariamente nascer como propriedade `ScoreV2` persistida. O projeto pode adotar execução paralela, feature flag ou auditoria comparativa offline. | Média |
| 23. Substituir parte da camada temporal por médias móveis exponenciais (EMA) | Não existe | Concordo parcialmente | Adiada | É uma proposta avançada e potencialmente útil, mas ainda distante da maturidade atual do motor. Há risco de reduzir transparência antes de consolidar regras determinísticas mais simples. | Futuro |
| 24. Integrar Open Finance ao score como fonte futura de comportamento externo | Não existe | Concordo parcialmente | Adiada | Faz sentido como direção de longo prazo, mas conflita com o escopo atual e introduz dependências regulatórias, de consentimento e de reconciliação de dados externas ao motor atual. | Futuro |
| 25. Calibrar dinamicamente o modelo com Machine Learning, como XGBoost | Não existe | Não concordo | Rejeitada | No estado atual do projeto, essa proposta conflita com a necessidade de transparência, rastreabilidade, auditabilidade e explicação causal do MF Score. Antes de qualquer ML, o motor precisa amadurecer mais com regras claras e dados históricos confiáveis. | Baixa |
| 26. Definir explicitamente o apetite de risco do produto para orientar o quão severo o score deve ser | Parcial | Concordo integralmente | Aceita | A discussão já aparece implicitamente no produto e na calibração, mas merece formalização explícita porque influencia faixas, personas, penalidades e experiência do usuário. | Alta |

## Principais conflitos com a arquitetura atual

### 1. Comprometimento da Renda

O framework proposto aproxima esse indicador do pilar `Endividamento`, enquanto a arquitetura oficial atual o trata como pressão operacional de `Fluxo de Caixa`, além de usá-lo como proxy em `Planejamento`.

Posição oficial:

- a proposta faz sentido como hipótese de revisão
- ela não deve ser aceita literalmente sem recalibrar pilares e revisar risco de dupla penalização

### 2. Pilar Planejamento

O documento propõe uma visão mais madura do pilar, mas em alguns trechos injeta efeitos de colapso estrutural diretamente nele.

Posição oficial:

- o pilar de planejamento realmente precisa evoluir
- porém ele não deve virar “pilar residual” que absorve problemas de outros pilares sem semântica própria

### 3. Penalidades numéricas já fechadas

A proposta traz vários números fortes como se já pudessem ser oficializados.

Posição oficial:

- a direção conceitual é boa
- os números não são oficiais até passarem por auditoria operacional, personas e auditoria humana

### 4. Complexidade temporal

Reincidência, amortização, cura e EMA são evoluções plausíveis, mas aumentam muito a complexidade do motor.

Posição oficial:

- devem ser tratadas como evolução futura
- não devem entrar antes da consolidação das regras determinísticas já existentes

## Principais riscos técnicos encontrados

- risco de dupla penalização se alguns indicadores mudarem de pilar sem revisão completa
- risco de opacidade do motor ao adicionar camadas temporais avançadas cedo demais
- risco de “colar número” em penalidades sem validação das personas
- risco de quebra de comparabilidade histórica do `HistoricoMfScore`
- risco de inflar o pilar `Planejamento` com responsabilidades que não são dele
- risco de roadmap paralelo ao oficial ao antecipar Open Finance e Machine Learning

## Principais oportunidades encontradas

- amadurecer a inadimplência para algo mais realista e menos binário
- revisar oficialmente as faixas esperadas das personas
- consolidar uma política explícita de apetite de risco do produto
- atacar o achado `MF-001` com evolução real do pilar `Planejamento`
- estruturar um processo de `shadow run` para futuras revisões maiores do motor

## Recomendações oficiais de priorização futura

### Prioridade alta

1. Evoluir a inadimplência para modelo gradual.
2. Revisar as faixas esperadas das personas com auditoria humana.
3. Definir oficialmente o apetite de risco do produto.
4. Evoluir o pilar `Planejamento` com sinais mais reais de execução.
5. Recalibrar penalidades já existentes somente depois da revisão acima.

### Prioridade média

1. Revisar a posição do `Comprometimento da Renda` dentro dos pilares.
2. Estudar critério temporal de ausência de atualização.
3. Estruturar estratégia de `shadow run` para mudanças maiores.

### Futuro

1. Reincidência formal de inadimplência.
2. Curva de cura temporal.
3. EMA para tendências temporais.
4. Open Finance como nova fonte de dados.

### Rejeitado no estado atual

1. Calibragem automática por Machine Learning como próxima etapa do motor.

## Conclusão oficial

O framework proposto é valioso como material de calibração e mostra boa maturidade conceitual. Ele está mais forte nas ideias do que nos números finais.

A posição oficial do projeto é:

- aproveitar as direções conceituais que reforçam o `MF Score` como motor de risco
- rejeitar a adoção imediata de valores numéricos ainda não auditados
- adiar complexidades temporais e estatísticas que hoje aumentariam opacidade
- manter a governança atual: documentação oficial, personas, auditoria operacional, auditoria humana e rastreabilidade histórica

Nenhuma proposta deste documento se torna oficial automaticamente. Tudo o que foi aceito aqui continua dependente de implementação futura, auditoria e sincronização documental.
