# Music Streaming Service - Docker Guide

This guide provides instructions for managing the local development environment using Docker and performing manual database operations.

## Backend Runtime Configuration (.NET 8.0)

**Important Note:** The backend services have been retargeted from .NET 10.0 to **.NET 8.0** to ensure compatibility with Azure App Service Linux runtimes and the **Azure F1 (Free) tier**. This ensures reliable deployment and lower operational costs for the POC.

## Managing Containers

### Start the environment
Starts the SQL Server (Azure SQL Edge) container in the background.
```powershell
docker-compose up -d
```
...
