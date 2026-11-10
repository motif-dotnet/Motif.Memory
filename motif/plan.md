# Motif Memory: solution plan

*Exported by Motif Sketch from the project "Motif Memory". Stubs only: no functionality.*

## 1. Overview

- **System**: Motif Memory (Blazor WebAssembly (GitHub Pages))
- **Purpose**: Sets up an agent's external memory, with the roles, workflow and design guidelines you choose
- **Solution**: `Motif.Memory.sln`, targeting `net10.0`
- **Architecture**: ports and adapters. The application core (Domain and UseCases) knows nothing about the outside world; every external concern sits behind a port and is reached through an adapter.
- **Ports**: 1 driving, 1 driven

## 2. Solution and project layout

Projects are grouped by role, and the solution folders in the `.sln` mirror the folders on disk. There are no `src/` or `tests/` folders.

```
Motif.Memory.sln
  Motif.Memory.App/
  Motif.Memory/
    Motif.Memory.Domain/
    Motif.Memory.UseCases/
  Adapters/
    Motif.Memory.MemoryFiles/
  Checks/
    Motif.Memory.Fakes/
    Motif.Memory.Specs/
```

| Project | Folder | Template | Namespace | Role |
|---|---|---|---|---|
| `Motif.Memory.App` | `Motif.Memory.App/` | `blazorwasm` | `Motif.Memory.App` | The app: runnable adapter on the User port |
| `Motif.Memory.Domain` | `Motif.Memory/Motif.Memory.Domain/` | `classlib` | `Motif.Memory` | Domain-specific business rules |
| `Motif.Memory.UseCases` | `Motif.Memory/Motif.Memory.UseCases/` | `classlib` | `Motif.Memory` | Application rules (use cases) and all port interfaces |
| `Motif.Memory.MemoryFiles` | `Adapters/Motif.Memory.MemoryFiles/` | `razorclasslib` | `Motif.Memory.Adapters` | Adapters on the Memory Files port |
| `Motif.Memory.Fakes` | `Checks/Motif.Memory.Fakes/` | `classlib` | `Motif.Memory.Fakes` | Fake driven adapters, for specs |
| `Motif.Memory.Specs` | `Checks/Motif.Memory.Specs/` | `xunit` | `Motif.Memory.Specs` | Specs: the checks on every driving port |

## 3. Project references (dependency rules)

| Project | References |
|---|---|
| `Motif.Memory.App` | `Motif.Memory.UseCases` |
| `Motif.Memory.Domain` | nothing |
| `Motif.Memory.UseCases` | `Motif.Memory.Domain` |
| `Motif.Memory.MemoryFiles` | `Motif.Memory.UseCases` |
| `Motif.Memory.Fakes` | `Motif.Memory.UseCases` |
| `Motif.Memory.Specs` | `Motif.Memory.UseCases`, `Motif.Memory.Fakes` |

Rules:

- `Motif.Memory.Domain` references nothing.
- `Motif.Memory.UseCases` references `Motif.Memory.Domain` only.
- Every adapter project references `Motif.Memory.UseCases` only. **Adapters never reference each other.**
- The specs project may also reference the fakes it wires up by hand.
- These references are the dependency rule. Don't add others.

## 4. Namespaces

- `Motif.Memory.Domain` and `Motif.Memory.UseCases` both use the plain `Motif.Memory` namespace, so the port interfaces are `Motif.Memory.UserTasks` and so on.
- Every other project uses the namespace in section 2. Real and simulated adapters share one namespace, and so do the fakes.
- Folders inside a project don't change its namespace.
- Set `RootNamespace` in each `.csproj` to its namespace, so new files created in an IDE get it.
- Every namespace sits inside `Motif.Memory`, so adapters, fakes and specs see the port interfaces without a `using`.

## 5. Ports

All port interfaces live in `Motif.Memory.UseCases`, namespace `Motif.Memory`, one file each. Interfaces take the plain name of the port, **with no `I` prefix**. Leave them empty: their members are design work still to do.

```
Motif.Memory.UseCases/
  UserTasks.cs
  UserQueries.cs
  MotifMemoryApplication.cs
  Ports/
    MemoryFiles.cs
```

### User (driving)

The User port: how people (and anything acting for them) use the application. It has two halves, commands and queries.

`UserTasks.cs`:

```csharp
namespace Motif.Memory;

public interface UserTasks
{
}
```

`UserQueries.cs`:

```csharp
namespace Motif.Memory;

public interface UserQueries
{
}
```

### Memory Files (driven)

Writes the starter memory out: its Markdown files, and the pointer for the agent's instructions file

`Ports/MemoryFiles.cs`:

```csharp
namespace Motif.Memory;

public interface MemoryFiles
{
}
```

Also add one stub class, `MotifMemoryApplication` in `Motif.Memory.UseCases` (namespace `Motif.Memory`), implementing the driving interfaces (`UserTasks`, `UserQueries`). It is the application itself; its constructor will later take the driven ports.

## 6. Adapters

Kinds: **Real** is the production adapter; **Fake** is a simple in-process stand-in for fast specs; **Simulated** is a higher-fidelity stand-in for people running the whole app locally; **Checks** drive the application through a driving port and check what it does.

| Port | Adapter | Kind | Technology | Project | Class | Source |
|---|---|---|---|---|---|---|
| User | Web App | Real | Blazor WebAssembly UI | `Motif.Memory.App` | - | Context diagram: Developer |
| User | Specs | Checks | xUnit | `Motif.Memory.Specs` | - | Added in the sketch |
| Memory Files | Browser Files | Real | Folder picker or zip download (JS interop) | `Motif.Memory.MemoryFiles` | `BrowserFilesMemoryFiles` | Context diagram: Memory Repo |
| Memory Files | Fake Memory Files | Fake | - | `Motif.Memory.Fakes` (`MemoryFiles/`) | `FakeMemoryFiles` | Added in the sketch |

For each driven adapter, add the stub class named above implementing its port's interface, in its project's namespace and in the folder shown. For example:

```csharp
namespace Motif.Memory.Adapters;

public sealed class BrowserFilesMemoryFiles : MemoryFiles
{
}
```

Driving adapters (the app among them) keep their project template's default files. Checks and fakes on driving ports are the specs (section 7).

Adapter notes:

- **Web App** (`Motif.Memory.App`): The setup page: pick roles, shape the workflow, pick and mix design guidelines, then write the memory out
- **Specs** (`Motif.Memory.Specs`): Checks that drive the application through the User port, using the fake driven adapters
- **Browser Files** (`Motif.Memory.MemoryFiles`): Writes the files into a folder the Developer picks, or downloads them as a zip where the browser can't

## 7. Fakes and specs

- `Motif.Memory.Fakes` (`Checks/Motif.Memory.Fakes/`): every fake driven adapter, with a folder per port (`MemoryFiles/`). The namespace is flat: `Motif.Memory.Fakes`.
- `Motif.Memory.Specs` (`Checks/Motif.Memory.Specs/`): xUnit. Packages: `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `coverlet.collector`, `AwesomeAssertions`. Keep the template's packages and add `AwesomeAssertions`.
  Add one placeholder spec class with a single passing `[Fact]`, so `dotnet test` runs. Specs wire the fake adapters up by hand; there is no composition root yet.
- The browser adapters (`Motif.Memory.MemoryFiles`) also take `Microsoft.JSInterop`: each reaches the browser through a small JS module it ships as a static web asset.
- Use the latest stable package versions. Use AwesomeAssertions (`.Should()` style) in specs.

## 8. Not placed

Every element of the context diagram is on the sketch.

## 9. Build this

Instructions for the coding agent:

1. Create `Motif.Memory.sln` (classic `.sln` format: `dotnet new sln --format sln`).
2. Create every project in section 2 with its template, in its folder, and add it to the solution in the matching solution folder (`dotnet sln add <project> --solution-folder <folder>`, or `--in-root` for top-level projects). Set each project's `RootNamespace`.
3. Delete template placeholder files (`Class1.cs`, `UnitTest1.cs`, and the `razorclasslib` samples (`Component1.razor`, `ExampleJsInterop.cs`, the files in `wwwroot/`)), except `Class1.cs` in `Motif.Memory.Domain`: keep it, with its namespace changed to `Motif.Memory`, to show the namespace choice.
4. Add exactly the project references in section 3, and the packages in section 7.
5. Add a root `.editorconfig` with a naming rule that says interfaces are plain PascalCase, so the IDE doesn't ask for an `I` prefix:

   ```ini
   root = true

   [*.cs]
   dotnet_naming_rule.interfaces_no_prefix.symbols = interfaces
   dotnet_naming_rule.interfaces_no_prefix.style = pascal_case
   dotnet_naming_rule.interfaces_no_prefix.severity = warning

   dotnet_naming_symbols.interfaces.applicable_kinds = interface
   dotnet_naming_symbols.interfaces.applicable_accessibilities = *

   dotnet_naming_style.pascal_case.capitalization = pascal_case
   ```

6. Add the port interfaces and the application stub from section 5, and the adapter stub classes from section 6.
7. Add the placeholder spec from section 7.
8. Check that `dotnet build` and `dotnet test` both succeed.

Ground rules:

- **Stubs only.** No functionality, no interface members, no wiring.
- **No composition root** (no host project, no dependency injection setup) in this version.
- **No `I` prefix** on interfaces. Implementations carry the meaningful extra (`FileSystemStorage`), never `Impl`.
- **Two-letter acronyms stay in capitals** in code names (`IndexedDB`, `UI`).
- Don't add projects, references or packages the plan doesn't list.
