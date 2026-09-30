# Git e GitHub

Repositório: `MendesMat/Gye-Nyame` (**público**: nada de segredos).

## Branches

- `main` sempre compila e roda. Ninguém comita direto nela.
- Uma branch por issue: `<tipo>/<número>-<resumo-curto>`. Exemplos: `refactor/12-prefab-do-player`, `feature/31-vidas-compartilhadas`, `docs/5-guia-de-tuning`.
- O game designer usa o mesmo padrão: `tuning/<resumo>`.

## Commits

- Mensagens em **português**, no imperativo, curtas: `Converte Player em prefab`.
- Um commit por passo lógico. O PR é integrado com *squash*.

## Pull requests

- Título: `#<issue> <resumo>`.
- Corpo:

  ```markdown
  Closes #<issue>

  ## O que mudou
  ## Como testar
  ## Documentação atualizada
  ## Fora do escopo / pendências
  ```

- Só o arquiteto aprova e faz o merge ([agents.md](agents.md#4-merge)).

## Unity Smart Merge

Cenas e prefabs são YAML. O `.gitattributes` já manda o git usar o **UnityYAMLMerge** nesses arquivos, mas o merge driver precisa ser registrado **em cada máquina** (inclusive a do game designer):

```bash
unity vcs merge-setup --check
```

```bash
unity vcs merge-setup
```

Sem isso, um conflito numa cena vira um merge de texto cego.

## Git LFS

Já ativo para imagens (sprites). Instale o LFS antes de clonar (`git lfs install`). Arquivos binários novos (áudio, fontes, modelos) devem entrar no LFS: confira o `.gitattributes` antes de comitar.

## Nunca versionar

`Library/`, `Temp/`, `Logs/`, `obj/`, `Build/`, `UserSettings/`, `Assets/_Recovery/` e qualquer arquivo com credenciais.
