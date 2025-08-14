# -------------------------
# Etapa de build
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo el archivo del proyecto para restaurar dependencias
COPY ComeCat/ComeCat.csproj ./ComeCat/
RUN dotnet restore ./ComeCat/ComeCat.csproj

# Copiar todo el código fuente
COPY . .

# Publicar la aplicación en modo Release
WORKDIR /src/ComeCat
RUN dotnet publish -c Release -o /out

# -------------------------
# Etapa de runtime
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar los archivos publicados desde la etapa de build
COPY --from=build /out ./

# Exponer puerto para Docker Compose
EXPOSE 5000

# Ejecutar la API escuchando en todas las interfaces
ENTRYPOINT ["dotnet", "ComeCat.dll", "--urls", "http://0.0.0.0:5000"]
