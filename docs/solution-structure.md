# AppSandbox Solution Structure

## Overview

A .NET 10 multi-web-project solution following the [davidfowl layout](https://gist.github.com/davidfowl/ed7564297c61fe9ab814). Local development is orchestrated by .NET Aspire. Azure hosting uses App Service (F1 free tier, upgradeable to B1).

## Folder Structure

```
app-sandbox/
  src/
    AppSandbox.AppHost/         ← Aspire orchestrator
    AppSandbox.ServiceDefaults/ ← shared observability/health checks
    AppHub/                     ← Razor Pages navigation hub
  tests/
  docs/
  build/
  artifacts/
  infra/
    main.bicep                  ← F1 App Service Plan + App Service
  .vscode/
    launch.json                 ← "Run AppSandbox (Aspire)" + "Run AppHub only"
    tasks.json                  ← default build task
  .editorconfig
  .gitattributes
  .gitignore
  global.json
  NuGet.config
  AppSandbox.slnx
```

## Running Locally

F5 in VS Code → select **"Run AppSandbox (Aspire)"** → Aspire dashboard opens with AppHub listed.

Alternatively, run AppHub in isolation using the **"Run AppHub only"** launch configuration.

## Adding a Future Web Project

1. Scaffold the project:
   ```bash
   dotnet new razor -n MyNewApp -o src/MyNewApp
   ```
2. Add ServiceDefaults reference to `src/MyNewApp/MyNewApp.csproj`:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\AppSandbox.ServiceDefaults\AppSandbox.ServiceDefaults.csproj" />
   </ItemGroup>
   ```
3. Call `builder.AddServiceDefaults()` in `src/MyNewApp/Program.cs`.
4. Register in `src/AppSandbox.AppHost/AppHost.cs`:
   ```csharp
   builder.AddProject<Projects.MyNewApp>("mynewapp");
   ```
5. Add to the solution:
   ```bash
   dotnet sln add src/MyNewApp/MyNewApp.csproj
   ```
6. Add a Bicep module in `infra/` for its App Service (see `infra/main.bicep` as a reference).

## Deploying to Azure

### First-time setup

```bash
az group create -n rg-appsandbox -l eastus
az deployment group create -g rg-appsandbox -f infra/main.bicep
```

### Upgrading from F1 to B1

When the free tier limits are hit (60 CPU min/day, no custom domain), upgrade in one command:

```bash
az appservice plan update --name <plan-name> --resource-group rg-appsandbox --sku B1
```

No redeployment needed — apps keep running on the upgraded tier immediately.

## Prerequisites

- .NET 10 SDK
- `dotnet workload install aspire` (or Aspire NuGet packages — workload is deprecated in .NET 10)
- Azure CLI (`az login`) for infrastructure deployment
