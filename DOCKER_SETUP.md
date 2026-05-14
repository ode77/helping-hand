# Docker Development Workflow for Helping Hand

This setup enables you to develop the Helping Hand project entirely within Docker containers using VS Code Dev Containers.

## Prerequisites

- **Docker Desktop** (v20.10+) – [Download](https://www.docker.com/products/docker-desktop)
- **VS Code** (v1.75+) – [Download](https://code.visualstudio.com/)
- **VS Code Extension**: Dev Containers – Install from Extensions marketplace

## Quick Start

### Option 1: Using VS Code Dev Containers (Recommended)

1. **Install the Dev Containers extension** in VS Code if you don't have it:
   - Press `Ctrl+Shift+X` → Search "Dev Containers" → Install the Microsoft extension

2. **Open the project in a container**:
   - Press `Ctrl+Shift+P` → Search "Dev Containers: Reopen in Container"
   - VS Code will build and start the containers automatically
   - Wait for the terminal to show "Dev container running"

3. **First time setup**:
   - The dev container runs `dotnet restore` and `dotnet build` automatically
   - Once built, your app is ready at `http://localhost:5000`

4. **Access the running app**:
   - Open browser to `http://localhost:5000`
   - The SQL Server database is automatically initialized

5. **To stop**:
   - Press `Ctrl+Shift+P` → "Dev Containers: Reopen Locally"
   - Or close the dev container window

---

### Option 2: Manual Docker Compose (Without Dev Containers)

If you prefer to not use Dev Containers, you can run docker-compose directly from your host machine:

```bash
# Build and start all services
docker-compose up --build

# In another terminal, apply database migrations
docker-compose exec web dotnet ef database update

# Access the app at http://localhost:5000
```

---

## Available VS Code Tasks

Press `Ctrl+Shift+B` to see all build tasks, or `Ctrl+Shift+P` → search "Run Task":

| Task | Purpose |
|------|---------|
| **Docker: Build and Start** | Build Docker image and start containers (Default) |
| **Docker: Start (without rebuild)** | Start containers without rebuilding image |
| **Docker: Stop** | Stop and remove all containers |
| **Docker: Logs** | Stream logs from all containers |
| **Docker: Rebuild database (EF migrations)** | Run Entity Framework Core migrations |
| **Docker: Run tests** | Run unit tests in container |

---

## Development Workflow

### Inside Dev Container

Once in the dev container, you can:

1. **Edit code normally** – Any changes sync automatically to the container
2. **Run the app** – Use `dotnet run` in the integrated terminal
3. **Debug** – Set breakpoints in C# files and press `F5` (debugger should auto-attach)
4. **Manage database** – Use EF Core commands:
   ```bash
   dotnet ef migrations add MigrationName
   dotnet ef database update
   ```
5. **Install NuGet packages** – `dotnet add package PackageName`

### Database Access

- **SQL Server is running on port 1433** (inside and outside container)
- **Connection string**: `Server=localhost,1433;Database=HelpingHandDb;User Id=sa;Password=DevPassword123!;...`
- **Connect from SSMS or VS Code**:
  - Server: `localhost,1433`
  - User: `sa`
  - Password: `DevPassword123!`

---

## Common Tasks

### Apply Database Migrations
```bash
# Inside dev container terminal
dotnet ef database update

# Or use VS Code task: Docker: Rebuild database (EF migrations)
```

### Add a New Migration
```bash
dotnet ef migrations add YourMigrationName
```

### View Container Logs
```bash
# In a VS Code terminal
docker-compose logs -f web   # App logs
docker-compose logs -f sqlserver  # Database logs
```

### Rebuild Everything from Scratch
```bash
# Stop containers
docker-compose down -v

# Rebuild with fresh database
docker-compose up --build
```

### Connect to Container Bash
```bash
docker-compose exec web bash

# Or directly execute commands
docker-compose exec web dotnet --version
```

---

## Troubleshooting

### "Dev Container failed to start"
1. Check Docker Desktop is running
2. Press `Ctrl+Shift+P` → "Dev Containers: Reopen Locally"
3. Try again

### "Application won't start – connection string error"
1. Verify SQL Server is healthy: `docker-compose ps`
2. Check SQL Server logs: `docker-compose logs sqlserver`
3. Rebuild: `docker-compose down -v && docker-compose up --build`

### "Port 5000 already in use"
1. Edit `docker-compose.yml` and change `5000:5000` to `5001:5000` (or any free port)

### "Database migrations fail"
1. Ensure SQL Server is healthy and running
2. Check connection string in `appsettings.json` or `docker-compose.yml`
3. Manually run: `docker-compose exec web dotnet ef database update --verbose`

### "Files aren't syncing to container"
1. Check Docker Desktop settings: Ensure the workspace directory is shared
2. Restart Dev Container: `Ctrl+Shift+P` → "Dev Containers: Rebuild Container"

---

## Production Considerations

This setup is **development-focused** with:
- ✅ Live code sync via volume mounts
- ✅ Development SQL Server (Express edition)
- ✅ Debug-friendly environment variables
- ⚠️ Weak default passwords (for dev only!)

**For production**, you would:
1. Use strong passwords from environment variables
2. Use Azure SQL Database or managed SQL Server instead of container
3. Remove volume mounts (immutable containers)
4. Use non-root user in Dockerfile
5. Add proper health checks and logging
6. Set `ASPNETCORE_ENVIRONMENT=Production`

---

## File Structure

```
helping-hand/
├── Dockerfile                 # Multi-stage build for .NET 8
├── docker-compose.yml        # Web app + SQL Server services
├── .dockerignore              # Files excluded from Docker build
├── .devcontainer/
│   └── devcontainer.json      # VS Code dev container config
├── .vscode/
│   └── tasks.json             # VS Code build tasks
├── .env.example               # Environment variable template
├── HelpingHand/
│   ├── HelpingHand.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── ...
└── ...
```

---

## Next Steps

1. ✅ Copy these files to your repo root (already done)
2. ✅ Ensure Docker Desktop is running
3. **Open this folder in VS Code**
4. **Press `Ctrl+Shift+P` → "Dev Containers: Reopen in Container"**
5. Wait for setup to complete (~2-3 minutes on first run)
6. Open `http://localhost:5000` in your browser

That's it! Happy coding! 🚀

---

## Useful Docker Commands Reference

```bash
# View running containers
docker-compose ps

# Stop everything
docker-compose down

# Stop and remove volumes (full reset)
docker-compose down -v

# Rebuild image without cache
docker-compose build --no-cache

# Execute command in running container
docker-compose exec web dotnet --version

# View container logs
docker-compose logs -f web

# Interactive bash shell in container
docker-compose exec web bash

# Remove dangling images and volumes
docker system prune -a
```

---

**Questions?** See [Docker documentation](https://docs.docker.com) or [Dev Containers guide](https://code.visualstudio.com/docs/devcontainers/containers).
