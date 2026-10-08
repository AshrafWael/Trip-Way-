FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Travel.Web/Travel.Web.csproj", "Travel.Web/"]
COPY ["Travel.BLL/Travel.BLL.csproj", "Travel.BLL/"]
COPY ["Travel.DAL/Travel.DAL.csproj", "Travel.DAL/"]
RUN dotnet restore "Travel.Web/Travel.Web.csproj"
COPY . .
WORKDIR /src/Travel.Web
RUN dotnet build "Travel.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Travel.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Travel.Web.dll"]
