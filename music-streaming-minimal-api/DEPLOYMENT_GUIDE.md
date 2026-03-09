# Azure Deployment Guide: .NET 8 Backend

This guide details the automated process for deploying the `music-streaming-minimal-api` to the Azure App Service (Linux) using GitHub Actions.

## Automated Deployment (CI/CD)

The backend is configured to automatically build and deploy whenever changes are pushed to the `main` branch.

### 1. GitHub Actions Workflow
The workflow is defined in `.github/workflows/azure-web-app.yml`. It performs the following steps:
- Restores and builds the .NET 8 solution.
- Publishes the application specifically for **linux-x64**.
- Deploys the artifacts to the Azure App Service using a **Publish Profile**.

### 2. Automated Secret Management (Terraform)
To enable the pipeline, the following secrets are automatically provisioned in your GitHub repository by the **Terraform Infrastructure** code:

- `AZURE_CLIENT_ID`: The Application (client) ID of the Service Principal.
- `AZURE_CLIENT_SECRET`: The Client Secret value of the Service Principal.
- `AZURE_TENANT_ID`: The Directory (tenant) ID.
- `AZURE_SUBSCRIPTION_ID`: Your Azure Subscription ID.
- `AZURE_WEBAPP_NAME`: The name of the generated App Service (e.g., `app-api-music-xxxx`).

### 3. Manual Configuration (Optional)
If you are not using Terraform, you must manually create an Azure Service Principal and add these values to **Settings > Secrets and variables > Actions**.

### 4. Manual Trigger
You can also trigger a deployment manually from the **Actions** tab in GitHub by selecting the "Build and Deploy .NET 8 Backend to Azure" workflow and clicking **Run workflow**.

---

## Troubleshooting

- **Deployment Logs**: Check the **Actions** tab in GitHub for detailed build and deployment logs.
- **Kudu Console**: For file-level verification on the server, visit `https://<app-name>.scm.azurewebsites.net/DebugConsole`.
- **Runtime Mismatch**: Ensure the project targets `net8.0`.
- **Swagger missing**: Ensure `ASPNETCORE_ENVIRONMENT` is set to `Development` in the App Service settings via the Azure Portal.
