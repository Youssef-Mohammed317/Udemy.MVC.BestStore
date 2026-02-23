# 🛒 BestStore – ASP.NET Core MVC E-Commerce (Main MVC Project)

A full-featured **ASP.NET Core MVC (Razor Views)** E-Commerce application built with a clean, maintainable architecture and real-world features: **Identity + Roles**, **Email workflows**, **Cookie-based cart**, **PayPal checkout**, **image handling**, and a unified **Result<T>** pattern.

🌐 **Live Demo (Azure):** *(Coming soon — will be deployed on Azure)*  
🎥 **Walkthrough Video:** https://drive.google.com/file/d/1kFe04116QCRUnx0q7SxTS2tMT79i_R3D/view?usp=drive_link  
📜 **Certificate:** https://drive.google.com/file/d/1YIg6vdumR7D6KExPfneZDi7ZP7JNzIwE/view?usp=drive_link  
🎓 **Udemy Course:** https://www.udemy.com/course/aspnet-core-mvc-guide/  

---

## ✨ Highlights

- Clean **3-Layer Architecture + Common Layer**
- **Result<T> pattern** for unified success/failure handling
- **Repository + Unit of Work** patterns
- **AutoMapper** mapping:
  - Entity ↔ DTO
  - DTO ↔ ViewModel
- Full **ASP.NET Core Identity**:
  - Registration / Login
  - Email verification
  - Admin / Customer roles
  - Change / Forgot / Reset password flows
- **Brevo** email sender integration
- Product listing with:
  - Pagination
  - Sorting (single column)
  - Search (Name / Brand)
- Shopping cart:
  - Cookie-based (JSON → encoded storage)
  - Decoded & deserialized per request
- **PayPal** payment gateway integrated into checkout
- Images:
  - Upload product images
  - Safe delete from storage

---

## 🧱 Architecture

This solution is structured into multiple layers with clear responsibilities:

### ✅ Common Layer (Shared)
- Domain Entities
- `Result<T>` pattern for consistent API/UI responses
- Shared base models / constants

### ✅ Data Access Layer (Infrastructure)
- Entity Framework Core
- Application DbContext
- Identity DbContext
- Fluent API configurations
- Repository implementations
- Identity service implementations

### ✅ Services Layer (Application)
- DTOs
- Interfaces for repositories & services
- Business services implementations
- Email service implementation
- Non-Identity domain services

### ✅ Web Layer (MVC)
- MVC Controllers
- ViewModels
- Razor Views
- `CurrentUserService` (based on HttpContext)
- TempData alerts / notifications

---

## 🔁 Design Patterns Used

- Repository Pattern
- Unit of Work Pattern
- Result Pattern
- Dependency Injection per layer
- AutoMapper for consistent mapping

---

## 🔐 Authentication & Authorization

- ASP.NET Core Identity
- Registration & Login
- Email verification (supported workflows):
  - Verify during login **or**
  - Allow login and verify later
- Role-based authorization:
  - **Admin**
  - **Customer**
- Password management:
  - Change Password
  - Forgot Password
  - Reset Password

---

## 📧 Email Integration (Brevo)

Brevo Email Sender is used for:
- Email confirmation
- Password reset
- Notifications

Implemented as a dedicated service and injected via DI.

---

## 🛍️ E-Commerce Features

### Products
- Pagination
- Sorting (single column)
- Search (Name / Brand)

### Image Handling
- Upload product images
- Safe delete from storage when removing/updating a product image

### TempData
- Alerts & notifications
- `TempData.Keep()` for multi-step flows

---

## 🛒 Shopping Cart

Cookie-based cart implementation:
- Stored as **JSON**
- Encoded before storage
- Decoded + deserialized on each request  
Designed to work across multiple requests/pages without session dependence.

---

## 💳 Payment (PayPal)

PayPal integration included in the checkout workflow:
- Payment confirmation
- Order completion after successful payment

---

## 📁 Solution Structure (Top Level)

```text
.
├─ BestStore.Shared
├─ BestStore.Infrastructure
├─ BestStore.Application
├─ BestStore.Web
└─ BestStore.slnx
