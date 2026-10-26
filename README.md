# MarketGo Backend

Estructura inicial para una API ASP.NET Core 10 organizada por bounded contexts. Por ahora solo contiene carpetas base; las entidades, servicios, endpoints, persistencia, migraciones y pruebas se implementaran despues.

## Ramas

- `main`: base del proyecto.
- `develop`: estructura compartida e integracion.
- `feature/iam`
- `feature/organizations`
- `feature/catalog-inventory`
- `feature/conservation-quality`
- `feature/procurement-orders`
- `feature/alerts-notifications`
- `feature/commercial-contact`

Cada rama `feature/*` parte de `develop` y corresponde a un bounded context. Las ramas aun no contienen implementaciones independientes.

## Bounded contexts

| Carpeta | Alcance inicial |
| --- | --- |
| `Iam` | Usuarios, credenciales, roles, permisos y sesiones. |
| `Organizations` | Minimarkets, proveedores, miembros y direcciones. |
| `CatalogInventory` | Productos, categorias, inventario y lotes. |
| `ConservationQuality` | Condiciones ambientales, inspecciones y riesgo de merma. |
| `ProcurementOrders` | Pedidos, respuestas de proveedores y entregas. |
| `AlertsNotifications` | Reglas, alertas y notificaciones. |
| `CommercialContact` | Prospectos, solicitudes de contacto y planes. |

Cada contexto tiene `Domain`, `Application`, `Infrastructure` e `Interfaces/Rest`. `Shared` contiene piezas transversales que realmente se compartan. Las carpetas vacias llevan `.gitkeep` para versionarlas.

## Estado

El archivo `Program.cs` conserva el endpoint `weatherforecast` de la plantilla de Rider. No hay conexion a MySQL ni funcionalidad de MarketGo. El proyecto usa `net10.0` y necesita el SDK de .NET 10 para restaurar y compilar.
