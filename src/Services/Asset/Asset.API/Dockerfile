FROM origov2acr.azurecr.io/baseimage-dotnetcore6-sonarcloud:latest AS build

ARG SONAR_PROJECT_KEY=mytos_OrigoV2_AssetServices
ARG SONAR_ORGANIZATION_KEY=mytos
ARG SONAR_HOST_URL=https://sonarcloud.io
ARG SONAR_TOKEN

# Define build arguments required by NuGet
ARG FEED_SOURCE
ARG FEED_ACCESSTOKEN

# Register the NuGet package-feed
RUN dotnet nuget add source "$FEED_SOURCE" --name "source-name" --username "useless" --password "$FEED_ACCESSTOKEN" --store-password-in-clear-text

WORKDIR /src

COPY ["src/Core/Common.csproj", "Core/Common.csproj"]
COPY ["src/Services/Asset/AssetServices/AssetServices.csproj", "Services/Asset/AssetServices/AssetServices.csproj"]
COPY ["src/Services/Asset/Asset.API/Asset.API.csproj", "Services/Asset/Asset.API/Asset.API.csproj"]
COPY ["src/Services/Asset/Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj", "Services/Asset/Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj"]
COPY ["src/Services/Asset/Tests/Asset.IntegrationTests/Asset.IntegrationTests.csproj", "Services/Asset/Tests/Asset.IntegrationTests/Asset.IntegrationTests.csproj"]
RUN dotnet restore "Services/Asset/Asset.API/Asset.API.csproj"
RUN dotnet restore "Services/Asset/Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj"
RUN dotnet restore "Services/Asset/Tests/Asset.IntegrationTests/Asset.IntegrationTests.csproj"

COPY /src/Core/. Core/.
COPY /src/Services/Asset/. Services/Asset/.
COPY /src/Services/Asset/Tests/AssetServices.UnitTests/. Services/Asset/Tests/AssetServices.UnitTests/.
COPY /src/Services/Asset/Tests/Asset.IntegrationTests/. Services/Asset/Tests/Asset.IntegrationTests/.

WORKDIR "/src/Services/Asset/Asset.API"

RUN dotnet sonarscanner begin \
  /k:"${SONAR_PROJECT_KEY}" \
  /o:"${SONAR_ORGANIZATION_KEY}" \
  /d:sonar.pullrequest.base="trunk"
  /d:sonar.scm.enabled=false
  /d:sonar.host.url="${SONAR_HOST_URL}" \
  /d:sonar.login="${SONAR_TOKEN}" \
  /d:sonar.cs.opencover.reportsPaths=/CodeCoverage/coverage.cobertura.xml \
  /d:sonar.cs.vstest.reportsPaths=/TestResults/*.trx \
  /d:sonar.sourceEncoding=UTF-8

RUN dotnet build "Asset.API.csproj" -c Release --no-restore -o /app/build

ARG BUILD_ID
LABEL test=${BUILD_ID}

RUN dotnet build \
	--configuration Release \
	--no-restore \
	../Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj

RUN dotnet test \
	--logger "trx;LogFileName=test_results.trx" \
	--collect:"XPlat Code Coverage" \
	--results-directory /TestResults/UnitTests \
	/p:CollectCoverage=true \
	/p:CoverletOutputFormat="cobertura%2copencover" \
	/p:CoverletOutput=/TestResults/Coverage1/ \
	--configuration Release \
	--no-restore \
	--no-build \
	../Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj

RUN dotnet build \
	--configuration Release \
	--no-restore \
	../Tests/Asset.IntegrationTests/Asset.IntegrationTests.csproj

RUN dotnet test \
	--logger "trx;LogFileName=test_results2.trx" \
	--collect:"XPlat Code Coverage" \
	--results-directory /TestResults/IntegrationTests \
	/p:CollectCoverage=true \
	/p:CoverletOutputFormat="cobertura%2copencover" \
	/p:CoverletOutput=/TestResults/Coverage2/ \
	--configuration Release \
	--no-restore \
	--no-build \
	../Tests/Asset.IntegrationTests/Asset.IntegrationTests.csproj

RUN reportgenerator \
	-reports:/TestResults/**/coverage.cobertura.xml \
	-targetdir:/CodeCoverage \
	-reporttypes:"Cobertura;HTMLInline;HTMLChart;HTMLSummary"

RUN find /TestResults -mindepth 2 -maxdepth 2 -name "coverage.cobertura.xml" -print -exec cp {} /CodeCoverage/ \;

# convert absolute docker paths to relative paths
RUN find /CodeCoverage -name "coverage.cobertura.xml" -print | xargs sed -i 's/\/src/\./g'

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

FROM build AS publish
RUN dotnet publish "Asset.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asset.API.dll"]

EXPOSE 80