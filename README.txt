Movie Example

Movie Example is a web application and rest api, running on with docker.

Prerequisites:
- .NET SDK 6.0
- Dotnet EF
- Docker
- Docker Compose

=====================================
Development Environment
=====================================

Run and stop application:
- On any terminal move to the "src" folder (the folder containing the "docker-compose.yml" file) and execute these commands:

docker-compose up
docker-compose down


=====================================
Migrate Database
=====================================
movie api:
- On any terminal move to the "movie.api" folder (the folder containing the "movie.api.csproj" file) and execute these commands:

dotnet ef migrations add init
dotnet database update
