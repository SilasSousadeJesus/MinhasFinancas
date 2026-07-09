# Project Rules

Use este arquivo para registrar apenas regras permanentes de desenvolvimento do projeto.

Não registrar roadmap, histórico de funcionalidades ou changelog neste documento.

## Filosofia do projeto

- Toda regra de negócio fica prioritariamente na camada `Application`.

## Princípios gerais

- Toda implementação deve priorizar rastreabilidade, histórico, clareza e controle financeiro.
- Evitar soluções improvisadas que resolvem apenas o problema imediato.
- Sempre que possível, modelar os dados pensando em consultas futuras, auditoria, agrupamento e relatórios.
- Não apagar informações financeiras relevantes sem preservar rastreabilidade.
- Preferir modelos explícitos em vez de regras escondidas em descrições ou convenções frágeis.

## Arquitetura

- `Controller` não deve conter regra de negócio.
- `Application/Services` devem concentrar a regra de negócio operacional.
- `Repository` deve cuidar de acesso a dados, não de decisões de negócio.
- Evitar duplicação de lógica entre frontend e backend.
- Regras críticas devem ser validadas no backend, mesmo quando existirem no frontend.
- Infraestruturas globais devem ser reutilizáveis, desacopladas e não específicas de uma tela.

## Idioma padrão do projeto

- Todo código, documentação, nomes de classes, métodos, propriedades, pastas, componentes, telas, labels e mensagens devem utilizar português brasileiro.
- Evitar nomes em inglês, salvo quando forem termos técnicos inevitáveis, nomes de bibliotecas, APIs externas ou padrões consolidados da plataforma.
- A linguagem do domínio deve permanecer em português para manter consistência e legibilidade do projeto.

## Camada analítica

- Toda regra analítica deve ser implementada na camada `AnaliseFinanceira`.
- Dashboards, telas, exportações e APIs nunca devem recalcular indicadores diretamente.
- Toda informação analítica deve ser consumida dessa camada.

## Indicadores financeiros oficiais

- As fórmulas, pesos, classificações e prioridades dos indicadores financeiros devem ser documentados e mantidos em `docs/INDICADORES_FINANCEIROS.md`.
- Sempre que um indicador for criado, removido, alterado ou tiver fórmula, peso, classificação ou texto oficial modificado, a documentação dos indicadores deve ser atualizada na mesma entrega.
- Nenhuma alteração em indicador financeiro deve ser considerada concluída sem sincronizar o documento oficial de indicadores.

## Dados x Informação x Inteligência

- O projeto deve manter uma separação clara entre três níveis.
- Dados: entidades persistidas (`Lançamentos`, `Patrimônio`, `Metas`, `Perfil Financeiro` etc.).
- Informação: cálculos e indicadores derivados dos dados (`AnaliseFinanceira`).
- Inteligência: interpretações e recomendações construídas sobre os indicadores (`Saúde Financeira`, insights, IA).
- Cada nível deve consumir apenas o nível anterior, evitando duplicação de regras de negócio.
- Exemplo: `Dados -> Indicadores -> Saúde Financeira -> Insights -> IA`.

## Evolução incremental

- Sempre que uma funcionalidade fizer parte de um plano maior, implementar primeiro uma versão simples, porém arquiteturalmente correta.
- Evitar implementar toda a visão futura de uma única vez.
- Cada versão deve deixar a arquitetura preparada para a próxima evolução, sem gerar retrabalho ou quebra de compatibilidade.

## Frontend

- Evitar loading manual espalhado pelas páginas.
- Preferir componentes globais e reutilizáveis.
- Labels devem ser claros e não induzir o usuário ao erro.
- Filtros devem deixar explícito qual campo estão filtrando.
- Toda operação demorada deve dar feedback visual ao usuário.

## Organização da interface

- Toda tela deve ser organizada de acordo com o fluxo mental do usuário, e não pela ordem em que as funcionalidades foram implementadas.
- Campos relacionados devem permanecer agrupados visualmente.
- Sempre que possível, utilizar a seguinte hierarquia:
  1. Pesquisa
  2. Classificação
  3. Períodos
  4. Ordenação
  5. Ações
- Evitar distribuir informações relacionadas em regiões diferentes da tela.
- A interface deve reduzir esforço cognitivo e facilitar produtividade.

## Padronização de interface

- Toda interface deve utilizar português brasileiro.
- Todos os textos devem possuir acentuação correta.
- Utilizar linguagem simples, natural e profissional.
- Manter consistência de nomenclatura em todo o sistema.
- Evitar abreviações desnecessárias.
- Sempre revisar novos textos antes de finalizar uma implementação.

## Encoding dos arquivos

- Todos os arquivos de código-fonte, componentes, arquivos de configuração e documentos devem ser salvos em UTF-8.
- Nunca remover acentos para contornar problema de encoding.
- Caso exista problema de codificação, corrigir o encoding do arquivo em vez de alterar a grafia.

## Dados financeiros

- Lançamentos gerados automaticamente devem manter vínculo com sua origem.
- Lançamentos parcelados devem possuir identificador de grupo, número da parcela e total de parcelas.
- Lançamentos fixos ou programados devem permitir rastrear que foram gerados a partir de uma mesma configuração.
- Alterações em lançamentos financeiros devem preservar o máximo possível de contexto histórico.
- Campos importantes para relatórios futuros devem ser estruturados, não derivados apenas de descrição.

## Infraestrutura de relatórios

- Toda exportação do sistema deve utilizar uma infraestrutura reutilizável.
- Cada relatório deve fornecer apenas os dados.
- A responsabilidade pela apresentação deve permanecer centralizada.
- Evitar duplicação de código de formatação.
- Toda melhoria realizada na infraestrutura de relatórios deve beneficiar automaticamente todas as exportações do sistema.

## Histórico e rastreabilidade analítica

- Sempre que uma informação puder ser útil para análise futura, comparação temporal, auditoria, geração de indicadores ou uso por IA analítica, preferir preservar histórico em vez de sobrescrever o dado.
- O sistema deve priorizar rastreabilidade, histórico e capacidade de cruzamento de informações.
- Nem toda alteração precisa gerar histórico, mas alterações com significado financeiro ou analítico devem ser preservadas.

## Manutenção com IA

- Antes de implementar qualquer feature, ler `AI_CONTEXT.md` e `PROJECT_RULES.md`.
- Ao evoluir a inteligência do sistema, revisar primeiro a implementação já existente antes de criar novas classes, serviços, modelos ou cálculos.
- Consolidar e completar a arquitetura atual sempre que possível, evitando duplicação desnecessária.
- Ao alterar arquitetura, fluxo técnico, infraestrutura ou padrão estrutural, avaliar atualização do `AI_CONTEXT.md`.
- Ao surgir nova regra permanente de desenvolvimento, atualizar `PROJECT_RULES.md`.
- Ao final de cada implementação, informar se a documentação foi atualizada.

## Integrações com IA

- A IA nunca deve consultar diretamente o banco de dados.
- Toda integração com IA deve consumir contexto preparado pelo sistema, preferencialmente a partir do `ResumoFinanceiroIA`.
- Provedores externos devem ficar isolados na camada de infraestrutura.
- Nenhuma chave de API deve ser versionada no repositório.

## Construção de Contexto para IA

- Antes de enviar qualquer solicitação para um modelo de IA, o sistema deve fornecer o máximo possível de contexto estruturado produzido pelas regras de negócio.
- Sempre priorizar:
  - indicadores
  - interpretações
  - tendências
  - resumos
  - histórico
  - contexto agregado
- Evitar enviar dados brutos quando existir uma interpretação equivalente produzida pelo domínio.
- A IA deve atuar como camada de comunicação, interpretação e aconselhamento, nunca como substituta das regras de negócio do sistema.

## Documentação viva

- Toda implementação relevante deve atualizar a documentação correspondente.
- Não utilizar o `AI_CONTEXT.md` como repositório de todas as informações do projeto.
- Cada informação deve ser registrada apenas no documento responsável por aquele assunto.
- A documentação faz parte do código e deve evoluir junto com ele.

## Changelog obrigatório

- Toda fase concluída, implementação relevante ou evolução arquitetural deve ser registrada em `docs/CHANGELOG.md`.
- As entradas do changelog devem seguir a ordem cronológica real do trabalho, usando a data correspondente ao registro.
- O changelog complementa o roadmap e o AI_CONTEXT, mas não os substitui.


## MF Score

- O `MF Score` é o modelo oficial de avaliação de risco financeiro pessoal do sistema.
- Sempre que um indicador, peso, fórmula, pilar, penalidade, regra crítica, classificação ou tendência do `MF Score` mudar, devem ser atualizados obrigatoriamente `docs/INDICADORES_FINANCEIROS.md` e `docs/MF_SCORE.md`.
- Nenhuma alteração no `MF Score` deve ser considerada concluída sem a sincronização simultânea desses dois documentos.
