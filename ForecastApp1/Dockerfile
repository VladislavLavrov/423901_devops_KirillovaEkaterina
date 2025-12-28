FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5005

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ForecastApp1/ForecastApp1.csproj", "ForecastApp1/"]
RUN dotnet restore "ForecastApp1/ForecastApp1.csproj"
COPY . .
WORKDIR "/src/ForecastApp1"
RUN dotnet build "ForecastApp1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForecastApp1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForecastApp1.dll"]

