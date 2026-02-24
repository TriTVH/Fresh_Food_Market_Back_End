# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AccService.API/AccService.API.csproj", "AccService.API/"]
COPY ["AccService.Service/AccService.Service.csproj", "AccService.Service/"]
COPY ["AccService.Repository/AccService.Repository.csproj", "AccService.Repository/"]
COPY ["AccService.Model/AccService.Model.csproj", "AccService.Model/"]
COPY ["JwtConfiguration/JwtConfiguration.csproj", "JwtConfiguration/"]
RUN dotnet restore "./AccService.API/AccService.API.csproj"
COPY . .
WORKDIR "/src/AccService.API"
RUN dotnet build "./AccService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AccService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccService.API.dll"]