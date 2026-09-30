# Combate (`GyeNyame.Combat`)

Pasta: `Assets/_Project/Scripts/Combat`. Regras **universais** de combate: detectar golpes, aplicar dano, dados de ataque e coordenação dos atacantes. O que é exclusivo do jogador fica em [player.md](player.md); o dos inimigos, em [enemy.md](enemy.md).

| Arquivo | Papel |
|---|---|
| `Components/HitboxComponent.cs` | Collider de ataque. Implementa `IHitbox` |
| `Components/HurtboxComponent.cs` | Collider que recebe golpes. Implementa `IHurtbox` |
| `Components/EntityHealth.cs` | Vida e morte. Implementa `IDamageable`, `IEntityHealth` |
| `Components/CombatDirector.cs` | Tokens de ataque e slots dos inimigos. Implementa `IAttackDirector` |
| `Data/AttackDataSO.cs` | Dados de um ataque e árvore de combo |
| `Animation/CombatAnimationEventHandler.cs` | Ponte entre eventos de animação e o componente de combate |
| `Visuals/DamageFlashFeedback.cs` | Pisca o sprite ao tomar dano |
| `Contracts/Interfaces/IEntityCombatContext.cs` | Contexto que os estados de ataque leem |

## Hitbox e hurtbox

- **`HitboxComponent`**: fica num filho de `Visuals/AttackHitboxes`, **um por ataque**, com `BoxCollider` trigger desligado por padrão. Tem um `AttackDataSO` associado (`BoundAttackData`). `EnableHitbox`/`DisableHitbox` ligam o collider. Em `OnTriggerEnter` com algo que tenha `IHurtbox`, monta um `DamageData` a partir do SO, com origem na posição da raiz do atacante, e chama `ReceiveHit`.
- **`HurtboxComponent`**: fica no filho `Hurtbox`, junto do `EntityHealth`. Repassa `ReceiveHit` para `IDamageable.TakeDamage`.
- **Quem acerta quem é decidido pelas layers**, não pelo código: `PlayerHitbox` só colide com `EnemyHurtbox`, e `EnemyHitbox` só com `PlayerHurtbox`. Veja [scene-and-assets.md](scene-and-assets.md#layers-e-matriz-de-colisão).
- **Não há deduplicação de acerto:** um mesmo swing acerta cada hurtbox uma vez por entrada no trigger. Como a hitbox fica ligada só alguns frames, isso basta hoje.

## EntityHealth

- `maxHealth` no Inspector (Player 6, Dummy 4). `currentHealth` começa cheio no `Awake`.
- `TakeDamage(data)`: subtrai, publica `EntityDamagedMessage(raiz, data)`, dispara `OnDamageTakenEvent`, chama `OnDamageReceived` (virtual) e verifica morte.
- Morte: publica `EntityDeadMessage(raiz)`. **O dano continua sendo publicado mesmo com a entidade morta** (para o combo terminar no corpo), mas a morte só é publicada uma vez.
- **Morte adiada:** `DeferDeath()` / `ExecuteDeferredDeath()` / `CancelDeathDeferral()`. Usado pelo `PlayerHealth` para não matar o jogador no meio de um ataque (a morte acontece quando o ataque termina).
- `SetDeadLayer(bool)` troca a layer do próprio GameObject (a hurtbox) para `Dead`, que não colide com nada.
- `GetEntityRoot()` devolve o GameObject da `StateMachine`. **É a identidade da entidade nas mensagens.**

## AttackDataSO

`[CreateAssetMenu("GyeNyame/Combat/Attack Data")]`. Um asset por ataque, em `Assets/_Project/ScriptableObjects/<Player|Enemy>/Combat`.

| Campo | Função |
|---|---|
| `damage` | Dano |
| `nextLightCombo` / `nextHeavyCombo` | Próximo ataque se apertar leve/pesado dentro da janela (árvore de combo) |
| `animationCategory` | Qual estado do Animator tocar |
| `bufferTime` | Quanto tempo o input fica guardado no buffer |
| `comboWindowTime` | Tempo, após o fim do ataque, em que o próximo golpe do combo ainda vale. 0 = ataque final |
| `cooldownTime` | Tempo travado antes de começar um novo combo (e cooldown do ataque do inimigo) |
| `knockbackForce` / `knockupForce` | Empurrão horizontal / para cima |
| `hitStopTime` | Duração do hitstop e da reação de hurt |
| `screenShakeMultiplier` | **Não usado** ainda |

Árvore atual do jogador: `AttackLight1` → leve → `AttackLight2`; `AttackLight1` → pesado → `AttackHeavy`. Os valores estão em [design/tuning.md](../design/tuning.md).

## IEntityCombatContext

```csharp
bool IsCancelWindowOpen { get; }
AttackDataSO CurrentAttackData { get; }
void ResetCombatState();
```

Implementado por `PlayerCombat` e `EnemyCombat`. É o que `GenericEntityAttackState` lê para saber qual animação tocar.

## CombatAnimationEventHandler

Fica em `Visuals`, ao lado do `Animator`. Os clipes chamam `OpenHitbox`, `CloseHitbox`, `FinishAttack`, `OpenCancelWindow` e `CloseCancelWindow`, e cada um dispara um `UnityEvent` ligado **no Inspector** ao `PlayerCombat` ou `EnemyCombat` da raiz.

> A sincronização dos golpes depende dos eventos colocados nos clipes. Mudar a animação exige conferir os eventos.

## CombatDirector

Fica em `Managers/GameManager`. Coordena os inimigos para que o combate pareça coreografado (padrão **attack tokens**).

- **Registro:** cada `EnemyCombat` se registra em `OnEnable` e sai em `OnDisable`. Inimigo morto também sai (`EntityDeadMessage`).
- **Tokens:** no máximo `maxSimultaneousAttacks` (atual: 2) inimigos com token. `RequestAttackToken(inimigo)` concede se o inimigo estiver entre os N mais próximos do player (desempate por `InstanceID`), não estiver em cooldown e houver vaga. `IsTokenAvailableFor` faz a mesma checagem sem conceder. `ReleaseAttackToken(id)` devolve.
- **Slots:** `GetAvailablePositionSlot(inimigo)` devolve `player ± slotDistance` no eixo X (lado em que o inimigo está), com um ruído aleatório fixo por inimigo. `slotDistance` atual: 0,75.
- **Alcance:** `IsInAttackRange` = distância X ≤ `slotDistance + 0,5` e distância Z ≤ 0,1.
- **Morte do player:** zera tokens e para de conceder.

Hoje ele conhece **um único player** (`playerTransform` no Inspector). A versão para 2 jogadores está em [multiplayer/combat-director.md](../multiplayer/combat-director.md).

## DamageFlashFeedback

Fica em `Visuals`. Ao receber `EntityDamagedMessage` da própria entidade, troca o material do `SpriteRenderer` por `flashMaterial` (`Art/Materials/DamageTakenFlash.mat`) por `HitStopTime` segundos **em tempo real**.
