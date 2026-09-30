# 0020 — Escopo da transição e não-objetivos

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Hoje o jogo tem uma cena, 4 dummies e a regra "matou todos, reinicia". Misturar a migração para rede com sistemas novos de conteúdo faria as milestones de rede nunca fecharem.

## Decisão

A transição entrega **só o necessário para o coop funcionar**, em cima do conteúdo atual: rede, 2 jogadores, seleção de personagem (com o segundo provisório), vidas e renascimento, câmera compartilhada, escalonamento de dificuldade, diretor de combate por alvo, menus e sala, HUD de coop e documentação.

### Critério de sucesso

> Dá para jogar a fase atual inteira **em dupla online**, do menu até o game over ou a vitória, e também **em solo offline**.

### Fora do escopo (não-objetivos)

| Item | Situação |
|---|---|
| Coop local na mesma tela | Futuro; barato depois da identidade de jogador |
| Migração de host | Não previsto |
| Entrada no meio da fase e reconexão | Futuro |
| Pareamento com desconhecidos e lista pública de salas | Não previsto |
| Convite de amigo da Steam | Futuro, se necessário |
| Epic Online Services | Rejeitado ([0005](0005-servico-de-conexao-e-descoberta.md)) |
| Anti-cheat | Não previsto (coop) |
| Consoles | Futuro distante |
| Ondas de inimigos, arenas, várias fases, inimigos novos | Backlog de conteúdo, depois da transição |
| Arte e moveset definitivos do segundo personagem | Conteúdo, mas **bloqueia a primeira build pública** |
| Menu de opções, localização | Futuro |
| CI e automação de build | Futuro (o pacote de Cloud Build já está instalado) |

## Consequências

- Issues que tentem trazer conteúdo novo para dentro das milestones de transição são recusadas ou movidas para o backlog de conteúdo.
- Riscos em aberto ao final da sessão de decisões:
  - não sabemos em detalhe o que a publicadora exige de multiplayer (assumimos que coop online com código de sala basta);
  - Unity CLI e pacote Pipeline em beta;
  - console na Unity é só por convite;
  - um único programador concentra o conhecimento (mitigado por esta documentação).
