# Módulo: Player Movement (`GyeNyame.Player.Movement`)

Este módulo é o coração da física e locomoção do jogador. Ele traduz intenções (mensagens) em forças aplicadas a um `Rigidbody` através de um comportamento pautado pela `StateMachine`.

## Mecânicas e Funcionalidades

- **Movimento Horizontal e Profundidade:** Lê os inputs `X` e `Y` e aplica velocidade respeitando as regras do jogo.
- **Salto (Jump):** Lógica que lida com a gravidade de forma customizada (`_verticalVelocity`), aplicando uma força impulsiva e reduzindo-a gradativamente em cada `FixedUpdate` (simulando gravidade própria em ambiente isométrico ou pseudo-3D dependendo da câmera).
- **Queda (Fall):** Acionado automaticamente quando o jogador atinge o ápice de um salto ou caminha para fora de uma beirada, aplicando a gravidade de descida e isolando a animação de queda da de pulo.
- **Dash Direcional:** Uma esquiva rápida para o lado em que o jogador aponta ou se movimenta, controlada via transição de estado na `StateMachine`.
- **Controle de Rosto (Facing Direction):** Identifica para qual lado o jogador está virado e permite o travamento dessa direção (útil quando atacando).

## Como Usar

1. O GameObject precisa conter os componentes `StateMachine`, `Rigidbody` e `PlayerMovement`.
2. A classe `PlayerMovement` implementa `IPlayerMovementContext` e `IEntityLocomotion`, que servem para expor dados da locomoção de forma encapsulada aos *States* ou a outros sistemas.
3. No Unity Editor, ajuste os *Speed*, *Gravity*, *Jump Force*, e *Dash Duration*.

## Fluxo de Comunicação e Arquitetura

O sistema atua como **Consumidor** primário das intenções de movimento.

### De onde recebe informação?
- **Core (EventBus):** Escuta mensagens como `PlayerMoveMessage`, `PlayerJumpMessage`, `PlayerDashMessage` (lançadas pelo `PlayerInputHandler`) e `EndCombatMessage` (lançada pelo `PlayerCombat` ou `PlayerAnimationHandler` no final do ataque).

### O que faz com a informação?
- Armazena as intenções em *flags* internas ou guarda a direção (`_currentMoveInput`, `_jumpRequested`, `_dashRequested`).
- A `StateMachine` que gerencia este módulo itera pelos estados (`PlayerIdleState`, `PlayerWalkState`, `PlayerFallState`, etc.), lendo essas variáveis através da interface de contexto `IPlayerMovementContext`.
- Dependendo do estado, altera transformações ou velocidades físicas.

### Para onde envia informação?
- Ele não envia muitas informações diretas via `EventBus`. Sua comunicação de saída acontece, principalmente, provendo variáveis acessíveis através de interfaces como `IEntityLocomotion` (consumidas, por exemplo, pela animação para virar o sprite) e pelas trocas de estado (A `StateMachine` dispara o evento `OnStateChanged`, alertando a animação para mudar os clipes).

---

- **EventBus** ➔ *Move/Jump/Dash Messages* ➔ **PlayerMovement**
- **EventBus** ➔ *EndCombatMessage* ➔ **PlayerMovement**
- **PlayerMovement** ➔ *Lê Contexto* ➔ **States (Idle, Walk, Jump, Fall, Dash)**
- **States** ➔ *Atualiza Física* ➔ **Rigidbody**
- **PlayerMovement** ➔ *Implementa* ➔ **IEntityLocomotion**
- **IEntityLocomotion** ➔ *É lida por* ➔ **PlayerAnimationHandler**
