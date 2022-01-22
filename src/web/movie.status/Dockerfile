#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/web/movie.status/movie.status.csproj", "src/web/movie.status/"]
RUN dotnet restore "src/web/movie.status/movie.status.csproj"
COPY . .
WORKDIR "/src/src/web/movie.status"
RUN dotnet build "movie.status.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "movie.status.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "movie.status.dll"]