@echo off
title Docker Build: ProductCatalog.API
cls

@echo Make sure you are authenticated with the Docker container register. 
@echo See [https://docs.microsoft.com/en-us/azure/container-registry/container-registry-authentication?tabs=azure-cli#az-acr-login-with---expose-token] for details.
echo.

@echo Make sure you have created a personal access tolken to be used for the NuGet Artifacts feed. This must have the ^"Packaging -^> Read^" permission.
@echo See [https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate] for details.
echo.

set /p sonarToken="Enter SonarCloud token (optional): "
set /p buildId="Enter a build id (optional): "
set /p nuGetFeedToken="Enter your NuGet access token: "
echo.

docker build -f ".\Dockerfile" "..\..\..\.." -t "productcatalogapi" --build-arg SONAR_TOKEN=%sonarToken% --build-arg BUILD_ID=%buildId% --build-arg FEED_SOURCE=https://pkgs.dev.azure.com/mytos/OrigoV2/_packaging/Origo2/nuget/v3/index.json --build-arg FEED_ACCESSTOKEN=%nuGetFeedToken%
echo.

pause