FROM mcr.microsoft.com/dotnet/sdk:5.0

# Install Sonar Scanner, Coverlet and Java (required for Sonar Scanner)
# Sonarcloud setup taken from https://pumpingco.de/blog/how-to-run-a-sonarcloud-scan-during-docker-builds-for-dotnet-core/
# Needed for openjdk to be installed properly
RUN mkdir /usr/share/man/man1/
RUN apt-get update && apt-get install -y openjdk-11-jdk
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-sonarscanner
RUN dotnet tool install --global coverlet.console
RUN dotnet tool install --global dotnet-reportgenerator-globaltool
