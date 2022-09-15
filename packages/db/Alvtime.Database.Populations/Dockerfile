FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
COPY . .
RUN dotnet publish -o /publish
WORKDIR /publish
ENTRYPOINT ["sh", "-c", "dotnet Alvtime.Database.Populations.dll \"${DB_CONNECTION}\""]