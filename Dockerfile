# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# all that we need
COPY src/model/. ./model/
COPY src/core/. ./core/
COPY src/dataaccess/. ./dataaccess/
COPY src/auth/. ./auth/

# restore and publish
WORKDIR /source/auth
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "auth.dll"]