#!/bin/bash
set -e

echo "Building Trailarr1 containers..."

# Ensure we're in the root directory
cd "$(dirname "$0")"

# Build the containers
docker-compose build

echo "Build complete. Run ./docker-start.sh to start the application."
