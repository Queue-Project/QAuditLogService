FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5152

ENV ASPNETCORE_URLS=http://+:5152
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["LocalNuget/", "/src/LocalNuget/"]
COPY ["nuget.config", "/src/"]


COPY ["QAuditLogService/QAuditLogService.API/QAuditLogService.API.csproj", "QAuditLogService.API/"]
COPY ["QAuditLogService/QAuditLogService.Infrastructure/QAuditLogService.Infrastructure.csproj", "QAuditLogService.Infrastructure/"]
COPY ["QAuditLogService/QAuditLogService.Domain/QAuditLogService.Domain.csproj", "QAuditLogService.Domain/"]
COPY ["QAuditLogService/QAuditLogService.Contracts/QAuditLogService.Contracts.csproj", "QAuditLogService.Contracts/"]
COPY ["QAuditLogService/QAuditLogService.Application/QAuditLogService.Application.csproj", "QAuditLogService.Application/"]
RUN dotnet restore "QAuditLogService.API/QAuditLogService.API.csproj"
COPY . .
WORKDIR "/src/QAuditLogService/QAuditLogService.API"
RUN dotnet build "./QAuditLogService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./QAuditLogService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QAuditLogService.API.dll"]
