# -------------------------
# Etapa de build y pruebas .NET
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo archivos de proyecto para restaurar dependencias
COPY ComeCatTest/ComeCatTest.csproj ./ComeCatTest/
RUN dotnet restore ./ComeCatTest/ComeCatTest.csproj

# Copiar el resto del código
COPY . .

# Ejecutar las pruebas .NET en el build (opcional, útil en CI/CD)
WORKDIR /src/ComeCatTest
RUN dotnet test

# -------------------------
# Etapa final: contenedor de pruebas mixtas
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Instalar Python y herramientas necesarias
RUN apt-get update && apt-get install -y python3 python3-venv python3-pip && rm -rf /var/lib/apt/lists/*

WORKDIR /tests

# Crear un virtual environment y usar pip dentro
RUN python3 -m venv venv
RUN ./venv/bin/pip install --upgrade pip
RUN ./venv/bin/pip install requests

# Copiar proyecto de pruebas .NET desde el stage de build
COPY --from=build /src/ComeCatTest ./ComeCatTest

# Copiar script Python auxiliar
COPY servidorPruebas.py .

# Comando por defecto: ejecutar pruebas .NET
CMD ["dotnet", "test", "ComeCatTest/ComeCatTest.csproj"]

# Opcional: para ejecutar el script Python dentro del contenedor,
# podés hacer: docker run <imagen> ./venv/bin/python servidorPruebas.py
