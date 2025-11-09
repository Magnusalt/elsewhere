FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["apps/gateway/gateway.csproj", "apps/gateway/"]
COPY ["common/dotnet-common/common-extensions/common-extensions.csproj", "common/dotnet-common/common-extensions/"]
RUN dotnet restore "apps/gateway/gateway.csproj"
COPY . .
WORKDIR "/src/apps/gateway"
RUN dotnet build "./gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gateway.dll"]
