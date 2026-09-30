# Autoridade e sincronização

> Arquitetura-alvo. Base: [ADR 0004](../decisions/0004-modelo-de-autoridade.md).

## Regra geral

- **Personagem jogável:** a máquina do **dono** simula (input, estados, movimento, combos, vida, reação a golpes). A outra máquina exibe uma réplica.
- **Todo o resto** (inimigos, IA, diretor de combate, vidas compartilhadas, dificuldade, fluxo da fase, sala): o **host** simula. O convidado exibe réplicas.
- **Golpes:** decide a máquina do **jogador envolvido**. Golpe de player em inimigo: decide a máquina do atacante. Golpe de inimigo em player: decide a máquina do atingido.

> **Pergunta que todo código novo responde:** isto roda em **toda máquina**, **só no dono** ou **só no host**?

## Tabela de autoridade

| Sistema | Quem simula | Como o outro lado vê |
|---|---|---|
| Input | dono (local, nunca sincronizado) | — |
| Posição do personagem | dono | transform sincronizado com autoridade do dono, interpolado |
| Estado do personagem (`EntityStateCategory`) | dono | variável sincronizada; a réplica chama `Animator.Play` |
| Direção do rosto | dono | variável sincronizada |
| Combo e ataque corrente | dono | pelo estado sincronizado (a animação traz o ataque) |
| Vida do personagem | dono | variável sincronizada (escrita pelo dono) |
| Morte do personagem | dono detecta | avisa o host por RPC; o host decide renascer ou game over |
| Renascimento | host decide, dono executa | host manda RPC ao dono com o ponto (borda esquerda da câmera dele) |
| Posição e estado dos inimigos | host | transform e estado sincronizados |
| IA (Behavior Graph) | host | desligada no convidado |
| Diretor de combate | host | não existe no convidado |
| Vida dos inimigos | host | variável sincronizada |
| Vidas compartilhadas | host | variável sincronizada (HUD) |
| Dificuldade e jogadores conectados | host | variável sincronizada, se a UI precisar |
| Sala: jogadores, seleção, pronto | host | variáveis sincronizadas |
| Câmera | cada máquina, local | não sincronizada |
| Hitstop, flash de dano, câmera lenta do game over | cada máquina, local | disparados por eventos de golpe |
| HUD | cada máquina, local | lê o estado sincronizado |

## Golpes, passo a passo

### Player acerta inimigo

1. Na máquina do atacante, a animação liga a hitbox (evento de clipe), como hoje.
2. A hitbox encosta na hurtbox da **réplica** do inimigo. Como o atacante é local, **o acerto conta**.
3. O atacante aplica localmente os efeitos visuais imediatos (hitstop do próprio personagem, flash) e chama no host um RPC de dano: quem atacou, qual inimigo, qual ataque, identificador do golpe.
4. O host valida (inimigo vivo, golpe não duplicado), aplica o dano, a reação e o knockback no inimigo real, e avisa todas as máquinas do golpe (para hitstop e flash nas réplicas).
5. Se dois players acertarem o mesmo inimigo, o host processa na ordem de chegada.

### Inimigo acerta player

1. No host, a IA manda atacar; o estado do inimigo é sincronizado.
2. Em cada máquina, a réplica do inimigo toca a animação pelo estado, e os eventos de clipe ligam a hitbox **localmente**.
3. Na máquina **dona do player atingido**, a hitbox encosta na hurtbox local. Como o atingido é local, **o acerto conta**: o dono aplica dano, reação e knockback no próprio personagem.
4. A vida sincronizada informa o host e o parceiro.

### Regra de filtragem

> **Um acerto só é processado na máquina que controla o jogador envolvido.** Hitbox de player local contra inimigo: conta. Hitbox de inimigo contra player local: conta. Qualquer outra combinação (réplica contra réplica) é ignorada.

Isso evita que o mesmo golpe seja contado duas vezes. A matriz de layers continua garantindo que player não acerta player ([0009](../decisions/0009-diretor-de-combate-com-tokens-por-alvo.md): sem fogo amigo).

## Hitstop

Local por entidade ([0008](../decisions/0008-hitstop-local-por-entidade.md)). Cada entidade tem um "relógio" que pode ser pausado por N segundos. Enquanto pausado, a entidade não avança estado, animação nem movimento. O mundo segue. Quem dispara: o evento de golpe, em cada máquina que exibe o atacante e o alvo.

## O que **não** é sincronizado

- Input bruto.
- `EventBus`: é local a cada máquina ([0007](../decisions/0007-eventbus-local-e-identidade.md)).
- Câmera, HUD, efeitos visuais.
- Valores de design: estão nos ScriptableObjects da build, iguais nas duas máquinas.

## Custos de banda (ordem de grandeza)

Com 2 personagens e poucos inimigos, o tráfego estimado fica em torno de 10 KB/s. É relevante só para a cota do serviço de conexão ([0005](../decisions/0005-servico-de-conexao-e-descoberta.md)). Sincronize estado compacto (enums, bytes, floats necessários), não objetos inteiros.
