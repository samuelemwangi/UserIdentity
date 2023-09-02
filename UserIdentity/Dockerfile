FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine  AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine  AS build

WORKDIR /src
COPY ["UserIdentity/UserIdentity.csproj", "UserIdentity/"]

RUN dotnet restore "UserIdentity/UserIdentity.csproj" --use-current-runtime /p:PublishReadyToRun=true

COPY . .
WORKDIR "/src/UserIdentity"

RUN dotnet publish "UserIdentity.csproj" -c Release -o /app/publish \
  --no-restore \
  --use-current-runtime \
  --self-contained true \
  /p:PublishTrimmed=true \  
  /p:PublishReadyToRun=true \
  /p:PublishSingleFile=true


FROM base AS final

RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
RUN apk upgrade musl

USER dotnetuser
COPY --from=build /app/publish .

ENTRYPOINT ["./UserIdentity"]
