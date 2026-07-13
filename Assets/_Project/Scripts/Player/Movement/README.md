# Módulo: Player Movement

Este módulo é responsável por gerenciar a física e a lógica de movimentação do jogador. Ele atua como o principal consumidor de eventos de input e interage diretamente com o `Rigidbody` e a `StateMachine`.

## Fluxo de Funcionamento

1. **Inscrição de Eventos**: Durante o `OnEnable`, o `PlayerMovement` se inscreve no `EventBus` para ouvir mensagens do tipo:
   - `PlayerMoveMessage`
   - `PlayerJumpMessage`
   - `PlayerDashMessage`
2. **Atualização de Estado**: Quando uma mensagem é recebida, o script atualiza variáveis internas (ex: cache do vetor de input horizontal, flags de requisição de pulo ou dash).
3. **Máquina de Estados (State Machine)**: A movimentação não ocorre de forma linear. O `PlayerMovement` inicializa e delega a execução frame-a-frame para os estados (ex: `PlayerIdleState`, `PlayerWalkState`, `PlayerJumpState`, `PlayerDashState`).
4. **Execução Física**: Cada estado invoca métodos expostos pelo `PlayerMovement` (através da interface `IPlayerMovementContext`), como `UpdateMovement()` ou `ExecuteJump()`, que por sua vez manipulam o `Rigidbody` de forma determinística utilizando o `FixedUpdate` (geralmente orquestrado pelos próprios estados).

## Interação com outros Módulos
- **Depende de**: `Core` (para o EventBus e StateMachine).
- **Consome**: Mensagens (ex: `PlayerMoveMessage`) publicadas pelo módulo `Input`.
- **Fornece**: O namespace `Messages` e a estrutura base de requisições de movimento, servindo de fundação para o que o jogador pode fazer.
