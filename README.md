# asp.net core ef is4 spa
asp.net core, entity framework core, identityserver4, single page web app  

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

## Javascript client
[Examples from identityserver](http://docs.identityserver.io/en/latest/quickstarts/4_javascript_client.html)

## Entity framework and auth server db
```sh
dotnet ef migrations add InitialApplicationDbContext -c ApplicationDbContext -o Data/Migrations/ApplicationDb
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/PersistedGrantDb

dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c PersistedGrantDbContext
```


## Auth server copy lib files to wwwroot
```sh
npm install
npm run copy
```