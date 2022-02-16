FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Define build arguments required by NuGet
ARG FEED_SOURCE
ARG FEED_ACCESSTOKEN

# Register the NuGet package-feed
RUN dotnet nuget add source "$FEED_SOURCE" --name "source-name" --username "useless" --password "$FEED_ACCESSTOKEN" --store-password-in-clear-text

WORKDIR /src
COPY ["src/Core/Common.csproj", "Core/Common.csproj"]
COPY ["src/Services/SubscriptionManagement/SubscriptionManagementServices/SubscriptionManagementServices.csproj", "Services/SubscriptionManagement/SubscriptionManagementServices/SubscriptionManagementServices.csproj"]
COPY ["src/Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj", "Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj"]
COPY ["src/Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj", "Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj"]

RUN dotnet restore "Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj"
RUN dotnet restore "Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj"

COPY /src/Core/. Core/.
COPY /src/Services/SubscriptionManagement/. Services/SubscriptionManagement/.
COPY /src/Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/. Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/.

WORKDIR "/src/Services/SubscriptionManagement/SubscriptionManagement.API"
RUN dotnet build "SubscriptionManagement.API.csproj" -c Release  --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "SubscriptionManagement.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriptionManagement.API.dll"]

EXPOSE 80