# stage 1: build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# restore
WORKDIR /src
COPY ["TransactionService.csproj", "./"]
RUN dotnet restore "./TransactionService.csproj"

# build
COPY . .
WORKDIR "/src/"
RUN dotnet build "./TransactionService.csproj" -c Release -o /app/build

# stage 2: publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TransactionService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# stage 3: run stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
ENV ASPNETCORE_HTTP_PORT=5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionService.dll"]