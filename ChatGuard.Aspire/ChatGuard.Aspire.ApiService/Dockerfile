# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChatGuard.Aspire/ChatGuard.Aspire.ApiService/ChatGuard.Aspire.ApiService.csproj", "ChatGuard.Aspire/ChatGuard.Aspire.ApiService/"]
COPY ["ChatGuard.Infrastructure/ChatGuard.Infrastructure.csproj", "ChatGuard.Infrastructure/"]
COPY ["ChatGuard.Domain/ChatGuard.Domain.csproj", "ChatGuard.Domain/"]
COPY ["ChatGuard.Aspire/ChatGuard.Aspire.ServiceDefaults/ChatGuard.Aspire.ServiceDefaults.csproj", "ChatGuard.Aspire/ChatGuard.Aspire.ServiceDefaults/"]
RUN dotnet restore "./ChatGuard.Aspire/ChatGuard.Aspire.ApiService/ChatGuard.Aspire.ApiService.csproj"
COPY . .
WORKDIR "/src/ChatGuard.Aspire/ChatGuard.Aspire.ApiService"
RUN dotnet build "./ChatGuard.Aspire.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChatGuard.Aspire.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app

ENV ApiId=""
ENV ApiHash=""
ENV BotToken=""

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatGuard.Aspire.ApiService.dll"]