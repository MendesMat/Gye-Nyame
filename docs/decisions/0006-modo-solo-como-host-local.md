# 0006 — Modo solo como host local offline

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

O modo solo continua existindo. Manter um caminho singleplayer separado do multiplayer dobraria o código a manter.

## Decisão

- O solo roda como **host sem convidados**: o mesmo código do coop, com um jogador.
- O solo **inicia o host do NGO localmente, sem contatar nenhum serviço online**. Funciona sem internet, em evento ou offline.
- No solo, a pausa **para o jogo**. No online, não para ([0014](0014-fluxo-de-menus-cenas-e-hud.md)).

## Alternativas consideradas

- **Modo offline separado, sem NGO:** duplicaria spawn, fluxo de jogo e regras.

## Consequências

- Todo sistema novo precisa funcionar com 1 ou 2 jogadores. O solo é o caso de 1 alvo, 1 personagem, multiplicadores de dificuldade iguais a 1.
- Os testes de lógica cobrem o caso de 1 jogador sem precisar de rede.
- A pausa do solo precisa congelar a simulação sem depender de `Time.timeScale` para hitstop ([0008](0008-hitstop-local-por-entidade.md)).
