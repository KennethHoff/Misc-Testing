FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/KH.Orleans.Silo/KH.Orleans.Silo.csproj", "src/KH.Orleans.Silo/"]
COPY ["src/KH.Orleans.Grains/KH.Orleans.Grains.csproj", "src/KH.Orleans.Grains/"]
COPY ["src/KH.Orleans.GrainInterfaces/KH.Orleans.GrainInterfaces.csproj", "src/KH.Orleans.GrainInterfaces/"]
RUN dotnet restore "src/KH.Orleans.Silo/KH.Orleans.Silo.csproj"
COPY . .
WORKDIR "/src/src/KH.Orleans.Silo"
RUN dotnet build "KH.Orleans.Silo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KH.Orleans.Silo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KH.Orleans.Silo.dll"]
