# PRN232 Assignment 1 - FUNewsManagement

## Architecture
- **Backend**: .NET 8.0 Web API
- **Frontend**: ASP.NET Core MVC (.NET 8.0)
- **Database**: SQL Server
- **Entity Framework Core**: Code First approach

## Instructions to run

1. **Database Setup**:
   - Open SSMS and create a new database `FUNewsManagement`.
   - Run the provided `Database.sql` script to create tables.
   - Alternatively, use `dotnet ef database update` in the `ass01` project folder to run migrations if migrations are configured.
   
2. **Backend Setup (API)**:
   - Go to `ass01` folder.
   - Update `appsettings.json` with your connection string: `Server=(local);uid=sa;pwd=12345;database=FUNewsManagement;TrustServerCertificate=True`.
   - Run `dotnet run`. The backend API will start (usually at `https://localhost:7111` or `http://localhost:5037`).
   
3. **Frontend Setup (MVC)**:
   - Go to `ass01_FE/ass01_FE` folder.
   - Update `appsettings.json` to point `ApiSettings:BaseUrl` to the backend URL above.
   - Run `dotnet run`. Access the application via the provided localhost port.
   
## Deliverables
- The entire source code structure with separated API and MVC projects.
- `Database.sql` file containing the schema definition.
- `README.md` file (this file) with running instructions.
