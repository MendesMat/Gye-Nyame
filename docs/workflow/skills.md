# Skills: quais usar em cada caso

> Base: [ADR 0022](../decisions/0022-skills-obrigatorias-para-agentes.md).

## Regras

1. **`clean-code` é obrigatória** para gerar ou alterar qualquer código.
2. **Skills da Unity são obrigatórias** sempre que a tarefa cair no domínio delas.
3. **Defina as skills antes de começar:** a pesquisa lista as skills no comentário da issue; a execução carrega todas antes de escrever código; a revisão confere se o código segue as orientações delas.
4. Se uma skill contradizer uma ADR ou `docs/`, **vale a ADR**. Registre o conflito na issue.

Os nomes abaixo são os identificadores das skills no Claude Code. Em outro cliente de IA, use a skill equivalente de mesmo nome.

## Obrigatória em todo código

| Skill | Quando |
|---|---|
| `anthropic-skills:clean-code` | **Sempre** que escrever ou alterar C# (gameplay, rede, UI, testes, ferramentas de Editor) |

## Skills da Unity por caso

| Caso no projeto | Skill |
|---|---|
| Operar o Editor: criar/alterar GameObjects, prefabs, cenas; rodar C# no Editor; testes e builds pela CLI | `unity:unity-cli` |
| Instalar, remover ou atualizar pacotes (NGO, Multiplayer Services, Multiplayer Play Mode, Test Framework…) | `unity:unity-package-management` |
| Rede: sessões, código de sala, conexão direta, host/cliente, integração com o Netcode for GameObjects | `unity:setup-multiplayer-services` |
| Qualquer UI (menus, sala, HUD, pausa). Começa por esta para confirmar o sistema de UI do projeto | `unity:ui` |
| UI em Canvas/uGUI (o sistema atual do projeto): prefabs de UI, RectTransform, layouts | `unity:ui-ugui` |
| Ferramentas de Editor novas (janelas, inspectors customizados) | `unity:ui-uitk` |
| Colisão, triggers, layers, `SweepTest`/`SphereCast`, hitbox que não dispara | `unity:physics-3d-collision` |
| Textos com TextMeshPro (HUD, menus) | `unity:optimize-text-mesh-pro` |
| Sprites, fatiamento de spritesheets, pivôs | `unity:sprite-editor` |
| Sprite atlas | `unity:manage-sprite-atlas` |
| Renderização em pixel art nítida | `unity:2d-pixel-perfect` |
| Pós-processamento URP (Volume) | `unity:urp-postprocessing` |
| Áudio: importação e mixers | `unity:optimize-audio`, `unity:audio-setup-mixers` |
| Localizar assets ou objetos de cena pelo Unity Search | `unity:generate-editor-search-query` |

Skills da Unity **fora do escopo atual** (não usar sem uma ADR que mude o escopo): `initialize-ai-navigation` (a IA usa Behavior Graph, não NavMesh), `build-live-game`, `implement-in-app-purchases`, `levelplay-unity-integration`, `localization`, `setup-vivox-voice-chat`, `optimize-web`, `migrate-birp-to-urp`.

## Exemplo de seção no comentário de pesquisa

```markdown
### Skills necessárias
- `anthropic-skills:clean-code`: todo o código C# da issue
- `unity:setup-multiplayer-services`: criação de sessão com código de sala
- `unity:unity-package-management`: instalar `com.unity.services.multiplayer`
- `unity:unity-cli`: criar o prefab do NetworkManager na cena MainMenu
```
