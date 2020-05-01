# asp.net core ef is4 spa
asp.net core, entity framework core, identityserver4, single page web app  

## Implementation details
- Stand alone authentication server using IdentityServer4
- Set up one client with Oidc and Authorization Code flow
- Asp.net core Identity with Entity Framework Core for persisting users and roles etc.
- Set up local login, Azure AD and Google providers.
- Web api with resources protected by the authentication server
- Web client with oidc-client that calls the auth server and receiving access tokens
- Asp.Net Core Data Protection
- Signing keys from Azure Key Vault - see [Signing-Key-POC](Signing-Key-POC.md)

## How to run
- First the two sql databases, can be anything that EntityFrameworkCore supports, update your connection strings in appsettings.json
- go to src/auth and run `dotnet run` should be `https://localhost:6001`
- go to src/webapi and run `dotnet run` should be `https://localhost:5001`
- go to src/webclient and run `npm run dev` should be `https://localhost:8080`

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

### app permissions
Remember to turn on managed identity on your app and give it proper permissions, like this:
```sh
$ az keyvault set-policy --name {KEY VAULT NAME} --object-id {APP OBJECT ID} --secret-permissions get list
```