FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7500

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderService_Redis.API/OrderService_Redis.API.csproj", "OrderService_Redis.API/"]
COPY ["RedisConfiguration/RedisConfiguration.csproj", "RedisConfiguration/"]
RUN dotnet restore "./OrderService_Redis.API/OrderService_Redis.API.csproj"
COPY . .
WORKDIR "/src/OrderService_Redis.API"
RUN dotnet build "./OrderService_Redis.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderService_Redis.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService_Redis.API.dll"]
