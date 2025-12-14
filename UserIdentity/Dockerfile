FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["UserIdentity/UserIdentity.csproj", "UserIdentity/"]
RUN dotnet restore "UserIdentity/UserIdentity.csproj" -r linux-musl-x64
COPY . .
WORKDIR "/src/UserIdentity"
RUN dotnet publish "UserIdentity.csproj" -r linux-musl-x64 \
	-c Release \
	-o /app \
	/p:SelfContained=true \
	/p:PublishSingleFile=true \
	/p:PublishTrimmed=true \
	/p:TrimMode=partial \
	/p:EnableCompressionInSingleFile=true \
	/p:InvariantGlobalization=true \
	/p:DebugType=none

FROM base AS final
COPY --from=build /app ./
USER $APP_UID
ENTRYPOINT ["./UserIdentity"]
