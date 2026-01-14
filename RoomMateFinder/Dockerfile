# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RoomMateFinder.sln ./
COPY RoomMateFinder/RoomMateFinder.csproj ./RoomMateFinder/

RUN dotnet restore RoomMateFinder/RoomMateFinder.csproj

COPY RoomMateFinder/ ./RoomMateFinder/
WORKDIR /src/RoomMateFinder

RUN dotnet publish RoomMateFinder.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RoomMateFinder.dll"]
