#!/usr/bin/env bash
npm run build
# requires az login
az storage blob upload-batch -s public -d \$web --account-name mytestspaclient
