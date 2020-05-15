# asp.net core ef is4 spa
IdentityServer4, asp.net core, entity framework core, single page web app, xamarin mobile app  

## Implementation details
Servers
- Stand alone authentication server using IdentityServer4
    - Asp.net core Identity with Entity Framework Core for persisting users and roles etc.
    - Local login, Azure AD and Google providers.
    - Asp.Net Core Data Protection encrypted with Azure key vault
    - Signing keys from Azure Key Vault - see [Signing-Key-POC](Signing-Key-POC.md)
- Web api with resources protected by the authentication server
    - customer CRUD api that requires authentication

Clients  
- SPA/web client with Oidc and Authorization Code flow
    - Can create new auth users and consumes customers api
- Mobile client Xamarin
    - consumes customers api

## How to run
- Create two sql databases, can be anything that EntityFrameworkCore supports, update your connection strings in appsettings.json, use same connection string on both if you prefer one db
- go to src/auth and run `dotnet run` should be `https://localhost:6001`
- go to src/webapi and run `dotnet run` should be `https://localhost:7001`
- go to src/webclient and run `npm run dev` should be `https://localhost:8000`

## Test
 - Auth server should display something on the default page `https://localhost:6001/`
 - Also the discovery document should show something `https://localhost:6001/.well-known/openid-configuration`

# Notes

## Grant types
Should use [Authorization Code](https://oauth.net/2/grant-types/authorization-code/) with [PKCE](https://oauth.net/2/pkce/)  
Web client should use [Silent refresh](https://github.com/IdentityModel/oidc-client-js/wiki)  
Mobile client should use [Refresh token](https://oauth.net/2/grant-types/refresh-token/)

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

### Azure Ad/Google
Redirect url:
`https://localhost:6001/signin-oidc`

## Javascript client
[Examples from identityserver](http://docs.identityserver.io/en/latest/quickstarts/4_javascript_client.html)


## Auth server copy lib files to wwwroot
```sh
npm install
npm run copy
```

## Azure Key vault stuff
[Good Microsoft article to get going](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-3.1#sample-app)

### app permissions Azure
Remember to turn on managed identity on your app and give it proper permissions, like this:
```sh
$ az keyvault set-policy --name {KEY VAULT NAME} --object-id {APP OBJECT ID} --secret-permissions get list
```