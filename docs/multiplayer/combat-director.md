# Diretor de combate para N jogadores

> Arquitetura-alvo. Base: [ADR 0009](../decisions/0009-diretor-de-combate-com-tokens-por-alvo.md). Estado atual: [architecture/combat.md](../architecture/combat.md#combatdirector).

## Objetivo

Manter o combate coreografado (poucos inimigos atacam por vez, os outros rondam) com 1 ou 2 jogadores, distribuindo a pressão entre eles.

## Roda só no host

O diretor é criado pelo host e não existe no convidado. Os nós de IA também só rodam no host. Portanto nada aqui precisa de sincronização.

## Conceitos

| Conceito | Definição |
|---|---|
| **Alvo** | Um player ativo (vivo e conectado). Solo: 1 alvo. Dupla: até 2 |
| **Orçamento por alvo** | Máximo de inimigos com token atacando o mesmo alvo ao mesmo tempo (vem de `DifficultySettings`) |
| **Token** | Permissão de um inimigo para atacar **um alvo específico**, com prazo de validade |
| **Slot** | Posição de ataque ao redor de um alvo, ocupada por no máximo um inimigo |
| **Estratégia de alvo** | Regra plugável que escolhe o alvo de cada inimigo |

## Os 5 ajustes em relação ao atual

### 1. Tokens por alvo

Cada alvo tem o próprio orçamento. Um inimigo pede token **para o seu alvo**. A regra de ranking atual (os N mais próximos do alvo, fora de cooldown) continua, só que calculada por alvo.

### 2. Escolha de alvo plugável

```csharp
public interface ITargetSelectionStrategy
{
    // Devolve o alvo escolhido para o inimigo, ou null se não houver alvo válido.
    Transform SelectTarget(GameObject enemy, IReadOnlyList<Transform> activeTargets, ITargetingState state);
}
```

Primeira implementação (`NearestAvailableTargetStrategy`):
- escolhe o alvo **mais próximo que ainda tenha orçamento livre**;
- se nenhum tiver vaga, escolhe o mais próximo (o inimigo vai rondar esse alvo);
- **mantém o alvo** por um tempo mínimo (`targetStickinessSeconds`, em `DifficultySettings`), a não ser que o alvo deixe de ser válido;
- player morto, esperando renascer ou desconectado **não é alvo**.

Estratégias futuras (menos vida, quem bateu por último) são só novas implementações.

### 3. Token com prazo de validade

Cada token concedido tem um prazo (`tokenLeaseSeconds`). O diretor revoga sozinho quando:
- o prazo vence sem renovação;
- o inimigo morre, é desativado ou é destruído;
- o alvo deixa de ser válido.

Isso elimina vazamento de token quando um nó de IA falha no meio da sequência.

### 4. Slots por alvo

Em volta de cada alvo existem slots (inicialmente dois: esquerda e direita no eixo X, a `slotDistance`, com o ruído atual). Cada slot tem **um ocupante**. `GetAvailablePositionSlot` passa a reservar o slot para o inimigo e liberar quando ele sai (substitui o `ReleasePositionSlot` que hoje nunca é chamado).

### 5. Só no host

Criado no início da fase pelo host. O convidado não tem `CombatDirector`, e a IA do convidado fica desligada.

## Mudanças na interface `IAttackDirector`

| Hoje | Alvo |
|---|---|
| `Transform GetPlayerTransform()` | `Transform GetTarget(GameObject enemy)` |
| `bool RequestAttackToken(GameObject enemy)` | igual, mas o token é para o alvo atual do inimigo |
| `void ReleaseAttackToken(int enemyId)` | `void ReleaseAttackToken(GameObject enemy)` (sem `GetInstanceID` espalhado) |
| `Vector3 GetAvailablePositionSlot(GameObject enemy)` | igual, com reserva de slot |
| `bool IsInAttackRange(GameObject enemy)` | igual, em relação ao alvo do inimigo |
| — | `void NotifyTargetsChanged()` quando um player morre, renasce, entra ou sai |

Os nós de IA trocam `GetPlayerTransform()` por `GetTarget(agent)`. É a única mudança necessária neles.

## Valores de design

| Parâmetro | Atual | Onde fica |
|---|---|---|
| Atacantes por alvo (solo/dupla) | 2 (global) | `DifficultySettings` |
| `slotDistance` | 0,75 | `CombatDirectorSettings` (SO) |
| Tempo mínimo com o mesmo alvo | — (novo) | `DifficultySettings` |
| Prazo do token | — (novo) | `CombatDirectorSettings` |

## Testes

Toda a lógica de tokens, slots e escolha de alvo fica numa classe C# pura, testada em EditMode com alvos e inimigos falsos: 1 alvo, 2 alvos, alvo morrendo, token vencendo, dois inimigos disputando slot.
