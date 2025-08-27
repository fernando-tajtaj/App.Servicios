# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.sln .
COPY App.Servicios/*.csproj ./App.Servicios/
RUN dotnet restore
COPY App.Servicios/. ./App.Servicios/
WORKDIR /src/App.Servicios
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Copiar certificado de desarrollo (pfx) al contenedor
COPY aspnetapp.pfx /https/aspnetapp.pfx

# Configurar Kestrel para HTTPS
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_URLS=https://+:7293
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=passwordone

EXPOSE 7293

ENTRYPOINT ["dotnet", "App.Servicios.dll"]