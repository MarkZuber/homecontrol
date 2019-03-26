# FROM microsoft/dotnet:3.0-aspnetcore-runtime-stretch-slim AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview3-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview3-stretch AS build
WORKDIR /src
COPY ["src/HomeControl.Web/HomeControl.Web.csproj", "src/HomeControl.Web/"]
RUN dotnet restore "src/HomeControl.Web/HomeControl.Web.csproj"
COPY . .
WORKDIR "/src/src/HomeControl.Web"
RUN dotnet build "HomeControl.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HomeControl.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HomeControl.Web.dll"]
