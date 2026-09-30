# 0005 — Serviço de conexão da Unity, código de sala e IP direto

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

Jogadores em casa estão atrás de roteadores e firewalls. É preciso um serviço que conecte os dois sem abrir portas, e uma forma de um encontrar o outro. O jogo vai para Steam e itch.io, e há interesse distante em consoles.

## Decisão

- **Conexão:** serviço de sessões e retransmissão dos **Unity Multiplayer Services** (pacote `com.unity.services.multiplayer`), nas builds da Steam e do itch.
- **Tudo atrás de uma interface própria** no assembly de rede, para que o provedor possa ser trocado sem tocar na gameplay.
- **Descoberta:** o host cria uma sala e recebe um **código**; o amigo digita o código.
- **Eventos:** **conexão direta por IP** em rede local, que funciona sem internet.
- **Convite de amigo da Steam:** fica para depois, se for necessário.
- **Pareamento com desconhecidos ou lista pública de salas:** fora do escopo.

## Alternativas consideradas

| Opção | Por que foi rejeitada |
|---|---|
| Epic Online Services | Grátis e independente de loja, mas **não há conector mantido entre EOS e NGO** (o único encontrado estava parado desde fevereiro de 2024). Seria preciso escrever e manter um transporte próprio: tradução dos modos de entrega, fragmentação de pacotes acima de ~1.170 bytes, aperto de mão, login por Device ID e um fluxo especial de login para testar duas instâncias na mesma máquina. Estimativa: 3 a 6 semanas de encanamento de rede. O argumento do crossplay não se sustenta, porque a Unity também suporta login de console e crossplay |
| Rede da Steam | Grátis, mas só funciona na Steam; exigiria um segundo caminho para o itch |
| Dois provedores (Steam na Steam, Unity no itch) | Dois caminhos de código para um único programador manter |

## Consequências

- **Custo:** grátis durante todo o desenvolvimento e os playtests. Depois do lançamento, a cota gratuita é de 50 usuários simultâneos médios por mês e 150 GiB de banda; acima disso, cerca de US$ 0,16 por usuário simultâneo médio e US$ 0,09 por GiB (EUA/Europa). Estimativa: mil horas de partida pagas custam em torno de US$ 3.
- Ao estourar a cota, o serviço é bloqueado até configurar pagamento. É preciso acompanhar o painel depois do lançamento.
- Console na Unity é **só por convite**. Não bloqueia nada agora.
- O projeto já está ligado a um projeto na nuvem da Unity (nome antigo "Nye-Gyame"), que precisa ser renomeado na configuração.
