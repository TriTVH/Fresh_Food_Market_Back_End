# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7700


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderService.API/OrderService.API.csproj", "OrderService.API/"]
COPY ["JwtConfiguration/JwtConfiguration.csproj", "JwtConfiguration/"]
COPY ["OrderService.Service/OrderService.Service.csproj", "OrderService.Service/"]
COPY ["OrderService.Repository/OrderService.Repository.csproj", "OrderService.Repository/"]
COPY ["OrderService.Model/OrderService.Model.csproj", "OrderService.Model/"]
RUN dotnet restore "./OrderService.API/OrderService.API.csproj"
COPY . .
WORKDIR "/src/OrderService.API"
RUN dotnet build "./OrderService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_HTTP_PORTS=7500
ENTRYPOINT ["dotnet", "OrderService.API.dll"]