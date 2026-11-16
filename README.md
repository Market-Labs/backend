# MarketGo Backend

API REST de MarketGo desarrollada con **ASP.NET Core 10**, **Entity Framework Core 10** y **MySQL 8**. El código sigue Domain-Driven Design (DDD), separa comandos y consultas por bounded context y usa JWT para proteger las operaciones de negocio.

- Backend publicado: https://marketgo-backend.vercel.app/
- Frontend: https://front-end-tan-kappa-94.vercel.app/
- Landing page: https://landing-page-acme-758f.vercel.app/
- Repositorio: https://github.com/Market-Labs/backend

## Tecnologías y requisitos

| Componente | Uso |
| --- | --- |
| .NET SDK 10 | Compilar y ejecutar la API |
| ASP.NET Core | Controladores REST, OpenAPI y Swagger UI |
| EF Core + MySql.EntityFrameworkCore | Persistencia y migraciones de MySQL |
| JWT + BCrypt | Autenticación y contraseñas |
| Docker | Construcción del contenedor para Vercel mediante `Dockerfile.vercel` |

Para la evaluación con la base compartida se necesita conexión a Internet y **.NET SDK 10**. No hace falta instalar MySQL Server en la computadora del evaluador: la base está en Enzonic. MySQL Workbench es opcional para inspeccionar tablas. Los comandos siguientes están escritos para **PowerShell**, incluida la terminal de Rider.

## Inicio rápido en Rider

1. Clona el repositorio y abre la solución `marketgo-backend.sln` en Rider. En su terminal:

   ```powershell
   git clone https://github.com/Market-Labs/backend.git marketgo-backend
   cd marketgo-backend
   dotnet --version
   ```

   `dotnet --version` debe mostrar **10.x**. Si muestra 8.x, instala o selecciona el SDK 10 antes de continuar.

2. En **la misma terminal**, configura la base de evaluación y una clave JWT local:

   ```powershell
   $env:ConnectionStrings__DefaultConnection = 'Server=marketgo-sql-8t32nr.db.enzonic.com;Port=3306;Database=main;User=u_tqm3lqtnsl.marketgo-sql-8t32nr;Password=sqP81Ha6YWcwkaKtDwRc0A0Q5KwwVcpK;SslMode=Required'
   $env:TokenSettings__Secret = ([guid]::NewGuid().ToString('N') + [guid]::NewGuid().ToString('N'))
   ```

   El segundo comando genera automáticamente la clave JWT local; no tienes que crearla ni escribirla. Si abres una terminal nueva, vuelve a copiar los dos comandos de este paso antes de iniciar la API.

3. Restaura dependencias y ejecuta el perfil HTTP de desarrollo:

   ```powershell
   dotnet restore .\marketgo-backend.sln
   dotnet run --project .\marketgo-backend\marketgo-backend.csproj --launch-profile http
   ```

4. Cuando la terminal muestre `Now listening on: http://localhost:5262`, abre:

   - Swagger UI: http://localhost:5262/swagger
   - Scalar: http://localhost:5262/scalar/v1
   - OpenAPI JSON: http://localhost:5262/openapi/v1.json
   - Health: http://localhost:5262/api/v1/health

   La raíz `http://localhost:5262/` no tiene página y puede responder **404**; usa `/swagger`. Detén la API con `Ctrl+C`.

### Cuentas de evaluación

| Tipo | Correo | Contraseña |
| --- | --- | --- |
| Administrador de minimarket | `administrador@marketgo.com` | `administrador` |
| Proveedor | `proveedor@marketgo.com` | `proveedor` |

En Swagger, usa `POST /api/v1/auth/sign-in` con el correo y contraseña de una cuenta. La respuesta contiene `token` y `user`. Pulsa **Authorize**, pega **solo el token**, confirma y prueba las rutas protegidas. El `organizationId` de la respuesta identifica a la organización; las rutas privadas deben utilizar el ID y tipo de cuenta correspondientes. Para alternar entre minimarket y proveedor, inicia sesión con la otra cuenta y reemplaza el token en **Authorize**. No se necesita registrar estas cuentas otra vez.

### Comprobar MySQL en Workbench

Usa una conexión **Standard (TCP/IP)** con host `marketgo-sql-8t32nr.db.enzonic.com`, puerto `3306`, usuario `u_tqm3lqtnsl.marketgo-sql-8t32nr`, contraseña `sqP81Ha6YWcwkaKtDwRc0A0Q5KwwVcpK`, esquema `main` y SSL **Required**. La misma conexión se comprobó desde el cliente MySQL. Para consultar el historial sin cambiar datos:

```powershell
mysql -h marketgo-sql-8t32nr.db.enzonic.com -P 3306 -u u_tqm3lqtnsl.marketgo-sql-8t32nr -p --ssl-mode=REQUIRED main
```

El cliente `mysql` pedirá la contraseña de la tabla anterior. Después puedes ejecutar:

```sql
USE main;
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```

La base compartida ya tiene las cinco migraciones indicadas abajo. **No ejecutes `dotnet ef database update` contra `main` para seguir este inicio rápido.** Ese comando se reserva para crear una base nueva o un entorno de pruebas propio.

## Estructura del proyecto

```text
marketgo-backend.sln
Dockerfile.vercel
marketgo-backend/
├── Program.cs                  # Registro de servicios y pipeline HTTP
├── appsettings*.json           # Configuración común y por entorno
├── Migrations/                # Evolución del esquema EF Core
├── Iam/                       # Registro, usuarios y JWT
├── Profiles/                  # Perfiles y contactos de clientes
├── Suppliers/                 # Directorio de proveedores
├── Products/                  # Catálogo de productos
├── Inventory/                 # Existencias, lotes, mermas y donaciones
├── Requisition/               # Solicitudes de abastecimiento
├── Procurements/              # Órdenes y recepción
├── Conservation/              # Monitoreo de condiciones
├── Communication/             # Mensajes y alertas
├── Dashboard/                 # Indicadores operativos
├── Analytics/                 # Resúmenes e informes
└── Shared/                    # Infraestructura y conceptos compartidos
```

Cada bounded context conserva las carpetas `Application`, `Domain`, `Infrastructure`, `Interfaces` y `Resources`. `Domain` contiene las reglas y agregados; `Application`, los casos de uso y fachadas ACL; `Infrastructure`, persistencia y adaptadores; `Interfaces`, controladores y DTO REST; `Resources`, textos y recursos propios del contexto. `Shared` contiene el `AppDbContext`, repositorios base, unidad de trabajo, auditoría y manejo común de errores.

## Configuración por entorno

| Origen | Función |
| --- | --- |
| `appsettings.json` | Valores comunes, emisor/audiencia JWT y opciones base |
| `appsettings.Development.json` | Logging y CORS para frontends locales |
| `appsettings.Production.json` | Logging y CORS del frontend publicado |
| Variables de entorno | Cadena de MySQL y secreto JWT usados al ejecutar |

ASP.NET Core combina estos orígenes de configuración. `ASPNETCORE_ENVIRONMENT=Development` activa Swagger y Scalar; el perfil `http` ya lo establece. En Vercel se usa `Production`, por lo que Swagger no se expone allí. El backend lee `ConnectionStrings__DefaultConnection` y `TokenSettings__Secret`; no utiliza las variables `DATABASE_URL`, `JWT_SECRET` ni `SPRING_PROFILES_ACTIVE` del proyecto Java de referencia. El Dockerfile de Vercel utiliza .NET 10 y escucha en el puerto 80.

### Ejecución opcional con Docker

Si Docker está instalado, usa las variables de entorno definidas en el paso 2 del inicio rápido y, desde la raíz del repositorio:

```powershell
docker build -f Dockerfile.vercel -t marketgo-backend .
docker run --rm -p 8080:80 -e ASPNETCORE_ENVIRONMENT=Production -e ConnectionStrings__DefaultConnection -e TokenSettings__Secret marketgo-backend
```

Comprueba `http://localhost:8080/api/v1/health`. Swagger solo está habilitado en `Development`, por lo que para explorarlo desde Rider usa el perfil `http` del inicio rápido. No hace falta Docker para la evaluación local.

## API y flujos principales

| Contexto | Rutas de referencia |
| --- | --- |
| IAM | `POST /api/v1/auth/sign-up`, `POST /api/v1/auth/sign-in`, `GET /api/v1/auth/me` |
| Profiles | `/api/v1/profiles`; contactos propios del proveedor en `/api/v1/suppliers/{organizationId}/clients` |
| Suppliers | `/api/v1/suppliers` para el directorio del minimarket |
| Products | `/api/v1/products` y `/api/v1/suppliers/{organizationId}/products` |
| Inventory | `/api/v1/{minimarkets|suppliers}/{organizationId}/inventory` y `/waste`; `/donations` para minimarkets |
| Requisition | `/api/v1/{minimarkets|suppliers}/{organizationId}/requisitions` |
| Procurements | `/api/v1/{minimarkets|suppliers}/{organizationId}/purchase-orders` |
| Conservation | `/api/v1/{minimarkets|suppliers}/{organizationId}/conservation/monitoring` |
| Communication | `/api/v1/{minimarkets|suppliers}/{organizationId}/communication/messages` y `/alerts` |
| Dashboard y Analytics | `/api/v1/{minimarkets|suppliers}/{organizationId}/dashboard` y `/analytics` |

Las rutas de negocio requieren JWT. Un minimarket vincula una organización proveedora usando `supplierOrganizationId`, crea una solicitud y el proveedor puede generar una orden. Al aceptar la recepción, el minimarket envía lotes, vencimientos y cantidades; el backend registra movimientos y actualiza inventario en una transacción. Cada organización accede solamente a sus recursos privados.

## Migraciones de EF Core

En `marketgo-backend/Migrations` están:

1. `20261114082351_InitialCreate`
2. `20261114114926_LinkSupplierOrganization`
3. `20261114180000_ReceivePurchaseOrderIntoInventory`
4. `20261114190000_RouteMessagesBetweenOrganizations`
5. `20261116090000_AddClientContacts`

El historial de estas cinco migraciones se comprobó en la base compartida `main`. Para una **base nueva y aislada**, configura su propia cadena de conexión y ejecuta `dotnet ef database update --project .\marketgo-backend\marketgo-backend.csproj --startup-project .\marketgo-backend\marketgo-backend.csproj --context AppDbContext` con `dotnet-ef` compatible con .NET 10.

## Compilación y comprobación

Desde la raíz del repositorio:

```powershell
dotnet build .\marketgo-backend.sln
```

La ruta `/api/v1/health` indica que el proceso responde. Para comprobar MySQL desde la API, usa `sign-in` con una cuenta de evaluación siguiendo el inicio rápido.

## Resolución rápida de problemas

| Resultado | Revisión |
| --- | --- |
| `NETSDK1045` | `dotnet --version` debe mostrar SDK 10.x |
| `/` responde 404 | Abre `/swagger` en Development |
| API responde 503 | Falta `ConnectionStrings__DefaultConnection` en la terminal que ejecuta la API |
| API responde 401 | Inicia sesión y pega el JWT en **Authorize** |
| API responde 403 | Comprueba el tipo de cuenta y que el `organizationId` de la ruta sea el del usuario |
| Error de conexión MySQL | Comprueba host, puerto, usuario completo, base `main`, contraseña y SSL Required |
