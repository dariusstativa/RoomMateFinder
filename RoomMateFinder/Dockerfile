# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RoomMateFinder.sln ./
COPY RoomMateFinder/RoomMateFinder.csproj ./RoomMateFinder/
COPY RoomMateFinder.Client/RoomMateFinder.Client.csproj ./RoomMateFinder.Client/
COPY RoomMateFinderTests/RoomMateFinderTests.csproj ./RoomMateFinderTests/

RUN dotnet restore

COPY . .
WORKDIR /src/RoomMateFinder
RUN dotnet publish -c Release -o /app/publish

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE 8080

ENTRYPOINT ["dotnet", "RoomMateFinder.dll"]
