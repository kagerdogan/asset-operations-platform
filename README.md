# ⚓ Asset Operations Platform (Maritime PMS)

A Clean Architecture based Asset-First Maritime Planned Maintenance System (PMS) built with .NET 8.

This project is designed as a domain-driven, template-based maintenance management platform focusing on scalability, versioning, and operational reliability for maritime fleets.

---

## 🧠 Architecture Approach

This project follows:

- Clean Architecture
- Domain-Driven Design (DDD)
- SOLID principles
- Separation of concerns
- Testable domain logic

Layered structure:
src/
├── AssetOperations.Domain
├── AssetOperations.Application
├── AssetOperations.Infrastructure
└── AssetOperations.Api


---

## ⚙️ Core Concepts

### 1️⃣ Asset-First Design

Maintenance operations are built around the Asset (vessel/equipment) as the aggregate root.

Each asset contains:
- Maintenance tasks
- Task status
- Due & overdue tracking
- Critical classification

---

### 2️⃣ Template & Versioning Engine

A template-driven maintenance model:

- MaintenanceTemplate
- TemplateVersion
- MaintenanceTaskDefinition

Features:
- Apply template to multiple vessels
- Version tracking
- Safe template evolution
- Controlled propagation

---

### 3️⃣ Scheduler Logic

Built-in due calculation logic including:

- Calendar-based interval tracking
- %20 due warning policy
- Overdue detection
- Critical task prioritization

Example rule:

- If task interval = 100 days
- Warning triggered at 80 days (20% rule)

---

### 4️⃣ Extensible Design (Planned)

- Approval workflow engine
- Notification domain events
- Background job processing
- Multi-vessel propagation engine
- Event-driven extensions

---

## 🏗 Tech Stack

- .NET 8
- ASP.NET Core Web API
- Clean Architecture
- In-Memory Repository (Initial Implementation)
- Dependency Injection
- Swagger

---

## 📌 Current Status

- Domain layer completed
- Template versioning structure implemented
- Scheduler rule implemented
- Overdue endpoint exposed
- Infrastructure layer ready for database integration

---

## 🎯 Vision

To build a production-grade maritime PMS platform that:

- Eliminates duplicated maintenance definitions
- Enables fleet-wide template governance
- Provides deterministic scheduling
- Supports scalable operational growth

---

## 👨‍💻 Author

Built as a portfolio engineering project to demonstrate:
- Architectural thinking
- Domain modeling capability
- Scalable backend system design
