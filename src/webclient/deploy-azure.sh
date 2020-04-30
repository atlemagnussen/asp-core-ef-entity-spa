#!/usr/bin/env bash
git pull
npm i
npm run build
# requires az login
az storage blob upload-batch -s public -d \$web --account-name mytestspaclient
