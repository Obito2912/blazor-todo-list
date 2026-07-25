# My Quest — Auth Setup & Deployment

## What was added

- **Student/Teacher accounts** via ASP.NET Core Identity (`Data/ApplicationUser.cs`, `AccountRole` enum).
- **Register** (`/register`) — mirrors the "I am joining as a..." card picker, with Student/Teacher instead of Buyer/Artisan.
- **Login** (`/login`) — email + password, show/hide password, "remember me", forgot-password stub.
- **Logout** (`/logout`) — signs out and redirects home.
- **Profile edit** (`/profile`) — update full name/role, change password. Requires sign-in.
- `/tasks` and `/tasks/new` now require sign-in (`@attribute [Authorize]`); anonymous visitors are redirected to `/login` and bounced back after signing in.
- Data layer: `AppDbContext` (EF Core + Identity tables) backed by SQLite.

## 1. Restore packages (do this first, on a machine with internet/NuGet access)

This sandbox can't reach nuget.org, so the exact package versions in the `.csproj` haven't been restore-tested. Run this in the project folder to pull whatever is current and fix up the csproj automatically:

```bash
cd MyQuest
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet restore
```

If `dotnet restore` complains about the versions already pinned in the `.csproj` (`10.0.0`), delete those `<PackageReference>` lines first and re-run the commands above — they'll re-add the right versions for your installed SDK.

## 2. Run it locally

```bash
dotnet run
```

- First run creates `myquest.db` (SQLite) in the project folder automatically (via `Database.EnsureCreated()` in `Program.cs`) — no migration step needed to get started.
- Visit `/register`, create a Student or Teacher account, then `/tasks`.

If you'd rather use EF Core migrations (recommended once you start changing the data model):

```bash
dotnet tool install --global dotnet-ef   # once
dotnet ef migrations add InitialCreate
dotnet ef database update
```
Then replace `db.Database.EnsureCreated();` in `Program.cs` with `db.Database.Migrate();`.

## 3. Configuration

Connection string lives in `appsettings.json` / `appsettings.Development.json` under `ConnectionStrings:DefaultConnection`. In development it points at a local file (`myquest.db`); in the shipped `appsettings.json` it points at `/app/data/myquest.db` for the Docker setup below.

For production, don't rely on a local SQLite file, prefer either:
- A persistent volume/disk on your host (see Docker section), or
- A managed database (swap `UseSqlite` for `UseNpgsql`/`UseSqlServer` and add the matching EF Core provider package).

## 4. Deploy with Docker

Build and run:

```bash
docker build -t myquest .
docker run -d --name myquest \
  -p 8080:8080 \
  -v myquest-data:/app/data \
  myquest
```

- The container exposes port `8080`; map it to whatever the host needs.
- `-v myquest-data:/app/data` gives the SQLite file a named volume so accounts and tasks survive container restarts/redeploys. Without it, a new container = an empty database.
- Put the container behind a reverse proxy (Caddy/Nginx/your host's load balancer) for TLS in production, since the app itself is only listening on plain HTTP internally.

## 5. Deploying to a PaaS (Azure App Service, Render, Railway, Fly.io, etc.)

- Any of these can build directly from the `Dockerfile` above — point the platform at this repo and it should just work.
- Make sure you attach persistent storage for `/app/data` (or switch to a managed database) — most PaaS container filesystems are ephemeral between deploys.
- Set `ASPNETCORE_ENVIRONMENT=Production` (already set in the Dockerfile) and confirm HTTPS is terminated by the platform.

## 6. Secrets

`UserSecretsId` is set in the `.csproj` for local dev secrets (`dotnet user-secrets`). Don't commit real connection strings/passwords — the SQLite default here has none, but if you switch to SQL Server/Postgres in production, store that connection string as an environment variable or platform secret, not in `appsettings.json`.
