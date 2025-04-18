# API.VIVABM.COM

```bash

dotnet list package
dotnet tool list --global
dotnet-outdated
dotnet restore
dotnet restore --use-lock-file


chmod 644 ~/Library/LaunchAgents/com.vivakr.api.plist
launchctl load ~/Library/LaunchAgents/com.vivakr.api.plist

launchctl list | grep com.vivakr.api
ps aux | grep dotnet

cd ~/WebProjects/VIVAKR-COM/API-VIVAKR-COM/api.vivakr.com
dotnet ef migrations add SeedRoles


dotnet ef database update -- --environment Production
dotnet aspnet-codegenerator controller -name UserInfoController -async -api -m UserInfo -dc VivabmDbContext -outDir Controllers

dotnet aspnet-codegenerator controller -name QnAController -async -api -m QnA -dc VivabmDbContext -outDir Controllers
dotnet aspnet-codegenerator controller -name BackupManagerController -async -api -m BackupManager -dc VivabmDbContext -outDir Controllers

```
