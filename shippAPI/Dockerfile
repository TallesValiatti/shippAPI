FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["shippAPI.csproj", "./"]
RUN dotnet restore "./shippAPI.csproj"
COPY . .

WORKDIR "/src/."
RUN dotnet build "shippAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "shippAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir DatabaseFolder/
COPY DatabaseFolder/* DatabaseFolder/

ENTRYPOINT ["dotnet", "shippAPI.dll"]



