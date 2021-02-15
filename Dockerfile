FROM origov2acr.azurecr.io/baseimage-dotnetcore-sonarcloud:latest AS build

ARG SONAR_PROJECT_KEY=97dfb3ac-dac5-4667-8e4e-d19bba085612
ARG SONAR_ORGANIZATION_KEY=mytos
ARG SONAR_HOST_URL=https://sonarcloud.io
ARG SONAR_TOKEN

WORKDIR /src

COPY *.sln .
COPY ["./OrigoAssetServices/OrigoAssetServices.csproj", "OrigoAssetServices/"]
COPY ["./OrigoAssetServicesTests/OrigoAssetServicesTests.csproj", "OrigoAssetServicesTests/"]
RUN dotnet restore

COPY . .

RUN dotnet sonarscanner begin \
  /k:"${SONAR_PROJECT_KEY}" \
  /o:"${SONAR_ORGANIZATION_KEY}" \
  /d:sonar.host.url="${SONAR_HOST_URL}" \
  /d:sonar.login="${SONAR_TOKEN}" \
  /d:sonar.cs.opencover.reportsPaths=/coverage.opencover.xml

RUN dotnet build -c Release -o /app/build --no-restore OrigoAssetServices/OrigoAssetServices.csproj

ARG BUILD_ID
LABEL test=${BUILD_ID}

RUN dotnet test --logger "trx;LogFileName=test_results.trx" --collect:"XPlat Code Coverage" --results-directory /TestResults /p:CoverletOutputFormat=json%2cCobertura /p:CoverletOutput=/TestResults/Coverage --configuration Release --no-restore OrigoAssetServicesTests/OrigoAssetServicesTests.csproj

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

RUN reportgenerator -reports:/TestResults/**/coverage.cobertura.xml -targetdir:/CodeCoverage -reporttypes:HtmlInline_AzurePipelines

FROM build AS publish
RUN dotnet publish "OrigoAssetServices/OrigoAssetServices.csproj" -c Release -o /app/publish

# Create app image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrigoAssetServices.dll"]

EXPOSE 80