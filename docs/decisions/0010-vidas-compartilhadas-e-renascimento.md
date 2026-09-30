# 0010 — Vidas compartilhadas e renascimento

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Hoje a morte do player encerra a partida. Em coop, é preciso uma regra de falha que funcione para a dupla e para o solo.

## Decisão

- A **dupla compartilha um estoque de vidas**. Cada morte, de qualquer jogador, consome uma vida.
- **Game over quando o total de mortes esgota as vidas.** Exemplo com 3 vidas: o jogador 2 morre duas vezes e o jogador 1 uma vez → game over.
- **Renascimento:** quem morre com vidas sobrando volta **na borda esquerda da tela**, depois de um tempo, com **invencibilidade temporária**.
- O **mesmo sistema vale para o solo**.

Valores iniciais (o game designer decide os definitivos):

| Parâmetro | Valor inicial |
|---|---|
| Vidas iniciais | 3 |
| Espera até renascer | 3 s |
| Invencibilidade após renascer | 1,5 s |

**Todos esses valores ficam em ScriptableObject**, editáveis no Inspector ([0015](0015-valores-de-design-em-scriptableobjects.md)).

## Consequências

- As vidas são estado do **host** ([0004](0004-modelo-de-autoridade.md)) e são sincronizadas para o HUD.
- O `PlayerDeadState` deixa de publicar `GameOverMessage` diretamente. Ele avisa a morte, e o sistema de vidas decide entre renascer e game over.
- Se o convidado desconectar, as vidas **não mudam** ([0013](0013-escalonamento-de-dificuldade.md)).
- **A morte que zera o contador encerra a partida na hora**, mesmo que o parceiro esteja vivo. Com 3 vidas: a 1ª e a 2ª mortes renascem; a 3ª é game over. O HUD mostra as vidas restantes (3 → 2 → 1 → game over).
