docker build . -t dotnetcore-sonarcloud -f sonarcloud-base.dockerfile # Builds
docker tag dotnetcore-sonarcloud:latest origov2acr.azurecr.io/baseimage-dotnetcore-sonarcloud
acr login --name origov2acr
docker push origov2acr.azurecr.io/baseimage-dotnetcore-sonarcloud
