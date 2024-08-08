# API

## EntityFrameworkdCore

```bash

# PostgreSQL Code First
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design

# Create Database and User
-- create database "vivabm" with owner "vivabm" ENCODING 'UTF8'
-- create user vivabm with encrypted password 'Password'
-- grant all privileges on database vivabm to vivabm;
```
