# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and csproj files
COPY *.sln ./
COPY MovieWebApi/*.csproj ./MovieWebApi/
COPY MovieWebApi.Mvc/*.csproj ./MovieWebApi.Mvc/

# Restore dependencies
RUN dotnet restore

# Copy everything else and build the MVC project
COPY . ./
RUN dotnet publish MovieWebApi.Mvc/MovieWebApi.Mvc.csproj -c Release -o out

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "MovieWebApi.Mvc.dll"]