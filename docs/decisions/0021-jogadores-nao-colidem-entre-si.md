# 0021 — Os jogadores não colidem entre si

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Hoje a layer `Player` colide com ela mesma na Layer Collision Matrix. Com dois jogadores, um personagem bloquearia o outro: daria para empurrar o parceiro ou prendê-lo contra a borda da câmera ([0012](0012-camera-compartilhada.md)).

## Decisão

**Os dois personagens jogáveis atravessam um ao outro.** Eles continuam colidindo com inimigos, chão e paredes.

## Alternativas consideradas

- **Colidir:** gera bloqueios frustrantes com a câmera compartilhada e dessincronias visuais, porque cada personagem é simulado numa máquina diferente ([0004](0004-modelo-de-autoridade.md)).

## Consequências

- Desmarcar `Player × Player` na Layer Collision Matrix (Project Settings → Physics).
- **Verificar na implementação** se o `Rigidbody.SweepTest` usado pelo `KinematicPhysics` respeita a matriz. Se não respeitar, o `KinematicPhysics` precisa ignorar explicitamente a layer `Player` quando a própria entidade for um player.
- Sem fogo amigo continua valendo ([0009](0009-diretor-de-combate-com-tokens-por-alvo.md)): hitboxes de player não acertam hurtboxes de player.
