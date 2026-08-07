# My Quest — Task Manager

A Blazor Server task management app with user accounts, built for CSE325.

## Tech Stack

- .NET 10 / Blazor Server (Interactive Server render mode)
- ASP.NET Core Identity (authentication, roles)
- Entity Framework Core + SQLite

## Features

- Register, log in, log out, with role-based accounts (Student / Teacher)
- Profile editing (username, password, profile image)
- Create, edit, and delete tasks
- Search tasks and filter by status
- Mark tasks complete/pending, with a dedicated Completed page
- Error logging across the data and service layers

## Getting Started

**Prerequisites:** .NET 10 SDK

1. Clone the repo
2. Restore packages: `dotnet restore`
3. Run the app: `dotnet run`

Database migrations are applied automatically on startup — no manual setup step needed. The app uses a local SQLite file (`app.db`), created automatically on first run.

## Test Account

A test account is seeded automatically on first run:

- Email: `admin@example.com`
- Password: `Admin@123`
- Role: Teacher

## Project Structure

- `Components/Pages` — Razor pages (auth, tasks, profile)
- `Data` — EF Core `DbContext` and models (`ApplicationUser`, `TaskItem`)
- `Services` — business logic (`AccountService`, `TaskService`)
- `Migrations` — EF Core database migrations
- `Tests` — unit and component tests

## Team

- Rakell Bandeira
- Ovinson Abel Lugo Rosado
- Nyantakyi Francis
- Herzan Carcache Huerta
- Emmanuel Oluwatosin Ologe

## Link to project on Render

[https://blazor-todo-list.onrender.com]
