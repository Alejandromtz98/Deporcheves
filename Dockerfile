# ============================================================
# ETAPA 1: Build
# Usa la imagen completa del SDK para compilar y publicar
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiar archivos de proyecto y restaurar dependencias
# Esto se hace primero para aprovechar la caché de Docker.
# Si no cambias los .csproj, Docker no vuelve a descargar los paquetes NuGet.
COPY ["src/Deporcheves.Domain/Deporcheves.Domain.csproj", "src/Deporcheves.Domain/"]
COPY ["src/Deporcheves.Application/Deporcheves.Application.csproj", "src/Deporcheves.Application/"]
COPY ["src/Deporcheves.Infrastructure/Deporcheves.Infrastructure.csproj", "src/Deporcheves.Infrastructure/"]
COPY ["src/Deporcheves.Web/Deporcheves.Web.csproj", "src/Deporcheves.Web/"]

RUN dotnet restore "src/Deporcheves.Web/Deporcheves.Web.csproj"

# 2. Copiar el resto del código y publicar en modo Release
COPY . .
WORKDIR "/src/src/Deporcheves.Web"
RUN dotnet publish "Deporcheves.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================================
# ETAPA 2: Runtime
# Usa la imagen ligera (solo runtime) para ejecutar
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Configurar el puerto por defecto para escuchar dentro del contenedor
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copiar los archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Comando de entrada
ENTRYPOINT ["dotnet", "Deporcheves.Web.dll"]