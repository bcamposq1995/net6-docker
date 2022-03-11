#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["HttpMicroservice/HttpMicroservice.csproj", "HttpMicroservice/"]
# RUN dotnet restore "HttpMicroservice/HttpMicroservice.csproj"
# COPY . .
# WORKDIR "/src/HttpMicroservice"
# RUN dotnet build "HttpMicroservice.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "HttpMicroservice.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "HttpMicroservice.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY ./bin/Release/net6.0/publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "HttpMicroservice.dll"]