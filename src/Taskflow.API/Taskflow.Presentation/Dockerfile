# Hata ayıklama kapsayıcınızı özelleştirme ve Visual Studio’nun daha hızlı hata ayıklama için görüntülerinizi derlemek üzere bu Dockerfile'ı nasıl kullandığı hakkında bilgi edinmek için https://aka.ms/customizecontainer sayfasına bakın.

# Bu aşama, VS'den hızlı modda çalıştırıldığında kullanılır (Hata ayıklama yapılandırması için varsayılan olarak ayarlıdır)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Bu aşama, hizmet projesini oluşturmak için kullanılır
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Taskflow.API/Taskflow.Presentation/Taskflow.Presentation.csproj", "src/Taskflow.API/Taskflow.Presentation/"]
COPY ["src/Modules/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "src/Modules/Identity/Identity.Infrastructure/"]
COPY ["src/Modules/Identity/Identity.Application/Identity.Application.csproj", "src/Modules/Identity/Identity.Application/"]
COPY ["src/Modules/Identity/Identity.Domain/Identity.Domain.csproj", "src/Modules/Identity/Identity.Domain/"]
COPY ["src/BuildingBlocks/TaskFlow.BuildingBlocks/TaskFlow.BuildingBlocks.csproj", "src/BuildingBlocks/TaskFlow.BuildingBlocks/"]
COPY ["src/Modules/Tenant/Tenant.Infrastructure/Tenant.Infrastructure.csproj", "src/Modules/Tenant/Tenant.Infrastructure/"]
COPY ["src/Modules/Tenant/Tenant.Application/Tenant.Application.csproj", "src/Modules/Tenant/Tenant.Application/"]
COPY ["src/Modules/Tenant/Tenant.Domain/Tenant.Domain.csproj", "src/Modules/Tenant/Tenant.Domain/"]
RUN dotnet restore "./src/Taskflow.API/Taskflow.Presentation/Taskflow.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Taskflow.API/Taskflow.Presentation"
RUN dotnet build "./Taskflow.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Bu aşama, son aşamaya kopyalanacak hizmet projesini yayımlamak için kullanılır
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Taskflow.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Bu aşama üretimde veya VS'den normal modda çalıştırıldığında kullanılır (Hata Ayıklama yapılandırması kullanılmazken varsayılan olarak ayarlıdır)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Taskflow.Presentation.dll"]