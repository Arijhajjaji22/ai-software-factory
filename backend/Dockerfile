FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie les fichiers csproj d'abord pour profiter du cache Docker
COPY AiSoftwareFactory.Api/*.csproj AiSoftwareFactory.Api/
COPY AiSoftwareFactory.Agents/*.csproj AiSoftwareFactory.Agents/

RUN dotnet restore AiSoftwareFactory.Api/AiSoftwareFactory.Api.csproj

# Copie tout le reste du code
COPY . .

WORKDIR /src/AiSoftwareFactory.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AiSoftwareFactory.Api.dll"]