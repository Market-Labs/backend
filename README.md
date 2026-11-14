# MarketGo Backend

API ASP.NET Core 10 reconstruida con la arquitectura de Learning Center. Esta etapa implementa Shared, Iam y Profiles con cinco carpetas cada uno: Application, Domain, Infrastructure, Interfaces y Resources. Los otros bounded contexts se incorporarán después en DevelopR.

## Requisitos

SDK .NET 10 y MySQL Server para persistencia. MySQL Workbench sirve para administrar la base, pero no reemplaza al servidor.

## Ejecutar y ver Swagger

Desde la raíz del repositorio ejecuta:

    dotnet restore .\marketgo-backend.sln
    dotnet run --project .\marketgo-backend\marketgo-backend.csproj

Abre /swagger en la URL que anuncie Rider o la terminal. En Development también está /scalar/v1. GET /api/v1/health funciona sin MySQL y los endpoints IAM y Profiles aparecen en Swagger. Hasta configurar MySQL, sus peticiones responden HTTP 503.

Configura la conexión y el secreto JWT fuera de Git en PowerShell:

    $env:ConnectionStrings__DefaultConnection = 'server=127.0.0.1;port=3306;database=marketgo;user=marketgo;password=TU_CLAVE'
    $env:TokenSettings__Secret = 'UNA_CLAVE_ALEATORIA_DE_AL_MENOS_32_BYTES'

El secreto se valida al iniciar cuando se configura MySQL. La API todavía no crea tablas ni aplica migraciones; esa es la siguiente etapa.

## IAM

- POST /api/v1/auth/sign-up o POST /api/v1/users: registro con fullName, email, password, organizationName, accountType (minimarket o supplier) y acceptedTerms=true.
- POST /api/v1/auth/sign-in o POST /api/v1/tokens: inicio de sesión con email y password.
- GET /api/v1/auth/me: usuario autenticado.
- GET /api/v1/users y GET /api/v1/users/{id}: usuarios de la organización del token.

Registro e inicio de sesión entregan token y user. Usa el token JWT en Swagger con Authorize. Las contraseñas se guardan como hash BCrypt. El frontend aún usa su autenticación simulada o Firebase; su integración con esta API queda pendiente.

## Profiles

El registro de una organización crea su perfil mediante UserRegisteredEventHandler. GET /api/v1/profiles y GET /api/v1/profiles/{id} muestran el directorio de negocios a usuarios autenticados. POST /api/v1/profiles permite crear un perfil si la organización aún no tiene uno. PATCH /api/v1/profiles/{id} actualiza solo el perfil de la organización del token. La respuesta incluye los campos businessName, email, phone, address, district, specialty y coverageArea que consume el frontend. Los perfiles de proveedores se exponen como type=provider para coincidir con el frontend.

## Comprobar sin MySQL

    dotnet build .\marketgo-backend.sln
    dotnet run --project .\tests\Unit\MarketGo.Iam.Smoke\MarketGo.Iam.Smoke.csproj
    dotnet run --project .\tests\Unit\MarketGo.Profiles.Smoke\MarketGo.Profiles.Smoke.csproj

Las pruebas comprueban IAM, Profiles, validaciones y el modelo EF Core sin conectarse a MySQL. La migración y las pruebas de persistencia siguen pendientes.
