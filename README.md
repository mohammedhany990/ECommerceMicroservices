# 📦 E-Commerce Microservices Platform

A fully modular **E-Commerce Microservices Architecture** built with **ASP.NET Core**, applying **Clean Architecture**, **CQRS**, **MediatR**, and **REST APIs**.
This solution demonstrates a realistic, enterprise-grade distributed system containing independent services for:

* **Products**
* **Categories**
* **Cart**
* **Orders**
* **Payments (Stripe)**
* **Shipping**
* **User Management**
* **Notification**
* **API Gateway**

Each service is completely isolated with its own **Domain, Infrastructure, and Application layers**, following clean boundaries and high-quality software engineering practices.

---

## 🚀 Technologies Used

* **ASP.NET Core 9**
* **Entity Framework Core**
* **MediatR** (CQRS + Handlers + Behaviors)
* **JWT Authentication**
* **Refresh Tokens**
* **Role-based Authorization**
* **Automapper**
* **Stripe Payment Integration**
* **PostgreSQL**
* **Redis (for Cart caching)**
* **RabbitMQ**
* **Oclet & Consul**

---

## 🏛 Architecture Overview

The solution follows **Clean Architecture + Vertical Slice Architecture**, ensuring:

* Separation of concerns
* Independent deployment of services
* Easy maintainability and scalability
* Clear boundaries between Domain, Application, and Infrastructure

```
ServiceName/
│
├── API
│   ├── Controllers
│   ├── Middlewares
│   └── Models (Requests & Responses)
|   └── Extenstions   
│
├── Application
│   ├── Commands
│   ├── Queries
│   ├── Behaviors
│   ├── DTOs
│   ├── Mapping
│   └── MediatR Handlers
│
├── Domain
│   ├── Entities
│   └── Interfaces
│
└── Infrastructure
    ├── Data + Migrations
    ├── Services
    └── Repositories
    └── MessageBus

```

---

## 🧩 Microservices Included

### **🔹 Product Service**

* CRUD products
* Image storage
* Price management

### **🔹 Category Service**

* Manage categories
* Category-based filtering

### **🔹 Cart Service**

* Add/Remove items
* Update quantity
* Uses **Redis** for fast performance
* “Restore items” feature

### **🔹 Order Service**

* Create order
* Cancel/Update order
* User order history

### **🔹 Payment Service (Stripe)**

* Create PaymentIntent
* Confirm payments
* Refund payments
* Update payment status

### **🔹 Shipping Service**

* Shipping methods
* Shipping addresses
* Shipment CRUD
* Calculate shipping cost

### **🔹 User Service**

* Register/Login
* JWT + Refresh Tokens
* Role-based authorization

### **🔹 API Gateway**

* Routes all incoming requests
* Authentication handling
* Cross-service communication

---

## 🔐 Authentication & Authorization

The system uses:

* **JWT Access Tokens**
* **Refresh Tokens**
* **Role-Based Authorization**

  * `Admin`
  * `User`

Access tokens ensure security; refresh tokens ensure smooth user experience without forced logouts.

---

## 💳 Payments (Stripe)

Integrated with **Stripe** using:

* PaymentIntent
* Confirmation & refund process
* Error handling & secure communication
* Linked with OrderService for payment status updates

---

## 🗄 Database

Each service uses its **own database** to maintain independence (Database per Microservice pattern).

* PostgreSQL (Primary DB)
* Redis (Cart caching)

---

## ▶️ Running the Project

### Requirements

* .NET 9 SDK
* PostgreSQL
* Redis (for Cart Service)

### Steps

1. Clone the repository:

   ```
   git clone <repository-url>
   ```
2. Update connection strings in each service `appsettings.json`.
3. Run migrations for each service:

   ```
   dotnet ef database update
   ```
4. Launch all API projects (you can run them in Visual Studio multi-startup mode).

---

## 📁 Shared Library

Contains reusable elements:

* Shared DTOs
* Shared Response models

Used across services to ensure consistency.

---

## 🎯 Goals of This Project

* Practice microservices architecture
* Implement distributed systems with clean separation
* Improve real-world backend engineering skills
* Learn multi-service communication and domain-driven design patterns

---

## 🤝 Contributing

Pull requests are welcome.
Open an issue for bugs or feature requests.
