# Buddham.co.kr

## Commands

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet tool update --global dotnet-aspnet-codegenerator

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package MailKit

dotnet ef migration add Init
dotnet ef database update
dotnet ef database update -- --environment Production
dotnet ef database drop


dotnet watch --launch-profile https
dotnet watch run --project  ./HelloWorld.csproj
dotnet watch run -- arg0
dotnet watch -- run arg0

# Jwt
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.IdentityModel.Tokens
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Swashbuckle.AspNetCore.Filters

# packages update check and update
dotnet new update --check-only
dotnet new update

dotnet user-jwts create --project Buddham.API
dotnet user-jwts list
dotnet user-jwts print id --show-all

# Controller
dotnet new apicontroller -n AccountController -o Controllers --actions true
```

## Added

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
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite

# 스캐폴드 (package -> Microsoft.EntityFrameworkCore.SqlServer)
dotnet aspnet-codegenerator controller -name CodeController -async -api -m Code -dc ApiDbContext -outDir Controllers
dotnet aspnet-codegenerator controller -name BibleController -async -api -m BibleModel -dc BibleContext -outDir Controllers
dotnet aspnet-codegenerator controller -name CategoryController -async -api -m Category -dc BibleContext -outDir Controllers
dotnet aspnet-codegenerator controller -name AccountController -async -api -m Category -dc BibleContext -outDir Controllers
dotnet aspnet-codegenerator controller -name TodayMessageController -async -api -m TodayMessage -dc BibleContext -outDir Controllers

dotnet aspnet-codegenerator controller -name PlayGroundController -async -api -m PlayGround -dc BuddhaContext -outDir Controllers

dotnet aspnet-codegenerator controller -name TodaySutraController -async -api -m TodaySutra -dc BuddhaContext -outDir Controllers

dotnet aspnet-codegenerator controller -name GuestCommentsController -async -api -m GuestComment -dc BuddhaContext -outDir Controllers
dotnet aspnet-codegenerator controller -name SutraImageController -async -api -m SutraImage -dc BuddhaContext -outDir Controllers

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

/*
--> (1) dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
--> (2) dotnet ef migrations add InitialCreate --output-dir Data/Migrations
 */
