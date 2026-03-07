# Music Streaming Service - Backend

A high-performance, scalable music streaming platform built with a **.NET 8** Minimal API. This project demonstrates modern software architecture patterns and is fully integrated with Microsoft Azure for production hosting.

## 🚀 Tech Stack

### Core Frameworks
- **Runtime:** .NET 8 (LTS) — Chosen for long-term support and optimal performance on Azure Linux App Services.
- **API Style:** Minimal APIs — Provides a lightweight, high-performance routing layer with minimal boilerplate.
- **ORM:** Entity Framework Core (EF Core) 8 — Handles relational data mapping and migrations.

### Data & Storage
- **Primary Database:** SQL Server.
  - **Production:** Azure SQL Database (Serverless tier for cost-efficiency).
  - **Local Development:** Azure SQL Edge via Docker.
- **Asset Storage:** Azure Blob Storage — Scalable object storage for high-bitrate audio files (`.mp3`, `.wav`).
- **Access Control:** Shared Access Signatures (SAS) for secure, time-limited access to media assets.

## 🏗️ Architectural Patterns

The system follows a **CQRS-lite** and **Hexagonal (Ports & Adapters)** approach to ensure maintainability and testability:

- **Application Layer:** Contains Use Cases (Commands) and Query Handlers, decoupling business logic from the delivery mechanism.
- **Infrastructure Layer:** Implements Repository and Query Service interfaces (Ports) via Entity Framework or Mock implementations (Adapters).
- **Domain Layer:** Defines the core entities (`Song`, `Like`) and domain-specific logic.
- **Minimal API:** Acts as the entry point, mapping HTTP routes to Application Layer handlers.

---

## 🐳 Local Development (Docker)

To simplify local setup, the project uses **Docker** to provide a consistent SQL environment.

### Prerequisites
- **Docker Desktop**
- A root `.env` file containing:
  ```env
  MSSQL_PORT=1433
  MSSQL_SA_PASSWORD=YourStrong!Password123
  ```

### Running Locally
1. **Start Database:** `docker-compose up -d` (Starts Azure SQL Edge).
2. **Apply Migrations:** `dotnet ef database update --project music-streaming-infrastructure --startup-project music-streaming-minimal-api`
3. **Seed Data:** Run the `seed.sql` script against `localhost:1433`.
4. **Run API:** `dotnet run --project music-streaming-minimal-api`

*Note: Docker is used strictly for the local database container; the API itself runs on the host during development for faster iteration.*

---

## ☁️ Production Infrastructure (Azure)

While local development relies on Docker, the production environment is a fully managed **Platform-as-a-Service (PaaS)** stack:

- **Compute:** **Azure App Service (Linux)** — Hosts the .NET 8 API.
- **Database:** **Azure SQL Database** — A fully managed relational database with automated backups and scaling.
- **Storage:** **Azure Storage Account** — Dedicated blob containers for media assets.
- **CI/CD:** **GitHub Actions** — Automated pipelines for building, testing, and deploying the backend on every push to `main`.

For detailed infrastructure definitions, see the `/music_streaming_cloud_architecture` directory.
