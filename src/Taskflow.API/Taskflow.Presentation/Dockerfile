# Hata ayiklama kapsayicinizi ozellestirme ve Visual Studio'nun daha hizli hata ayiklama
# icin goruntulerinizi derlemek uzere bu Dockerfile'i nasil kullandigi hakkinda bilgi
# edinmek icin https://aka.ms/customizecontainer sayfasina bakin.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
WORKDIR /src/src/Taskflow.API/Taskflow.Presentation
RUN dotnet restore "./Taskflow.Presentation.csproj"
RUN dotnet build "./Taskflow.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Taskflow.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Taskflow.Presentation.dll"]
