FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GoogleAPI/GoogleAPI.csproj", "GoogleAPI/"]
RUN dotnet restore "GoogleAPI/GoogleAPI.csproj"
COPY . .
WORKDIR "/src/GoogleAPI"
RUN dotnet build "GoogleAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoogleAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoogleAPI.dll"]