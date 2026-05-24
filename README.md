# app-sandbox

Personal .NET 10 Razor Pages sandbox, starting with a single navigation hub app.

## Layout

Structured from the recommended .NET solution layout:

- `/src` product apps
- `/tests` test projects
- `/docs` docs and notes
- `/samples` optional examples
- `/lib` non-NuGet assets
- `/build` build customizations
- `/artifacts` build outputs
- `/packages` local package artifacts

## Current app

- `src/AppSandbox` - Razor Pages index app that acts as the jump-off navigation point for future apps.

## Run locally

```bash
dotnet build app-sandbox.slnx
dotnet run --project src/AppSandbox/AppSandbox.csproj
```

## Azure deployment target

The current app is a standard ASP.NET Core Razor Pages app and can be deployed to low-cost Azure App Service plans (including free/shared tiers when available).