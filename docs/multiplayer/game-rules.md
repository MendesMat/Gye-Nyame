# Regras de coop

> Arquitetura-alvo. Todos os números aqui são **valores iniciais**: quem decide os definitivos é o game designer, no Inspector ([0015](../decisions/0015-valores-de-design-em-scriptableobjects.md)).

## Jogadores e personagens

- **1 ou 2 jogadores.** Solo é o mesmo jogo com 1 ([0006](../decisions/0006-modo-solo-como-host-local.md)).
- **2 personagens jogáveis.** No solo, o jogador escolhe um. Na dupla, cada um joga com um personagem diferente ([0011](../decisions/0011-dois-personagens-e-selecao.md)).
- Cada personagem é uma **ficha** (`CharacterDefinition`, ScriptableObject):

| Campo | Conteúdo |
|---|---|
| Nome e retrato | Para sala e HUD |
| Prefab | Prefab de rede do personagem |
| Controlador de animação | Override do `AgentAnimatorController` |
| Ataques iniciais | `AttackDataSO` de início do combo leve e do pesado |
| Atributos | Vida, velocidade, pulo, dash (hoje espalhados em componentes) |

- Na transição, o **segundo personagem é provisório**: cópia do primeiro, com outra paleta. Nenhuma build pública sai sem os dois definitivos.

## Vidas e renascimento

Base: [0010](../decisions/0010-vidas-compartilhadas-e-renascimento.md). Sistema do host.

| Parâmetro | Inicial | Onde |
|---|---|---|
| Vidas compartilhadas | 3 | `MatchRules` (SO) |
| Espera até renascer | 3 s | `MatchRules` |
| Invencibilidade ao renascer | 1,5 s | `MatchRules` |

- Cada morte consome 1 vida, de qualquer jogador. A morte que zera as vidas é **game over imediato**.
- Com vidas sobrando: o jogador morto aguarda a espera (o HUD mostra a contagem) e renasce na **borda esquerda da câmera**, no chão, com invencibilidade.
- Durante a espera, o jogador morto **não é alvo** dos inimigos.
- Vitória: todos os inimigos da fase derrotados (como hoje).
- Fim da partida (vitória ou game over): efeito local de câmera lenta só no game over; depois, volta à Sala (online) ou ao Menu (solo).

## Dificuldade

Base: [0013](../decisions/0013-escalonamento-de-dificuldade.md). Sistema do host. Depende de **jogadores conectados**, não de vivos.

| Parâmetro | Solo | Dupla | Onde |
|---|---|---|---|
| Atacantes simultâneos por player | 2 | a definir | `DifficultySettings` (SO) |
| Multiplicador de vida dos inimigos | 1 | a definir | `DifficultySettings` |
| Multiplicador de dano dos inimigos | 1 | a definir | `DifficultySettings` |

Quando o convidado desconecta:
1. atacantes simultâneos voltam ao valor do solo na hora;
2. inimigos vivos recalculam a vida máxima **mantendo a porcentagem**;
3. inimigos novos nascem com os valores do solo;
4. vidas compartilhadas não mudam;
5. jogador morto esperando renascer não conta como desconectado.

## Câmera

Base: [0012](../decisions/0012-camera-compartilhada.md).

- Uma câmera que enquadra os dois (o centro acompanha o ponto médio no eixo X; Y e Z seguem o enquadramento atual).
- As bordas esquerda e direita são **paredes** para os jogadores: ninguém sai da tela.
- A câmera só avança se os dois couberem no quadro. Quem ficou para trás "segura" a câmera.
- Com um jogador (solo, parceiro morto ou desconectado), a câmera segue só ele.
- Calculada localmente em cada máquina.

## Desconexão e saída

| Situação | Resultado |
|---|---|
| Convidado desconecta | Host continua sozinho; dificuldade ajustada; aviso "Parceiro desconectado"; o personagem do convidado some |
| Host desconecta ou sai | Partida acaba; convidado volta ao Menu com aviso |
| Convidado quer voltar | Não na mesma partida (reconexão fora do escopo) |

## Colisão entre os dois jogadores

**Os dois jogadores não colidem entre si:** um atravessa o outro ([0021](../decisions/0021-jogadores-nao-colidem-entre-si.md)). Eles continuam colidindo com inimigos, chão e paredes. Hoje a layer `Player` ainda colide com ela mesma; a mudança na matriz faz parte da migração.

## Fogo amigo

Não existe. Garantido pela matriz de layers (`PlayerHitbox` só colide com `EnemyHurtbox`).
