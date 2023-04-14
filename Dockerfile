# syntax=docker/dockerfile:1

### Build the server
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine3.16 AS serverbuild

WORKDIR /app
COPY . ./

# Publish app
RUN dotnet publish HelloWorld/HelloWorld.csproj -c Release -o /app/publish

### Final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine3.16
WORKDIR /app
COPY --from=serverbuild /app/publish ./
ENTRYPOINT ["./HelloWorld"]
