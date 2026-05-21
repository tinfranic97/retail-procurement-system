FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/RetailProcurement.API/RetailProcurement.API.csproj", "src/RetailProcurement.API/"]
COPY ["src/RetailProcurement.Core/RetailProcurement.Core.csproj", "src/RetailProcurement.Core/"]
COPY ["src/RetailProcurement.Infrastructure/RetailProcurement.Infrastructure.csproj", "src/RetailProcurement.Infrastructure/"]
RUN dotnet restore "src/RetailProcurement.API/RetailProcurement.API.csproj"
COPY . .
WORKDIR "/src/src/RetailProcurement.API"
RUN dotnet build "RetailProcurement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RetailProcurement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetailProcurement.API.dll"]
