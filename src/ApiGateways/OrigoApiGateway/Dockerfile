# Global argument to be used across all stages
ARG PROJECT_NAME=OrigoApiGateway

# To run Sonarcloud and code coverage reports we need a dotnet.core SDK image with some extra tools installed.
FROM origov2acr.azurecr.io/baseimage-dotnetcore-sonarcloud:latest AS build

# Parameters needed by Sonarcloud
ARG SONAR_PROJECT_KEY=Mytos_OrigoApiGateway
ARG SONAR_ORGANIZATION_KEY=mytos
ARG SONAR_HOST_URL=https://sonarcloud.io
ARG SONAR_TOKEN

ARG PROJECT_NAME

WORKDIR /src

# Restore Nuget-packages for projects
COPY *.sln .
COPY ["./${PROJECT_NAME}/${PROJECT_NAME}.csproj", "${PROJECT_NAME}/"]
COPY ["./${PROJECT_NAME}.Tests/${PROJECT_NAME}.Tests.csproj", "${PROJECT_NAME}.Tests/"]
RUN dotnet restore

COPY . .

RUN dotnet sonarscanner begin \
  /k:"${SONAR_PROJECT_KEY}" \
  /o:"${SONAR_ORGANIZATION_KEY}" \
  /d:sonar.host.url="${SONAR_HOST_URL}" \
  /d:sonar.login="${SONAR_TOKEN}" \
  /d:sonar.cs.opencover.reportsPaths=/CodeCoverage/coverage.cobertura.xml

RUN dotnet build -c Release -o /app/build --no-restore ${PROJECT_NAME}/${PROJECT_NAME}.csproj

ARG BUILD_ID
LABEL test=${BUILD_ID}

RUN dotnet test --logger "trx;LogFileName=test_results.trx" --collect:"XPlat Code Coverage" --results-directory /TestResults /p:CoverletOutputFormat=json%2cCobertura /p:CoverletOutput=/TestResults/Coverage --configuration Release --no-restore ${PROJECT_NAME}.Tests/${PROJECT_NAME}.Tests.csproj

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

RUN reportgenerator -reports:/TestResults/**/coverage.cobertura.xml -targetdir:/CodeCoverage -reporttypes:HtmlInline_AzurePipelines
RUN find /TestResults -mindepth 2 -maxdepth 2 -name "coverage.cobertura.xml" -print -exec cp {} /CodeCoverage/ \;
# convert absolute docker paths to relative paths
RUN find /CodeCoverage -name "coverage.cobertura.xml" -print | xargs sed -i 's/\/src/\./g'

FROM build AS publish
WORKDIR "/src/OrigoApiGateway"
RUN dotnet publish "OrigoApiGateway.csproj" -c Release -o /app/publish

# Create app image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrigoApiGateway.dll"]

EXPOSE 80