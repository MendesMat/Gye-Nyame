# Módulo: Player Animation

Este módulo cuida do retorno visual (feedback visual) para as ações do jogador. Ele traduz as mudanças de estado lógico e inputs em animações e transformações na tela.

## Fluxo de Funcionamento

1. **Reação à State Machine**: O `PlayerAnimationHandler` se inscreve no evento `OnStateChanged` da `StateMachine` do jogador. Sempre que o jogador muda de um `PlayerIdleState` para um `PlayerWalkState`, por exemplo, este módulo é notificado.
2. **Mapeamento de Estados para Clipes**: Utilizando um dicionário interno, ele mapeia o tipo de estado atual para um *Hash* do Animator.
3. **Animator Override**: O módulo utiliza um `AnimatorOverrideController` no `Awake()` para substituir os clipes padrão ("Idle", "Walk", etc.) pelos clipes configurados via Inspector. Isso permite que a mesma lógica seja reaproveitada caso o personagem troque de skin ou conjunto de animações no futuro.
4. **Atualização Visual (Flip)**: O script também assina o evento `PlayerMoveMessage` no `EventBus` para interceptar a direção horizontal do input. No `Update()`, ele utiliza esse valor para espelhar (flip) o `SpriteRenderer` para a esquerda ou para a direita, garantindo que o personagem olhe para onde está se movendo.

## Interação com outros Módulos
- **Depende de**: `Core` (para `StateMachine` e `EventBus`) e `Movement` (para os tipos de Estado específicos, como `PlayerWalkState`, e mensagens como `PlayerMoveMessage`).
- **Consome**: Mudanças de estado da física/lógica e direção de input.
- **Fornece**: Nenhuma lógica de volta. É um módulo puramente reativo (espectador) projetado para não interferir nas mecânicas de gameplay, mantendo a arquitetura limpa (MVC-like: sendo estritamente a "View").
