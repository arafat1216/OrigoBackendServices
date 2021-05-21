# To run Sonarcloud and code coverage reports we need a dotnet.core SDK image with some extra tools installed.
FROM origov2acr.azurecr.io/baseimage-dotnetcore-sonarcloud:latest AS build

# Parameters needed by Sonarcloud
ARG SONAR_PROJECT_KEY=Mytos_OrigoApiGateway
ARG SONAR_ORGANIZATION_KEY=mytos
ARG SONAR_HOST_URL=https://sonarcloud.io
ARG SONAR_TOKEN

WORKDIR /src

# Restore Nuget-packages for projects
COPY ["src/ApiGateways/OrigoApiGateway/OrigoApiGateway/OrigoApiGateway.csproj", "ApiGateways/OrigoApiGateway/OrigoApiGateway/OrigoApiGateway.csproj"]
COPY ["src/ApiGateways/OrigoApiGateway/OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj", "ApiGateways/OrigoApiGateway/OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj"]
RUN dotnet restore "ApiGateways/OrigoApiGateway/OrigoApiGateway/OrigoApiGateway.csproj"
RUN dotnet restore "ApiGateways/OrigoApiGateway/OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj"

COPY /src/ApiGateways/OrigoApiGateway/. ApiGateways/OrigoApiGateway/.

WORKDIR "/src/ApiGateways/OrigoApiGateway"

RUN dotnet sonarscanner begin \
  /k:"${SONAR_PROJECT_KEY}" \
  /o:"${SONAR_ORGANIZATION_KEY}" \
  /d:sonar.host.url="${SONAR_HOST_URL}" \
  /d:sonar.login="${SONAR_TOKEN}" \
  /d:sonar.cs.opencover.reportsPaths=/CodeCoverage/coverage.cobertura.xml

RUN dotnet build -c Release -o /app/build --no-restore "OrigoApiGateway/OrigoApiGateway.csproj"

ARG BUILD_ID
LABEL test=${BUILD_ID}

RUN dotnet test --logger "trx;LogFileName=test_results.trx" --collect:"XPlat Code Coverage" --results-directory /TestResults /p:CoverletOutputFormat=json%2cCobertura /p:CoverletOutput=/TestResults/Coverage --configuration Release --no-restore OrigoApiGateway.Tests/OrigoApiGateway.Tests.csproj

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

RUN reportgenerator -reports:/TestResults/**/coverage.cobertura.xml -targetdir:/CodeCoverage -reporttypes:HtmlInline_AzurePipelines
RUN find /TestResults -mindepth 2 -maxdepth 2 -name "coverage.cobertura.xml" -print -exec cp {} /CodeCoverage/ \;
# convert absolute docker paths to relative paths
RUN find /CodeCoverage -name "coverage.cobertura.xml" -print | xargs sed -i 's/\/src/\./g'

FROM build AS publish
WORKDIR "/src/ApiGateways/OrigoApiGateway/OrigoApiGateway"
RUN dotnet publish "OrigoApiGateway.csproj" -c Release -o /app/publish

# Create app image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrigoApiGateway.dll"]

EXPOSE 8090