# 0002 — Coop online P2P de 2 jogadores, com um jogador como host

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

A publicadora exige multiplayer. Não há verba para servidores dedicados, mas há tempo de desenvolvimento. Os planos incluem Steam (Windows), builds de playtest no itch.io e apresentações em eventos. O multiplayer é **só cooperativo**, sem PvP.

## Decisão

- **Coop online de exatamente 2 jogadores.**
- **Topologia host-cliente (listen server):** um jogador cria a partida e roda a simulação; o outro se conecta a ele. Não existe servidor dedicado.
- **Se o host sair, a partida acaba** e o convidado volta ao menu.
- **O convidado só entra no início da fase**, pela sala. Não há entrada no meio da fase (*drop-in*).
- **Se o convidado cair, o host continua sozinho** e o convidado não volta para aquela partida.

## Alternativas consideradas

- **Coop local (mesma tela):** muito mais barato (semanas em vez de meses) e ótimo para eventos. Rejeitado como caminho principal porque o objetivo é o melhor ponto de partida para o multiplayer que a publicadora exige, e o tempo não é restrição. Fica como possibilidade futura, barata depois que a arquitetura tiver identidade de jogador.
- **Coop local + Steam Remote Play Together:** online sem código de rede, mas só na Steam e com qualidade dependente da conexão do host.
- **Servidor dedicado:** custo recorrente e sem necessidade num coop de 2.
- **Migração de host, drop-in e reconexão:** exigem sincronizar e transferir o estado completo da partida. Adiados (ver [0020](0020-escopo-da-transicao.md)).

## Consequências

- Desenvolver é gratuito. Custo só existe no serviço que conecta os jogadores, depois do lançamento e em escala ([0005](0005-servico-de-conexao-e-descoberta.md)).
- O código precisa distinguir "sou o host" de "sou o convidado" e "este personagem é meu" de "é do outro".
- Eventos exigem 2 PCs em rede local ([0005](0005-servico-de-conexao-e-descoberta.md)).
