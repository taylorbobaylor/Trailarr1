# syntax=docker/dockerfile:1.4

FROM ghcr.io/linuxserver/baseimage-ubuntu:jammy

# Set environment variables
ENV DEBIAN_FRONTEND=noninteractive \
    S6_BEHAVIOUR_IF_STAGE2_FAILS=2 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_URLS=http://+:5000 \
    NODE_ENV=production

# Install .NET SDK and runtime
RUN curl -fsSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 8.0 && \
    ln -s ~/.dotnet/dotnet /usr/bin/dotnet

# Install Node.js and serve
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get update && \
    apt-get install -y nodejs ffmpeg && \
    npm install -g serve && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Create app directory structure
RUN mkdir -p /app/frontend /app/backend /config /movies && \
    chown -R abc:abc /app /config /movies

# Build frontend
WORKDIR /app/frontend
COPY src/frontend/package*.json ./
RUN npm ci
COPY src/frontend/ .
RUN npm run build && rm -rf node_modules

# Build backend
WORKDIR /app/backend
COPY src/backend/Trailarr.Core/*.csproj ./
RUN dotnet restore
COPY src/backend/Trailarr.Core/ .
RUN dotnet publish -c Release -o . && \
    rm -rf obj

# Copy s6-overlay configuration
COPY src/docker/root/ /

# Labels for container metadata
LABEL maintainer="taylorbobaylor" \
      org.opencontainers.image.source="https://github.com/taylorbobaylor/Trailarr1" \
      org.opencontainers.image.description="Movie trailer downloader based on Radarr" \
      org.opencontainers.image.licenses="GPL-3.0"

# Volumes and ports
VOLUME ["/config", "/movies"]
EXPOSE 3000 5000

# Use s6-overlay init system
ENTRYPOINT ["/init"]
