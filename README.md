# Rest API with ASP.NET Core

## Description
an API User Order with ASP.NET Core
* CRUD User

## Getting Started

### Dependencies

* ASP.NET Core API
* SQL Server
* Entity Framework
* Fluent Validation
* Swagger

### Installation

1. Clone the repository:
```
git clone https://github.com/umiatiyah/UserOrdersAPI.git
```

2. Setup appsettings.json
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local)\\SQLEXPRESS;Database=UserOrderDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

3. Migration database:
to migrate the database and tables can use ef migrations or run the query in file querymigration.sql
#### ef migrations
```
dotnet ef migrations add UserOrderMigration
```
```
dotnet ef database update
```
#### query SQL
``` 
CREATE DATABASE UserOrderDb
```
``` 
USE UserOrderDb
```
then open file querymigration.sql and execute it.

### Executing program

```
dotnet run
```

### Credits

- Author: [@umiatiyah](https://github.com/umiatiyah/UserOrdersAPI.git)