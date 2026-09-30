# Testes do multiplayer

> Base: [ADR 0018](../decisions/0018-testes-e-definicao-de-pronto.md).

## Três níveis

| Nível | Ferramenta | Quem roda | Quando |
|---|---|---|---|
| Lógica pura | Unity Test Framework, **EditMode** | agentes (`unity test`) e arquiteto | Todo PR |
| Integração leve | Unity Test Framework, **PlayMode** | agentes e arquiteto | Onde for barato |
| Rede | **Multiplayer Play Mode** + 2 builds + latência simulada | **arquiteto** (checklist manual) | Fim de cada milestone de rede e PRs de rede |

## Testes automatizados

- Pastas: `Assets/_Project/Tests/EditMode` e `Assets/_Project/Tests/PlayMode`, cada uma com seu `.asmdef` de teste.
- A lógica de regra fica em classes C# puras para poder ser testada sem cena: vidas, dificuldade, combo, tokens e alvo do diretor, buffer de input, seleção de personagem.
- Rodar:

  ```bash
  unity test . --mode EditMode --report-format junit --output ./TestResults/editmode.xml
  ```

  Código de saída `8` = testes falharam; outros códigos diferentes de `0` = a execução não completou (compilação, licença, timeout).

## Multiplayer Play Mode

Pacote `com.unity.multiplayer.playmode`. Abre jogadores virtuais no mesmo Editor (até 4). Uso diário:

1. Window → Multiplayer → Multiplayer Play Mode; ativar 1 jogador virtual.
2. Play. Na instância principal: Criar sala (ou conexão direta). No jogador virtual: Entrar.
3. Para lag: configurar a simulação de rede do Unity Transport (latência, variação, perda de pacotes) na instância.

## Checklist de fumaça (toda milestone)

Build Windows, sem o Editor:

- [ ] O jogo abre no menu sem erros no log.
- [ ] Solo: escolher personagem, jogar a fase até vencer.
- [ ] Solo: morrer até o game over; volta ao menu.
- [ ] Pausa no solo para o jogo.
- [ ] Combos (leve → leve, leve → pesado), pulo, dash e cancelamento funcionam.

## Checklist de rede (milestones de rede)

Duas instâncias (build + build, ou build + Editor), com **150 ms de latência e 2% de perda** simulados:

- [ ] Criar sala mostra um código; o outro entra pelo código.
- [ ] Conexão direta por IP funciona em rede local, sem internet.
- [ ] Seleção: o personagem de um fica travado para o outro; "Pronto" só com personagens diferentes.
- [ ] Os dois entram na fase com o personagem certo; cada um controla só o seu.
- [ ] Movimento e ataques do próprio personagem respondem sem atraso perceptível.
- [ ] Golpes do convidado causam dano e reação nos inimigos; hitstop e flash aparecem nas duas telas.
- [ ] Inimigos atacam os dois jogadores (tokens por alvo), sem cercar um só.
- [ ] Morte consome vida compartilhada; renascimento na borda esquerda com invencibilidade.
- [ ] Última vida: game over para os dois.
- [ ] Câmera enquadra os dois; ninguém sai da tela.
- [ ] Convidado fecha o jogo: host continua, aviso aparece, dificuldade cai.
- [ ] Host fecha o jogo: convidado volta ao menu com aviso.
- [ ] Vitória leva os dois de volta à sala; dá para trocar de personagem e jogar de novo.
