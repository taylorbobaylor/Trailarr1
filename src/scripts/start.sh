#!/bin/bash
set -e

echo "Starting Trailarr1..."

# Ensure we're in the root directory
cd "$(dirname "$0")"

# Start the containers
docker-compose up -d

echo "Waiting for services to start..."
sleep 5

echo "Trailarr1 is running!"
echo "Frontend: http://localhost:3000"
echo "Backend API: http://localhost:5000"
echo "Health check: http://localhost:5000/health"

echo "To view logs, run: docker-compose logs -f"
echo "To stop the application, run: docker-compose down"
