
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy central package configuration
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy Gateway project file
COPY ApiGateway/CareBridge.ApiGateway/CareBridge.ApiGateway.csproj ApiGateway/CareBridge.ApiGateway/

# Restore NuGet packages
RUN dotnet restore ApiGateway/CareBridge.ApiGateway/CareBridge.ApiGateway.csproj

# Copy source code
COPY . .

# Publish application
RUN dotnet publish ApiGateway/CareBridge.ApiGateway/CareBridge.ApiGateway.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "CareBridge.ApiGateway.dll"]
