FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Travel.API/Travel.API.csproj", "Travel.API/"]
COPY ["Travel.BLL/Travel.BLL.csproj", "Travel.BLL/"]
COPY ["Travel.DAL/Travel.DAL.csproj", "Travel.DAL/"]
RUN dotnet restore "Travel.API/Travel.API.csproj"
COPY . .
WORKDIR /src/Travel.API
RUN dotnet build "Travel.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Travel.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080

# Secrets (ConnectionStrings__DefaultConnection, JwtSettings__Secret,
# SeedAdmin__Email, SeedAdmin__Password) are injected as environment
# variables at deploy time -- never baked into this image. See
# GETTING_STARTED.md and docker-compose.yml.
ENTRYPOINT ["dotnet", "Travel.API.dll"]
