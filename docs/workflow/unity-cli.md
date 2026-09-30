# Operando o Editor pela Unity CLI

> Base: [ADR 0019](../decisions/0019-agentes-operam-o-editor-via-unity-cli.md).

Agentes criam e alteram cenas, prefabs e assets **pelo Editor**, usando a Unity CLI (`unity`) e o pacote `com.unity.pipeline` instalado no projeto. **Não usamos servidor MCP.**

## Versões fixadas

| Ferramenta | Versão |
|---|---|
| Unity Editor | 6000.3.9f1 |
| Unity CLI | 1.0.0-beta.11 |
| `com.unity.pipeline` | 0.8.0-exp.1 |

Ambos estão em beta ou experimental. **Não atualize por conta própria**: atualização é uma issue de `infra`.

## Antes de mexer em cena, prefab ou asset

```bash
unity status
```

- Estado `ready`: há Editor conectado. **Use comandos do Editor, nunca edite o YAML à mão.**
- Sem instâncias: verifique as duas causas falsas antes de concluir que o Editor está fechado:
  1. **Safe Mode.** Com erro de compilação, o Editor sobe em Safe Mode e o Pipeline não carrega. Rode `unity pipeline list`; se indicar Safe Mode, corrija o C# e reinicie o Editor.
  2. **Sandbox.** O sandbox do agente pode esconder um Editor aberto. Não improvise: pergunte ao arquiteto se o Editor está aberto.
- Só edite arquivos da Unity à mão com o Editor comprovadamente fechado, e **diga isso explicitamente** no PR.

## Comandos do Editor

```bash
unity command --caller plugin --skill gye-nyame                  # lista os comandos disponíveis
unity command <nome> --caller plugin --skill gye-nyame [args]    # executa
unity command eval --caller plugin --skill gye-nyame '<C#>'      # roda C# no Editor
```

- Os nomes dos comandos são definidos pelo Editor: **liste antes de usar**, não adivinhe.
- Sempre passe `--caller plugin --skill gye-nyame` (identifica a origem da chamada).
- Com mais de um Editor aberto, passe `--project-path <caminho>`.
- Depois de alterar uma cena, salve-a pelo Editor.

## Testes e build

```bash
unity test . --mode EditMode --report-format junit --output ./TestResults/editmode.xml
unity test . --mode PlayMode
```

| Código de saída | Significado |
|---|---|
| 0 | Todos passaram |
| 8 | A execução completou e **testes falharam** (não tente de novo; corrija) |
| outro | A execução não completou (compilação, licença, timeout): problema de infraestrutura |

```bash
unity build . --profile "Assets/Settings/Build Profiles/Windows  Protype Test v0.1.0.asset"
```

O perfil de build define a plataforma (Windows 64 bits). Atenção ao nome do arquivo: tem dois espaços e "Protype".

## Logs

Log do Editor no Windows: `%LOCALAPPDATA%\Unity\Editor\Editor.log`. Útil para erros de compilação e exceções em Play Mode.

## Guia completo da CLI

```bash
unity skill show
```
