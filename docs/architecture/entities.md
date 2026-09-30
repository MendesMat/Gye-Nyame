# Entidades (`GyeNyame.Entities`)

Pasta: `Assets/_Project/Scripts/Entities`. Contém tudo que é **comum a player e inimigos**: movimento base, estados genéricos, animação e visuais. Player e Enemy herdam ou reaproveitam o que está aqui.

| Arquivo | Papel |
|---|---|
| `Movement/BaseEntityMovement.cs` | Movimento base, gravidade, knockback. Implementa `IEntityLocomotion` |
| `Movement/States/Entity{Idle,Walk,Jump,Fall,Dash}State.cs` | Estados de locomoção |
| `Combat/States/BaseEntityAttackState.cs` | Base de todo ataque |
| `Combat/States/GenericEntityAttackState.cs` | Ataque dirigido por `AttackDataSO` |
| `Combat/States/EntityHurtState.cs` | Reação a dano com knockback |
| `Combat/States/EntityDeadState.cs` | Morte, fade e desativação |
| `Animation/EntityAnimationHandler.cs` | Toca a animação do estado e vira o sprite |
| `Visuals/EntityVisuals.cs` | Alfa do sprite (fade). Implementa `IEntityVisuals` |

## BaseEntityMovement

Classe abstrata, `[RequireComponent(typeof(Rigidbody))]`. Guarda o estado de locomoção e expõe tudo via `IEntityLocomotion`.

**Estado interno:** `currentMoveInput` (Vector2: x = horizontal, y = profundidade), `facingDirectionX` (±1), `isGrounded`, `verticalVelocity`, `externalForceVelocity` (knockback), `isFacingDirectionLocked`.

**`UpdateMovement(speedMultiplier, lockDepth)`**, chamado pelos estados no `FixedUpdate`:

1. Movimento pretendido = `(input.x, 0, input.y × depthSpeedMultiplier) × speedMultiplier × speed × fixedDeltaTime`. Com `lockDepth`, o Z é zerado.
2. Soma o knockback (`externalForceVelocity`), que desacelera por `knockbackDeceleration` (10× mais devagar no ar).
3. Corta o movimento com `IKinematicPhysics.CalculateAllowedMovement`.
4. Aplica gravidade em `verticalVelocity` e checa o chão (`ApplyVerticalMovement`). Ao pousar, chama `OnLanded()`.
5. `Rigidbody.MovePosition(destino)`.

**Direção do rosto:** `UpdateFacingDirection` atualiza `facingDirectionX` a partir do input, a não ser que esteja travada (`SetFacingDirectionLock(true)`, usado em ataques, hurt, morte e pela IA). `ForceFacingDirectionX` força um lado.

**Knockback:** `ApplyExternalForce(direção, força, velocidadeVertical)` define `externalForceVelocity` e tira a entidade do chão.

**Pontos de extensão (virtuais):** `ConsumeJumpRequest`, `ConsumeDashRequest`, `ExecuteJump`, `ExecuteDash`, `UpdateDirectionalMovement`, `AirSpeedMultiplier`, `LockDepthDuringJump`, `DashSpeedMultiplier`, `DashDuration`, `OnLanded`. A base devolve "não faz nada"; `PlayerMovement` implementa pulo e dash.

## Estados de locomoção

Todos recebem `IEntityLocomotion` pelo construtor e fazem o trabalho no `FixedUpdate`.

```
            ┌──── sem chão ────► Fall ── pousou ──► Idle/Walk
Idle ◄──────┤
  ▲ │ input │──── dash ────────► Dash ── acabou ──► Walk (ver bug)
  │ ▼       │──── pulo ────────► Jump ── vel. ≤ 0 ─► Fall
Walk ───────┘
```

- **Idle** (`UpdateMovement(0)`) e **Walk** (`UpdateMovement(1)`) checam, nessa ordem: sem chão → Fall; `ConsumeDashRequest` → Dash; `ConsumeJumpRequest` → Jump; input → Walk / sem input → Idle.
- **Jump**: chama `ExecuteJump` no `Enter`; move com `AirSpeedMultiplier` e `LockDepthDuringJump`; descarta pedidos de dash no ar; vai para Fall quando a velocidade vertical fica ≤ 0.
- **Fall**: igual ao Jump sem o impulso; ao pousar vai para Walk ou Idle.
- **Dash**: direção = input normalizado ou, sem input, o lado do rosto; move por `DashDuration` com `UpdateDirectionalMovement`. **Bug conhecido** na saída do dash: [known-issues.md](known-issues.md).

Os métodos `TransitionTo*` são `protected virtual` para que subclasses redirecionem transições.

## Estados de combate

### BaseEntityAttackState / GenericEntityAttackState

- `Enter`: marca o tempo, **trava o rosto** e chama `IEntityCombatContext.ResetCombatState()`.
- `FixedUpdate`: `UpdateMovement(0)` (fica parado, mas o knockback e a gravidade continuam).
- `OnAnimationFinish()`: chamado pelo dono do combate quando o clipe termina. Ignora chamadas nos primeiros 0,1 s (proteção contra evento do clipe anterior) e publica `EndCombatMessage`, que faz o movimento voltar a Idle/Walk.
- `AllowInterrupt` (padrão `true`): se dash/pulo podem cancelar o ataque durante a janela de cancelamento.
- `GenericEntityAttackState.StateCategory` vem de `combatContext.CurrentAttackData.AnimationCategory`. **Um único estado serve para todos os ataques**; o que muda é o `AttackDataSO` corrente.

### EntityHurtState

- `InitializeHurt(damage, posição)` deve ser chamado **antes** de `ChangeState`.
- `Enter`: trava o rosto e aplica knockback na direção `(posição − origem do golpe)` achatada no Y, com `KnockbackForce` e `KnockupForce`.
- Duração = `HitStopTime` do golpe. Se houve knockup, espera também pousar.
- Se a entidade morreu: espera mais 0,5 s (`ComboWaitDuration`, para o combo do atacante "terminar" no corpo) e vai para Dead. Senão, vai para Idle.

### EntityDeadState

- `Enter`: trava o rosto, desliga a física (`isKinematic`), troca a layer da hurtbox para `Dead` (para de receber golpes), **desliga o `BehaviorGraphAgent`** se houver, e assina `AnimationFinishDeathMessage`.
- Quando a animação de morte termina (evento no clipe → `EntityAnimationHandler.OnDeathAnimationFinish`), inicia o fade de alfa por `IEntityVisuals.FadeDuration`.
- `FinishDeath()` (virtual): `SetActive(false)` na raiz. `PlayerDeadState` sobrescreve para publicar `GameOverMessage` antes.
- `Exit` desfaz a layer e o alfa (permite reviver no futuro).

## EntityAnimationHandler

`[RequireComponent(Animator, SpriteRenderer)]`, fica no filho `Visuals`.

- Encontra `IStateMachine` e `IEntityLocomotion` com `GetComponentInParent`.
- Mapeia cada `EntityStateCategory` para `Animator.StringToHash(nome)`. Em cada `OnStateChanged`, chama `animator.Play(hash, -1, 0)`. Estados sem categoria podem ser tratados em `HandleCustomAnimation` (virtual).
- No `Update`, vira o `Visuals` (rotação Y 0° ou 180°) conforme `FacingDirectionX`.
- `OnDeathAnimationFinish()` é chamado por **evento no clipe `Dead`** e publica `AnimationFinishDeathMessage` com a raiz.

## EntityVisuals

`SetAlpha(alpha)` e `ResetVisuals()` sobre o `SpriteRenderer`. `fadeDuration` configura o tempo de fade na morte (valor atual: 1 s).
