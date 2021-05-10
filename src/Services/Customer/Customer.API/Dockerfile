#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Core/Common.csproj", "Core/Common.csproj"]
COPY ["src/Services/Customer/CustomerServices/CustomerServices.csproj", "Services/Customer/CustomerServices/CustomerServices.csproj"]
COPY ["src/Services/Customer/Customer.API/Customer.API.csproj", "Services/Customer/Customer.API/Customer.API.csproj"]
RUN dotnet restore "Services/Customer/Customer.API/Customer.API.csproj"
COPY /src/Core/. Core/.
COPY /src/Services/Customer. Services/Customer/.
WORKDIR "/src/Services/Customer/Customer.API"
RUN dotnet build "Customer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Customer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.API.dll"]