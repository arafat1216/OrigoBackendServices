FROM origov2acr.azurecr.io/baseimage-dotnetcore-sonarcloud:latest AS build

ARG SONAR_PROJECT_KEY=97dfb3ac-dac5-4667-8e4e-d19bba085612
ARG SONAR_ORGANIZATION_KEY=mytos
ARG SONAR_HOST_URL=https://sonarcloud.io
ARG SONAR_TOKEN

WORKDIR /src

COPY ["src/Core/Common.csproj", "Core/Common.csproj"]
COPY ["src/Services/Asset/AssetServices/AssetServices.csproj", "Services/Asset/AssetServices/AssetServices.csproj"]
COPY ["src/Services/Asset/Asset.API/Asset.API.csproj", "Services/Asset/Asset.API/Asset.API.csproj"]
COPY ["src/Services/Asset/Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj", "Services/Asset/Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj"]
RUN dotnet restore "Services/Asset/Asset.API/Asset.API.csproj"
RUN dotnet restore "Services/Asset/Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj"

COPY /src/Core/. Core/.
COPY /src/Services/Asset/. Services/Asset/.
COPY /src/Services/Asset/Tests/AssetServices.UnitTests/. Services/Asset/Tests/AssetServices.UnitTests/.

WORKDIR "/src/Services/Asset/Asset.API"

RUN dotnet sonarscanner begin \
  /k:"${SONAR_PROJECT_KEY}" \
  /o:"${SONAR_ORGANIZATION_KEY}" \
  /d:sonar.host.url="${SONAR_HOST_URL}" \
  /d:sonar.login="${SONAR_TOKEN}" \
  /d:sonar.cs.opencover.reportsPaths=/CodeCoverage/coverage.cobertura.xml

RUN dotnet build "Asset.API.csproj" -c Release -o /app/build

ARG BUILD_ID
LABEL test=${BUILD_ID}

RUN dotnet test --logger "trx;LogFileName=test_results.trx" --collect:"XPlat Code Coverage" --results-directory /TestResults /p:CoverletOutputFormat=json%2cCobertura /p:CoverletOutput=/TestResults/Coverage --configuration Release --no-restore ../Tests/AssetServices.UnitTests/AssetServices.UnitTests.csproj

RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

RUN reportgenerator -reports:/TestResults/**/coverage.cobertura.xml -targetdir:/CodeCoverage -reporttypes:HtmlInline_AzurePipelines
RUN find /TestResults -mindepth 2 -maxdepth 2 -name "coverage.cobertura.xml" -print -exec cp {} /CodeCoverage/ \;
# convert absolute docker paths to relative paths
RUN find /CodeCoverage -name "coverage.cobertura.xml" -print | xargs sed -i 's/\/src/\./g'

FROM build AS publish
RUN dotnet publish "Asset.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Asset.API.dll"]

EXPOSE 3810