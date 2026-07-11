FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore CollegeControlSystem.Presentation/CollegeControlSystem.Presentation.csproj

RUN dotnet publish CollegeControlSystem.Presentation/CollegeControlSystem.Presentation.csproj \
    -c Release \
    -o /app/publish


RUN cp CollegeControlSystem.Presentation/Secret.json /app/publish/Secret.json

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet","CollegeControlSystem.Presentation.dll"]