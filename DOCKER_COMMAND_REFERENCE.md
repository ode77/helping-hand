# Docker Command Reference - Quick Guide

## Essential Daily Commands

### Start the Application Stack
```powershell
cd C:\Users\samod\source\repos\helping-hand
docker-compose up -d
```
✅ Starts both web app and SQL Server in background

### Check Status
```powershell
docker-compose ps
```
Shows all containers, their status, and port mappings

### View Live Logs
```powershell
# All services
docker-compose logs -f

# Just the web app
docker-compose logs -f web

# Just the database
docker-compose logs -f sqlserver
```
Press `Ctrl+C` to stop viewing logs

### Stop Everything
```powershell
docker-compose down
```
✅ Gracefully stops all containers (database volume preserved)

---

## Database Management

### Connect to SQL Server
```powershell
# Using sqlcmd (if installed)
sqlcmd -S localhost,1433 -U sa -P "DevPassword123!" -Q "SELECT name FROM sys.databases"

# Or using Docker
docker-compose exec sqlserver sqlcmd -U sa -P "DevPassword123!" -Q "SELECT 1"
```

### Run Entity Framework Migrations
```powershell
# Add a new migration
dotnet ef migrations add AddNewFeature

# Apply pending migrations
dotnet ef database update

# Show migration history
dotnet ef migrations list
```

### Execute SQL Query
```powershell
docker-compose exec sqlserver sqlcmd -U sa -P "DevPassword123!" -Q "SELECT COUNT(*) FROM AspNetUsers"
```

---

## Troubleshooting & Rebuilding

### Rebuild Docker Image (After Code Changes)
```powershell
docker-compose build
```
Rebuilds image without starting containers

### Rebuild and Start Fresh
```powershell
docker-compose up --build -d
```
Rebuilds image and starts containers

### Full Reset (Delete Everything)
```powershell
docker-compose down -v
```
⚠️ Removes containers, networks, AND volumes (database deleted!)

### Rebuild After Full Reset
```powershell
docker-compose down -v
docker-compose up --build -d
```

---

## Container Operations

### Execute Command in Container
```powershell
# Check .NET version
docker-compose exec web dotnet --version

# Run tests
docker-compose exec web dotnet test

# Interactive bash shell
docker-compose exec web bash
```

### View Container Details
```powershell
# Detailed container info
docker-compose ps --no-trunc

# Just the app container
docker ps | Select-String helping-hand-app

# Full logs from app
docker logs helping-hand-app

# Last 50 lines of logs
docker logs --tail 50 helping-hand-app
```

### Remove Specific Container
```powershell
docker-compose rm web    # Remove app container
docker-compose rm sqlserver  # Remove database container
```

---

## Image Management

### List Docker Images
```powershell
docker images | Select-String helping-hand
```

### Remove Image
```powershell
docker rmi helping-hand-web
```
⚠️ Must stop container first with `docker-compose down`

### Clean Up Unused Resources
```powershell
docker system prune -a
```
⚠️ Removes all unused images, containers, networks, volumes

---

## Network & Port Operations

### Test Port Connectivity
```powershell
# Test if app is responding on port 5000
Invoke-WebRequest -Uri "http://localhost:5000" -UseBasicParsing

# Test if database is responding on port 1433
Test-NetConnection -ComputerName localhost -Port 1433
```

### Change Ports
Edit `docker-compose.yml`:
```yaml
services:
  web:
    ports:
      - "5001:5000"    # Change 5000 to 5001

  sqlserver:
    ports:
      - "1434:1433"    # Change 1433 to 1434
```

---

## Development Workflow

### 1. Start Development Session
```powershell
cd C:\Users\samod\source\repos\helping-hand
docker-compose up -d
```

### 2. Edit Code
Use Visual Studio 2026 or VS Code - edit files normally

### 3. For C# Changes
```powershell
# Rebuild if needed
docker-compose build

# Restart container
docker-compose restart web
```

### 4. For Database Schema Changes
```powershell
# Create migration
dotnet ef migrations add YourMigrationName

# Apply migration
dotnet ef database update

# Verify in SQL Server
sqlcmd -S localhost,1433 -U sa -P "DevPassword123!" -Q "SELECT * FROM __EFMigrationsHistory"
```

### 5. Test Changes
- Visit http://localhost:5000
- Test in application
- Check logs: `docker-compose logs -f web`

### 6. End Development Session
```powershell
docker-compose down
```

---

## Useful One-Liners

```powershell
# Check if app is running
docker-compose ps | Select-String "Up"

# Get app container ID
docker-compose ps -q web

# Stream logs from past 10 minutes
docker logs --since 10m helping-hand-app

# Export database backup
docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P DevPassword123! -Q "BACKUP DATABASE [HelpingHandDb] TO DISK='/var/opt/mssql/backup/HelpingHandDb.bak'"

# Restart just the app
docker-compose restart web

# Restart just the database
docker-compose restart sqlserver

# View resource usage
docker stats --no-stream
```

---

## Environment Variables

Located in `docker-compose.yml`:

```env
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5000
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=HelpingHandDb;User Id=sa;Password=DevPassword123!;Encrypt=False;TrustServerCertificate=True;
SA_PASSWORD=DevPassword123!
MSSQL_PID=Express
```

---

## Visual Studio Integration

### Add Docker Support to Solution (Optional)
```powershell
# Add Docker files to project via Visual Studio
# Right-click Project → Add → Docker Support

# Or manually use our setup (already done)
```

### Debug in Container from VS 2026
1. Set startup project to your web project
2. Debug → Attach to Process
3. Select `HelpingHand.dll` from `helping-hand-app` container

---

## Tips & Tricks

✅ **Always check `docker-compose ps` first** - tells you what's running

✅ **Use `-f` flag with logs** - follows logs in real-time

✅ **Save frequently used commands** - Create a PowerShell profile script

✅ **Monitor disk space** - `docker system df` shows Docker disk usage

✅ **Use descriptive container names** - Makes debugging easier

⚠️ **Never modify Dockerfile manually in production** - Use version control

---

## Common Issues & Solutions

### "Port 5000 already in use"
```powershell
# Option 1: Change port in docker-compose.yml
# Option 2: Kill process on port 5000
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Option 3: Use different port
# Edit docker-compose.yml: "5001:5000"
```

### "Cannot connect to database"
```powershell
# Check SQL Server is running
docker-compose logs sqlserver | Select-String "ready for client"

# Wait longer for SQL Server to start
docker-compose restart sqlserver
Start-Sleep -Seconds 30
docker-compose ps
```

### "Application errors"
```powershell
# View full error log
docker logs helping-hand-app | Select-Object -Last 100

# Rebuild everything
docker-compose down -v
docker-compose up --build -d
```

---

## Quick Reference Card (Print This!)

```
╔════════════════════════════════════════════════════════════════╗
║            HELPING HAND DOCKER QUICK REFERENCE                ║
╠════════════════════════════════════════════════════════════════╣
║ START:    docker-compose up -d                                ║
║ STOP:     docker-compose down                                 ║
║ STATUS:   docker-compose ps                                   ║
║ LOGS:     docker-compose logs -f                              ║
║ REBUILD:  docker-compose up --build -d                        ║
║ RESET:    docker-compose down -v && docker-compose up -d      ║
║                                                                ║
║ DATABASE: localhost,1433 | User: sa | Pass: DevPassword123!   ║
║ WEB APP:  http://localhost:5000                               ║
╚════════════════════════════════════════════════════════════════╝
```

---

Last Updated: 2026-05-14
