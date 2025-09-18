# used as variable for lines further down to reference the .NET version used rather than harccoding
ARG DOTNET_VERSION=9.0

# same as above, and set build config to release 
ARG BUILD_CONFIGURATION=Release

# retrieves the base image from the microsoft registry
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build

# all following commands like build and publish will be done in this directory.
# it's like doing mkdir /folder/path and then cd /folder/path
WORKDIR /TransactionService

# copy the csproj file to the workdir defined above here, and then it us used to 
# check all external packages it needs and downloads them
COPY ["TransactionService.csproj", "./"]
RUN dotnet restore "TransactionService.csproj"

COPY . .

# compiles the code from workdirectory and output it in /app/build. Not necessary for the final image,
# but a good practice since it checks for general compilation errors?
RUN dotnet build "TransactionService.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# starts the new publish stage
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS publish
WORKDIR /app
COPY --from=build /TransactionService .
RUN dotnet publish "TransactionService.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

# makes sure the ports are exposed and available for the container
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "TransactionService.dll" ]