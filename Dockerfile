FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV AWS_ACCESS_KEY_ID=some_access_key
ENV AWS_SECRET_ACCESS_KEY=some_secret
ENV AWS_DEFAULT_REGION=some_region

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PicHub.API/PicHub.API.csproj", "PicHub.API/"]
RUN dotnet restore "./PicHub.API/PicHub.API.csproj"
COPY . .
WORKDIR "/src/PicHub.API"
RUN dotnet build "./PicHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PicHub.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PicHub.API.dll"]