#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5274

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ApiRetry/ApiRetry.csproj", "ApiRetry/"]
RUN dotnet restore "ApiRetry/ApiRetry.csproj"
COPY . .
WORKDIR "/src/ApiRetry"
RUN dotnet build "ApiRetry.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiRetry.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS http://+:5274
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiRetry.dll"]