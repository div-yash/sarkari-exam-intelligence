# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for cached restore
COPY SarkariExamIntelligence.slnx ./
COPY src/SarkariExamIntelligence.Domain/SarkariExamIntelligence.Domain.csproj src/SarkariExamIntelligence.Domain/
COPY src/SarkariExamIntelligence.Application/SarkariExamIntelligence.Application.csproj src/SarkariExamIntelligence.Application/
COPY src/SarkariExamIntelligence.Infrastructure/SarkariExamIntelligence.Infrastructure.csproj src/SarkariExamIntelligence.Infrastructure/
COPY src/SarkariExamIntelligence.API/SarkariExamIntelligence.API.csproj src/SarkariExamIntelligence.API/

RUN dotnet restore src/SarkariExamIntelligence.API/SarkariExamIntelligence.API.csproj

# Copy source and publish Release build
COPY src/ src/
RUN dotnet publish src/SarkariExamIntelligence.API/SarkariExamIntelligence.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Install curl for Docker HEALTHCHECK and create writable directories for uploads/logs
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/wwwroot/notifications /app/logs \
    && chmod -R 775 /app/wwwroot /app/logs

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD curl -fsS http://localhost:8080/api/health || exit 1

ENTRYPOINT ["dotnet", "SarkariExamIntelligence.API.dll"]
