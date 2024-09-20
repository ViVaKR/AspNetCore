# API.TEXT.OR.KR

```bash
# Add-Migration
# Update-Database
# Remove-Migration
# Get-Migration
# Get-DbContext
# Drop-Database
# Bundle-Migration
# Get-Help EntityFramework
# Optimize-DbContext
# Script-DbContext
# Script-Migration
# Scaffold-DbContext

dotnet tool install -g dotnet-aspnet-codegenerator
dotnet tool update -g dotnet-aspnet-codegenerator
dotnet tool update --global dotnet-ef

dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

# 스캐폴드 (package -> Microsoft.EntityFrameworkCore.SqlServer)
dotnet aspnet-codegenerator controller -name CodeController -async -api -m Code -dc ApiDbContext -outDir Controllers
dotnet aspnet-codegenerator controller -name BibleController -async -api -m BibleModel -dc BibleContext -outDir Controllers
dotnet aspnet-codegenerator controller -name CategoryController -async -api -m Category -dc BibleContext -outDir Controllers
dotnet aspnet-codegenerator controller -name AccountController -async -api -m Category -dc BibleContext -outDir Controllers
dotnet aspnet-codegenerator controller -name TodayMessageController -async -api -m TodayMessage -dc BibleContext -outDir Controllers

$ dotnet ef migrations add InitialCreate --output-dir Data/Migrations
$ dotnet ef database update
```

## user-secrets

- `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Authentication.MicrosoftAccount
dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Microsoft.Data.SqlClient
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.IdentityModel.Tokens
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Swashbuckle.AspNetCore
dotnet add package Swashbuckle.AspNetCore.Filters
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

dotnet ef migrations add InitialCreate --output-dir Data/Migrations

#* publish
dotnet publish --configuration Release --output ~/WebServer/kr.or.text/api/
```
