# Jogador (`GyeNyame.Player.*`)

Pasta: `Assets/_Project/Scripts/Player`. Três assemblies:

| Assembly | Arquivos | Papel |
|---|---|---|
| `GyeNyame.Player.Input` | `PlayerInputHandler.cs` | Traduz o hardware em mensagens de intenção |
| `GyeNyame.Player.Movement` | `PlayerMovement.cs`, `IPlayerMovementContext.cs` | Locomoção: andar, pular, dash |
| `GyeNyame.Player.Combat` | `PlayerCombat.cs`, `PlayerHealth.cs`, `States/PlayerHurtState.cs`, `States/PlayerDeadState.cs` | Combos, dano e morte do jogador |

## Input

`PlayerInputHandler` instancia `PlayerInputActions` (classe gerada a partir de `Assets/_Project/UnityInputSystem/InputSystem_Actions.inputactions`) e publica no EventBus:

| Ação | Teclado/mouse | Controle | Mensagem |
|---|---|---|---|
| Move | WASD / setas | analógico esquerdo, D-pad | `PlayerMoveMessage(Vector2)` (e `Vector2.zero` ao soltar) |
| Jump | Espaço | Sul (A/✕) | `PlayerJumpMessage` |
| Dash | Shift esquerdo | Gatilho direito | `PlayerDashMessage` |
| AttackLight | Botão esquerdo do mouse | Oeste (X/□) | `PlayerAttackLightMessage` |
| AttackHeavy | Botão direito do mouse | Norte (Y/△) | `PlayerAttackHeavyMessage` |
| Interact | E | Ombro direito | **não usado** |

- O mapa `UI` (Navigate, Point, ScrollWheel, Click) existe. `SwitchToUIMode`/`SwitchToPlayerMode` alternam os mapas. Ao receber `PlayerDiedMessage`, passa para UI.
- **As mensagens não dizem de qual jogador vieram.** Isso precisa mudar para o coop: [ADR 0007](../decisions/0007-eventbus-local-e-identidade.md).

## Movimento

`PlayerMovement : BaseEntityMovement, IPlayerMovementContext`.

- No `Awake`, registra `EntityIdleState`, `EntityWalkState`, `EntityJumpState`, `EntityFallState` e `EntityDashState` na `StateMachine`. No `Start`, vai para Idle.
- Assina `PlayerMoveMessage` (atualiza `currentMoveInput` e o rosto), `PlayerJumpMessage` e `PlayerDashMessage` (marcam pedidos) e `EndCombatMessage` (volta para Walk/Idle quando o ataque termina).
- **Pulo:** `ConsumeJumpRequest` só permite pular `jumpCooldown` segundos depois de pousar. `ExecuteJump` define `verticalVelocity = jumpForce`.
- **Dash:** `ConsumeDashRequest` respeita `dashCooldown`. `UpdateDirectionalMovement` move na direção do dash **sem** somar knockback.
- O **pedido** é marcado pelo evento, e o **estado** decide quando consumi-lo. Isso mantém a lógica de transição dentro dos estados.

Valores atuais em [design/tuning.md](../design/tuning.md).

## Combate

`PlayerCombat : MonoBehaviour, IPlayerCombatContext` (`IPlayerCombatContext` só estende `IEntityCombatContext`).

**Fluxo de um combo:**

1. `PlayerAttackLightMessage` ou `PlayerAttackHeavyMessage` → guarda no `InputBuffer` com o `BufferTime` do **próximo** ataque possível.
2. No `Update`, se o estado atual é Idle ou Walk:
   - se passou `ComboWindowTime` desde o fim do último ataque (e a janela de cancelamento está fechada), o combo **zera**;
   - se há comando no buffer, consome. Sem combo em andamento, começa pelo `lightAttackStarter`/`heavyAttackStarter` (respeitando o cooldown). Com combo, avança para `NextLightCombo`/`NextHeavyCombo` (se for `null`, não faz nada).
   - troca para `GenericEntityAttackState`.
3. Eventos de animação chamam `OpenHitbox`/`CloseHitbox` (liga a hitbox cujo `BoundAttackData` é o ataque atual), `OpenCancelWindow` e `FinishAttack`.
4. `FinishAttack` registra `_lastAttackTime` e chama `attackState.OnAnimationFinish()`, que publica `EndCombatMessage`.
5. Ao sair de qualquer estado de ataque, **todas** as hitboxes são desligadas por segurança.

**Detalhes que importam:**

- **O combo encadeia entre ataques, não durante.** O próximo golpe só sai quando o ataque atual termina e o estado volta a Idle/Walk. O buffer é o que dá a sensação de fluidez.
- **Andar zera o combo** (`HasMoveInput` fora de ataque).
- **Cancelamento:** dash ou pulo durante a janela de cancelamento aberta encerram o ataque (`EndCombatMessage`). Fora da janela, só zeram o combo.
- O mapa `AttackDataSO → IHitbox` é montado no `Awake` a partir dos `HitboxComponent` filhos.

## Vida, dano e morte

`PlayerHealth : EntityHealth`. No `Awake`, registra `PlayerHurtState` e `PlayerDeadState`.

- **Dano fora de ataque:** vai para `PlayerHurtState` com knockback.
- **Dano durante um ataque:** o ataque **não é interrompido** (super armor). Se o golpe for fatal, a morte é **adiada** até o estado de ataque sair (`DeferDeath` + `OnStateExit`).
- **Rede de segurança:** se estiver morto e a máquina entrar em qualquer estado que não seja Dead, Hurt ou ataque, força `PlayerDeadState`. Isso corrigiu o bug do "player imortal ao atacar enquanto morre".
- `PlayerHurtState` limpa o `InputBuffer` ao entrar.
- `PlayerDeadState`: publica `PlayerDiedMessage`, pede câmera lenta (0,2× por 2 s), faz o fade e, em `FinishDeath`, publica `GameOverMessage`.
