# -------------------------
# Etapa de build
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo el archivo del proyecto para restaurar dependencias
COPY ComeCat/ComeCat.csproj ./ComeCat/
RUN dotnet restore ./ComeCat/ComeCat.csproj

# Copiar todo el código fuente
COPY ComeCat/. ./ComeCat/

# Publicar la aplicación Blazor en modo Release
WORKDIR /src/ComeCat
RUN dotnet publish -c Release -o /out /p:PublishTrimmed=false

# -------------------------
# Etapa de runtime (servir archivos estáticos)
# -------------------------
FROM nginx:alpine AS runtime
# Copiar los archivos publicados a la carpeta de Nginx
COPY --from=build /out/wwwroot /usr/share/nginx/html

# Exponer el puerto que usará Blazor
EXPOSE 5002
