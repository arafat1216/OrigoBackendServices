#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install Azure Artifacts Credential Provider, and inject the authentication credentials for our internal NuGet package-well
# Sources:
# https://stackoverflow.com/a/65788814 (and their linked references) 
# https://github.com/dotnet/dotnet-docker/blob/main/documentation/scenarios/nuget-credentials.md#using-the-azure-artifact-credential-provider
RUN curl -L https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | sh
ARG FEED_ACCESSTOKEN
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/mytos/OrigoV2/_packaging/Origo2/nuget/v3/index.json\", \"password\":\"${FEED_ACCESSTOKEN}\"}]}"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src/Boilerplate/
COPY ["src/Boilerplate/Boilerplate.EntityFramework", "Boilerplate.EntityFramework/"]
COPY ["nuget.config", "Boilerplate.EntityFramework/"]

WORKDIR /src/Services/ProductCatalog/
COPY ["src/Services/ProductCatalog/", "."]
COPY ["nuget.config", "ProductCatalog.API/"]
COPY ["nuget.config", "ProductCatalog.Common/"]
COPY ["nuget.config", "ProductCatalog.Infrastructure/"]

RUN dotnet restore "ProductCatalog.API/ProductCatalog.API.csproj"

WORKDIR "/src/Services/ProductCatalog/ProductCatalog.API"
RUN dotnet build "ProductCatalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductCatalog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductCatalog.API.dll"]