# Inimigos (`GyeNyame.Enemy`)

Pasta: `Assets/_Project/Scripts/Enemy`. É o **corpo** do inimigo: vida, movimento, ataque e estados. As **decisões** ficam na IA ([ai.md](ai.md)). A IA só envia intenções (`SetMovementIntent`, `TryAttack`) e a máquina de estados do inimigo executa.

| Arquivo | Papel |
|---|---|
| `BaseEnemy.cs` | Base abstrata: registra estados genéricos e reage a dano |
| `Dummy/DummyDoll.cs` | Único inimigo concreto hoje (sem lógica própria) |
| `Movement/EnemyMovement.cs` | Locomoção dirigida pela IA. Implementa `IEnemyMovement` |
| `Combat/EnemyCombat.cs` | Ataque único por `AttackDataSO`. Implementa `IEnemyCombat`, `IEntityCombatContext` |

## BaseEnemy

- `Awake`: encontra `IEntityLocomotion` (raiz), `IEntityHealth` e `IEntityVisuals` (filhos). Loga erro se faltar algum. Registra Idle, Walk, Fall, Hurt e Dead.
- `EntityDamagedMessage` para si → `EntityHurtState` com knockback, **a não ser que já esteja em Dead**. Inimigos não têm super armor: um golpe interrompe o ataque.
- `PlayerDiedMessage` → volta para Idle.

Para criar um inimigo novo: herde de `BaseEnemy` e sobrescreva `ApplyKnockback` ou o `Awake` se precisar de estados extras.

## EnemyMovement

`EnemyMovement : BaseEntityMovement, IEnemyMovement`.

- Registra Idle, Walk e Fall (os de `BaseEnemy` se sobrepõem; a última fábrica registrada vale).
- `SetMovementIntent(Vector2)`: a IA define a direção (x = horizontal, y = profundidade). O rosto acompanha, a não ser que esteja travado.
- No `Update`, alterna entre Idle e Walk conforme há intenção de movimento (enquanto vivo).
- `EndCombatMessage` para si → volta a Idle/Walk.
- Não implementa pulo nem dash.

## EnemyCombat

- Um único `attackData` (hoje `BasicEnemyAttack`). Registra `GenericEntityAttackState`.
- Se não tiver `combatDirector` no Inspector, procura com `FindAnyObjectByType`. Registra-se no diretor em `OnEnable` e sai em `OnDisable`.
- `TryAttack()`: ataca se vivo, fora de cooldown e não atacando.
- `FinishAttack()` (evento de animação): inicia o cooldown (`attackData.CooldownTime`) e encerra o ataque.
- `IsAttacking` e `IsInCooldown` são lidos pela IA e pelo diretor.
- `OpenHitbox`/`CloseHitbox`: ligam a hitbox associada ao `attackData`.

## Configuração atual (Dummy)

| Onde | Campo | Valor |
|---|---|---|
| EntityHealth (filho Hurtbox) | `maxHealth` | 4 |
| EnemyMovement | `speed` / `depthSpeedMultiplier` / `gravity` / `knockbackDeceleration` | 3 / 2,5 / 25 / 15 |
| BasicEnemyAttack.asset | dano / cooldown / hitstop | 1 / 2 s / 0,1 s |

Os 4 `DummyDoll` da cena são **cópias soltas**, não um prefab. Veja [known-issues.md](known-issues.md).
