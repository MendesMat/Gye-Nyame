# Módulo: Player Input

Este módulo é responsável por capturar as ações físicas do jogador (teclado, mouse, gamepad) através do Unity Input System e traduzi-las em eventos para o restante do jogo.

## Fluxo de Funcionamento

1. **Captura de Input**: O script `PlayerInputHandler` ouve os eventos disparados pelo `PlayerInputActions` (gerado automaticamente pelo Unity Input System).
2. **Conversão e Desacoplamento**: Em vez de invocar diretamente métodos no script de movimentação ou combate, o Input Handler converte os inputs brutos em mensagens estruturadas (estruturas localizadas em `Player/Movement/Messages`), como:
   - `PlayerMoveMessage(Vector2)`
   - `PlayerJumpMessage()`
   - `PlayerDashMessage()`
3. **Publicação (EventBus)**: Essas mensagens são disparadas utilizando o `EventBus` (fornecido pelo módulo `Core`).

## Interação com outros Módulos
- **Depende de**: `Core` (para o EventBus) e `Movement` (para importar as estruturas de Mensagem).
- **Consome**: Inputs diretamente do pacote Input System da Unity.
- **Fornece**: Sinais de intenção de ação. Como ele apenas publica no `EventBus`, ele não possui conhecimento de *quem* vai realizar o pulo ou o movimento. Isso garante baixo acoplamento e facilita a criação de mocks para testes ou transições entre controle de jogador e controle de IA.
