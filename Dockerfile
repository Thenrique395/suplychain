# ==============================================
# BUILD STAGE
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install EF Tools (optional but OK)
RUN dotnet tool install --global dotnet-ef --version 8.0.11
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy only csproj first (best practice)
COPY supplychain.csproj ./
RUN dotnet restore

# Copy all project files
COPY . .

# Build and publish
RUN dotnet publish supplychain.csproj -c Release -o /app/publish

# ==============================================
# RUNTIME STAGE
# ==============================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "supplychain.dll"]
