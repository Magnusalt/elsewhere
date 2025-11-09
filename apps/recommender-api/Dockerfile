FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "apps/recommender-api/recommender-api.csproj"
RUN dotnet publish "apps/recommender-api/recommender-api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "recommender-api.dll", "--urls", "http://0.0.0.0:8080"]