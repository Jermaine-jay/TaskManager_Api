#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TaskManagerApi/TaskManager.Api.csproj", "TaskManagerApi/"]
COPY ["TaskManager.Data/TaskManager.Data.csproj", "TaskManager.Data/"]
COPY ["TaskManager.Models/TaskManager.Models.csproj", "TaskManager.Models/"]
COPY ["TaskManager.Migrations/TaskManager.Migrations.csproj", "TaskManager.Migrations/"]
COPY ["TaskManager.Services/TaskManager.Services.csproj", "TaskManager.Services/"]
RUN dotnet restore "TaskManagerApi/TaskManager.Api.csproj"
COPY . .
WORKDIR "/src/TaskManagerApi"
RUN dotnet build "TaskManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]