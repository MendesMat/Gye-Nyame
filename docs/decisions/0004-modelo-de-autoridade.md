# 0004 — Dono do personagem manda nele; host manda no resto

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Num beat 'em up, o combate precisa responder na hora. Se o host decidisse tudo, o convidado sentiria de 50 a 150 ms de atraso em cada soco e pulo. Como o jogo é cooperativo, trapaça tem pouca importância: os dois jogadores estão do mesmo lado.

## Decisão

- **Cada jogador tem autoridade sobre o próprio personagem:** movimento, estados, ataques, combos e a detecção dos próprios golpes rodam localmente, sem esperar a rede.
- **O host tem autoridade sobre todo o resto:** inimigos, IA, diretor de combate, vida dos inimigos, vidas compartilhadas, renascimento, dificuldade e fluxo da fase.
- **Golpe do jogador em inimigo:** o atacante detecta o acerto localmente e **avisa o host**, que aplica o dano ("favorecer o atacante"). O host aplica regras simples de sanidade (inimigo vivo, não duplicar o mesmo golpe).
- **Golpe de inimigo em jogador:** a máquina **dona do jogador atingido** detecta o acerto (a hitbox do inimigo, replicada, encostando na hurtbox local) e aplica o dano e a reação no próprio personagem. O host fica sabendo pela vida sincronizada e pela morte.

Nos dois sentidos, **quem decide é a máquina do jogador envolvido** ("favorecer o jogador"): o que o jogador vê na própria tela é o que vale para ele.

A tabela detalhada de quem é dono de quê está em [multiplayer/authority.md](../multiplayer/authority.md).

## Alternativas consideradas

- **Host decide tudo:** mais seguro contra dessincronização, mas o convidado sente atraso em todo input. Exigiria predição com reconciliação, que o NGO não oferece pronta.
- **Autoridade distribuída (serviço em nuvem da Unity):** única forma de migrar o host no NGO, mas faz até a partida depender da nuvem. Migração de host está fora do escopo.

## Consequências

- O convidado às vezes vê o inimigo reagir um pouco depois do golpe (a reação passa pelo host).
- Dois jogadores podem acertar o mesmo inimigo "ao mesmo tempo". O host resolve na ordem de chegada.
- Como o jogador atingido decide se foi atingido, esquivas e invencibilidade (dash, renascimento) funcionam exatamente como aparecem na tela dele. O custo é que, na tela do parceiro, um golpe pode parecer ter acertado sem ter acertado. Aceitável em coop.
- Tudo isso é validado com latência simulada ([0018](0018-testes-e-definicao-de-pronto.md)).
