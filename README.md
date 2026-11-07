# MarketGo Backend

API ASP.NET Core 10 organizada por bounded contexts. La base compartida toma como guia el proyecto de ejemplo, pero usa los nombres y necesidades de MarketGo. Los contextos `Iam` y `Organizations` ya tienen un primer flujo de registro e inicio de sesion. Los demas contextos siguen preparados para el equipo.

## Requisitos

- SDK de .NET 10 para restaurar y compilar.
- MySQL Server accesible cuando se implementen entidades y persistencia. MySQL Workbench es opcional para administrar la base.

## Inicio local

Desde la raiz del repositorio:

```powershell
dotnet restore .\marketgo-backend.sln
dotnet run --project .\marketgo-backend\marketgo-backend.csproj
```

La API puede iniciar sin MySQL para comprobar salud y OpenAPI. Los endpoints de autenticacion se habilitan al definir la conexion. `GET /api/v1/health` responde en `/api/v1/health`. En Development, el documento OpenAPI se expone en `/openapi/v1.json` y Scalar en `/scalar/v1`.

Para habilitar MySQL, define la cadena de conexion fuera de Git antes de iniciar la API:

```powershell
$env:ConnectionStrings__DefaultConnection = 'server=localhost;port=3306;database=marketgo;user=marketgo;password=TU_CLAVE'
```

La variable `ConnectionStrings__DefaultConnection` reemplaza el valor vacio de `appsettings.json`. No guardes contrasenas reales en archivos versionados. La API no crea la base ni aplica migraciones automaticamente. Las entidades de organizaciones, usuarios y sesiones ya estan mapeadas; falta generar la migracion y aplicarla en MySQL antes de usar los endpoints de autenticacion.

## Estructura

```text
marketgo-backend/
├── README.md
├── .gitignore
├── marketgo-backend.sln
├── tests/{Unit,Integration}/
└── marketgo-backend/
    ├── Program.cs
    ├── marketgo-backend.csproj
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Iam/
    ├── Organizations/
    ├── CatalogInventory/
    ├── ConservationQuality/
    ├── ProcurementOrders/
    ├── AlertsNotifications/
    ├── CommercialContact/
    ├── Resources/
    └── Shared/
        ├── Application/Patterns/Result.cs
        ├── Domain/
        │   ├── Model/IAuditableEntity.cs
        │   └── Repositories/{IBaseRepository,IUnitOfWork}.cs
        └── Infrastructure/
            ├── Interfaces/AspNetCore/Configuration/KebabCaseRouteNamingConvention.cs
            └── Persistence/EFC/
                ├── Configuration/{AppDbContext,Extensions/ModelBuilderExtensions}.cs
                ├── Interceptors/AuditableEntityInterceptor.cs
                └── Repositories/{BaseRepository,UnitOfWork}.cs
```

Cada contexto conserva las capas `Application`, `Domain`, `Infrastructure` e `Interfaces/Rest` preparadas con `.gitkeep`.

## Contratos compartidos

- `Result<TValue,TError>` representa exito o error sin excepciones para resultados esperados.
- `IBaseRepository<TEntity,TKey>` permite que cada contexto elija su tipo de identificador (`Guid`, `int`, etc.). Las operaciones agregan cambios al contexto; `IUnitOfWork.CompleteAsync` los guarda.
- `IAuditableEntity` habilita `CreatedAt` y `UpdatedAt` en UTC mediante el interceptor.
- `AppDbContext` es el punto central donde cada equipo agregara sus configuraciones EF Core. Los nombres relacionales se convierten a `snake_case`; los nombres de tablas no se pluralizan automaticamente.
- Los controladores que usen `[controller]` en sus rutas obtendran segmentos `kebab-case`.

`Shared` contiene solo infraestructura transversal. Los equipos deben mantener entidades, validaciones, comandos, consultas, repositorios y contratos REST especificos dentro de su contexto.

## Contextos y frontend

| Backend | Funciones del frontend |
| --- | --- |
| `Iam` | `iam` |
| `Organizations` | `profiles`, `suppliers` |
| `CatalogInventory` | `products`, `inventory` |
| `ConservationQuality` | `conservation` |
| `ProcurementOrders` | `requisition`, `procurements` |
| `AlertsNotifications` | `communication` |
| `CommercialContact` | Contacto comercial de la landing |

`dashboard` y `analytics` consultaran datos de varios contextos. Sus contratos se definiran al implementar los casos de uso.

## Ramas

`main` contiene la base inicial. `develop` integra la estructura comun. Las ramas `feature/iam`, `feature/organizations`, `feature/catalog-inventory`, `feature/conservation-quality`, `feature/procurement-orders`, `feature/alerts-notifications` y `feature/commercial-contact` estan reservadas para el trabajo de cada contexto.

## Primer flujo de IAM

Con la base `marketgo` creada, su migracion aplicada y `ConnectionStrings__DefaultConnection` configurada:

- `POST /api/v1/auth/sign-up` registra un usuario propietario y su organizacion. El cuerpo incluye `fullName`, `email`, `password`, `organizationName`, `accountType` (`minimarket` o `supplier`) y `acceptedTerms: true`.
- `POST /api/v1/auth/sign-in` recibe `email` y `password`.
- `GET /api/v1/auth/me` devuelve el usuario y la organizacion actual.
- `POST /api/v1/auth/sign-out` revoca la sesion actual.

El registro e inicio devuelven `accessToken` y `expiresAt`. En `me` y `sign-out` envia `Authorization: Bearer <accessToken>`. El token dura 12 horas; solo se almacena su hash SHA-256. Las contrasenas se protegen con PBKDF2-SHA256 y salt aleatorio. Este flujo usa sesiones persistidas en MySQL y no requiere cambiar el `.csproj` original.

Para ejecutar las pruebas de logica y mapeo sin MySQL:

```powershell
dotnet run --project .\tests\Unit\MarketGo.Iam.Smoke\MarketGo.Iam.Smoke.csproj
```

## Siguiente etapa

Falta generar y revisar la migracion EF Core, probar los endpoints contra MySQL y conectar el frontend. Los demas contextos de negocio todavia no tienen casos de uso. La configuracion CORS de Development permite los puertos 4200 y 5173; ajustala si el frontend usa otro puerto.
