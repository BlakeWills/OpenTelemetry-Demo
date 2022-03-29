#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ForecastService/ForecastService.csproj", "ForecastService/"]
RUN dotnet restore "ForecastService/ForecastService.csproj"
COPY . .
WORKDIR "/src/ForecastService"
RUN dotnet build "ForecastService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForecastService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForecastService.dll"]
