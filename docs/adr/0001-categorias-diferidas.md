# ADR 0001: Categorías de productos diferidas al MVP

## Estado
Aceptada.

## Contexto
El sistema POS de Deporcheves maneja un catálogo de cerveza, botanas y abarrotes. Durante la Fase 1 y 2 se discutió la necesidad de agrupar productos por categorías (ej. bebidas, botanas) para facilitar búsquedas o generar reportes.

## Decisión
No se implementará una entidad `Category` ni un campo `ProductCategory` en esta fase del MVP. El catálogo se gestionará y buscará por nombre y SKU.

## Consecuencias
- **Positivas**: Se mantiene el alcance del MVP acotado. Se evita la sobreingeniería en la base de datos y en la UI (CRUD de categorías).
- **Negativas**: No habrá filtros por categoría en el punto de venta.
- **Mitigación**: El buscador por nombre/SKU es suficiente para el volumen actual del negocio.

## Alternativas consideradas
- Enum `ProductCategory` (descartada por falta de flexibilidad si el negocio crece).
- Tabla `Category` (descartada para el MVP, evaluada para la Fase 7 de escalabilidad y reportes).