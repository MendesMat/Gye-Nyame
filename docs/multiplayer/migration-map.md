# Mapa de migração: o que muda em cada módulo

> Liga o código atual ([architecture/](../architecture/overview.md)) à arquitetura-alvo. Serve de base para quebrar as milestones em issues. **Não define a ordem** das issues; a ordem por dependência fica nas milestones do GitHub.

Legenda: ✅ mantém · 🔶 adapta · 🔴 reescreve · ➕ novo

## Pré-requisitos (independentes da rede)

| Item | Mudança |
|---|---|
| ➕ Prefabs | Player, Dummy, HUD viram prefabs. Os 4 Dummies da cena viram instâncias do prefab |
| ➕ Testes | Assemblies de teste EditMode e PlayMode |
| ➕ Smart Merge | `unity vcs merge-setup` |
| 🔶 Limpeza | Remover `Assets/_Recovery`, referências de assembly sobrando, código morto de [known-issues.md](../architecture/known-issues.md) |
| ➕ ScriptableObjects de configuração | Mover valores de componentes para SOs ([0015](../decisions/0015-valores-de-design-em-scriptableobjects.md)) |

## Core

| Arquivo | Status | Mudança |
|---|---|---|
| `Events/EventBus.cs` | 🔶 | Continua local. Corrigir `Clear<T>` e o reset sem domain reload |
| `Contracts/Messages/MovementMessages.cs`, `CombatMessages.cs` | 🔶 | Mensagens de input ganham o índice do jogador local ([0007](../decisions/0007-eventbus-local-e-identidade.md)) |
| `Contracts/Messages/PlayerDiedMessage.cs`, `GameOverMessage.cs` | 🔶 | `PlayerDiedMessage` passa a identificar o jogador; game over passa a ser decidido pelo sistema de vidas |
| `Contracts/Messages/SlowMotionRequestMessage.cs` | 🔴 | Vira efeito visual local de game over |
| `Contracts/Interfaces/IAttackDirector.cs` | 🔶 | Por alvo ([combat-director.md](combat-director.md)) |
| `Contracts/Interfaces/` | ➕ | `IEntityAuthority` e contratos de sessão/partida necessários à gameplay |
| `StateMachine/*` | ✅ | Sem mudança estrutural. Pode ganhar suporte ao relógio de hitstop por entidade |
| `InputBuffer/*` | 🔶 | Usar o relógio da entidade em vez de `Time.time` |
| `TimeManagement/TimeManager.cs` | 🔴 | Removido; substituído por hitstop por entidade ([0008](../decisions/0008-hitstop-local-por-entidade.md)) |
| `GameFlow/GameResetHandler.cs` | 🔴 | Substituído pelo fluxo Fase → Sala/Menu |
| `GameFlow/LevelEnemyTracker.cs` | 🔶 | Roda só no host; conta inimigos criados pelo spawn |

## Physics

| Arquivo | Status | Mudança |
|---|---|---|
| `KinematicPhysics.cs` | ✅ | Roda só onde a entidade é simulada (dono ou host) |

## Entities

| Arquivo | Status | Mudança |
|---|---|---|
| `Movement/BaseEntityMovement.cs` | 🔶 | Simula só com autoridade; réplica só recebe posição |
| `Movement/States/*` | 🔶 | Corrigir bug do dash (B1). Sem mudança de rede: só rodam com autoridade |
| `Combat/States/EntityHurtState.cs` | 🔶 | Knockback aplicado por quem tem autoridade |
| `Combat/States/EntityDeadState.cs` | 🔶 | Parar de desligar o `BehaviorGraphAgent` direto (A1); `FinishDeath` não chama `SetActive(false)` em objetos de rede, e sim despawn pelo host |
| `Animation/EntityAnimationHandler.cs` | 🔶 | Tocar animação também a partir do estado sincronizado (réplica) |
| `Visuals/EntityVisuals.cs` | ✅ | — |

## Combat

| Arquivo | Status | Mudança |
|---|---|---|
| `Components/HitboxComponent.cs` | 🔶 | Aplica a regra de filtragem ([authority.md](authority.md#regra-de-filtragem)); golpe em inimigo vira pedido ao host; identificador de golpe para não duplicar |
| `Components/HurtboxComponent.cs` | ✅ | — |
| `Components/EntityHealth.cs` | 🔶 | Vida sincronizada (dono do player ou host); morte do player avisa o sistema de vidas |
| `Components/CombatDirector.cs` | 🔴 | Tokens por alvo, estratégia de alvo, prazo, slots, só no host ([0009](../decisions/0009-diretor-de-combate-com-tokens-por-alvo.md)) |
| `Data/AttackDataSO.cs` | ✅ | Pode ganhar um identificador estável para ir em RPC |
| `Visuals/DamageFlashFeedback.cs` | 🔶 | Disparado pelo evento de golpe local, com o relógio da entidade |
| `Animation/CombatAnimationEventHandler.cs` | ✅ | — |

## Player

| Arquivo | Status | Mudança |
|---|---|---|
| `Input/PlayerInputHandler.cs` | 🔶 | Só existe/escuta no personagem do jogador local; mensagens com índice do jogador |
| `Movement/PlayerMovement.cs` | 🔶 | Ignora input que não é do dono; valores vão para a ficha do personagem |
| `Combat/PlayerCombat.cs` | 🔶 | Idem; `CloseCancelWindow` (B4) |
| `Combat/PlayerHealth.cs` | 🔶 | Morte avisa o host; renascimento com invencibilidade |
| `Combat/States/PlayerDeadState.cs` | 🔴 | Sem câmera lenta, sem game over direto; aguarda renascer ou fim de partida |
| `Combat/States/PlayerHurtState.cs` | ✅ | — |
| ➕ `CharacterDefinition` | ➕ | Ficha do personagem ([game-rules.md](game-rules.md#jogadores-e-personagens)) |

## Enemy

| Arquivo | Status | Mudança |
|---|---|---|
| `BaseEnemy.cs` | 🔶 | Reage a dano só no host; aplica multiplicadores de dificuldade |
| `Movement/EnemyMovement.cs` | 🔶 | Simula só no host |
| `Combat/EnemyCombat.cs` | 🔶 | Diretor injetado pelo spawn, sem `FindAnyObjectByType` (A4) |

## AI

| Arquivo | Status | Mudança |
|---|---|---|
| `Nodes/*` | 🔶 | `GetPlayerTransform()` → `GetTarget(agent)` |
| `BasicEnemyBG.asset` | 🔶 | Resolver `IsStunned` (B2); IA desligada no convidado |
| `Debugging/AINodesDebugger.cs` | ✅ | — |

## UI

| Arquivo | Status | Mudança |
|---|---|---|
| `Views/UIHealthBar.cs` | ✅ | — |
| `Presenters/PlayerHealthPresenter.cs` | 🔴 | Dois painéis, por jogador da sessão, sem tag |
| `Presenters/EnemyHealthPresenter.cs` | 🔶 | Último inimigo acertado **pelo jogador local** |
| ➕ Menus, sala, pausa, vidas, renascimento, aviso de desconexão | ➕ | [session-flow.md](session-flow.md) |

## Novos

| Item | Onde |
|---|---|
| `GyeNyame.Networking` (adaptadores, spawn, sessão) | `Scripts/Networking` |
| Sistema de vidas e renascimento | host |
| Sistema de dificuldade | host |
| Câmera compartilhada (Cinemachine) | local |
| Cenas `MainMenu`, `Room`, `Level_01` | `Assets/Scenes` |
