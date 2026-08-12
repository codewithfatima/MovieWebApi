# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.sln ./
COPY MovieWebApi/*.csproj ./MovieWebApi/
COPY MovieWebApi.Mvc/*.csproj ./MovieWebApi.Mvc/
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "MovieWebApi.dll"]