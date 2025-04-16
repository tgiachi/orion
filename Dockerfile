# Base image for the final container
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app

# Install curl for healthcheck
RUN apk add --no-cache curl

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["src/Orion.Server/Orion.Server.csproj", "src/Orion.Server/"]
RUN dotnet restore "src/Orion.Server/Orion.Server.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/src/Orion.Server"
RUN dotnet build "Orion.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH

# Publish image with single file
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
RUN dotnet publish "Orion.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    -a $TARGETARCH \
    -p:PublishSingleFile=true \
    -p:PublishReadyToRun=true

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN unset ASPNETCORE_URLS

ENV ORION_SERVER_ROOT=/app
# Set non-root user for better security
# Creating user inside container rather than using $APP_UID since Alpine uses different user management
RUN adduser -D -h /app orion && \
    chown -R orion:orion /app

# Create directories for data persistence
RUN mkdir -p /app/data /app/logs /app/scripts && \
    chown -R orion:orion /app/data /app/logs /app/scripts

# Health check using the environment variable for the web port
# Default to port 20001 if not set
HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:${ORION_HTTP_PORT:-23021}/api/v1/status || exit 1

USER orion

ENTRYPOINT ["./Orion.Server"]
