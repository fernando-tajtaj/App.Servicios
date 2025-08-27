# fernando-tajtaj/App.Servicios

# Visión General

## Propósito y Alcance
**App.Servicios** es un sistema de gestión de partidos de baloncesto que proporciona seguimiento en tiempo real de puntuaciones, gestión del estado del juego y actualizaciones en vivo.  
El sistema expone tanto **endpoints REST API** como **conexiones WebSocket** para soportar interacciones cliente-servidor tradicionales y comunicación en tiempo real para escenarios de partidos en vivo.

Este documento ofrece una visión general de alto nivel sobre la **arquitectura del sistema**, sus **componentes principales** y el **stack tecnológico**.  

- Para información detallada de la arquitectura del sistema, ver [Arquitectura del Sistema](#arquitectura-del-sistema).  
- Para detalles sobre los endpoints de la API, ver [API de Gestión de Partidos](#api-de-gestión-de-juegos).  
- Para la comunicación en tiempo real, ver [Comunicación en Tiempo Real](#comunicación-en-tiempo-real).  
- Para el modelo de datos, ver [Modelo de Datos](#modelo-de-datos).  

## Visión General del Sistema

**App.Servicios** es una aplicación web en **.NET 8.0** que actúa como backend para la gestión de partidos de baloncesto.  
Está diseñada para manejar sesiones concurrentes de partidos con actualizaciones en tiempo real transmitidas a múltiples clientes conectados, típicamente en escenarios como:  

- Marcadores en vivo.  
- Interfaces de administración de partidos.  

La aplicación está compuesta por **tres capas arquitectónicas principales**:

1. **Capa REST API** → para operaciones estándar **CRUD**.  
2. **Capa de comunicación en tiempo real** → mediante **SignalR** para actualizaciones en vivo.  
3. **Capa de persistencia de datos** → implementada con **Entity Framework Core** y **PostgreSQL**.  

**Fuentes:**  
- `App.Servicios/Program.cs` (líneas 1-55)  
- `App.Servicios/App.Servicios.csproj` (líneas 1-19)  

---
# Arquitectura Principal del Sistema

![Diagrama de arquitectura](./docs/Arquitectura del Sistema.png)