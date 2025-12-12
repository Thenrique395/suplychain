FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install EF Tools
RUN dotnet tool install --global dotnet-ef --version 8.0.11
ENV PATH="$PATH:/root/.dotnet/tools"

COPY . .
RUN dotnet restore "supplychain.csproj"
RUN dotnet publish "supplychain.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "supplychain.dll"]
