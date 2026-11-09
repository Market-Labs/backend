# MarketGo Backend

API ASP.NET Core 10 organizada en los bounded contexts del frontend: `Iam`, `Profiles`, `Suppliers`, `Products`, `Inventory`, `Requisition`, `Procurements`, `Conservation`, `Communication`, `Dashboard` y `Analytics`. `Shared` contiene el contexto EF Core, repositorios base, auditoria y el tipo `Result`.

## Requisitos

- SDK .NET 10.
- MySQL Server para ejecutar operaciones de negocio. MySQL Workbench es una interfaz opcional para administrar la base.

## Ejecutar

Desde la raiz del repositorio:

```powershell
dotnet restore .\marketgo-backend.sln
dotnet run --project .\marketgo-backend\marketgo-backend.csproj
```

En Development, abre `/swagger` o `/scalar/v1`. `GET /api/v1/health` responde sin MySQL. Swagger muestra todos los endpoints aun sin conexion; las operaciones de negocio devuelven HTTP 503 hasta configurar MySQL.

Configura secretos fuera de Git:

```powershell
$env:ConnectionStrings__DefaultConnection = 'server=localhost;port=3306;database=marketgo;user=marketgo;password=TU_CLAVE'
$env:Jwt__SigningKey = 'UNA_CLAVE_ALEATORIA_DE_AL_MENOS_32_BYTES'
```

La API no crea la base ni aplica migraciones automaticamente. Las migraciones existentes son anteriores a los modelos de perfiles, conservacion, comunicacion, mermas y donaciones agregados en esta etapa. **No apliques esas migraciones como esquema final:** primero hay que generar y revisar una nueva migracion, y luego probarla contra MySQL. La migracion se hara en la siguiente etapa.

## Autenticacion

`POST /api/v1/auth/sign-up` crea una organizacion y su propietario. El cuerpo requiere `fullName`, `email`, `password`, `organizationName`, `accountType` (`minimarket` o `supplier`) y `acceptedTerms: true`. `POST /api/v1/auth/sign-in` devuelve el JWT. En Swagger, pega `accessToken` en **Authorize**; las rutas de negocio requieren ese token. `GET /api/v1/auth/me` y `POST /api/v1/auth/sign-out` consultan y cierran la sesion. El servidor guarda solo el hash del token de sesion; las contrasenas usan PBKDF2-SHA256.

`organizationId` es el GUID de la cuenta y debe coincidir con el de las rutas de su organizacion. Los IDs de productos, inventario, proveedores, requisiciones y ordenes tambien son GUID.

## Endpoints principales

| Contexto | Rutas |
| --- | --- |
| Profiles | `GET/PATCH /api/v1/profiles` y `/{id}` |
| Iam | `/api/v1/auth/*`, `GET/PATCH /api/v1/minimarkets/{organizationId}/users` |
| Products | `GET/POST /api/v1/products`, `GET/PATCH/DELETE /api/v1/products/{id}`, `GET /api/v1/suppliers/{organizationId}/products` |
| Inventory | `GET/POST/PATCH/DELETE .../inventory`, `GET .../lots`, `GET .../expirations`, `GET/POST .../waste`, `GET/POST .../donations` |
| Suppliers | `GET/POST /api/v1/suppliers`, `GET/PATCH /api/v1/suppliers/{id}` |
| Requisition | `GET/POST .../requisitions`, `PATCH .../requisitions/{id}` |
| Procurements | `GET/POST .../purchase-orders`, `PATCH .../purchase-orders/{id}` |
| Conservation | `GET/POST .../conservation/monitoring` |
| Communication | `GET/POST .../communication/alerts` y `.../messages`; `PATCH` para estado de lectura |
| Dashboard | `GET .../dashboard`, `GET .../activity-history` |
| Analytics | `GET .../analytics` |

`...` representa `/api/v1/minimarkets/{organizationId}` o `/api/v1/suppliers/{organizationId}` segun la ruta publicada en Swagger. Las consultas filtran por organizacion y las operaciones de merma y donacion descuentan el inventario en el mismo `SaveChanges`.

## Verificacion sin MySQL

```powershell
dotnet build .\marketgo-backend.sln
dotnet run --project .\tests\Unit\MarketGo.Iam.Smoke\MarketGo.Iam.Smoke.csproj
dotnet run --project .\tests\Unit\MarketGo.Business.Smoke\MarketGo.Business.Smoke.csproj
```

Las pruebas de humo cubren reglas de dominio y el modelo EF; no sustituyen las pruebas HTTP con MySQL. El frontend actualmente usa IDs de demostracion, URL base `http://localhost:3000/` y un inicio de sesion simulado cuando Firebase esta desactivado. Para conectarlo a esta API hay que configurar la URL y usar el JWT y los GUID reales. Ese ajuste corresponde a la integracion con el frontend.

## Ramas

`develop` integra los contextos antes de `main`. Cada bounded context conserva su rama: `feature/iam`, `feature/profiles`, `feature/suppliers`, `feature/products`, `feature/inventory`, `feature/requisition`, `feature/procurements`, `feature/conservation`, `feature/communication`, `feature/dashboard` y `feature/analytics`. La infraestructura comun esta en `feature/shared` y el conjunto de esta etapa en `feature/backend-completion`.
