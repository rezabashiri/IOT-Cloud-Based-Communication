FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

# Switch to root to modify permissions
USER root

# Add SSL certificate
COPY ../../aspnetapp.pfx /https/aspnetapp.pfx
RUN chmod 644 /https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=1234

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Broker/Broker.Interface/Broker.Interface.csproj", "Broker/Broker.Interface/"]
COPY ["Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "Shared/Shared.Infrastructure/"]
COPY ["Shared/Shared.Core/Shared.Core.csproj", "Shared/Shared.Core/"]
COPY ["Shared/Shared.DTOs/Shared.DTOs.csproj", "Shared/Shared.DTOs/"]
COPY ["Broker/Broker.Application/Broker.Application.csproj", "Broker/Broker.Application/Broker.Application/"]
COPY ["Broker/Broker.Domain/Broker.Domain.csproj", "Broker/Broker.Domain/Broker.Domain/"]
COPY ["Broker/Broker.Infrastructure/Broker.Infrastructure.csproj", "Broker/Broker.Infrastructure/Broker.Infrastructure/"]
RUN dotnet restore "Broker/Broker.Interface/Broker.Interface.csproj"
COPY . .
WORKDIR "/src/Broker/Broker.Interface"
RUN dotnet build "Broker.Interface.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Broker.Interface.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Broker.Interface.dll"]
