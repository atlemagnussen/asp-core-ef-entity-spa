# build
#docker build -t test.authserver .
# run
#docker run -it --rm -p 5000:80 test.authserver

# run docker with https
#dotnet dev-certs https -ep ${HOME}/.aspnet/https/authserver.pfx -p mysupersecretpassword666
#dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p mysupersecretpassword666

# with bash
#--entrypoint /bin/bash

#docker run -it --entrypoint /bin/bash \
docker run --rm -d \
-p 6000:80 -p 6001:443 \
-e ASPNETCORE_URLS="https://+;http://+" \
-e ASPNETCORE_HTTPS_PORT=6001 \
-e ASPNETCORE_Kestrel__Certificates__Default__Password="mysupersecretpassword666" \
-e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/authserver.pfx \
-v ${HOME}/.aspnet/https:/https/ test.authserver
