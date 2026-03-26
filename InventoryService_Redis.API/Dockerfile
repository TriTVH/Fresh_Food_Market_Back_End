FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7600

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["InventoryService_Redis.API/InventoryService_Redis.API.csproj", "InventoryService_Redis.API/"]
COPY ["RedisConfiguration/RedisConfiguration.csproj", "RedisConfiguration/"]
RUN dotnet restore "./InventoryService_Redis.API/InventoryService_Redis.API.csproj"
COPY . .
WORKDIR "/src/InventoryService_Redis.API"
RUN dotnet build "./InventoryService_Redis.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InventoryService_Redis.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService_Redis.API.dll"]
