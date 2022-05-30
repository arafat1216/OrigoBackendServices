FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ApiGateways/OrigoApiGateway/OrigoGateway.IntegrationTests/OrigoGateway.IntegrationTests.csproj", "src/ApiGateways/OrigoApiGateway/OrigoGateway.IntegrationTests/"]
COPY ["src/ApiGateways/OrigoApiGateway/OrigoApiGateway/OrigoApiGateway.csproj", "src/ApiGateways/OrigoApiGateway/OrigoApiGateway/"]
COPY ["src/Core/Common.csproj", "src/Core/"]
RUN dotnet restore "src/ApiGateways/OrigoApiGateway/OrigoGateway.IntegrationTests/OrigoGateway.IntegrationTests.csproj"
COPY . .
WORKDIR "/src/src/ApiGateways/OrigoApiGateway/OrigoGateway.IntegrationTests"
RUN dotnet build "OrigoGateway.IntegrationTests.csproj" -c Release -o /app/build
