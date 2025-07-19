FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Persistencia.API/Persistencia.API.csproj", "Persistencia.API/"]
COPY ["Compartilhado/Compartilhado.csproj", "Compartilhado/"]
RUN dotnet restore "Persistencia.API/Persistencia.API.csproj"
COPY . .
WORKDIR "/src/Persistencia.API"
RUN dotnet build "Persistencia.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Persistencia.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Persistencia.API.dll"]
