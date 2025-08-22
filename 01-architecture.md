# Arquitectura de SmartWallet

## Capas
- `API` → Presentación y controladores.
- `Application` → Casos de uso y lógica de negocio.
- `Domain` → Entidades y reglas de dominio.
- `Infrastructure` → Persistencia y acceso a datos.

---

## Principios
- Arquitectura limpia.
- Inyección de dependencias.
- Separación de responsabilidades.

---

## Diagramas
- Diagrama de capas
```
┌────────────────────┐
│   Presentation     │ → Controllers, DTOs, Swagger
└────────────────────┘
         │
         ▼
┌────────────────────┐
│   Application      │ → Use cases, interfaces, validations
└────────────────────┘
         │
         ▼
┌────────────────────┐
│   Infrastructure   │ → EF Core, repositories, external services
└────────────────────┘
         │
         ▼
┌────────────────────┐
│      Domain        │ → Entities, business logic
└────────────────────┘
```

- [Diagrama de clases](diagrams/class-diagram.png)
- [Diagrama entidad-relación](diagrams/er-diagram.png)