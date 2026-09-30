# 0011 — Dois personagens jogáveis e seleção na sala

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

O jogo terá **dois personagens jogáveis**. Hoje existe um único personagem, com arte provisória. O segundo existe só como conceito, mas é certeza de implementação.

## Decisão

- **Solo:** o jogador escolhe qual dos dois personagens quer.
- **Dupla:** cada jogador joga com um personagem **diferente**.
- **Seleção na tela de Sala:**
  - personagem escolhido fica travado para o outro jogador;
  - se os dois escolherem o mesmo ao mesmo tempo, o host decide (vale quem chegou primeiro);
  - "Pronto" só libera com personagens diferentes;
  - depois de uma vitória ou game over, os dois voltam à Sala e podem trocar.
- **Cada personagem é uma ficha em ScriptableObject** (`CharacterDefinition`): prefab, controlador de animação, ataques iniciais do combo, atributos e retrato/nome para o HUD. Um terceiro personagem no futuro é só outra ficha.
- **Na transição, o segundo personagem é provisório:** cópia do primeiro com outra paleta de cores e o mesmo moveset. Assim o coop pode ser testado desde já.

## Restrição de lançamento

**Nenhuma build pública (alpha, demo, playtest aberto) sai sem os dois personagens com arte e moveset próprios.** Isso é conteúdo, fora das milestones de rede, mas bloqueia a primeira build pública.

## Alternativas consideradas

- **Mesmo personagem para os dois, só com cor diferente:** mais barato, mas contraria a visão do jogo.

## Consequências

- O spawn do jogador passa a depender da ficha escolhida, e não de um objeto fixo na cena.
- A seleção é estado do host, sincronizado para a sala.
- A arte e o moveset do segundo personagem entram no backlog de conteúdo, com prioridade antes de qualquer build pública.
