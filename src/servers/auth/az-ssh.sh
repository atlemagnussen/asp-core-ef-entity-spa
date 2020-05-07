#!/usr/bin/env bash
echo "Try to open ssh to asp-core-auth-server"
az webapp ssh --resource-group dotnet-core --name asp-core-auth-server
