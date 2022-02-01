#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM origov2acr.azurecr.io/baseimage-dotnetcore6-sonarcloud:latest AS build

WORKDIR /src
COPY ["src/Core/Common.csproj", "Core/Common.csproj"]
COPY ["src/Services/SubscriptionManagement/SubscriptionManagementServices/SubscriptionManagementServices.csproj", "Services/SubscriptionManagement/SubscriptionManagementServices/SubscriptionManagementServices.csproj"]
COPY ["src/Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj", "Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj"]
COPY ["src/Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj", "Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj"]
WORKDIR /Services

RUN dotnet restore "Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj"
RUN dotnet restore "Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj"
COPY . .
WORKDIR "/src/Services/SubscriptionManagement/SubscriptionManagement.API"
RUN dotnet build "SubscriptionManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubscriptionManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriptionManagement.API.dll"]