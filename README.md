# MarketGo Backend

API ASP.NET Core 10 organizada en bounded contexts con la arquitectura de Learning Center. Cada contexto tiene Application, Domain, Infrastructure, Interfaces y Resources. Las carpetas internas (Commands, Queries, Events, ValueObjects, Repositories, Rest, Transform y Acl) se usan según las responsabilidades del contexto.

## Requisitos y ejecución

Se necesita el SDK .NET 10 y MySQL Server para persistir datos. MySQL Workbench sirve para administrar la base, pero no reemplaza al servidor. En PowerShell, desde la raíz del repositorio:

    dotnet restore .\marketgo-backend.sln
    dotnet run --project .\marketgo-backend\marketgo-backend.csproj

En Development, abre /swagger o /scalar/v1 usando el puerto que anuncie Rider. GET /api/v1/health responde sin MySQL. Las demás rutas aparecen en Swagger y responden 503 hasta configurar la conexión.

Configura las credenciales fuera de Git:

    $env:ConnectionStrings__DefaultConnection = 'server=127.0.0.1;port=3306;database=marketgo;user=marketgo;password=TU_CLAVE'
    $env:TokenSettings__Secret = 'UNA_CLAVE_ALEATORIA_DE_AL_MENOS_32_BYTES'

## Migración de MySQL

La migración inicial sigue la estructura de Learning Center: `marketgo-backend/Migrations` contiene `InitialCreate.cs`, su archivo `Designer.cs` y `AppDbContextModelSnapshot.cs`. Incluye las tablas de IAM, Profiles y los contextos de negocio actuales. Se puede generar o inspeccionar sin conectarse a MySQL; todavía no se ha aplicado a una base real.

Con MySQL Server en ejecución y una base de datos vacía llamada `marketgo`, configura `ConnectionStrings__DefaultConnection` como arriba. Desde la raíz del repositorio, aplica la migración con:

    dotnet ef database update --project .\marketgo-backend\marketgo-backend.csproj --startup-project .\marketgo-backend\marketgo-backend.csproj --context AppDbContext

Para comprobar las migraciones existentes sin modificar la base:

    dotnet ef migrations list --project .\marketgo-backend\marketgo-backend.csproj --startup-project .\marketgo-backend\marketgo-backend.csproj --context AppDbContext --no-connect

Requiere el SDK .NET 10 y la herramienta `dotnet-ef` compatible. No guardes la contraseña en los archivos versionados.


## Contextos

| Contexto | Responsabilidad y rutas principales |
| --- | --- |
| Iam | Registro y JWT: /api/v1/auth/sign-up, /sign-in, /me y /api/v1/users |
| Profiles | Perfil de organización y contacto: /api/v1/profiles |
| Suppliers | Directorio de proveedores de una organización: /api/v1/suppliers |
| Products | Catálogo, edición y retiro: /api/v1/products; catálogo propio del proveedor en /api/v1/suppliers/{organizationId}/products |
| Inventory | Existencias, lotes, vencimientos, mermas y donaciones: /api/v1/minimarkets/{organizationId}/inventory, /waste, /donations y rutas equivalentes de proveedor |
| Requisition | Solicitudes de abastecimiento y revisión por proveedor: /api/v1/minimarkets/{organizationId}/requisitions y ruta de proveedor |
| Procurements | Órdenes desde solicitudes aceptadas y revisión de recepción: /api/v1/minimarkets/{organizationId}/purchase-orders y ruta de proveedor |
| Conservation | Lecturas de temperatura y humedad: /api/v1/minimarkets/{organizationId}/conservation/monitoring y ruta de proveedor |
| Communication | Alertas y mensajes: /api/v1/minimarkets/{organizationId}/communication/alerts y /messages; también rutas de proveedor |
| Dashboard | Indicadores operativos: /api/v1/minimarkets/{organizationId}/dashboard y ruta de proveedor |
| Analytics | Resumen de inventario, abastecimiento, mermas, conservación y proveedores: /api/v1/minimarkets/{organizationId}/analytics y ruta de proveedor |

Shared contiene repositorios base, unidad de trabajo, eventos, auditoría, configuración EF y respuestas de error. Las fachadas ACL permiten que Inventory consulte Products, que Procurements consulte Requisition y que Dashboard/Analytics obtengan métricas sin depender de repositorios ajenos.

Todas las rutas de negocio requieren JWT. El organizationId de la URL debe coincidir con el del token. Las contraseñas usan BCrypt. En el registro de IAM se crea un perfil de organización mediante un evento; una lectura de conservación riesgosa genera una alerta. Mermas y donaciones descuentan existencias en la misma unidad de trabajo.

## Comprobar sin MySQL

    dotnet build .\marketgo-backend.sln
    dotnet run --project .\tests\Unit\MarketGo.Iam.Smoke\MarketGo.Iam.Smoke.csproj
    dotnet run --project .\tests\Unit\MarketGo.Profiles.Smoke\MarketGo.Profiles.Smoke.csproj
    dotnet run --project .\tests\Unit\MarketGo.Business.Smoke\MarketGo.Business.Smoke.csproj

Estas pruebas cubren reglas de dominio y el modelo EF Core sin abrir una conexión. Las pruebas HTTP con persistencia real, concurrencia y aplicación de la migración en MySQL siguen pendientes. El frontend todavía usa identificadores de demostración y autenticación simulada o Firebase; para consumir esta API debe configurar la URL, JWT y los GUID reales.
