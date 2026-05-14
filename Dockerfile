# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY ["HelpingHand/HelpingHand.csproj", "HelpingHand/"]

# Restore dependencies
RUN dotnet restore "HelpingHand/HelpingHand.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "HelpingHand/HelpingHand.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "HelpingHand/HelpingHand.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Expose default ASP.NET Core ports
EXPOSE 80 443

# Set environment to production by default (override in docker-compose for development)
ENV ASPNETCORE_ENVIRONMENT=Development

# Run the application
ENTRYPOINT ["dotnet", "HelpingHand.dll"]
