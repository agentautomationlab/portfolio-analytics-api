FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["portfolio-analytics-api.csproj", "."]
RUN dotnet restore "./portfolio-analytics-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "portfolio-analytics-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "portfolio-analytics-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "portfolio-analytics-api.dll"]