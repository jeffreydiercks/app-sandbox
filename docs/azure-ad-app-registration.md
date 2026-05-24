# Azure AD App Registration

**App name:** AppSandbox  
**Client ID:** c2e2687b-af23-4e02-94c4-ec2b997a129a  
**Supported account types:** Personal Microsoft accounts only (`TenantId: consumers`)

---

## Redirect URIs

Configured under **Authentication → Web platform** in the [Entra portal](https://entra.microsoft.com).

### Local development

| App | Sign-in redirect URI | Sign-out redirect URI |
|---|---|---|
| AppHub | `https://localhost:7089/signin-oidc` | `https://localhost:7089/signout-callback-oidc` |
| MyVerses | `https://localhost:7059/signin-oidc` | `https://localhost:7059/signout-callback-oidc` |

> Aspire is configured with `.WithLaunchProfile("https")` on both projects, binding AppHub to port 7089 and MyVerses to port 7059 over HTTPS. Requires a trusted dev certificate (`dotnet dev-certs https --trust`).

### Production (add when deploying)

| App | Sign-in redirect URI |
|---|---|
| AppHub | `https://app-appsandbox-apphub-dev.azurewebsites.net/signin-oidc` |
| MyVerses | `https://app-appsandbox-myverses-dev.azurewebsites.net/signin-oidc` |

---

## Client Secret

The client secret value is **not** stored in source control. It is stored in:

- **Local dev:** .NET user secrets in each project
  ```
  dotnet user-secrets set "AzureAd:ClientSecret" "<value>" --project src/AppHub
  dotnet user-secrets set "AzureAd:ClientSecret" "<value>" --project src/MyVerses
  ```
- **Production:** Set manually after deployment
  ```
  az webapp config appsettings set -g <resource-group> -n <app-name> --settings AzureAd__ClientSecret=<value>
  ```

Use the **Secret Value** (not the Secret ID) from the Entra portal.

---

## Where ClientId is stored in source

- `src/AppHub/appsettings.json` → `AzureAd:ClientId`
- `src/MyVerses/appsettings.json` → `AzureAd:ClientId`
- `infra/main.bicep` → `AzureAd__ClientId` app setting on both App Services
