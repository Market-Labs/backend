# MarketGo Backend

Estructura inicial de la API ASP.NET Core 10. Las carpetas siguen el patron del proyecto de referencia y los siete bounded contexts definidos en los diagramas de MarketGo. Solo se prepararon directorios: no se copiaron clases, migraciones ni configuraciones de datos del ejemplo.

## Arbol

```text
marketgo-backend/
├── marketgo-backend.sln
├── README.md
├── tests/
│   ├── Unit/
│   └── Integration/
└── marketgo-backend/
    ├── Program.cs
    ├── marketgo-backend.csproj
    ├── appsettings.json
    ├── Resources/
    ├── Contexts/
    │   ├── Iam/
    │   ├── Organizations/
    │   ├── CatalogInventory/
    │   ├── ConservationQuality/
    │   ├── ProcurementOrders/
    │   ├── AlertsNotifications/
    │   └── CommercialContact/
    └── Shared/
        ├── Application/Patterns/
        ├── Domain/{Model,Repositories}/
        ├── Infrastructure/
        │   ├── Interfaces/AspNetCore/Configuration/
        │   └── Persistence/EFC/{Configuration,Interceptors,Migrations,Repositories}/
        └── Interfaces/
```

Cada carpeta dentro de `Contexts` tiene el mismo patron:

```text
<Context>/
├── Application/
│   ├── Commands/
│   ├── Queries/
│   ├── Services/
│   ├── Errors/
│   └── Internal/{CommandServices,QueryServices}/
├── Domain/
│   ├── Model/{Aggregates,Commands,Queries,ValueObjects,Events}/
│   └── Repositories/
├── Infrastructure/Persistence/EFC/{Configuration,Repositories}/
└── Interfaces/Rest/{Resources,Transform}/
```

Las carpetas contienen `.gitkeep` para que Git las conserve hasta que haya codigo real.

## Bounded contexts y frontend

| Contexto backend | Funciones del frontend relacionadas |
| --- | --- |
| `Iam` | `iam` |
| `Organizations` | `profiles`, `suppliers` |
| `CatalogInventory` | `products`, `inventory` |
| `ConservationQuality` | `conservation` |
| `ProcurementOrders` | `requisition`, `procurements` |
| `AlertsNotifications` | `communication` |
| `CommercialContact` | Contacto comercial de la landing |

`dashboard` y `analytics` son vistas de lectura de varios contextos. Sus contratos se definiran cuando se implementen los casos de uso; esta estructura no presupone un octavo contexto de dominio.

## Ramas

- `main`: proyecto inicial.
- `develop`: estructura compartida e integracion.
- `feature/iam`, `feature/organizations`, `feature/catalog-inventory`, `feature/conservation-quality`, `feature/procurement-orders`, `feature/alerts-notifications`, `feature/commercial-contact`: trabajo por contexto.

## Estado actual

`Program.cs` conserva el endpoint `weatherforecast` de la plantilla. No hay entidades, endpoints de MarketGo, migraciones ni conexion a MySQL. El proyecto usa `net10.0` y requiere el SDK de .NET 10 para compilar.
