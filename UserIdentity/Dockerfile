FROM mcr.microsoft.com/dotnet/aspnet:8.0.2-jammy-chiseled-composite AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=80;$ASPNETCORE_HTTP_PORTS 

FROM mcr.microsoft.com/dotnet/sdk:8.0.201-jammy  AS build
WORKDIR /src
COPY ["UserIdentity/UserIdentity.csproj", "UserIdentity/"]
RUN dotnet restore "UserIdentity/UserIdentity.csproj"
COPY . .
WORKDIR "/src/UserIdentity"
RUN dotnet publish "UserIdentity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
USER $APP_UID
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","UserIdentity.dll"]
