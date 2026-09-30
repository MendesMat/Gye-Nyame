# 0003 — Netcode for GameObjects como biblioteca de rede

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

É preciso escolher a biblioteca de rede. O código é todo em MonoBehaviour, com máquina de estados em classes, sprites com Animator e IA no Unity Behavior (GameObject). A pergunta feita foi: "mesmo que recomeçássemos do zero, qual seria a melhor opção?".

## Decisão

Usar **Netcode for GameObjects (NGO) 2.x**, versão fixada no `Packages/manifest.json`.

## Alternativas consideradas

| Opção | Onde brilha | Por que foi rejeitada |
|---|---|---|
| Netcode for Entities | Servidor autoritativo com predição e rollback; muitos jogadores; PvP | Exige ECS. Sprites, Animator e Behavior Graph não são nativos de ECS. Resolve a predição, que a [0004](0004-modelo-de-autoridade.md) torna desnecessária |
| Rollback estilo jogo de luta | Precisão por frame em PvP | Exige simulação determinística; a física da Unity não é. Seria um motor inteiro para manter |
| Fish-Net, Mirror, PurrNet | Gratuitos, integrações da comunidade | Fora do ecossistema oficial: sem integração com os serviços da Unity, sem as skills da Unity, suporte menor |
| Photon | Predição pronta | Pago acima da cota gratuita; dependência de fornecedor |

## Consequências

- O NGO não tem predição embutida. Isso é aceitável porque cada jogador simula o próprio personagem localmente ([0004](0004-modelo-de-autoridade.md)).
- Reavaliar só se o jogo passar a ter 4 ou mais jogadores com dezenas de inimigos, ou incluir PvP.
- Código de rede fica num assembly próprio, `GyeNyame.Networking` ([multiplayer/overview.md](../multiplayer/overview.md)).
