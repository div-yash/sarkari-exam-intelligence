# syntax=docker/dockerfile:1
# Root Dockerfile automatically detected by Render.com (Docker Runtime)

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for cached restore
COPY backend/SarkariExamIntelligence.slnx backend/
COPY backend/src/SarkariExamIntelligence.Domain/SarkariExamIntelligence.Domain.csproj backend/src/SarkariExamIntelligence.Domain/
COPY backend/src/SarkariExamIntelligence.Application/SarkariExamIntelligence.Application.csproj backend/src/SarkariExamIntelligence.Application/
COPY backend/src/SarkariExamIntelligence.Infrastructure/SarkariExamIntelligence.Infrastructure.csproj backend/src/SarkariExamIntelligence.Infrastructure/
COPY backend/src/SarkariExamIntelligence.API/SarkariExamIntelligence.API.csproj backend/src/SarkariExamIntelligence.API/

RUN dotnet restore backend/src/SarkariExamIntelligence.API/SarkariExamIntelligence.API.csproj

# Copy source and publish Release build
COPY backend/src/ backend/src/
RUN dotnet publish backend/src/SarkariExamIntelligence.API/SarkariExamIntelligence.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/wwwroot/notifications /app/logs \
    && chmod -R 775 /app/wwwroot /app/logs

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Bind dynamically to Render's $PORT (defaults to 8080 locally)
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} exec dotnet SarkariExamIntelligence.API.dll"]
