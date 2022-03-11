# #See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
# WORKDIR /app

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["WorkerMicroservice/WorkerMicroservice.csproj", "WorkerMicroservice/"]
# COPY ["Commons/Commons.csproj", "Commons/"]
# RUN dotnet restore "WorkerMicroservice/WorkerMicroservice.csproj"
# COPY . .
# WORKDIR "/src/WorkerMicroservice"
# RUN dotnet build "WorkerMicroservice.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "WorkerMicroservice.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "WorkerMicroservice.dll"]

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY ./bin/Release/net6.0/publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "WorkerMicroservice.dll"]