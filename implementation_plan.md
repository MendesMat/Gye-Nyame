# Renaming Unity Project "Nye-Gyame" to "Gye-Nyame"

This plan outlines the steps to safely rename your project, both in the file system and internally within Unity, avoiding broken references.

> [!CAUTION]
> The folder renaming steps (Step 1 and 2) must be done manually by you outside of this IDE because renaming the currently active workspace directory while the IDE is running can cause fatal errors and crash the current session.

## Proposed Changes

Here is the step-by-step plan:

### 1. Close all editors
Make sure to close Unity Hub, Unity Editor, and your IDE (Visual Studio / VS Code / etc.) to release any file locks.

### 2. Rename the Root Folders (Manual Step)
Rename the folders in your file explorer:
- Change the outer folder: `c:\Projetos\Unity\Nye-Gyame` ➔ `c:\Projetos\Unity\Gye-Nyame`
- Change the inner folder: `c:\Projetos\Unity\Gye-Nyame\Nye-Gyame` ➔ `c:\Projetos\Unity\Gye-Nyame\Gye-Nyame`

### 3. Clean up Auto-Generated Files (Manual Step)
Within the new `c:\Projetos\Unity\Gye-Nyame\Gye-Nyame` directory, delete the following auto-generated folders and files so Unity recreates them with the new name:
- `Library/` (folder)
- `Logs/` (folder)
- `obj/` (folder)
- `Nye-Gyame.slnx` (or any `.sln` file)
- `*.csproj` files (e.g., `Assembly-CSharp.csproj`, `MerryYellow.CodeAssist.Editor.csproj`)

### 4. Update Internal Configurations
We can perform these file edits now, before you do the folder renames (since we currently have access to the files in their old location).

#### [MODIFY] ProjectSettings.asset (ProjectSettings/ProjectSettings.asset)
- Update `productName`, `projectName`, `metroPackageName`, and `metroApplicationDescription` from `Nye-Gyame` to `Gye-Nyame`.

#### [MODIFY] AGENTS.md (AGENTS.md)
- Update the project name instruction from `Nye-Gyame` to `Gye-Nyame`.

#### [MODIFY] SampleScene.unity (Assets/Scenes/SampleScene.unity)
- Update stale namespace identifiers (`m_EditorClassIdentifier: Assembly-CSharp::NyeGyame...` ➔ `Assembly-CSharp::GyeNyame...`) to match your C# scripts which are already using the `GyeNyame` namespace.

### 5. Re-open in Unity
- Open **Unity Hub**.
- Click **Add** and select the new `c:\Projetos\Unity\Gye-Nyame\Gye-Nyame` directory.
- Open the project. Unity will regenerate the Library and solution files automatically.
