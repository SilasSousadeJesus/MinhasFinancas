# Project Rules

Use este arquivo para registrar acordos operacionais e arquiteturais do projeto.

## Filosofia do projeto

- Toda regra de negocio fica na camada Application.

## Regras atuais

- Preencha aqui as regras que a IA deve seguir ao alterar o projeto.

## Exemplo de regras que podem ser adicionadas

- Nunca rodar migrations automaticamente.
- Nunca alterar contratos da API sem alinhar o frontend.
- Sempre manter `RetornoGenerico` como formato padrao de resposta.
- Sempre validar `usuarioId` antes de operar em dados do usuario.
- Toda nova tela autenticada deve usar o fluxo de sessao existente.


O arquivo AI_CONTEXT.md será a principal documentação técnica deste projeto e deve ser tratado como um documento vivo.

Sempre que uma funcionalidade for implementada, removida ou alterada de forma relevante, este arquivo deve ser atualizado para refletir o estado atual do sistema.

Ao implementar qualquer feature, além das alterações de código, verifique se é necessário atualizar o AI_CONTEXT.md.

Nunca deixe o documento desatualizado em relação ao projeto.