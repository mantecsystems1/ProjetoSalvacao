# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/BibliaOnline.sln ./backend/
COPY backend/src ./backend/src

WORKDIR /src/backend
RUN dotnet restore BibliaOnline.sln
RUN dotnet publish src/BibliaOnline.Api/BibliaOnline.Api.csproj -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BibliaOnline.Api.dll"]
