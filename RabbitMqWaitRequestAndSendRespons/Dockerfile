﻿FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RabbitMqWaitRequestAndSendRespons/RabbitMqWaitRequestAndSendRespons.csproj", "RabbitMqWaitRequestAndSendRespons/"]
RUN dotnet restore "RabbitMqWaitRequestAndSendRespons/RabbitMqWaitRequestAndSendRespons.csproj"
COPY . .
WORKDIR "/src/RabbitMqWaitRequestAndSendRespons"
RUN dotnet build "RabbitMqWaitRequestAndSendRespons.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RabbitMqWaitRequestAndSendRespons.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMqWaitRequestAndSendRespons.dll"]
