FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update \
    && apt-get install -y curl

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
WORKDIR /src
COPY ["src/Web/IdentityTesting.Web/IdentityTesting.Web.csproj", "src/Web/IdentityTesting.Web/"]
RUN dotnet restore "src/Web/IdentityTesting.Web/IdentityTesting.Web.csproj"
COPY . .
WORKDIR "/src/src/Web/IdentityTesting.Web"
RUN dotnet build "IdentityTesting.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityTesting.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityTesting.Web.dll"]
