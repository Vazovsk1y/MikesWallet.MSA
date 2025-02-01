FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8088

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MikesWallet.Users/MikesWallet.Users.csproj", "MikesWallet.Users/"]
RUN dotnet restore "MikesWallet.Users/MikesWallet.Users.csproj"
COPY . .
WORKDIR "/src/MikesWallet.Users"
RUN dotnet build "MikesWallet.Users.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MikesWallet.Users.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MikesWallet.Users.dll"]
