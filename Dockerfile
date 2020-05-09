FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

WORKDIR /app
COPY . .

RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

WORKDIR /app
COPY --from=build /out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "DevSecOps.dll"]
