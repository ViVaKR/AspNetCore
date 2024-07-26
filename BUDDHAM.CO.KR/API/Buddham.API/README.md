# Buddham.co.kr

## Commands

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet tool update --global dotnet-aspnet-codegenerator
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools

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
dotnet user-jwsts list
dotnet user-jwts print id --show-all

# Controller
dotnet new apicontroller -n AccountController -o Controllers --actions true
```
