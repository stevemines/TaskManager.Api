FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Development
WORKDIR /src
COPY ["TaskManager.Api.csproj", "TaskManager.Api/"]
RUN dotnet restore "TaskManager.Api/TaskManager.Api.csproj" 

WORKDIR "/src/TaskManager.Api"
COPY . .
RUN dotnet build "TaskManager.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "TaskManager.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]
