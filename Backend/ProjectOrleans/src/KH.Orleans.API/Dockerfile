FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/KH.Orleans.API/KH.Orleans.API.csproj", "src/KH.Orleans.API/"]
COPY ["src/KH.Orleans.GrainInterfaces/KH.Orleans.GrainInterfaces.csproj", "src/KH.Orleans.GrainInterfaces/"]
COPY ["src/KH.Orleans.Identity/KH.Orleans.Identity.csproj", "src/KH.Orleans.Identity/"]
RUN dotnet restore "src/KH.Orleans.API/KH.Orleans.API.csproj"
COPY . .
WORKDIR "/src/src/KH.Orleans.API"
RUN dotnet build "KH.Orleans.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KH.Orleans.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KH.Orleans.API.dll"]
