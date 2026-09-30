# 0001 — Refatorar e migrar em vez de reescrever

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

O jogo mudou de direção: de singleplayer para coop online de 2 jogadores, por exigência de uma publicadora. O código tem cerca de 4 mil linhas em estágio de protótipo. Tempo de desenvolvimento não é restrição, então reescrever do zero foi considerado a sério.

A análise mostrou que o problema não está no desenho do código, e sim em **quatro premissas de singleplayer**, cada uma concentrada em poucos arquivos: existe um único player; o tempo é global (`Time.timeScale`); a morte do player encerra a partida; tudo roda localmente.

Várias partes já favorecem a rede: input convertido em intenções, máquina de estados com `EntityStateCategory` (um enum basta para sincronizar a animação), física cinemática previsível, dados de ataque em ScriptableObjects, IA desacoplada por interfaces, assemblies separados.

## Decisão

Manter a base e migrar módulo por módulo para o multiplayer. **Não** corrigir nem polir o singleplayer antes: sistemas que vão mudar (tempo, fluxo de jogo, diretor de combate, UI) são reescritos direto na forma multiplayer. Bugs conhecidos são corrigidos dentro da issue de refatoração do módulo correspondente.

## Alternativas consideradas

- **Reescrever do zero:** mesmo começando do zero, a tecnologia escolhida seria a mesma (ver [0003](0003-netcode-for-gameobjects.md)). Recomeçar custaria semanas só para voltar ao ponto atual, sem ganho técnico.
- **Corrigir o singleplayer primeiro e migrar depois:** a maior parte das correções cairia em sistemas que serão reescritos. Retrabalho garantido.

## Consequências

- Estimativa: 60 a 70% do código aproveitado. Fluxo de jogo, tempo e UI nascem de novo.
- A única preparação independente da rede que vem primeiro é transformar Player e inimigos em **prefabs**, porque o netcode exige.
- A lista de bugs e dívidas fica em [architecture/known-issues.md](../architecture/known-issues.md) até cada módulo ser migrado.
