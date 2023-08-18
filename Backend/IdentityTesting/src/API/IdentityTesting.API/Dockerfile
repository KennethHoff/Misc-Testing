FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update \
    && apt-get install -y curl

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY ["src/IdentityTesting.API/IdentityTesting.API.csproj", "src/IdentityTesting.API/"]
COPY ["src/IdentityTesting.API.Identity/IdentityTesting.API.Identity.csproj", "src/IdentityTesting.API.Identity/"]
COPY ["src/IdentityTesting.API.Models/IdentityTesting.API.Models.csproj", "src/IdentityTesting.API.Models/"]
RUN dotnet restore "src/IdentityTesting.API/IdentityTesting.API.csproj"
COPY . .
WORKDIR "/src/src/IdentityTesting.API"
RUN dotnet build "IdentityTesting.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityTesting.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityTesting.API.dll"]
