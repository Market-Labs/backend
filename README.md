# MarketGo Backend

API ASP.NET Core 10 organizada por bounded contexts. La base compartida toma como guia el proyecto de ejemplo, pero usa los nombres y necesidades de MarketGo. Los contextos de negocio aun estan vacios para que cada integrante implemente su parte.

## Requisitos

- SDK de .NET 10 para restaurar y compilar.
- MySQL Server accesible cuando se implementen entidades y persistencia. MySQL Workbench es opcional para administrar la base.

## Inicio local

Desde la raiz del repositorio:

```powershell
dotnet restore .\marketgo-backend.sln
dotnet run --project .\marketgo-backend\marketgo-backend.csproj
```

La API puede iniciar sin MySQL mientras no haya endpoints que usen persistencia. `GET /api/v1/health` responde en `/api/v1/health`. En Development, el documento OpenAPI se expone en `/openapi/v1.json` y Scalar en `/scalar/v1`.

Para habilitar MySQL, define la cadena de conexion fuera de Git antes de iniciar la API:

```powershell
$env:ConnectionStrings__DefaultConnection = 'server=localhost;port=3306;database=marketgo;user=marketgo;password=TU_CLAVE'
```

La variable `ConnectionStrings__DefaultConnection` reemplaza el valor vacio de `appsettings.json`. No guardes contrasenas reales en archivos versionados. La API no crea la base ni aplica migraciones automaticamente; eso se hara cuando existan entidades y migraciones revisadas.

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
