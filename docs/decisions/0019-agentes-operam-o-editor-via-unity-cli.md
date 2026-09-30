# 0019 — Agentes operam o Editor pela Unity CLI

- **Status:** Aceita
- **Data:** 2026-09-30

## Contexto

A transição exige criar prefabs de rede, alterar cenas e rodar testes. Editar `.unity`, `.prefab` e `.asset` à mão (YAML com fileIDs e GUIDs) é frágil e invisível para um Editor aberto.

## Decisão

- Agentes operam o Editor pela **Unity CLI** (`unity`), com o pacote **`com.unity.pipeline`** instalado no projeto. **Não é necessário servidor MCP.**
- Com o Editor aberto e conectado, o agente cria e altera GameObjects, prefabs e cenas e roda C# no Editor com `unity command`.
- Testes rodam com `unity test`; builds com `unity build`.
- **Versões fixadas:** a CLI e o pacote Pipeline estão em beta/experimental. Atualizar é uma decisão consciente, não automática.

Procedimento em [workflow/unity-cli.md](../workflow/unity-cli.md).

## Alternativas consideradas

- **Servidor MCP da Unity:** redundante; a CLI já cobre o necessário.
- **Agentes só mexem em C#:** tudo que é cena, prefab e teste ficaria manual com o arquiteto, e a transição é cheia de prefabs.

## Consequências

- Se o projeto tiver erro de compilação, o Editor entra em *Safe Mode* e a CLI não conecta. O agente precisa corrigir o C# primeiro.
- Um agente em sandbox pode não enxergar um Editor que está aberto. Nesse caso, pergunta antes de editar arquivos à mão.
- Os checklists manuais (jogar a build, testar com lag) continuam humanos.
