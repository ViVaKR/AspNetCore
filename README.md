# AspNetCore

## Start

```bash
  dotnet new webapi -o COM.ViVaKR.API
  cd COM.ViVaKR.API

  dotnet tool install --global dotnet-ef
  dotnet tool update --global dotnet-ef
  dotnet ef

  dotnet add package

  dotnet add package Microsoft.EntityFrameworkCore
  dotnet add package Microsoft.EntityFrameworkCore.SqlServer
  dotnet add package Microsoft.EntityFrameworkCore.Tools
  dotnet add package Microsoft.EntityFrameworkCore.Design
  dotnet add package Microsoft.EntityFrameworkCore.InMemory
  # For Scafoldding
  dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
  # Create Model Class
  dotnet new class -n PaymentDetail -o ./Models

```

```bash

  dotnet tool install -g dotnet-aspnet-codegenerator

  # Controller PaymentDetail Scaffolding (뼈대) Cli
  dotnet aspnet-codegenerator controoler -name PaymentDetailController -async -api -m PaymentDetail -dc PaymentDetailContext -outDir Controllers

  # Controller `TodoItem` Scaffolding (뼈대)
  dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc PaymentDetailContext -outDir Controllers

  // Migration : 변경이 있은 후 업데이트 표준 명령어
  dotnet ef migrations add InitialCreate
  dotnet build
  dotnet ef database update

  # First Run : Solution Root with https
  dotnet run --project <projectName> --launch-profile https

```

## HTTPS Start

>- appsettings.json

```json
    "Kestrel": {
        "Endpoints": {
            "Https": {
                "Url": "https://localhost:54321"
            }
        }
    },
    ...
```

- .NET: Generate Assets for Build and Debug
- `dotnet dev-certs https --trust`
- `dotnet run --project COM.ViVaKR.API --launch-profile https`
- <https://localhost:7291/swagger>

## DTO

- 과도한 게시 방지
- 클라이언트에 표시되지 않아야 하는 속성을 숨기
- 페이로드 크기를 줄이기 위해 일부 속성을 생략
- 중첩된 개체를 포함하는 개체 그래프를 평면화

## Casing

- Pascal casing : 첫글자와 중간 글자 대문자, ASP.NET Core API, (MyName, PhoneNumber)
- Camel casing : 첫글자는 소문자 중간글자는 대문자 Angular , (myName, phoneNumber)
- Snake casing : 모든 단어는 소문자 또는 대문자 단어는 '_' 언더바로 구분, (my_name, phone_number)
- Kebab casing : 단어를 '-' 대시로 구분, (my-name, phone-number)
- Hungurian Notation : 접두어에 자료형을 붙임, C, (strUserName, iUserAge)

## Create Database & User Login

```sql
USE master
GO

CREATE DATABASE ViVaKR
GO

ALTER DATABASE ViVaKR SET QUERY_STORE=ON;

ALTER DATABASE ViVaKR
    COLLATE Korean_Wansung_CI_AS
GO

Select name, collation_name
From sys.databases
Where name = N'ViVaKR'

USE ViVaKR
GO

EXEC sp_configure 'default language', 29
GO
RECONFIGURE
GO

USE master
ALTER DATABASE ViVaKR SET RECOVERY FULL
GO

ALTER DATABASE ViVaKR SET MULTI_USER
GO

USE [master]
GO

CREATE LOGIN [ViVaKR]
 WITH PASSWORD=N'비밀번호',
 DEFAULT_DATABASE=[ViVaKR],
 DEFAULT_LANGUAGE=[Korean],
 CHECK_EXPIRATION=OFF,
 CHECK_POLICY=OFF
GO

USE [ViVaKR]
GO
CREATE USER [ViVaKR] FOR LOGIN [ViVaKR]
GO

ALTER USER [ViVaKR] WITH DEFAULT_SCHEMA=[dbo]
GO

ALTER ROLE [db_owner] ADD MEMBER [ViVaKR]
GO

```

## Code First EntityFramework Migrations

```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update

```

## Web UI

- Server Rendered
    - MVC
    - Razor Pages
    - Client Rendered (SPA)
    - `Blazor`

## Backend Services

- Microservies
    - Web APIs
    - SignalR
    - gRPC
    - Workers

## What is Blazor?

> Blazor is a framework for building interactive client-side web UI with .NET

## What is WebAssembly?

> WebAssembly is a web technology that allows code written in different languages to run in a browser
> A single-page application(SPA) framework for building interactive client-side web apps with .NET

SPA : Angular, React, Vue

---

## Razor Component

- `.NET`
- `WebAssembly`
- `MyApp.dll`
    - C# + HTML
    - Component 2
    - Component 3
