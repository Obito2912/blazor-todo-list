# My Quest

My Quest is a responsive, authenticated todo application built with .NET 10 and Blazor Interactive Server. Each user can create, edit, search, filter, complete, and delete only their own tasks.

## Features

- Email/password registration and sign-in with Student and Teacher roles
- Per-user task storage with ownership checks on every service operation
- Create, edit, delete, and completion workflows backed by SQLite
- Search and All, Pending, and Completed filters
- Profile, profile-image URL, and password management
- Responsive desktop and mobile layouts
- EF Core migrations applied automatically at startup
- Automated service security and form validation tests

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQLite is bundled through the NuGet provider; no separate database server is required

## Run locally

```powershell
dotnet restore
dotnet run
```

The development profile uses `Data Source=app.db`. Open the HTTP or HTTPS address printed by `dotnet run` (normally `http://localhost:5209` or `https://localhost:7259`). The first startup creates the database, applies migrations, and creates the Student and Teacher roles.

## Build and test

```powershell
dotnet build
dotnet test Tests/blazor-todo-list.Tests.csproj
```

The tests use an isolated in-memory SQLite database. They verify CRUD persistence, user ownership boundaries, and form validation.

## Database migrations

After changing an entity, install the EF tool if needed and create a migration:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add DescribeYourChange
dotnet ef database update
```

Do not commit local `app.db`, `app.db-wal`, or `app.db-shm` files.

## Docker

```powershell
docker build -t my-quest .
docker run --rm -p 8080:8080 -v my-quest-data:/app/data my-quest
```

The production connection string points to `/app/data/app.db`; mount that directory or use a named volume to preserve data across containers.

## Architecture

- `Components/Pages` contains routed Blazor pages.
- `Components` contains reusable task and form UI.
- `Services/TaskService.cs` owns user-scoped task persistence and validation.
- `Services/AccountService.cs` wraps ASP.NET Core Identity operations.
- `Data` contains the EF Core context and entities.
- `Migrations` contains the SQLite schema history.
- `Tests` contains the xUnit test project.

Task routes require authentication. The task service also filters mutations and reads by both task ID and authenticated user ID, preventing one user from accessing another user's records even if an ID is guessed.

## Team

- Rakell Bandeira
- Ovinson Abel Lugo Rosado
- Nyantakyi Francis
- Herzan Carcache Huerta
- Emmanuel Oluwatosin Ologe
