FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /app/src

COPY EmailApp.Business/*.csproj EmailApp.Business/
COPY EmailApp.DataAccess/*.csproj EmailApp.DataAccess/
COPY EmailApp.IntegrationTests/*.csproj EmailApp.IntegrationTests/
COPY EmailApp.UnitTests/*.csproj EmailApp.UnitTests/
COPY EmailApp.WebUI/*.csproj EmailApp.WebUI/
COPY *.sln ./
RUN dotnet restore

COPY . ./

RUN dotnet publish --configuration Release --no-restore -o ../dist EmailApp.WebUI

FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app

COPY --from=build /app/dist ./

ENV ConnectionStrings__SqliteEmailDb "Data Source=email.db"

CMD [ "dotnet", "EmailApp.WebUI.dll" ]

HEALTHCHECK CMD curl --fail http://localhost/health || exit
