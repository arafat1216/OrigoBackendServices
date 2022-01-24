#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj", "src/Services/SubscriptionManagement/SubscriptionManagement.API/"]
RUN dotnet restore "src/Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj"
COPY . .
WORKDIR "/src/src/Services/SubscriptionManagement/SubscriptionManagement.API"
RUN dotnet build "SubscriptionManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubscriptionManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriptionManagement.API.dll"]