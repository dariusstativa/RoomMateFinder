# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY RoomMateFinder.API/*.csproj ./RoomMateFinder.API/
COPY RoomMateFinder.Infrastructure/*.csproj ./RoomMateFinder.Infrastructure/
COPY RoomMateFinder.Features/*.csproj ./RoomMateFinder.Features/

RUN dotnet restore

COPY . .
WORKDIR /src/RoomMateFinder.API
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE 8080

ENTRYPOINT ["dotnet", "RoomMateFinder.API.dll"]
