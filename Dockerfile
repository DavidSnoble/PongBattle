#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PongBattle.sln", "."]
COPY ["PongBattle.Web/PongBattle.Web.csproj", "PongBattle.Web/"]
COPY ["PongBattle.Data/PongBattle.Data.csproj", "PongBattle.Data/"]
COPY ["PongBattle.Domain/PongBattle.Domain.csproj", "PongBattle.Domain/"]
COPY ["PongBattle.Utilities/PongBattle.Utilities.csproj", "PongBattle.Utilities/"]
RUN dotnet restore "PongBattle.sln"
COPY . .
WORKDIR "/src/PongBattle.Web"
RUN dotnet build "PongBattle.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PongBattle.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PongBattle.Web.dll"]
