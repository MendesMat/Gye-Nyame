# 0013 — Dificuldade pelo número de jogadores conectados

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Com dois jogadores, os inimigos do solo seriam "atropelados". E se o convidado cair no meio da fase, o host fica sozinho diante de uma fase ajustada para dois.

## Decisão

**A dificuldade depende de quantos jogadores estão conectados**, não de quantos estão vivos.

Com 2 jogadores:
- o diretor de combate libera **mais atacantes simultâneos** ([0009](0009-diretor-de-combate-com-tokens-por-alvo.md));
- inimigos têm **multiplicadores de vida e de dano**.

Se o convidado desconectar:
1. **Na hora:** o número de atacantes simultâneos volta ao valor do solo.
2. **Inimigos já vivos:** a vida máxima é recalculada **mantendo a porcentagem** (um inimigo com 60% continua com 60%, só que da vida máxima do solo).
3. **Inimigos que surgirem depois:** nascem com os valores do solo.
4. **Vidas compartilhadas:** não mudam. São da equipe.
5. **Jogador morto esperando renascer não altera a dificuldade.** Só desconexão altera.

Todos os multiplicadores ficam num ScriptableObject de dificuldade. No solo, os multiplicadores são 1.

## Alternativas consideradas

- **Não escalar:** a dupla atropela a fase.
- **Mais inimigos por onda:** depende de um sistema de ondas, que está fora do escopo da transição ([0020](0020-escopo-da-transicao.md)).
- **Limitar as vidas ao valor do solo quando o convidado cai:** discutido; decidido manter as vidas por enquanto. Pode ser revisto pelo game designer.

## Consequências

- O host é quem calcula e aplica a dificuldade.
- "Jogadores conectados" vira um valor observável, que diretor, inimigos e spawns consultam.
