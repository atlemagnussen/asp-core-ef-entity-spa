#!/bin/env bash
dotnet publish -c release -o publish
cd publish
zip -r webapi.zip *

# requires az login
az webapp deployment source config-zip \
--src webapi.zip \
--resource-group dotnet-core \
--name asp-core-webapi

# cleanup
cd ..
rm -rf publish