# TriviaGame

Aplicación web de trivia desarrollada en C# con .NET 8 como parte de una prueba técnica para BAC Credomatic.

El proyecto permite a los usuarios registrarse, iniciar sesión, jugar en diferentes categorías y consultar tablas de ranking.


<p align="center">
  <img src="https://github.com/user-attachments/assets/1ddd2069-17c2-45d0-9180-a4629870d0a8" width="32%" />
  <img src="https://github.com/user-attachments/assets/546fd7b5-78ac-4c53-b9a2-b3d5f0287341" width="32%" />
  <img src="https://github.com/user-attachments/assets/513ffba7-5c52-45b1-bff0-de9fc46b770f" width="32%" />
</p>

## Demo

La aplicación se encuentra desplegada en un VPS para facilitar su revisión y permitir probar sus funcionalidades directamente desde el navegador.

**Aplicación:** [http://5.189.152.96:8181](http://5.189.152.96:8181)

### Usuario de prueba

```text
Usuario: prueba@gmail.com
Contraseña: 1234prueba
```

> Actualmente, la aplicación utiliza HTTP debido a que está desplegada mediante una IP pública sin un dominio configurado con certificado SSL. El navegador puede mostrar una advertencia de seguridad al ingresar.

## Tecnologías

- C# / .NET 8
- ASP.NET Core MVC
- SQL Server
- Dapper
- Stored Procedures (SP)
- JWT (JSON Web Tokens)
- Docker
- Docker Compose

## Arquitectura

El proyecto está desarrollado bajo una arquitectura monolítica utilizando ASP.NET Core MVC.

El frontend y el backend forman parte de la misma aplicación, permitiendo mantener una estructura sencilla de desplegar y mantener. A nivel interno, el código se encuentra organizado mediante una separación de responsabilidades.

La solución utiliza las siguientes capas:

| Capa | Responsabilidad |
|---|---|
| Controllers | Gestionan las solicitudes y coordinan las operaciones. |
| Services | Contienen la lógica de negocio. |
| Repositories | Gestionan el acceso a datos mediante Dapper y SP. |
| Models | Representan las entidades del sistema. |
| Views | Contienen las interfaces de usuario. |

La autenticación se maneja mediante JWT y las operaciones de base de datos se realizan utilizando Dapper junto con procedimientos almacenados.

## Funcionalidades

- Registro de usuarios con validación de campos y usuarios únicos.
- Inicio de sesión mediante usuario y contraseña.
- Autenticación mediante JWT.
- Selección de dos categorías de trivia.
- Partidas de 3 preguntas por categoría.
- Tres posibles respuestas por pregunta, una correcta y dos incorrectas.
- Almacenamiento de preguntas y respuestas en SQL Server.
- Registro de resultados de las partidas.
- Tablas de ranking para consultar el desempeño de los jugadores.

Cada categoría cuenta con al menos 10 preguntas almacenadas en la base de datos.

## ¿Cómo ejecutar el proyecto?

### Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

### 1. Clonar el repositorio

```bash
git clone https://github.com/JeffreyMardoqueo-17/TriviaGamePrueba.git
```

```bash
cd TriviaGamePrueba
```

### 2. Configurar las variables de entorno

Crear un archivo `.env` en la raíz del proyecto.

Ejemplo de configuración:

```env
ACCEPT_EULA=Y
SA_PASSWORD=contraseña!
MSSQL_PORT=1433

APP_PORT=8181

DB_CONNECTION_STRING=Server=sqlserver,1433;Database=BDName;User Id=sa;Password=contraseña!;TrustServerCertificate=True;

JWT_SECRET=Your_Super_Secret_Key_At_Least_32_Characters
JWT_ISSUER=TriviaGame
JWT_AUDIENCE=TriviaGame
JWT_EXPIRE_MINUTES=60
```

Reemplazar las contraseñas y claves por valores seguros. Los valores deben coincidir con la configuración utilizada en el proyecto.

### 3. Inicializar Docker Compose

Desde la raíz del proyecto, ejecutar:

```bash
docker compose up -d
```

Esto inicializa los contenedores de SQL Server y de la aplicación.

Para verificar que los servicios estén funcionando:

```bash
docker compose ps
```

### 4. Acceder a la aplicación

Una vez inicializados los contenedores, ingresar desde el navegador:

```text
http://localhost:8181
```

## Flujo de la aplicación

El funcionamiento del juego es sencillo:

1. El usuario se registra o inicia sesión.
2. Selecciona una de las dos categorías disponibles.
3. Se inicia una partida con 3 preguntas relacionadas con la categoría seleccionada.
4. El sistema evalúa las respuestas y registra el resultado.
5. El usuario puede consultar las tablas de ranking.

## Base de datos

Se utiliza SQL Server para almacenar la información de usuarios, categorías, preguntas, respuestas y resultados de las partidas.

El acceso a datos se realiza mediante Dapper y Stored Procedures, manteniendo las operaciones de persistencia separadas de la lógica de negocio.

## Detener la aplicación

Para detener los contenedores:

```bash
docker compose down
```

Para eliminar también los volúmenes y los datos almacenados:

```bash
docker compose down -v
```

> Este último comando elimina los datos persistidos en los volúmenes de Docker.

## Repositorio

[GitHub - TriviaGamePrueba](https://github.com/JeffreyMardoqueo-17/TriviaGamePrueba.git)

