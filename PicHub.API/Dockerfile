FROM mcr.microsoft.com/dotnet/aspnet:8.0.3 AS base
WORKDIR /app
RUN adduser --disabled-password --gecos '' appuser

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0.3 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["PicHub.API.csproj", "./"]
RUN dotnet restore "./PicHub.API.csproj"

COPY . .
RUN dotnet build "./PicHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PicHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PicHub.API.dll"]