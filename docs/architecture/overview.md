# Visão geral da arquitetura (estado atual)

> Descreve o código **singleplayer** que existe hoje. Para a arquitetura-alvo com rede, veja [multiplayer/overview.md](../multiplayer/overview.md).

## Resumo

- **Gênero:** beat 'em up 2.5D. O mundo é 3D: **X** é horizontal, **Z** é profundidade, **Y** é altura (pulo). Os personagens são sprites 2D.
- **Engine:** Unity 6000.3.9f1, URP 17.3.
- **Pacotes relevantes:** Input System 1.18, Unity Behavior 1.0.16 (IA), Cinemachine 3.1.7 (instalado, ainda não usado no código), uGUI, Unity Pipeline 0.8 (automação do Editor por CLI).
- **Todo o código do jogo** fica em `Assets/_Project/Scripts`, dividido em assemblies (`.asmdef`).

## Assemblies e dependências

```
                           GyeNyame.Core
        ┌─────────┬───────────┼──────────┬─────────┬──────────────┐
    Physics     Combat       AI         UI     Player.Input    Core.Editor
                  │     (+Unity.Behavior)     (+InputSystem,
                  │                            +GyeNyame.InputSystem)
              Entities (+Combat, +Unity.Behavior)
       ┌──────────┼──────────────┐
Player.Movement  Enemy   Player.Combat (+Combat, +Entities, +Player.Movement)
```

| Assembly | Pasta | Referencia |
|---|---|---|
| `GyeNyame.Core` | `Scripts/Core` | nada do jogo |
| `GyeNyame.Core.Editor` | `Scripts/Core/Editor` | Core (só Editor) |
| `GyeNyame.Physics` | `Scripts/Physics` | Core |
| `GyeNyame.Combat` | `Scripts/Combat` | Core |
| `GyeNyame.Entities` | `Scripts/Entities` | Core, Combat, Unity.Behavior |
| `GyeNyame.Player.Input` | `Scripts/Player/Input` | Core, Unity.InputSystem, GyeNyame.InputSystem |
| `GyeNyame.Player.Movement` | `Scripts/Player/Movement` | Core, Entities, Unity.InputSystem |
| `GyeNyame.Player.Combat` | `Scripts/Player/Combat` | Core, Combat, Entities, Player.Movement |
| `GyeNyame.Enemy` | `Scripts/Enemy` | Core, Combat, Entities |
| `GyeNyame.AI` | `Scripts/AI` | Core, Unity.Behavior, Unity.Properties |
| `GyeNyame.UI` | `Scripts/UI` | Core |
| `GyeNyame.InputSystem` | `UnityInputSystem` | Unity.InputSystem (classe gerada `PlayerInputActions`) |

**Regras de dependência:**

1. `Core` não referencia nenhum assembly do jogo. Tudo que precisa ser compartilhado vira **interface** em `Core/Contracts/Interfaces` ou **mensagem** em `Core/Contracts/Messages`.
2. `Physics`, `AI` e `UI` só conhecem `Core`. Eles falam com entidades por interfaces (`IKinematicPhysics`, `IAttackDirector`, `IEntityHealth`…).
3. Não crie referência circular. Se dois módulos precisam conversar e um não pode referenciar o outro, a interface ou a mensagem vai para `Core/Contracts`.

Há referências sobrando e um acoplamento indevido (`Entities` → `Unity.Behavior`). Veja [known-issues.md](known-issues.md).

## Padrões de comunicação

| Mecanismo | Onde | Quando usar |
|---|---|---|
| **EventBus** (pub/sub estático e tipado) | `Core/Events/EventBus.cs` | Avisar "algo aconteceu" para quem quiser ouvir, sem conhecer os ouvintes: input, dano, morte, fim de jogo, pedidos de efeito |
| **Eventos C#** | `StateMachine.OnStateChanged`, `BaseState.OnStateExit`, `EntityHealth.OnDamageTakenEvent` | Reação dentro da **mesma entidade** (animação reage ao estado; vida adia a morte) |
| **UnityEvents de animação** | `Combat/Animation/CombatAnimationEventHandler.cs` | Eventos colocados nos clipes de animação chamam `OpenHitbox`, `CloseHitbox`, `FinishAttack`, `OpenCancelWindow`. A ligação é feita no Inspector |
| **Interfaces + `GetComponent`** | Em todos os módulos | Um componente encontra os serviços da **própria entidade** (locomoção, vida, visuais) |
| **ScriptableObjects de dados** | `Combat/Data/AttackDataSO.cs` | Dados de design: ataques e **árvore de combo** |

**Convenção do EventBus:** todo `MonoBehaviour` que assina faz `Subscribe` em `OnEnable` e `Unsubscribe` em `OnDisable`. Como toda mensagem vai para todos os ouvintes, quem assina **filtra pelo alvo** (`message.Target != meuRoot → return`).

## Anatomia de uma entidade

Toda entidade (player ou inimigo) tem **uma única `StateMachine`** no GameObject raiz. Cada componente **registra os próprios estados** nela no `Awake`, e o movimento escolhe o estado inicial no `Start`.

```
Player (raiz, layer Player, tag Player)            DummyDoll (raiz, layer Enemy, tag Enemy)
├ BoxCollider + Rigidbody (cinemático)             ├ BoxCollider + Rigidbody
├ StateMachine  ◄── compartilhada ──────────────►  ├ StateMachine
├ KinematicPhysics                                 ├ KinematicPhysics
├ PlayerMovement → Idle, Walk, Jump, Fall, Dash    ├ EnemyMovement → Idle, Walk, Fall
├ PlayerCombat   → GenericEntityAttackState        ├ EnemyCombat   → GenericEntityAttackState
├ InputBuffer, PlayerInputHandler                  ├ DummyDoll (BaseEnemy) → Hurt, Dead
├ StateMachineDebugger                             ├ BehaviorGraphAgent (BasicEnemyBG)
│                                                  ├ StateMachineDebugger, AINodesDebugger
├ Visuals (filho)                                  ├ Visuals (filho): idem ao Player
│  ├ SpriteRenderer, Animator                      ├ Hurtbox (filho, layer EnemyHurtbox)
│  ├ EntityAnimationHandler, EntityVisuals         │  └ EntityHealth, HurtboxComponent
│  ├ CombatAnimationEventHandler                   └ Visuals/AttackHitboxes/AttackLight1
│  ├ DamageFlashFeedback                              (layer EnemyHitbox)
│  └ AttackHitboxes (filho)
│     └ AttackLight1, AttackLight2, AttackHeavy
│        (layer PlayerHitbox, HitboxComponent)
└ HurtBox (filho, layer PlayerHurtbox)
   └ PlayerHealth → Hurt, Dead; HurtboxComponent
```

Pontos importantes:

- **A vida fica no filho `Hurtbox`**, junto com o `HurtboxComponent`. `EntityHealth.GetEntityRoot()` devolve o GameObject da `StateMachine` (a raiz), e é esse GameObject que vai nas mensagens `EntityDamagedMessage` e `EntityDeadMessage`.
- **A animação é dirigida pelo estado.** Cada estado declara uma `EntityStateCategory`. O `EntityAnimationHandler` escuta `OnStateChanged` e chama `Animator.Play(hash)` com o nome da categoria. Não há parâmetros nem transições no Animator.
- **O sprite vira pelo `localRotation` Y (0° ou 180°)** do filho `Visuals`, a partir de `IEntityLocomotion.FacingDirectionX`. As hitboxes, por serem filhas de `Visuals`, viram junto.

## Fluxos principais

### Combate do jogador

```
Teclado/controle
 → PlayerInputHandler publica PlayerAttackLightMessage
 → PlayerCombat grava no InputBuffer (janela = AttackDataSO.BufferTime)
 → PlayerCombat.Update (só em Idle ou Walk): consome o buffer e escolhe o ataque
     (início do combo ou próximo nó de NextLightCombo/NextHeavyCombo)
 → StateMachine vai para GenericEntityAttackState (a animação vem de AttackDataSO.AnimationCategory)
 → evento de animação OpenHitbox → HitboxComponent ativa o collider
 → OnTriggerEnter → IHurtbox.ReceiveHit → IDamageable.TakeDamage (EntityHealth)
 → EventBus: EntityDamagedMessage
      ├ TimeManager: hitstop (Time.timeScale = 0 por HitStopTime)
      ├ DamageFlashFeedback: troca o material por um instante
      ├ BaseEnemy/PlayerHealth: estado Hurt com knockback
      └ Presenters de UI: atualizam barras de vida
 → evento de animação FinishAttack → EndCombatMessage → Movement volta para Idle/Walk
```

### Morte

- **Inimigo:** `EntityHealth` publica `EntityDeadMessage`. O `EntityHurtState` espera o knockback terminar e vai para `EntityDeadState`: troca a layer da hurtbox para `Dead`, desliga a física e o `BehaviorGraphAgent`, espera o fim da animação, faz fade e chama `SetActive(false)`.
- **Player:** mesma base (`PlayerDeadState`), mais `PlayerDiedMessage` (o input passa para o mapa de UI, o diretor para de dar tokens, os inimigos voltam para Idle), câmera lenta e, ao fim do fade, `GameOverMessage`.

### Fim de partida

- `LevelEnemyTracker` conta os `IEnemyCombat` da cena no `Start` e publica `AllEnemiesDefeatedMessage` quando o último morre.
- `GameResetHandler` recebe `GameOverMessage` ou `AllEnemiesDefeatedMessage`, espera `resetDelay` segundos (tempo real), limpa o EventBus e recarrega a cena.

## Onde continuar

- Detalhes de cada módulo: arquivos desta pasta.
- Como isso muda com o multiplayer: [multiplayer/migration-map.md](../multiplayer/migration-map.md).
