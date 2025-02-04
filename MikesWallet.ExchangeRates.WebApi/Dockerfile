FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 1234

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MikesWallet.ExchangeRates.WebApi/MikesWallet.ExchangeRates.WebApi.csproj", "MikesWallet.ExchangeRates.WebApi/"]
COPY ["MikesWallet.Contracts/MikesWallet.Contracts.csproj", "MikesWallet.Contracts/"]
RUN dotnet restore "MikesWallet.ExchangeRates.WebApi/MikesWallet.ExchangeRates.WebApi.csproj"
COPY . .
WORKDIR "/src/MikesWallet.ExchangeRates.WebApi"
RUN dotnet build "MikesWallet.ExchangeRates.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MikesWallet.ExchangeRates.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MikesWallet.ExchangeRates.WebApi.dll"]
