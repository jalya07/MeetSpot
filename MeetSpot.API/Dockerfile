FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MeetSpot.API/MeetSpot.API.csproj", "MeetSpot.API/"]
COPY ["MeetSpot.Core/MeetSpot.Core.csproj", "MeetSpot.Core/"]
COPY ["MeetSpot.Infrastructure/MeetSpot.Infrastructure.csproj", "MeetSpot.Infrastructure/"]

RUN dotnet restore "MeetSpot.API/MeetSpot.API.csproj"

COPY . .
WORKDIR "/src/MeetSpot.API"
RUN dotnet build "MeetSpot.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MeetSpot.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeetSpot.API.dll"]