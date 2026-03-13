# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=build /app/publish .

HEALTHCHECK --interval=60s --timeout=5s --retries=3 \
CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "NetSQLAPIActions.dll"]