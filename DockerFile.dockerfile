FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source/
COPY . . 
RUN dotnet publish "RepairShop.sln" -o "/app/"

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

RUN apt-get update 
RUN apt-get install -y unzip
RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg

RUN apt-get install -y openssl

RUN openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes -subj '/CN=localhost'

RUN openssl pkcs12 -export -out aspnetapp.pfx -inkey key.pem -in cert.pem -passout pass:password

RUN mkdir /https && mv aspnetapp.pfx /https/

COPY wait-for-it.sh /app/
RUN chmod +x /app/wait-for-it.sh  # Make the script executable

WORKDIR /app/
COPY --from=build /app/ .
COPY wait-for-it.sh /app/

EXPOSE 7005/tcp 
EXPOSE 5134/tcp 

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=https://+:5134
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx

RUN chmod +x /app/wait-for-it.sh

ENTRYPOINT [ "/app/wait-for-it.sh", "kafka1:29092", "--", "dotnet", "RepairShop.API.dll"]