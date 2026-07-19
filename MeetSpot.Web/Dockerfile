FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MeetSpot.Web/MeetSpot.Web.csproj", "MeetSpot.Web/"]
COPY ["MeetSpot.Core/MeetSpot.Core.csproj", "MeetSpot.Core/"]
COPY ["MeetSpot.Infrastructure/MeetSpot.Infrastructure.csproj", "MeetSpot.Infrastructure/"]
RUN dotnet restore "MeetSpot.Web/MeetSpot.Web.csproj"
COPY . .
WORKDIR "/src/MeetSpot.Web"
RUN dotnet build "MeetSpot.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MeetSpot.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeetSpot.Web.dll"]
