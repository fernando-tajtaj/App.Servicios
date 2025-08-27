<h1>
  <img src="https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png" width="24" /> 
  <a href="https://github.com/fernando-tajtaj/App.Servicios">fernando-tajtaj/App.Servicios</a>
</h1>

# General
## Arquitectura del Sistema
### Propósito y Alcance

**App.Servicios** es un sistema de gestión de partidos de baloncesto que proporciona seguimiento en tiempo real de puntuaciones, gestión del estado del juego y actualizaciones en vivo.  
El sistema expone tanto **endpoints REST API** como **conexiones WebSocket** para soportar interacciones cliente-servidor tradicionales y comunicación en tiempo real para escenarios de partidos en vivo.

Este documento ofrece una visión general de alto nivel sobre la **arquitectura del sistema**, sus **componentes principales** y el **stack tecnológico**.  

- Para información detallada de la arquitectura del sistema, ver [Arquitectura del Sistema](#arquitectura-del-sistema).  
- Para detalles sobre los endpoints de la API, ver [API de Gestión de Partidos](#api-de-gestion-de-juegos)
- Para la comunicación en tiempo real, ver [Comunicación en Tiempo Real](#comunicacion-en-tiempo-real).  
- Para el modelo de datos, ver [Modelo de Datos](#modelo-de-datos).  

### Visión General del Sistema

**App.Servicios** es una aplicación web en **.NET 8.0** que actúa como backend para la gestión de partidos de baloncesto.  
Está diseñada para manejar sesiones concurrentes de partidos con actualizaciones en tiempo real transmitidas a múltiples clientes conectados, típicamente en escenarios como:  

- Marcadores en vivo.  
- Interfaces de administración de partidos.  

La aplicación está compuesta por **tres capas arquitectónicas principales**:

1. **Capa REST API** → para operaciones estándar **CRUD**.  
2. **Capa de comunicación en tiempo real** → mediante **SignalR** para actualizaciones en vivo.  
3. **Capa de persistencia de datos** → implementada con **Entity Framework Core** y **PostgreSQL**.  

---
### Arquitectura Principal del Sistema
El sistema App.Servicios sigue un patrón de arquitectura en capas con una clara separación entre las capas de presentación, lógica de negocios y acceso a datos:

![Diagrama de arquitectura](./docs/Arquitectura-sistema.png)

Fuentes:  `App.Servicios/Program.cs` `(1-55)`  
`App.Servicios/App.Servicios.csproj` `(1-19)` 

---
### Arquitectura de configuración de servicios
El inicio de la aplicación en `Program.cs` demuestra un claro patrón de inyección de dependencia y configuración de servicio:

![Diagrama de arquitectura](./docs/Arquitectura-configuracion.png)

Fuentes:  `App.Servicios/Program.cs` `(8-54)`

---
### Arquitectura de flujo de comunicación
El sistema implementa canales de comunicación duales para soportar patrones tradicionales de solicitud-respuesta y actualizaciones en tiempo real:

![Diagrama de arquitectura](./docs/Arquitectura-comunicacion.png)

Fuentes:  `App.Servicios/Program.cs` `(28-39)`

### Tecnologías y Dependencias
El sistema está construido sobre una moderna pila de .NET Core, con dependencias cuidadosamente seleccionadas para el desarrollo de Web API.

| Componente               | Tecnología                               | Versión  | Propósito                       |
|--------------------------|------------------------------------------|----------|---------------------------------|
| Framework                | .`NET 8.0`                              | 8.0     | Ejecución de Web API            |
| ORM                      | `EntityFrameworkCore`                      | 9.0.8   | Abstracción de la base de datos |
| Proveedor de Base de Datos | `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.0.4   | Integración con PostgreSQL      |
| Documentación de API     | `Swashbuckle.AspNetCore`                   | 6.6.2   | Generación de OpenAPI/Swagger   |
| Comunicación en tiempo real | `SignalR`                               | Incorporado | Gestión de hub WebSocket     |
| Base de Datos            | `PostgreSQL`                               | -       | Persistencia de datos           |

Fuentes:  `App.Servicios/App.Servicios.csproj` `(3-17)`

---
### Configuración del entorno de ejecución
La aplicación admite múltiples escenarios de implementación a través de una arquitectura basada en configuración:

![Diagrama de arquitectura](./docs/Arquitectura-comunicacion.png)

Fuentes:  `App.Servicios/Program.cs` `(15-39)`

La arquitectura demuestra patrones listos para la empresa con una clara separación de preocupaciones, capacidades integrales en tiempo real y una gestión de configuración robusta adecuada para escenarios de gestión de juegos de baloncesto que requieren actualizaciones de puntajes en vivo y sincronización de múltiples clientes.


# API de Gestion de Juegos
## Arquitectura del Controlador
Para la comunicación en tiempo real y la implementación del concentrador SignalR, consulte Comunicación en tiempo real. Para obtener más información sobre el modelo de datos del juego subyacente, consulte Entidad de juego. Para conocer los mecanismos de persistencia de la base de datos, consulte Contexto de la base de datos.

## API Endpoints
`JuegoController` implementa un patrón de API REST con transmisión en tiempo real integrada. Cada operación de la API realiza la persistencia de la base de datos, seguida de notificaciones de SignalR, para mantener el estado sincronizado en todos los clientes conectados.
![Diagrama de arquitectura](./docs/API-endpoint-1.png)

Fuentes: `App.Servicios/Controllers/JuegoController.cs` `(10-19)`
## API Endpoint Details
### Consulta de Juego

| Método | Ruta              | Propósito                         |
|--------|-----------------|----------------------------------|
| GET    | /api/Juego/{uuid} | Recuperar un juego específico por UUID |

**Detalles de `Get`:**

- Recupera una instancia de juego usando el UUID proporcionado.
- Devuelve `404 NotFound` si el juego no existe.

**Fuente:**  
`App.Servicios/Controllers/JuegoController.cs` `(21-26)`

### Creación de Juego
| Método | Ruta        | Propósito |
|--------|------------|-----------|
| GET   | `/api/Juego` | Crear un nuevo juego con configuración por defecto |

**Detalles de `CreateDefault`:**

- UUID autogenerado (minúsculas, sin guiones)
- `RemainingSeconds` por defecto: 600 (10 minutos)
- Valores por defecto para todas las demás propiedades de `Juego`

**Fuente:**  
`App.Servicios/Controllers/JuegoController.cs` `(28-40)`

### Registro de Puntuación
| Método | Ruta                   | Propósito                     | Parámetros de Consulta |
|--------|-----------------------|-------------------------------|----------------------|
| POST   | `/api/Juego/{uuid}/score` | Agregar puntos al marcador del equipo | team: "home" o "away" <br> points: valor entero |

**Detalles de `AddScore`:**

- Actualiza las propiedades `HomeScore` o `AwayScore` según el parámetro `team`.
- Los puntos se suman al valor existente del marcador.

**Fuente:** `App.Servicios/Controllers/JuegoController.cs (42-54)`

### Registro de Faltas
| Método | Ruta                   | Propósito                       | Parámetros de Consulta |
|--------|-----------------------|---------------------------------|----------------------|
| POST   | `/api/Juego/{uuid}/foul` | Incrementar el conteo de faltas del equipo | team: "home" o "away" |

**Detalles de `AddFoul`:**

- Incrementa en 1 las propiedades `HomeFouls` o `AwayFouls` según el parámetro `team`.

Fuente: `App.Servicios/Controllers/JuegoController.cs (56-68)`


### Actualización de Cuarto
| Método | Ruta                   | Propósito              |
|--------|-----------------------|------------------------|
| POST   | `/api/Juego/{uuid}/quarter` | Avanzar al siguiente cuarto |

**Detalles de `NextQuarter`:**

- Incrementa la propiedad `Quarter` (máximo 4).  
- Resetea `RemainingSeconds` a `QuarterDurationSeconds`.  
- Establece `IsRunning` en `false`.

Fuente: `App.Servicios/Controllers/JuegoController.cs (70-85)`

### Gestión del Tiempo

| Método | Ruta                   | Propósito                 | Parámetros de Consulta |
|--------|-----------------------|---------------------------|----------------------|
| POST   | `/api/Juego/{uuid}/timer` | Actualizar el estado del temporizador del juego | remainingSeconds: entero <br> isRunning: booleano |

**Detalles de `UpdateTimer`:**

- Establece directamente las propiedades `RemainingSeconds` e `IsRunning` con los valores proporcionados.

Fuente: `App.Servicios/Controllers/JuegoController.cs (87-99)`

### Flujo de solicitud-respuesta
![Diagrama de arquitectura](./docs/Flujo-Solicitud-Respuesta.png)

### Actualizaciones en Tiempo Real

Todos los endpoints que modifican el estado siguen el mismo patrón para actualizaciones en tiempo real:

1. **Actualización en la Base de Datos:** Persistir los cambios usando `context.SaveChangesAsync()`.  
2. **Broadcast con SignalR:** Enviar el evento `"MatchUpdated"` con el objeto completo `Juego`.  
3. **Respuesta HTTP:** Devolver el estado actualizado del juego al cliente que hizo la solicitud.

![Diagrama de arquitectura](./docs/SignalR.png)

El nombre del evento `"MatchUpdated"` está específicamente diseñado para coordinarse con implementaciones del cliente Angular, como se indica en los comentarios del código.

**Fuentes:**  
- `App.Servicios/Controllers/JuegoController.cs (37)`  
- `App.Servicios/Controllers/JuegoController.cs (52)`  
- `App.Servicios/Controllers/JuegoController.cs (66)`  
- `App.Servicios/Controllers/JuegoController.cs (82)`  
- `App.Servicios/Controllers/JuegoController.cs (97)`


# Comunicacion en Tiempo Real
## Propósito y Alcance

Este documento cubre el sistema de comunicación en tiempo real en **App.Servicios**, que utiliza **SignalR** para transmitir actualizaciones en vivo de partidos de baloncesto a los clientes conectados.  
El sistema asegura que todos los clientes reciban notificaciones inmediatas cuando cambie el estado del juego, como puntuaciones, faltas, cambios de cuarto y modificaciones del temporizador.

Para información sobre los endpoints de la API REST que disparan estas actualizaciones en tiempo real, ver **Game Management API**.  
Para detalles sobre el modelo de datos del juego que se transmite, ver **Game Entity**.

Fuente: `App.Servicios/Hubs/JuegoHub.cs (1-58)`

## Arquitectura SignalR Hub

El sistema de comunicación en tiempo real se centra en la clase **JuegoHub**, que extiende la clase base `Hub` de SignalR para proporcionar capacidades específicas de transmisión de eventos de baloncesto.

### Estructura de Clase JuegoHub

El `JuegoHub` proporciona cinco métodos distintos para transmitir diferentes tipos de eventos del juego, cada uno enfocado en aspectos específicos del estado del juego.

Fuente: `App.Servicios/Hubs/JuegoHub.cs (9-56)`

## Tipos de Eventos y Métodos de Transmisión

| Método                | Nombre de Evento SignalR | Propósito                        | Estructura de Datos |
|----------------------|-------------------------|---------------------------------|-------------------|
| ActualizarJuego       | `MatchUpdated`          | Actualización completa del juego | `{ uuid: string, juego: Juego }` |
| ActualizarPuntos      | `ScoreUpdated`          | Actualización de puntuación | `{ uuid: string, team: string, puntos: int }` |
| ActualizarFaltas      | `FoulUpdated`           | Actualización de faltas          | `{ uuid: string, team: string, faltas: int }` |
| CambiarCuarto         | `QuarterChanged`        | Cambio de cuarto             | `{ uuid: string, cuarto: int }` |
| ActualizarTimer       | `TimerUpdated`          | Cambio del tiempo          | `{ uuid: string, remainingSeconds: int, isRunning: bool }` |

### Flujo de Transmisión de Eventos

![Diagrama de arquitectura](./docs/Flujo-transmision-eventos.png)

Fuente: `App.Servicios/Hubs/JuegoHub.cs (9-56)`

---

## Conexión del cliente y flujo de comunicación

El hub de SignalR usa `Clients.All.SendAsync` para transmitir actualizaciones a **todos los clientes conectados** simultáneamente, asegurando consistencia en el estado del juego.
![Diagrama de arquitectura](./docs/Arquitectura-conexion.png)

### Arquitectura de Conexión

- **Clientes Angular / WebSocket** → Conectados a `JuegoHub` vía WebSocket.
- Todos los eventos (`MatchUpdated`, `ScoreUpdated`, `FoulUpdated`, `QuarterChanged`, `TimerUpdated`) se transmiten a todos los clientes.

---

## Estructura de Datos de Transmisión

Cada método de transmisión construye objetos anónimos con propiedades específicas:

- **MatchUpdated:** Contiene UUID del juego y objeto completo `Juego` para sincronización total.  
- **ScoreUpdated:** Payload mínimo con `uuid`, `team` y `puntos`.  
- **FoulUpdated:** Similar con `uuid`, `team` y `faltas`.  
- **QuarterChanged:** Identificador del juego y número del cuarto.  
- **TimerUpdated:** Estado completo del temporizador con segundos restantes y si está corriendo.

Fuente: `App.Servicios/Hubs/JuegoHub.cs (11-15, 21-26, 31-36, 41-45, 51-55)`

---

## Integración con la API de gestión de juegos

Los métodos de `JuegoHub` se llaman desde los controladores REST API cuando cambia el estado del juego, creando un patrón de doble comunicación.

### Patrón de integración típico
- `ActualizarJuego` recibe el objeto completo `Juego`.  
- Otros métodos reciben UUID del juego y valores específicos modificados.  
- Todos los métodos usan `await` para transmisión asíncrona y no bloquear las respuestas HTTP.

Este diseño permite que la API REST maneje la **persistencia de datos** mientras que `JuegoHub` gestiona la **distribución en tiempo real**, manteniendo separación de responsabilidades.

Fuente: `App.Servicios/Hubs/JuegoHub.cs (9-27)`


# Modelo de Datos
# Capa de Persistencia de Datos en App.Servicios

## Propósito y Alcance

Este documento cubre la capa de **persistencia de datos** en **App.Servicios**, incluyendo las entidades de la base de datos, la configuración de **Entity Framework Core** y los patrones de acceso a datos.  
El modelo de datos está centrado en la gestión de partidos de baloncesto con capacidades de seguimiento de estado en tiempo real.

Para un análisis detallado de las propiedades de la entidad `Juego` y su lógica de negocio, ver **Game Entity**.  
Para la configuración de Entity Framework y la conexión a la base de datos, ver **Database Context**.

---

## Visión General de la Arquitectura de Datos

La capa de datos utiliza **Entity Framework Core** con **PostgreSQL** para persistir el estado de los partidos de baloncesto.  
La arquitectura sigue un diseño simple de **entidad única**, centrado en el modelo `Juego`.

![Diagrama de arquitectura](./docs/Arquitectura-datos.png)

Fuentes: `App.Servicios/Models/BasketDbContext.cs (5-18)`
`App.Servicios/Models/Juego.cs (5-19)`

## Estructura del Modelo de Entidad

El modelo de datos principal consiste en una sola entidad que representa el estado de un partido de baloncesto, con seguimiento completo de puntuaciones, faltas, temporizador y flujo de juego.
![Diagrama de arquitectura](./docs/Estructura-datos.png)

## Propiedades de la Entidad

La entidad `Juego` contiene todas las propiedades necesarias para la gestión completa de un partido de baloncesto:

| Propiedad               | Tipo     | Valor por defecto | Propósito |
|-------------------------|---------|-----------------|-----------|
| Uuid                    | string  | requerido       | Clave primaria para identificar el juego |
| HomeTeam                | string  | "Local"         | Nombre del equipo local |
| AwayTeam                | string  | "Visitante"     | Nombre del equipo visitante |
| HomeScore               | int     | 0               | Puntuación actual del equipo local |
| AwayScore               | int     | 0               | Puntuación actual del equipo visitante |
| Quarter                 | int     | 1               | Cuarto actual (1-4) |
| HomeFouls               | int     | 0               | Conteo de faltas del equipo local |
| AwayFouls               | int     | 0               | Conteo de faltas del equipo visitante |
| QuarterDurationSeconds  | int     | 600             | Duración de cada cuarto (10 minutos) |
| RemainingSeconds        | int     | 0               | Tiempo restante del cuarto actual |
| IsRunning               | bool    | false           | Indica si el reloj del juego está activo |

Fuente: `App.Servicios/Models/Juego.cs (7-18)`

## Configuración de la Base de Datos

La clase `BasketDbContext` extiende `DbContext` de Entity Framework y proporciona la siguiente configuración:

### **Mapeo de Tabla:**
- La entidad `Juego` se mapea a la tabla `"juego"` en PostgreSQL mediante `OnModelCreating()`. <br/> 
Fuente: `App.Servicios/Models/BasketDbContext.cs (14)`

### **Configuración de DbSet:**
- Expone la propiedad `DbSet<Juego> Juego` para operaciones sobre la entidad. <br/>
  Fuente: `App.Servicios/Models/BasketDbContext.cs (10)`

### **Patrón de Constructor:**
- Usa inyección de dependencias con el parámetro `DbContextOptions<BasketDbContext>`.
Fuente: `App.Servicios/Models/BasketDbContext.cs (7-8)`

Fuente: `App.Servicios/Models/BasketDbContext.cs (12-15)`

