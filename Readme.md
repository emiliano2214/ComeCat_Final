# ComeCat\_Final

Proyecto distribuido en varios contenedores Docker para gestionar el sistema ComeCat.

## Contenedores incluidos

| Servicio | Contenido / Función | Puerto |
| -------- | ------------------- | ------ |
|          |                     |        |

| **API**            | Servidor ASP.NET Core que expone los endpoints REST | 5000 |
| ------------------ | --------------------------------------------------- | ---- |
| **DB**             | Base de datos de la aplicación                      | 5001 |
| **Blazor**         | Interfaz web con Blazor                             | 5002 |
| **MQTT**           | Servidor de mensajería MQTT                         | 1883 |
| **ServidorPrueba** | Contenedor con scripts de pruebas y automatización  | N/A  |
| **ServidorN8N**    | Contenedor de automatización con N8N                | 5678 |

Todos los contenedores están conectados mediante la red `red_gatos` definida en `docker-compose.yml`.

---

## Requisitos

- Docker >= 20.x
- Docker Compose >= 1.29.x
- Git
- (Opcional) Visual Studio 2022 o superior para editar los proyectos .NET

---

## Instrucciones para levantar el proyecto

1. Clonar el repositorio:

```bash
git clone https://github.com/emiliano2214/ComeCat_Final.git
cd ComeCat_Final
```

2. Levantar todos los contenedores:

```bash
docker compose up -d
```

3. Verificar los contenedores corriendo:

```bash
docker ps
```

4. Acceder a los servicios:

- API: `http://localhost:5000`
- Blazor: `http://localhost:5002`
- N8N: `http://localhost:5678` (usuario: `admin`, contraseña: `admin123`)
- MQTT: mediante cualquier cliente MQTT en `localhost:1883`

---

## Comandos útiles

- Detener todos los contenedores:

```bash
docker compose down
```

- Ver logs de un contenedor específico:

```bash
docker logs -f <nombre_contenedor>
```

- Reconstruir los contenedores después de cambios en el Dockerfile:

```bash
docker compose build
docker compose up -d
```

---

## Estructura del proyecto

```
Proyecto_ComeCat/
├─ docker-compose.yml
├─ ServidorAPI/
│  └─ Dockerfile
├─ ServidorBBDD/
│  └─ Dockerfile
├─ ServidorBlazor/
│  └─ Dockerfile
├─ ServidorMQTT/
│  └─ Dockerfile
├─ ServidorPrueba/
│  └─ Dockerfile
└─ ServidorN8N/
   └─ Dockerfile
```

---

## Notas

- Los volúmenes de Docker se usan para **persistir datos** y logs. No se suben al repositorio gracias al `.gitignore`.
- El proyecto está pensado para ser ejecutado **localmente** pero se puede adaptar a un entorno de producción con mínimas modificaciones (redes, puertos y credenciales).
- Para desarrollo de los proyectos .NET, se recomienda abrir cada carpeta de servicio en Visual Studio o Visual Studio Code.

---

## Contacto

- Emiliano Abate
- GitHub: [https://github.com/emiliano2214](https://github.com/emiliano2214)

