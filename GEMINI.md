# Music Streaming Service - Project Overview

A full-stack music streaming platform built with a .NET 8 backend and a Next.js (React 19) frontend. The project demonstrates modern software architecture patterns, including CQRS-lite, repository patterns, and a standardized SQL Server database setup.

## Project Structure

- `music-streaming-minimal-api/`: The main entry point for the backend. Uses .NET 8 Minimal APIs.
  - `extensions/`: Contains custom extension methods (e.g., for Swagger documentation).
  - **`DEPLOYMENT_GUIDE.md`**: Detailed instructions for Azure deployment.
- `music_streaming_service_frontend/` (Sibling directory): The frontend application built with Next.js 16, React 19, Tailwind CSS v4, and Shadcn UI.
- `music-streaming-domain/`: Core business logic and domain entities (Songs, Likes).
- `music-streaming-application/`: Contains use cases, query handlers, and command handlers (CQRS-lite).
- `music-streaming-infrastructure/`: Data access implementations, Entity Framework Core context (`AppDbContext`), and adapters (Azure Blob Storage).

## Key Technologies

- **Backend:** .NET 8 (retargeted for Azure compatibility), Entity Framework Core, SQL Server (via Azure SQL Edge), Azure Blob Storage.
- **Frontend:** Next.js 16, React 19, TypeScript, Tailwind CSS v4, Radix UI, Lucide React, Shadcn UI components.
- **API Documentation:** Swashbuckle (Swagger).
- **Database:** SQL Server (Azure SQL Edge) is used for local development via Docker. The system is architected to align with production SQL Server instances on Azure.

## Building and Running

### Prerequisites
- .NET 8 SDK (or later)
- Node.js (Latest LTS recommended)
- **Docker Desktop:** Required for running the local SQL Server instance.

### Infrastructure (Docker)
1. Ensure Docker Desktop is running.
2. Start the database: `docker-compose up -d`
   - This starts an **Azure SQL Edge** container (a lightweight version of SQL Server).
3. **First-time/New Container Setup:** Whenever you start a fresh container (e.g., after `docker-compose down`), you must recreate the database schema and seed the initial data:
   - **Apply Migrations:** `dotnet ef database update --project music-streaming-infrastructure --startup-project music-streaming-minimal-api`
   - **Seed Data:** `Get-Content seed.sql | docker exec -i sql1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P <YourPassword>`

### Backend
1. Navigate to the backend directory: `cd music-streaming-minimal-api`
2. Restore dependencies: `dotnet restore`
3. Run the application: `dotnet run`
   - The API will be available at `http://localhost:5119`.
   - Swagger API Reference: `http://localhost:5119/swagger`

### Frontend
1. Navigate to the frontend directory: `cd ../music_streaming_service_frontend`
2. Install dependencies: `npm install`
3. Run the development server: `npm run dev`
   - The frontend will be available at `http://localhost:3000`.

## Azure Deployment (Backend)

For detailed, step-by-step instructions on the "Clean and Deploy" strategy for the backend, please refer to:
**`music-streaming-minimal-api/DEPLOYMENT_GUIDE.md`**

## Development Conventions

- **CQRS-lite:** Logic is separated into Use Cases (Commands) and Query Handlers.
- **Repository Pattern:** Infrastructure implements repository interfaces defined in the domain/application layer.
- **Unified Connection Strings:** The application uses a single `DefaultConnection` string defined in `appsettings.Development.json` for local development.
- **CORS:** The API is configured to allow `http://localhost:3000` and Azure production origins.

## Configuration

- **Backend:** 
  - `appsettings.json`: Contains Azure Blob Storage configuration.
  - `appsettings.Development.json`: Contains the `DefaultConnection` string for the local SQL Edge instance.
- **Frontend:** `.env.local` contains the `NEXT_PUBLIC_ENV_URL` pointing to the backend API.
- **Environment:** A root `.env` file stores the SQL Server SA password and port used by Docker.

## Database Migrations and Seeding (Azure)

### Migrations
To apply migrations to the Azure SQL Database, temporarily update the connection string in `appsettings.Development.json` and run:
```bash
dotnet ef database update --project music-streaming-infrastructure --startup-project music-streaming-minimal-api
```

### Seeding
Use the provided `seed.sql` script (updating the `USE [database]` name as necessary for Azure) via a SQL client that has firewall access to the Azure SQL Server.
