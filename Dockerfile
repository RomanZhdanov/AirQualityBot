FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy csproj and restore as distinct layers
COPY *.sln .
COPY ./src/TelegramBot/*csproj ./src/TelegramBot/
COPY ./lib/IQAirApiClient/IQAirApiClient/*csproj ./lib/IQAirApiClient/IQAirApiClient/
RUN dotnet restore
COPY ./src/TelegramBot/. ./src/TelegramBot/
COPY ./lib/IQAirApiClient/. ./lib/IQAirApiClient/

WORKDIR "/src/."
RUN dotnet build . -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish . -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AirBro.TelegramBot.dll"]
