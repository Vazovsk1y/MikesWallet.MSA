FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MikesWallet.EmailNotifications.Worker/MikesWallet.EmailNotifications.Worker.csproj", "MikesWallet.EmailNotifications.Worker/"]
COPY ["MikesWallet.Contracts/MikesWallet.Contracts.csproj", "MikesWallet.Contracts/"]
RUN dotnet restore "MikesWallet.EmailNotifications.Worker/MikesWallet.EmailNotifications.Worker.csproj"
COPY . .
WORKDIR "/src/MikesWallet.EmailNotifications.Worker"
RUN dotnet build "MikesWallet.EmailNotifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MikesWallet.EmailNotifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MikesWallet.EmailNotifications.Worker.dll"]
