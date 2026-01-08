# Build Blazor WASM
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

COPY RoomMateFinder.Client/ RoomMateFinder.Client/
RUN dotnet publish RoomMateFinder.Client/RoomMateFinder.Client.csproj \
    -c Release \
    -o /app/publish

# Serve static files
FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
