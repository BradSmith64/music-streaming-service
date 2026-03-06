# Azure Deployment Guide: .NET 8 Backend

This guide details the definitive process for deploying the `music-streaming-minimal-api` to the Azure App Service (Linux). 

## The "Clean and Deploy" Strategy

To ensure a successful deployment on the Azure F1 (Free) tier and avoid issues with polluted build environments or malformed zip archives on Linux, always follow these steps:

### 1. Clean and Publish locally
Navigate to the `music-streaming-minimal-api` folder and run these commands to ensure only the necessary .NET 8 files are included:

```powershell
# Delete existing publish artifacts
Remove-Item -Path "publish", "publish.zip" -Recurse -Force -ErrorAction SilentlyContinue

# Publish fresh for Linux x64 (Framework-dependent)
dotnet publish -c Release -r linux-x64 --no-self-contained -o ./publish
```

### 2. Zip the CONTENTS
You must zip the **contents** of the `publish` folder, not the folder itself. This ensures the `.dll` and `runtimeconfig.json` files are at the root of the archive for the App Service to find.

```powershell
Push-Location publish
Compress-Archive -Path * -DestinationPath ../publish.zip -Force
Pop-Location
```

### 3. Deploy via Azure CLI
Use the `az webapp deploy` command. This is more reliable than the deprecated `config-zip` command for Linux hosts:

```powershell
az webapp deploy --resource-group rg-music-streaming-poc --name app-api-music-65395c94 --src-path publish.zip --type zip
```

---

## Troubleshooting

- **400/500 Errors during warm-up**: If the CLI returns an error but you see the files in `wwwroot` via the Kudu console, simply **Restart the App Service**. The deployment often succeeds even if the warm-up phase times out.
- **Runtime Mismatch**: Ensure the project targets `net8.0`. If you see .NET 10 errors in the logs, perform a `dotnet clean` before publishing.
- **Swagger missing**: Ensure `ASPNETCORE_ENVIRONMENT` is set to `Development` in the App Service settings.
