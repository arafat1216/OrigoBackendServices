# To run Sonarcloud and code coverage reports we need a dotnet.core SDK image with some extra tools installed.
FROM origov2acr.azurecr.io/baseimage-dotnetcore6-sonarcloud:latest AS build

# Parameters needed by Sonarcloud
ARG SONAR_PROJECT_KEY=mytos_OrigoV2_OrigoApiGatewayServices
ARG SONAR_ORGANIZATION_KEY=mytos
ARG SONAR_HOST_URL=https://sonarcloud.io
ARG SONAR_TOKEN

# Define build arguments required by NuGet
ARG FEED_SOURCE
ARG FEED_ACCESSTOKEN

# Register the NuGet package-feed
RUN dotnet nuget add source "$FEED_SOURCE" --name "source-name" --username "useless" --password "$FEED_ACCESSTOKEN" --store-password-in-clear-text

WORKDIR /src

# Restore Nuget-packages for projects
COPY ["src/Core/Common.csproj", "Core/Common.csproj"]
COPY ["src/ApiGateways/OrigoApiGateway/OrigoApiGateway/OrigoApiGateway.csproj", "ApiGateways/OrigoApiGateway/OrigoApiGateway/OrigoApiGateway.csproj"]
COPY ["src/ApiGateways/OrigoApiGateway/OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj", "ApiGateways/OrigoApiGateway/OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj"]
COPY ["src/ApiGateways/OrigoApiGateway/OrigoGateway.IntegrationTests/OrigoGateway.IntegrationTests.csproj", "ApiGateways/OrigoApiGateway/OrigoGateway.IntegrationTests/OrigoGateway.IntegrationTests.csproj"]

RUN dotnet restore "ApiGateways/OrigoApiGateway/OrigoApiGateway/OrigoApiGateway.csproj"
RUN dotnet restore "ApiGateways/OrigoApiGateway/OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj"
RUN dotnet restore "ApiGateways/OrigoApiGateway/OrigoGateway.IntegrationTests/OrigoGateway.IntegrationTests.csproj"

COPY /src/Core/. Core/.
COPY /src/ApiGateways/OrigoApiGateway/. ApiGateways/OrigoApiGateway/.

WORKDIR "/src/ApiGateways/OrigoApiGateway"

# Provide extra parameters if this is a PR
RUN if [ -z "$PULL_REQUEST_KEY" ] ; then \
  dotnet sonarscanner begin \
    /k:"${SONAR_PROJECT_KEY}" \
    /o:"${SONAR_ORGANIZATION_KEY}" \
    /d:sonar.host.url="${SONAR_HOST_URL}" \
    /d:sonar.login="${SONAR_TOKEN}" \
	/d:sonar.verbose=true \
	/d:sonar.coverageReportPaths=/CodeCoverage/SonarQube.xml \
    /d:sonar.cs.xunit.reportsPaths=/**/TestResults/**/*.trx \
    /d:sonar.sourceEncoding=UTF-8; \
else \
  dotnet sonarscanner begin \
    /k:"${SONAR_PROJECT_KEY}" \
    /o:"${SONAR_ORGANIZATION_KEY}" \
    /d:sonar.pullrequest.base=trunk \
    /d:sonar.pullrequest.key="${PULL_REQUEST_KEY}" \
    /d:sonar.pullrequest.branch="${PULL_REQUEST_SOURCEBRANCH}" \
    /d:sonar.host.url="${SONAR_HOST_URL}" \
    /d:sonar.login="${SONAR_TOKEN}" \
	/d:sonar.verbose=true \
	/d:sonar.coverageReportPaths=/CodeCoverage/SonarQube.xml \
    /d:sonar.cs.xunit.reportsPaths=/**/TestResults/**/*.trx \
    /d:sonar.sourceEncoding=UTF-8; \
fi
  
RUN dotnet build \
    -c Release \
    -o /app/build \
    --no-restore \
    "OrigoApiGateway/OrigoApiGateway.csproj"

ARG BUILD_ID
LABEL test=${BUILD_ID}

# Build and Test Unit tests
RUN dotnet build \
	--configuration Release \
	--no-restore \
	OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj

RUN dotnet test \
	--logger "trx;LogFileName=test_results.trx" \
	--collect:"XPlat Code Coverage" \
	--results-directory /TestResults/UnitTests \
	/p:CoverletOutput=/TestResults/Coverage1/ \
	--configuration Release \
	--no-restore \
	--no-build \
	OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj

# Build and Test Integration tests
RUN dotnet build \
	--configuration Release \
	--no-restore \
	OrigoGateway.IntegrationTests/OrigoGateway.IntegrationTests.csproj

RUN dotnet test \
	--logger "trx;LogFileName=test_results2.trx" \
	--collect:"XPlat Code Coverage" \
	--results-directory /TestResults/UnitTests \
	/p:CoverletOutput=/TestResults/Coverage1/ \
	--configuration Release \
	--no-restore \
	--no-build \
	OrigoGateway.IntegrationTests/OrigoGateway.IntegrationTests.csproj


RUN reportgenerator \
	-reports:/TestResults/**/coverage.cobertura.xml \
	-targetdir:/CodeCoverage \
	-reporttypes:"SonarQube;HtmlInline_AzurePipelines;HtmlInline_AzurePipelines_Dark;Cobertura" \
	-verbosity:Verbose

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

FROM build AS publish
WORKDIR "/src/ApiGateways/OrigoApiGateway/OrigoApiGateway"
RUN dotnet publish "OrigoApiGateway.csproj" -c Release -o /app/publish

# Create app image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OrigoApiGateway.dll"]

EXPOSE 80