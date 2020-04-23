#!/usr/bin/env bash

# this does not work
APPNAME="asp-core-webapi"	#$(az webapp list --query [1].name --output tsv)
APPRG="dotnet-core"		#$(az webapp list --query [0].resourceGroup --output tsv)
APPPLAN="linux-north-europe"	#$(az appservice plan list --query [0].name --output tsv)
APPSKU="B1"			#$(az appservice plan list --query [0].sku.name --output tsv)
APPLOCATION="North Europe"	#$(az appservice plan list --query [0].location --output tsv)
az webapp up --name $APPNAME --resource-group $APPRG --plan $APPPLAN --sku $APPSKU --location "$APPLOCATION"
