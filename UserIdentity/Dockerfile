FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=80;$ASPNETCORE_HTTP_PORTS 

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["UserIdentity/UserIdentity.csproj", "UserIdentity/"]
RUN dotnet restore "UserIdentity/UserIdentity.csproj" -r linux-musl-x64
COPY . .
WORKDIR "/src/UserIdentity"
RUN dotnet publish "UserIdentity.csproj" -r linux-musl-x64 --self-contained true -c Release -p:PublishSingleFile=true -o /app

FROM base AS final
COPY --from=build /app ./
USER $APP_UID
ENTRYPOINT ["./UserIdentity"]
