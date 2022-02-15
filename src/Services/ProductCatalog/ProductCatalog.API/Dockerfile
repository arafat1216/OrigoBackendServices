#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Define build arguments
ARG FEED_SOURCE
ARG FEED_ACCESSTOKEN

# Copy dependencies
WORKDIR /src/Boilerplate/
COPY ["src/Boilerplate/Boilerplate.EntityFramework", "Boilerplate.EntityFramework/"]

# Copy project folders
WORKDIR /src/Services/ProductCatalog/
COPY ["src/Services/ProductCatalog/", "."]

RUN dotnet nuget add source "$FEED_SOURCE" --name "source-name" --username "useless" --password "$FEED_ACCESSTOKEN" --store-password-in-clear-text
RUN dotnet restore "ProductCatalog.API/ProductCatalog.API.csproj" 

WORKDIR "/src/Services/ProductCatalog/ProductCatalog.API"
RUN dotnet build "ProductCatalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductCatalog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.API.dll"]