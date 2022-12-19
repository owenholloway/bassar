FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

RUN ["apt-get", "update"]
RUN ["apt-get", "install", "-y", "wkhtmltopdf"]

WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /App
COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "Bassza.dll"]