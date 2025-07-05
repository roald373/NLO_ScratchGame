### About
Simplefied setup for a SupriseScratchGame


### Prequisites

```bash 
docker run -d \--hostname my-rabbit \--name rabbitmq \-p 5672:5672 \-p 15672:15672 \rabbitmq:3-management
```
```bash 
docker run -e "ACCEPT_EULA=Y" \-e "MSSQL_SA_PASSWORD=SuperStrongPassword1!" \-p 1433:1433 \--name sql1 \--hostname sql1 \-d mcr.microsoft.com/mssql/server:2025-latest
```
---
### 📦 Entity Framework Core Migrations

#### Create Initial Migration

```bash
dotnet ef migrations add InitialCreate --project NLO_ScratchGame_Database\NLO_ScratchGame_Database.csproj --startup-project NLOScratchGame_Worker\NLOScratchGame_Worker.csproj
```

#### Apply Migration to Database

```bash
dotnet ef database update --project NLO_ScratchGame_Database\NLO_ScratchGame_Database.csproj --startup-project NLOScratchGame_Worker\NLOScratchGame_Worker.csproj
```
