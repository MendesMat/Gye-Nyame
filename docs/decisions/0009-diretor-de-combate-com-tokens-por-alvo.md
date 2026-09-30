# 0009 — Diretor de combate com tokens por alvo

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

O `CombatDirector` atual implementa o padrão consagrado de **attack tokens** (também chamado de *kung fu circle*): um diretor central limita quantos inimigos atacam ao mesmo tempo, e os demais rondam. É o que faz o combate parecer coreografado. Mas ele conhece um único player, o token só é devolvido se o nó da IA devolver, e os slots existem só em ±X do player.

## Decisão

Manter o padrão, com cinco ajustes:

1. **Tokens por alvo:** cada player tem o próprio orçamento de atacantes simultâneos, configurado em ScriptableObject.
2. **Escolha de alvo plugável:** uma interface de estratégia. A primeira implementação escolhe **o player mais próximo com token livre** e mantém esse alvo por um tempo mínimo, para não trocar a cada frame. Player morto ou esperando renascer não é alvo.
3. **Token com prazo de validade:** o token expira sozinho se o inimigo travar, morrer ou for desativado sem devolver.
4. **Slots em volta de cada player**, com ocupação controlada para que dois inimigos não disputem o mesmo ponto.
5. **Roda só no host.** Os clientes nem instanciam o diretor.

**Sem fogo amigo:** um player não acerta o outro.

Detalhes em [multiplayer/combat-director.md](../multiplayer/combat-director.md).

## Alternativas consideradas

- **Sem diretor (cada inimigo decide sozinho):** vira caos, com todos os inimigos cercando o mesmo jogador.
- **Um orçamento global para os dois jogadores:** permite que 4 inimigos cerquem um jogador enquanto o outro fica livre.

## Consequências

- O mesmo código serve para solo (1 alvo) e coop (2 alvos).
- Os nós de IA passam a perguntar "qual é o meu alvo?" ao diretor, em vez de usar "o player".
- Critérios de alvo mais sofisticados (menos vida, quem bateu por último) entram como novas estratégias, sem mudar o resto.
