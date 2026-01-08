# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copiem TOT proiectul client
COPY . .

# Publicăm Blazor WASM
RUN dotnet publish RoomMateFinder.Client.csproj \
    -c Release \
    -o /app/publish

# ---------- RUNTIME ----------
FROM nginx:alpine

# Config Nginx pentru Blazor routing
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Fișierele statice Blazor
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
