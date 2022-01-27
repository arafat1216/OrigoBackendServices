# dotnet 5
docker build . -t dotnetcore-sonarcloud -f sonarcloud-base-dotnet5.dockerfile # Builds
docker tag dotnetcore-sonarcloud:latest origov2acr.azurecr.io/baseimage-dotnetcore-sonarcloud
acr login --name origov2acr
docker push origov2acr.azurecr.io/baseimage-dotnetcore-sonarcloud

# dotnet 6
docker build . -t dotnetcore-sonarcloud -f sonarcloud-base-dotnet6.dockerfile # Builds
docker tag dotnetcore6-sonarcloud:latest origov2acr.azurecr.io/baseimage-dotnetcore6-sonarcloud
acr login --name origov2acr
docker push origov2acr.azurecr.io/baseimage-dotnetcore6-sonarcloud
