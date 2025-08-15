# 🛒 E-LaptopShop - Enterprise E-Commerce Platform

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

**🚀 Modern E-Commerce API với Clean Architecture & JWT Authentication 2025**

[📖 Documentation](#-features) • [🛠️ Installation](#️-quick-start) • [🔐 Authentication](#-authentication--authorization) • [📱 API Reference](#-api-endpoints) • [🧪 Testing](#-testing)

</div>

---

## 🌟 Features

### 🔥 **Core Features**
- ✅ **Complete E-Commerce Solution** - Products, Categories, Shopping Cart, Orders
- ✅ **JWT Authentication 2025** - Token rotation, HTTP-only cookies, Account lockout
- ✅ **Role-Based Authorization** - 9 roles with fine-grained permissions
- ✅ **File Upload System** - Chunked uploads for large files
- ✅ **RESTful API Design** - Consistent endpoints with proper HTTP methods
- ✅ **Swagger Documentation** - Interactive API documentation

### 🏛️ **Architecture Features**
- ✅ **Clean Architecture** - Domain, Application, Infrastructure, API layers
- ✅ **CQRS Pattern** - Command Query Responsibility Segregation with MediatR
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **AutoMapper** - Object-to-object mapping
- ✅ **FluentValidation** - Request validation
- ✅ **Global Exception Handling** - Centralized error management

### 🛡️ **Security Features**
- ✅ **Modern JWT Implementation** - Access + Refresh tokens
- ✅ **Account Security** - Login attempts tracking, account lockout
- ✅ **Password Security** - PBKDF2 with HMACSHA256 hashing
- ✅ **HTTP-Only Cookies** - Secure refresh token storage
- ✅ **CORS Configuration** - Cross-origin resource sharing

---

## 🛠️ Tech Stack

### **Backend**
- **Framework**: .NET 9.0
- **Language**: C# 12
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server
- **Authentication**: JWT Bearer Tokens
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **CQRS**: MediatR
- **Documentation**: Swagger/OpenAPI

### **Architecture Patterns**
- **Clean Architecture**
- **CQRS (Command Query Responsibility Segregation)**
- **Repository Pattern**
- **Dependency Injection**
- **Domain-Driven Design (DDD)**

---

## ⚡ Quick Start

### 📋 Prerequisites

```bash
# Required software
- .NET 9.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 / VS Code
- Git
```

### 🚀 Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/E-LaptopShop.git
cd E-LaptopShop
```

2. **Update database connection string**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ELaptopShopDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

3. **Run database migrations**
```bash
dotnet ef database update --project E-LaptopShop.Infra
```

4. **Seed initial data (roles)**
```sql
-- Execute update_roles_2025.sql in your database
-- Creates: Customer, Sales, Warehouse, Support, Moderator, Manager, Admin, VIP, Partner roles
```

5. **Run the application**
```bash
dotnet run --project E-LaptopShop
```

6. **Access Swagger UI**
```
🌐 http://localhost:5208/swagger
```

---

## 🔐 Authentication & Authorization

### 🎭 **Roles System**

| Role | Level | Description | Permissions |
|------|-------|-------------|-------------|
| 👤 **Customer** | 1 | Default user role | Shopping cart, orders, profile |
| 💼 **Sales** | 3 | Sales staff | Customer support, discounts |
| 📦 **Warehouse** | 2 | Warehouse staff | Inventory, shipping |
| 📞 **Support** | 2 | Customer support | Order assistance, tickets |
| 🛡️ **Moderator** | 3 | Content moderator | Review management |
| 👔 **Manager** | 5 | Store manager | Product management, reports |
| 👑 **Admin** | 10 | System administrator | Full system access |
| ⭐ **VIP** | 1+ | VIP customers | Special privileges |
| 🤝 **Partner** | 2+ | Business partners | Partner dashboard |

### 🔑 **JWT Configuration**

```yaml
🔒 Security Settings:
  Access Token Lifetime: 15 minutes
  Refresh Token Lifetime: 7 days
  Algorithm: HMACSHA256
  Token Rotation: Enabled
  HTTP-Only Cookies: Enabled
  Max Login Attempts: 5
  Account Lockout: 30 minutes
```

---

## 📱 API Endpoints

### 🔓 **Public Endpoints**
```http
# Authentication
POST   /api/auth/register              # User registration
POST   /api/auth/login                 # User login
POST   /api/auth/refresh-token         # Refresh access token

# Product Catalog
GET    /api/products                   # Get all products
GET    /api/products/{id}              # Get product by ID
GET    /api/categories                 # Get all categories
GET    /api/categories/{id}            # Get category by ID
```

### 👤 **Customer Endpoints** (JWT Required)
```http
# Profile Management
GET    /api/auth/me                    # Get current user info
POST   /api/auth/logout                # Logout user

# Shopping Cart
GET    /api/shoppingcart               # Get user's cart
POST   /api/shoppingcart/items         # Add item to cart
PUT    /api/shoppingcart/items/{id}    # Update cart item
DELETE /api/shoppingcart/items/{id}    # Remove cart item

# Orders
POST   /api/orders                     # Create new order
GET    /api/orders/my-orders           # Get user's orders
GET    /api/orders/{id}                # Get order details
POST   /api/orders/{id}/cancel         # Cancel order
```

### 👑 **Admin Endpoints** (Admin Role Required)
```http
# User Management
GET    /api/users                      # Get all users
POST   /api/users                      # Create new user
PUT    /api/users/{id}                 # Update user
DELETE /api/users/{id}                 # Delete user

# Product Management
POST   /api/products                   # Create product
PUT    /api/products/{id}              # Update product
DELETE /api/products/{id}              # Delete product

# Order Management
GET    /api/orders/admin/all           # Get all orders
PUT    /api/orders/admin/{id}/status   # Update order status
```

---

## 🧪 Testing

### 🔍 **Using Swagger UI**

1. **Access Swagger**: Navigate to `http://localhost:5208/swagger`
2. **Test Public APIs**: Try product catalog endpoints
3. **Register/Login**: Create account and get JWT tokens
4. **Authorize**: Click 🔒 button, enter `Bearer {your-token}`
5. **Test Protected APIs**: Try cart and order operations

### 📝 **Example API Calls**

#### Register a new user:
```bash
curl -X POST "http://localhost:5208/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!",
    "fullName": "John Doe",
    "phoneNumber": "1234567890"
  }'
```

#### Login:
```bash
curl -X POST "http://localhost:5208/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!"
  }'
```

---

## 🔧 Configuration

### 📝 **appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ELaptopShopDb;Trusted_Connection=true"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here-at-least-32-characters",
    "Issuer": "E-LaptopShop-API",
    "Audience": "E-LaptopShop-Client",
    "AccessTokenLifetime": 15,
    "RefreshTokenLifetime": 10080
  }
}
```

---

## 📊 Database Schema

### 🗄️ **Core Entities**

- **Users**: User accounts with authentication data
- **Roles**: Role-based authorization system
- **Products**: Product catalog with categories
- **ShoppingCart**: User shopping carts with items
- **Orders**: Order management with status tracking
- **SysFile**: Centralized file storage system

---

## 🚀 Deployment

### ☁️ **Cloud Deployment Options**

- **Azure App Service** - Recommended for .NET applications
- **AWS Elastic Beanstalk** - Easy deployment option
- **Digital Ocean** - Cost-effective VPS hosting

---

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

<div align="center">

### 🌟 **Star this repository if it helped you!** 🌟

**Made with ❤️ by the E-LaptopShop Team**

**🌐 Access Point**: `http://localhost:5208/swagger`  
**🛡️ Security**: PRODUCTION READY  
**🏆 Architecture**: ENTERPRISE GRADE

</div>