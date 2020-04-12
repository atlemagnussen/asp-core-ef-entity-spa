# asp.net core ef is4 spa
asp.net core, entity framework core, identityserver4, single page web app  

## How to run
- First the two sql databases, can be anything that EntityFrameworkCore supports, update your connection strings in appsettings.json
- go to src/auth and run `dotnet run` should be `https://localhost:6001`
- go to src/webapi and run `dotnet run` should be `https://localhost:5001`
- go to src/webclient and run `npm run dev` should be `https://localhost:8080`

Try to make a simple implementation of: 
- a standalone auth server using identityserver4, entityframework core and sql server
- stand alone web api protected by the auth server
- statically hosted spa client that logs in and consumes the web api resources
- mobile app that logs in and consumes the web api resources


https://localhost:6001/.well-known/openid-configuration

# Notes

## Grant types
Should use [Authorization Code](https://oauth.net/2/grant-types/authorization-code/) with [PKCE](https://oauth.net/2/pkce/) and [Refresh token](https://oauth.net/2/grant-types/refresh-token/)

[IdentityServer4 grant types](http://docs.identityserver.io/en/latest/topics/grant_types.html)


## Database/EntityFramework
### Install cli tool
```sh
# install cli tool
dotnet tool install --global dotnet-ef
```

### Create Initials
These are already done, just keeping the commands if they need to be redone
```sh
#auth
dotnet ef migrations add InitialAuthDbContext -c AuthDbContext -o Migrations/AuthDb
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Migrations/PersistedGrantDb
#dataprotection
dotnet ef migrations add InitialDataProtectionKeys -c DataProtectionDbContext -o Migrations/DataProtectionDb
#webapi
dotnet ef migrations add InitialBankContext -c BankContext -o Migrations/BankDb
```

### Create dbs
```sh
# auth
dotnet ef database update -c AuthDbContext
dotnet ef database update -c PersistedGrantDbContext
#dataprotection
dotnet ef database update -c DataProtectionDbContext
#webapi
dotnet ef database update -c BankContext
```
```

## Javascript client
[Examples from identityserver](http://docs.identityserver.io/en/latest/quickstarts/4_javascript_client.html)


## Auth server copy lib files to wwwroot
```sh
npm install
npm run copy
```