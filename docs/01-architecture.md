# Arquitectura de SmartWallet

## Capas
- `API` → Presentación y controladores.
- `Application` → Casos de uso y lógica de negocio.
- `Infrastructure` → Persistencia y acceso a datos.
- `Domain` → Entidades y reglas de dominio.

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
│   Infrastructure   │ → EF Core, repositories, external services
└────────────────────┘
         │
         ▼
┌────────────────────┐
│   Application      │ → Use cases, interfaces, validations
└────────────────────┘
         │
         ▼
┌────────────────────┐
│      Domain        │ → Entities, business logic
└────────────────────┘
```

- [Diagrama de clases](diagrams/class-diagram.png)
- [Diagrama entidad-relación](diagrams/er-diagram.png)