# Multi-stage Dockerfile for Football League API
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files
COPY ["FootballLeagueApi.csproj", ""]
RUN dotnet restore "FootballLeagueApi.csproj"

# Copy all source code
COPY . .

# Build the application
RUN dotnet build "FootballLeagueApi.csproj" -c Release -o /app/build

# Publish to a staging directory
RUN dotnet publish "FootballLeagueApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Tempoary - Copy a pre‑existing database into the image for testing - later can use propper Azue SQL Lite DatabASE
COPY league.db /app/data/league.db
# Set Database Connection String so Temporary Database auctually used
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/league.db"

# Copy published files from build stage
COPY --from=build /app/publish .

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Expose port 8080 for HTTP
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "FootballLeagueApi.dll"]
