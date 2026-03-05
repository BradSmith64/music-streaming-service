# Music Streaming Service - Docker Guide

This guide provides instructions for managing the local development environment using Docker and performing manual database operations.

## Managing Containers

### Start the environment
Starts the SQL Server (Azure SQL Edge) container in the background.
```powershell
docker-compose up -d
```

### First-Time / Fresh Setup
Whenever you start a new container (e.g., after `docker-compose down`), you must recreate the schema and seed the data:

1. **Wait for SQL Server to start:** Check `docker logs sql1` to ensure it's ready.
2. **Apply Migrations:**
   ```powershell
   dotnet ef database update --project music-streaming-infrastructure --startup-project music-streaming-minimal-api
   ```
3. **Seed Data:**
   ```powershell
   Get-Content seed.sql | docker exec -i sql1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Password123
   ```

### Stop the environment
Stops and removes the containers and networks defined in the compose file.
```powershell
docker-compose down
```

### View running containers
```powershell
docker ps
```

### View container logs
Useful for troubleshooting database startup issues.
```powershell
docker logs sql1
```

## Database Operations

### Query the database interactively (sqlcmd)
To enter an interactive SQL shell inside the container:
```powershell
docker exec -it sql1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Password123
```
Once inside, you can run T-SQL commands. Remember to type `GO` to execute them.
Example:
```sql
USE [music-streaming];
SELECT * FROM Songs;
GO
```

### Run a specific SQL command from the host
You can pipe commands directly into the container without entering the interactive shell:
```powershell
echo "SELECT * FROM [music-streaming].dbo.Songs" | docker exec -i sql1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Password123
```

### Seed the database
If you need to reset or re-seed your data using the provided `seed.sql` script:
```powershell
Get-Content seed.sql | docker exec -i sql1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Password123
```

## External Tools
You can connect to the database using tools like **Azure Data Studio** or **SQL Server Management Studio (SSMS)**:

- **Server:** `localhost,1433`
- **Authentication:** `SQL Server Authentication`
- **User:** `sa`
- **Password:** `YourStrong!Password123` (Check your root `.env` file)
- **Trust Server Certificate:** `True`
- **Database:** `music-streaming`
