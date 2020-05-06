#!/usr/bin/env bash
git pull
npm run build
dotnet publish -c release -o publish
cd publish
zip -r auth.zip *

# requires az login
az webapp deployment source config-zip --src auth.zip --resource-group dotnet-core --name asp-core-auth-server

# cleanup
cd ..
rm -rf publish
