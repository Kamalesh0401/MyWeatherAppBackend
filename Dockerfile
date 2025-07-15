# Base build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore MyWeatherApp.csproj

# Build
RUN dotnet publish MyWeatherApp.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "MyWeatherApp.dll"]
