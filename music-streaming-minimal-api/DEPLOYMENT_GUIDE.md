# Azure Deployment Guide: .NET 8 Backend

This guide details the automated process for deploying the `music-streaming-minimal-api` to the Azure App Service (Linux) using GitHub Actions.

## Automated Deployment (CI/CD)

The backend is configured to automatically build and deploy whenever changes are pushed to the `main` branch.

### 1. GitHub Actions Workflow
The workflow is defined in `.github/workflows/azure-web-app.yml`. It performs the following steps:
- Restores and builds the .NET 8 solution.
- Publishes the application specifically for **linux-x64**.
- Deploys the artifacts to the Azure App Service using a **Publish Profile**.

### 2. Required Secrets
To enable the pipeline, the following secret must be configured in your GitHub repository (**Settings > Secrets and variables > Actions**):
- `AZURE_WEBAPP_PUBLISH_PROFILE`: The XML publish profile downloaded from the Azure Portal (or via `az webapp deployment list-publishing-profiles`).

### 3. Manual Trigger
You can also trigger a deployment manually from the **Actions** tab in GitHub by selecting the "Build and Deploy .NET 8 Backend to Azure" workflow and clicking **Run workflow**.

---

## Troubleshooting

- **Deployment Logs**: Check the **Actions** tab in GitHub for detailed build and deployment logs.
- **Kudu Console**: For file-level verification on the server, visit `https://<app-name>.scm.azurewebsites.net/DebugConsole`.
- **Runtime Mismatch**: Ensure the project targets `net8.0`.
- **Swagger missing**: Ensure `ASPNETCORE_ENVIRONMENT` is set to `Development` in the App Service settings via the Azure Portal.
