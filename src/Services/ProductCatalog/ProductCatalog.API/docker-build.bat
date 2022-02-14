@echo off
title Docker Build: ProductCatalog.API
cls

@echo Make sure you are authenticated with the Docker container register. See [https://docs.microsoft.com/en-us/azure/container-registry/container-registry-authentication?tabs=azure-cli#az-acr-login-with---expose-token] for details.
echo.

set /p sonarToken="Enter SonarCloud token: "
set /p nuGetFeedToken="Enter NuGet feed token: "
echo.

docker build -f ".\Dockerfile" "..\..\..\.." -t "productcatalogapi" --build-arg SONAR_TOKEN=%sonarToken% --build-arg BUILD_ID=XXX --build-arg FEED_ACCESSTOKEN=%nuGetFeedToken%
echo.

pause