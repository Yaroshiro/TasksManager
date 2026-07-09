# TasksManager

TasksManager is a sample task-tracking web application built with .NET 10 using Razor Pages. It provides user authentication, role-based authorization, and CRUD operations for task management backed by Entity Framework Core.

Summary

- Web application demonstrating core ASP.NET Core capabilities: Razor Pages, Identity, EF Core, and role-based authorization.
- Intended as a runnable sample showing authentication, data access, validation, and a responsive UI.

Tech stack

- .NET 10 (ASP.NET Core Razor Pages)
- ASP.NET Core Identity for local user accounts and roles
- Entity Framework Core (migrations)
- SQL Server (default) or SQLite (configurable via connection string)
- Bootstrap and minimal client-side scripts (jQuery used for unobtrusive validation)

Features

- User registration and login (ASP.NET Core Identity)
- Role support (Leader, Member) and role-aware UI
- Task CRUD: create, read, update, delete
- Server- and client-side validation
- EF Core migrations and optional data seeding on startup
- Static assets served from wwwroot

Prerequisites

- .NET 10 SDK: https://dotnet.microsoft.com
- A local database: SQL Server, LocalDB, or SQLite
- Visual Studio 2022/2026, Visual Studio Code, or the dotnet CLI

Getting started (development)

1. Clone the repository

   git clone <repo-url>
   cd TasksManager

2. Configure the connection string

   - Edit appsettings.Development.json or appsettings.json and set ConnectionStrings:DefaultConnection to a valid connection string for SQL Server or SQLite.
   - Optionally use user secrets for local development:

	 dotnet user-secrets init
	 dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"

3. Apply EF Core migrations

   From the project folder run:

	 dotnet ef database update

   The application may run a data seeder on first startup if configured.

4. Run the application

   - From the command line:

	 dotnet run --project TasksManager.csproj

   - Or open the solution in Visual Studio and press F5.

Authentication & roles

- The application uses ASP.NET Core Identity for account management.
- Register a new account at /Identity/Account/Register or use seeded accounts if provided.
- Two role types are used in the UI: Leader and Member. Role membership affects available actions and views.

Configuration

- Environment-specific configuration is supported via appsettings.{Environment}.json and user secrets.
- Identity options and password requirements are configured in Program.cs.
- Background services (if present) are registered in the application startup.

Running with Docker (optional)

- Containerization can be added by creating a Dockerfile and docker-compose to include the application and a database container.

Tests

- If tests are included in the repository, run them with your chosen test runner (dotnet test, Visual Studio Test Explorer, etc.).

Contributing

- Fork the repository and open pull requests for changes. Follow existing code style and conventions.

License

This project is licensed under the MIT License. See LICENSE.txt for details.


