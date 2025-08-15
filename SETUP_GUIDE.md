# 🚀 E-LaptopShop - Quick Setup Guide

## 📋 Prerequisites Checklist

- [ ] ✅ .NET 9.0 SDK installed
- [ ] ✅ SQL Server LocalDB or SQL Server Express  
- [ ] ✅ Visual Studio 2022 or VS Code
- [ ] ✅ Git installed

## 🛠️ Step-by-Step Installation

### 1️⃣ Clone Repository
```bash
git clone https://github.com/yourusername/E-LaptopShop.git
cd E-LaptopShop
```

### 2️⃣ Configure Database
```bash
# Update connection string in appsettings.json if needed
# Default: LocalDB
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ELaptopShopDb;Trusted_Connection=true"
```

### 3️⃣ Run Migrations
```bash
dotnet ef database update --project E-LaptopShop.Infra
```

### 4️⃣ Seed Initial Data
Execute the SQL script in your database:
```sql
-- Run: update_roles_2025.sql
-- This creates all the necessary roles
```

### 5️⃣ Start Application
```bash
dotnet run --project E-LaptopShop
```

### 6️⃣ Test Setup
Open browser and navigate to:
```
🌐 http://localhost:5208/swagger
```

## 🧪 Quick Test

1. **Register a new user** via Swagger UI
2. **Login** to get JWT token  
3. **Authorize** in Swagger (click 🔒 button)
4. **Test protected endpoints** like shopping cart

## 🎯 What's Included

✅ **9 User Roles** - Customer, Sales, Manager, Admin, etc.  
✅ **JWT Authentication** - Modern security with token rotation  
✅ **Complete E-Commerce** - Products, Cart, Orders  
✅ **File Upload** - Chunked file uploads  
✅ **Swagger Documentation** - Interactive API docs  
✅ **Clean Architecture** - Enterprise-grade code structure  

## 🔐 Default Admin Account

After running the setup, you can create an admin account via:
1. Register normally via `/api/auth/register`
2. Manually update the user's role in database to "Admin"
3. Or create via SQL script

## 📞 Need Help?

- 📖 Check the full [README.md](README.md)
- 🌐 Browse API docs at `/swagger`
- 🐛 Report issues on GitHub
- 💬 Join our Discord community

**🎉 You're ready to go! Happy coding! 🚀**
