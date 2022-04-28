FROM origov2acr.azurecr.io/baseimage-dotnetcore6-sonarcloud:latest AS build

ARG SONAR_PROJECT_KEY=Mytos_BackendServices
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
COPY ["src/Services/Customer/CustomerServices/CustomerServices.csproj", "Services/Customer/CustomerServices/CustomerServices.csproj"]
COPY ["src/Services/Customer/Customer.API/Customer.API.csproj", "Services/Customer/Customer.API/Customer.API.csproj"]
COPY ["src/Services/Customer/Tests/CustomerServices.UnitTests/CustomerServices.UnitTests.csproj", "Services/Customer/Tests/CustomerServices.UnitTests/CustomerServices.UnitTests.csproj"]
COPY ["src/Services/Customer/Tests/Customer.API.Tests/Customer.API.IntegrationTests.csproj", "Services/Customer/Tests/Customer.API.Tests/Customer.API.IntegrationTests.csproj"]

RUN dotnet restore "Services/Customer/Customer.API/Customer.API.csproj"
RUN dotnet restore "Services/Customer/Tests/CustomerServices.UnitTests/CustomerServices.UnitTests.csproj"
RUN dotnet restore "Services/Customer/Tests/Customer.API.Tests/Customer.API.IntegrationTests.csproj"


COPY /src/Core/. Core/.
COPY /src/Services/Customer/. Services/Customer/.
COPY /src/Services/Customer/Tests/CustomerServices.UnitTests/. Services/Customer/Tests/CustomerServices.UnitTests/.
COPY /src/Services/Customer/Tests/Customer.API.Tests/. Services/Customer/Tests/Customer.API.Tests/.

WORKDIR "/src/Services/Customer/Customer.API"

RUN dotnet sonarscanner begin \
  /k:"${SONAR_PROJECT_KEY}" \
  /o:"${SONAR_ORGANIZATION_KEY}" \
  /d:sonar.host.url="${SONAR_HOST_URL}" \
  /d:sonar.login="${SONAR_TOKEN}" \
  /d:sonar.cs.opencover.reportsPaths=/CodeCoverage/coverage.cobertura.xml

RUN dotnet build "Customer.API.csproj" -c Release  --no-restore -o /app/build

ARG BUILD_ID
LABEL test=${BUILD_ID}

RUN dotnet test --logger "trx;LogFileName=test_results.trx" --collect:"XPlat Code Coverage" --results-directory /TestResults /p:CoverletOutputFormat=json%2cCobertura /p:CoverletOutput=/TestResults/Coverage --configuration Release --no-restore ../Tests/CustomerServices.UnitTests/CustomerServices.UnitTests.csproj
RUN dotnet test --logger "trx;LogFileName=test_results2.trx" --collect:"XPlat Code Coverage" --results-directory /TestResults /p:CoverletOutputFormat=json%2cCobertura /p:CoverletOutput=/TestResults/Coverage --configuration Release --no-restore ../Tests/Customer.API.Tests/Customer.API.IntegrationTests.csproj


RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

RUN reportgenerator -reports:/TestResults/**/coverage.cobertura.xml -targetdir:/CodeCoverage -reporttypes:HtmlInline_AzurePipelines
RUN find /TestResults -mindepth 2 -maxdepth 2 -name "coverage.cobertura.xml" -print -exec cp {} /CodeCoverage/ \;
# convert absolute docker paths to relative paths
RUN find /CodeCoverage -name "coverage.cobertura.xml" -print | xargs sed -i 's/\/src/\./g'

FROM build AS publish
RUN dotnet publish "Customer.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.API.dll"]

EXPOSE 80