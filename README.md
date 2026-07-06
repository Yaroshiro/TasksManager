# TasksManager

A Razor Pages task management sample application built with .NET 10.

Summary
- Simple, opinionated task tracker demonstrating Razor Pages, ASP.NET Core Identity, and EF Core with migrations.
- Good starter project for a junior .NET developer portfolio: authentication, CRUD, data access, validation, and responsive UI.

Tech stack
- .NET 10 (ASP.NET Core Razor Pages)
- Entity Framework Core (migrations)
- ASP.NET Core Identity (local accounts)
- SQL Server or SQLite (configurable via connection string)
- jQuery + jQuery Validation for client-side form validation

Getting started (Development)
1. Prerequisites
   - .NET 10 SDK installed: https://dotnet.microsoft.com
   - A local database (SQL Server, LocalDB, or SQLite)
   - Visual Studio 2022/2026 or VS Code

2. Clone repository
   git clone <repo-url>
   cd TasksManager

3. Configure connection string
   - Open appsettings.Development.json or appsettings.json and set the DefaultConnection to a valid connection string.
   - Optionally use User Secrets for local credentials: dotnet user-secrets init; dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-conn>"

4. Apply migrations and seed database
   - From the project folder run:
	 dotnet ef database update
   - If the project includes a data seeder it will run on startup; otherwise add seed data or create an account via the UI.

5. Run the app
   - From command line: dotnet run --project TasksManager.csproj
   - Or open the solution in Visual Studio and press F5.

Authentication
- The app uses ASP.NET Core Identity.
- Register a new user on the /Identity/Account/Register page or seed an admin account.

Features
- Authenticated user accounts with registration and login
- CRUD for tasks (create, read, update, delete)
- Input validation (server + client-side)
- EF Core migrations and a simple repository/service layering (check Services/ or Data/ folders)
- Static assets in wwwroot (CSS, JS, client validation)

Tests & CI
- Add unit tests with xUnit or NUnit to showcase TDD and business logic testing.
- Add a GitHub Actions workflow that builds the solution and runs tests on push.

Deployment
- Containerize with Docker (add Dockerfile and docker-compose for a complete demo)
- Deploy to Azure App Service or Azure Container Instances for a live demo link

What to add to make this portfolio-ready
- README with screenshots / demo GIF and a short design write-up (this file)
- Automated tests (unit + integration)
- CI pipeline (GitHub Actions or Azure Pipelines)
- A production-ready appsettings.Production.json and migration/seed documentation
- Simple API endpoints or Swagger to demonstrate backend skills

Contributing
- Create issues and PRs. Follow the repository coding conventions.

License
- This software is licensed under the MIT License.

You are free to use, copy, modify, merge, publish, distribute, sublicense, and sell copies of this software, subject to the terms of the MIT License.

