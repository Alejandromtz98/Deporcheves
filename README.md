# Deporcheves POS

Punto de Venta (POS) web desarrollado para un negocio local de venta de cerveza, botanas y abarrotes. Este proyecto fue construido como un proyecto curricular para demostrar habilidades en el desarrollo backend con .NET, arquitecturas limpias, DevOps y despliegue en la nube.

## Tecnologías Utilizadas

- **Backend**: C# 12, .NET 8 (ASP.NET Core MVC)
- **Base de Datos**: PostgreSQL (Producción), SQLite (Desarrollo inicial)
- **ORM**: Entity Framework Core 8
- **Frontend**: Razor Views, Bootstrap 5
- **Testing**: xUnit, Moq
- **DevOps**: Docker, Docker Compose, GitHub Actions (CI/CD)
- **Despliegue**: Render (App) + Neon (Base de Datos)

## Arquitectura

El proyecto sigue los principios de **Clean Architecture** simplificada, dividido en cuatro capas principales:

- `Deporcheves.Domain`: Entidades, reglas de negocio e interfaces (corazón del sistema).
- `Deporcheves.Application`: Casos de uso, servicios de aplicación y DTOs.
- `Deporcheves.Infrastructure`: Implementación de persistencia (EF Core), repositorios y seguridad.
- `Deporcheves.Web`: Punto de entrada (Controladores MVC, Vistas, configuración de DI).

## Características Principales

- **Módulo de Ventas**: Punto de venta rápido con búsqueda de productos, carrito en sesión y métodos de pago (Efectivo, Tarjeta, Transferencia).
- **Módulo de Compras e Inventario**: Registro de compras a proveedores, actualización automática de stock y costo.
- **Cierre de Caja**: Cortes parciales (X) y cierres totales (Z) con cálculo de efectivo esperado, gastos, retiros y diferencia.
- **Autenticación y Roles**: Login con cookies y hash BCrypt. Roles de Administrador (acceso total) y Cajero (solo ventas y caja).
- **Alertas de Stock**: Indicadores visuales en el POS cuando un producto está por debajo del stock mínimo.

## Cómo ejecutar el proyecto localmente

### Prerrequisitos
- .NET 8 SDK
- Docker y Docker Compose

### Pasos

1. Clona el repositorio:
   ```bash
   git clone https://github.com/Alejandromtz98/Deporcheves.git