# ===== BUILD =====
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

COPY RoomMateFinder.Client/ ./RoomMateFinder.Client/

RUN dotnet publish RoomMateFinder.Client/RoomMateFinder.Client.csproj \
    -c Release \
    -o /app/publish

# ===== RUNTIME =====
FROM nginx:alpine

# Config pentru Blazor routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

# ATENȚIE: COPIEM DOAR OUTPUT-UL PUBLICAT
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html