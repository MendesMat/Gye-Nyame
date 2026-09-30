# 0016 — Documentação em `/docs` como fonte única

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

O código será executado principalmente por agentes de IA, com um único programador como arquiteto, e ajustado por um game designer. Os READMEs que existiam dentro das pastas de scripts já estavam desatualizados (citavam classes que não existem mais).

## Decisão

- **Markdown em `/docs`, dentro do repositório**, é a fonte única da verdade. Estrutura:
  - `architecture/`: o código como ele é hoje, um arquivo por módulo;
  - `multiplayer/`: a arquitetura-alvo de rede e regras de coop;
  - `decisions/`: estas ADRs;
  - `design/`: guia para o game designer;
  - `workflow/`: como agentes e pessoas trabalham no repositório.
- **Pontos de entrada:** `CLAUDE.md` na raiz como guia principal para agentes. O `AGENTS.md` é gerado pelo **Unity Code Assist** (extensão de IDE), que só reescreve o trecho entre os marcadores `<!-- UNITY CODE ASSIST INSTRUCTIONS -->`. A linha que aponta para `docs/` fica **fora** dos marcadores.
- Os READMEs dentro de `Assets/_Project/Scripts` viram apenas um link para o documento correspondente.
- **Texto em português, identificadores em inglês.**
- **Todo PR atualiza os documentos que o código tocou.** Quando algo de `multiplayer/` é implementado, o texto migra para `architecture/`.

## Alternativas consideradas

- **Wiki do GitHub:** fora do versionamento do código; agentes e PRs não a atualizam junto.
- **READMEs por pasta:** espalhados e já provaram divergir.

## Consequências

- O repositório é público: nada de segredos nem informações confidenciais da publicadora em `docs/`.
- A revisão de cada PR confere se a documentação acompanhou o código.
