# Music Streaming Service - Project Overview

A full-stack music streaming platform built with a .NET 10 backend and a Next.js (React 19) frontend. The project demonstrates modern software architecture patterns, including CQRS-lite, repository patterns, and a standardized SQL Server database setup.

## Project Structure

- `music-streaming-minimal-api/`: The main entry point for the backend. Uses .NET 10 Minimal APIs.
  - `extensions/`: Contains custom extension methods (e.g., for OpenAPI/Scalar documentation).
- `music_streaming_service_frontend/` (Sibling directory): The frontend application built with Next.js 16, React 19, Tailwind CSS v4, and Shadcn UI.
- `music-streaming-domain/`: Core business logic and domain entities (Songs, Likes).
- `music-streaming-application/`: Contains use cases, query handlers, and command handlers (CQRS-lite).
- `music-streaming-infrastructure/`: Data access implementations, Entity Framework Core context (`AppDbContext`), and adapters (Azure Blob Storage).

## Key Technologies

- **Backend:** .NET 10, Entity Framework Core, SQL Server (via Azure SQL Edge), Azure Blob Storage.
- **Frontend:** Next.js 16, React 19, TypeScript, Tailwind CSS v4, Radix UI, Lucide React, Shadcn UI components.
- **API Documentation:** Microsoft.AspNetCore.OpenApi + Scalar UI.
- **Database:** SQL Server (Azure SQL Edge) is used for local development via Docker. The system is architected to align with production SQL Server instances on Azure.

## Building and Running

### Prerequisites
- .NET 10 SDK
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
   - Scalar API Reference: `http://localhost:5119/scalar/v1`

### Frontend
1. Navigate to the frontend directory: `cd ../music_streaming_service_frontend`
2. Install dependencies: `npm install`
3. Run the development server: `npm run dev`
   - The frontend will be available at `http://localhost:3000`.

## Development Conventions

- **CQRS-lite:** Logic is separated into Use Cases (Commands) and Query Handlers.
- **Repository Pattern:** Infrastructure implements repository interfaces defined in the domain/application layer.
- **Unified Connection Strings:** The application uses a single `DefaultConnection` string defined in `appsettings.Development.json` for local development.
- **CORS:** The API is configured to allow `http://localhost:3000`.

## Configuration

- **Backend:** 
  - `appsettings.json`: Contains Azure Blob Storage configuration.
  - `appsettings.Development.json`: Contains the `DefaultConnection` string for the local SQL Edge instance.
- **Frontend:** `.env.local` contains the `NEXT_PUBLIC_ENV_URL` pointing to the backend API.
- **Environment:** A root `.env` file stores the SQL Server SA password and port used by Docker.

## Database Migrations and Seeding

### Migrations
The project uses EF Core Migrations for SQL Server. To apply them to a fresh database:
```bash
dotnet ef database update --project music-streaming-infrastructure --startup-project music-streaming-minimal-api
```

### Seeding
Initial data (mock songs and likes) can be seeded using the `seed.sql` script executed directly in the Docker container:
```powershell
Get-Content seed.sql | docker exec -i sql1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P <YourPassword>
```
