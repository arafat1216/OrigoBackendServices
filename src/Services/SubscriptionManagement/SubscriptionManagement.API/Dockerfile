FROM origov2acr.azurecr.io/baseimage-dotnetcore6-sonarcloud:latest AS build

ARG SONAR_PROJECT_KEY=mytos_OrigoV2_SubscriptionManagementServices
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
COPY ["src/Services/SubscriptionManagement/SubscriptionManagementServices/SubscriptionManagementServices.csproj", "Services/SubscriptionManagement/SubscriptionManagementServices/SubscriptionManagementServices.csproj"]
COPY ["src/Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj", "Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj"]
COPY ["src/Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj", "Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj"]
COPY ["src/Services/SubscriptionManagement/Tests/SubscriptionManagement.IntegrationTests/SubscriptionManagement.IntegrationTests.csproj", "Services/SubscriptionManagement/Tests/SubscriptionManagement.IntegrationTests/SubscriptionManagement.IntegrationTests.csproj"]
RUN dotnet restore "Services/SubscriptionManagement/SubscriptionManagement.API/SubscriptionManagement.API.csproj"
RUN dotnet restore "Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj"
RUN dotnet restore "Services/SubscriptionManagement/Tests/SubscriptionManagement.IntegrationTests/SubscriptionManagement.IntegrationTests.csproj"


COPY /src/Core/. Core/.
COPY /src/Services/SubscriptionManagement/. Services/SubscriptionManagement/.
COPY /src/Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/. Services/SubscriptionManagement/Tests/SubscriptionManagement.UnitTests/.
COPY /src/Services/SubscriptionManagement/Tests/SubscriptionManagement.IntegrationTests/. Services/SubscriptionManagement/Tests/SubscriptionManagement.IntegrationTests/.

WORKDIR "/src/Services/SubscriptionManagement/SubscriptionManagement.API"

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
    "SubscriptionManagement.API.csproj"

ARG BUILD_ID
LABEL test=${BUILD_ID}

# Build and Test Unit tests
RUN dotnet build \
	--configuration Release \
	--no-restore \
	../Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj

RUN dotnet test \
	--logger "trx;LogFileName=test_results.trx" \
	--collect:"XPlat Code Coverage" \
	--results-directory /TestResults/UnitTests \
	/p:CoverletOutput=/TestResults/Coverage1/ \
	--configuration Release \
	--no-restore \
	--no-build \
	../Tests/SubscriptionManagement.UnitTests/SubscriptionManagement.UnitTests.csproj

# Build and Test Integration tests
RUN dotnet build \
	--configuration Release \
	--no-restore \
	../Tests/SubscriptionManagement.IntegrationTests/SubscriptionManagement.IntegrationTests.csproj

RUN dotnet test \
	--logger "trx;LogFileName=test_results2.trx" \
	--collect:"XPlat Code Coverage" \
	--results-directory /TestResults/IntegrationTests \
	/p:CoverletOutput=/TestResults/Coverage2/ \
	--configuration Release \
	--no-restore \
	--no-build \
	../Tests/SubscriptionManagement.IntegrationTests/SubscriptionManagement.IntegrationTests.csproj

RUN reportgenerator \
	-reports:/TestResults/**/coverage.cobertura.xml \
	-targetdir:/CodeCoverage \
	-reporttypes:"SonarQube;HtmlInline_AzurePipelines;HtmlInline_AzurePipelines_Dark;Cobertura" \
	-verbosity:Verbose

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

FROM build AS publish
RUN dotnet publish "SubscriptionManagement.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriptionManagement.API.dll"]

EXPOSE 80