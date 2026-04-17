# 🚗 ASP.NET Core MVC - Quick Start Guide

## ⚡ 5-Minute Setup

### Prerequisites
- .NET 8.0 SDK: https://dotnet.microsoft.com/download
- Visual Studio 2022 or VS Code
- MySQL Server (optional for database mode)

### Setup Steps

```bash
# 1. Navigate to project
cd VehicleMvcApp

# 2. Restore dependencies
dotnet restore

# 3. Run the application
dotnet run

# 4. Open browser
# https://localhost:7001/Vehicles
```

---

## 🔀 Switching Data Sources

### JSON Mode (Default)
```json
// appsettings.json
"DataSource": {
  "UseDatabase": false
}
```

### Database Mode
```json
// appsettings.json
"DataSource": {
  "UseDatabase": true,
  "ConnectionString": "Server=localhost;Database=car_park;User Id=root;Password=;"
}
```

**Then restart the application!**

---

## 📍 Application URLs

| Page | URL |
|------|-----|
| Search/List | https://localhost:7001/Vehicles |
| System Info | https://localhost:7001/Vehicles/SystemInfo |
| Details | https://localhost:7001/Vehicles/Details/1 |
| Create | https://localhost:7001/Vehicles/Create |

---

## 🔍 What to Look For

1. **Data Source Badge** - Shows current mode (Database or JSON)
2. **Logging Console** - Shows which repository is registered
3. **System Info Page** - Displays complete configuration
4. **CRUD Operations** - Work with either data source

---

## ✅ Testing

```bash
# Test 1: Run with JSON
dotnet run

# Test 2: Switch to Database
# Edit appsettings.json: "UseDatabase": true
# Restart: Ctrl+C, then dotnet run

# Test 3: Notice everything works exactly the same!
```

---

## 🐛 Common Issues

| Issue | Solution |
|-------|----------|
| Port already in use | Change port in Program.cs or use different port |
| JSON file not found | Ensure prq_cars.json exists in project root |
| Database connection error | Check MySQL running, verify connection string |
| Configuration not applied | Restart application after editing appsettings.json |

---

## 📊 File Locations

- Configuration: `VehicleMvcApp/appsettings.json` ← **MODIFY THIS TO SWITCH DATA SOURCES**
- Startup: `VehicleMvcApp/Program.cs`
- Controller: `VehicleMvcApp/Controllers/VehiclesController.cs`
- Data Access: `VehicleMvcApp/Services/Repositories/`
- Views: `VehicleMvcApp/Views/Vehicles/`

---

**Ready to start! Just run `dotnet run` and navigate to https://localhost:7001/Vehicles** 🚀
