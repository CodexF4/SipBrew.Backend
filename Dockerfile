# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 10000
ARG ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}
ENV PORT=10000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["SipBrew.WebAPI/SipBrew.WebAPI.csproj", "SipBrew.WebAPI/"]
COPY ["SipBrew.Core/SipBrew.Core.csproj", "SipBrew.Core/"]

RUN dotnet restore "SipBrew.WebAPI/SipBrew.WebAPI.csproj"

COPY . .
RUN dotnet publish "SipBrew.WebAPI/SipBrew.WebAPI.csproj" \
    --configuration "$BUILD_CONFIGURATION" \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["sh", "-c", "dotnet SipBrew.WebAPI.dll --urls http://0.0.0.0:${PORT:-10000}"]
