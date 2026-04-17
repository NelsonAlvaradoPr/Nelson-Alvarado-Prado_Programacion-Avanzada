# 🚗 ASP.NET Core MVC Vehicle Management Application
## Complete Guide: Database vs JSON File Data Source

---

## 📋 Overview

This is a **professional ASP.NET Core MVC application** that demonstrates how to work with **both database and JSON file data sources**. The application allows you to seamlessly switch between MySQL database and JSON file storage by modifying the `appsettings.json` configuration file.

### ✨ Key Features

✅ **Dual Data Source Support** - Automatically switches between Database and JSON  
✅ **Configuration-Driven Architecture** - Change data source in appsettings.json  
✅ **Dependency Injection** - Repositories injected based on configuration  
✅ **Comprehensive Logging** - Shows which repository is being used  
✅ **Year Range Filtering** - Search vehicles by year  
✅ **Full CRUD Operations** - Create, Read, Update, Delete vehicles  
✅ **Professional UI** - Bootstrap 5 responsive design  
✅ **System Information View** - Shows current data source configuration  

---

## 🏗️ Project Structure

```
VehicleMvcApp/
├── Models/
│   ├── Vehicle.cs                    # Vehicle model
│   └── VehicleSearchViewModel.cs     # View model
│
├── Services/
│   ├── Interfaces/
│   │   └── IVehicleRepository.cs     # Repository interface
│   └── Repositories/
│       ├── JsonVehicleRepository.cs  # JSON implementation
│       └── DatabaseVehicleRepository.cs # Database implementation
│
├── Controllers/
│   └── VehiclesController.cs         # Main controller
│
├── Views/Vehicles/
│   ├── Index.cshtml                  # Search & list page
│   ├── Details.cshtml                # Vehicle details
│   ├── Create.cshtml                 # Create form
│   ├── Edit.cshtml                   # Edit form
│   ├── Delete.cshtml                 # Delete confirmation
│   └── SystemInfo.cshtml             # System information
│
├── appsettings.json                  # Configuration (DATA SOURCE SWITCH)
├── Program.cs                        # Startup configuration
└── VehicleMvcApp.csproj             # Project file
```

---

## 🔧 Configuration: appsettings.json

The `appsettings.json` file controls which data source is used:

```json
{
  "DataSource": {
    "UseDatabase": false,
    "DatabaseType": "MySQL",
    "JsonFilePath": "../prq_cars.json",
    "ConnectionString": "Server=localhost;Database=car_park;User Id=root;Password=;Port=3306;"
  },
  "ApplicationSettings": {
    "CurrentYear": 2026,
    "DefaultMinYear": 2018,
    "DefaultMaxYear": 2026
  }
}
```

### Configuration Options

| Setting | Description | Value |
|---------|-------------|-------|
| **UseDatabase** | Enable/disable database | `true` or `false` |
| **DatabaseType** | Type of database | `MySQL` |
| **JsonFilePath** | Path to JSON file | Relative path |
| **ConnectionString** | Database connection | MySQL connection string |

---

## 📊 Architecture: Dependency Injection

The application uses **Dependency Injection** to automatically select the appropriate repository:

### In Program.cs:
```csharp
// Get configuration value
var useDatabase = builder.Configuration.GetValue<bool>("DataSource:UseDatabase");

// Register the appropriate repository
if (useDatabase)
{
    builder.Services.AddScoped<IVehicleRepository, DatabaseVehicleRepository>();
}
else
{
    builder.Services.AddScoped<IVehicleRepository, JsonVehicleRepository>();
}
```

### In VehiclesController:
```csharp
public VehiclesController(
    IVehicleRepository repository,  // ← Injected repository
    IConfiguration configuration,
    ILogger<VehiclesController> logger)
{
    _repository = repository;  // ← Can be JSON or Database
    _configuration = configuration;
    _logger = logger;
}

public async Task<IActionResult> Index(int? minYear = null, int? maxYear = null)
{
    // This works with EITHER repository implementation!
    var vehicles = await _repository.GetVehiclesByYearRangeAsync(minYear.Value, maxYear.Value);
    
    // Logs show which repository is being used
    _logger.LogWarning($"Repository Type: {_repository.GetType().Name}");
}
```

---

## 🚀 How to Use

### Installation

1. **Prerequisites**
   - .NET 8.0 SDK or later
   - Visual Studio 2022 or VS Code
   - MySQL Server (optional, for database mode)

2. **Clone/Setup Project**
   ```bash
   cd VehicleMvcApp
   dotnet restore
   ```

3. **Run the Application**
   ```bash
   dotnet run
   # Application runs on https://localhost:7001
   ```

---

## 🔀 Switching Between Data Sources

### **Option 1: Use JSON Files (Default)**

1. Open `appsettings.json`
2. Ensure this configuration:
   ```json
   "DataSource": {
     "UseDatabase": false,
     "JsonFilePath": "../prq_cars.json"
   }
   ```
3. Save the file
4. Restart the application
5. Navigate to http://localhost:7001

**Result:** Application reads/writes from `prq_cars.json` file

---

### **Option 2: Use MySQL Database**

1. Ensure MySQL is running with the database setup:
   ```sql
   CREATE DATABASE car_park;
   USE car_park;
   CREATE TABLE PRQ_Cars (
       ID INT AUTO_INCREMENT PRIMARY KEY,
       color VARCHAR(50) NOT NULL,
       year INT NOT NULL,
       make VARCHAR(100) NOT NULL,
       type ENUM('sedan', '4x4', 'motorcycle') NOT NULL,
       created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
   );
   ```

2. Open `appsettings.json`
3. Update configuration:
   ```json
   "DataSource": {
     "UseDatabase": true,
     "ConnectionString": "Server=localhost;Database=car_park;User Id=root;Password=;Port=3306;"
   }
   ```
4. Save the file
5. Restart the application
6. Navigate to http://localhost:7001

**Result:** Application reads/writes from MySQL database

---

## 📱 Application Pages

### 1. **Home/Search Page** (`/Vehicles`)
   - Shows current data source (Database or JSON)
   - Search vehicles by year range
   - Display results in table
   - Action buttons: View, Edit, Delete
   - Create new vehicle button

### 2. **Vehicle Details** (`/Vehicles/Details/{id}`)
   - Shows complete vehicle information
   - Color preview
   - Data source indicator
   - Edit/Delete buttons

### 3. **Create Vehicle** (`/Vehicles/Create`)
   - Form to add new vehicle
   - Shows which data source will be used
   - Validation
   - Submit button

### 4. **Edit Vehicle** (`/Vehicles/Edit/{id}`)
   - Form to modify vehicle
   - Pre-populated with current data
   - Shows data source

### 5. **Delete Vehicle** (`/Vehicles/Delete/{id}`)
   - Confirmation page
   - Shows vehicle details
   - Shows which data source will be updated
   - Confirm deletion

### 6. **System Information** (`/Vehicles/SystemInfo`)
   - Shows current data source (Database or JSON)
   - Shows repository type being used
   - Configuration details
   - Instructions on how to switch data sources

---

## 🔍 Demonstration: Logging Output

When you run the application, check the console to see which repository is being used:

### **Using JSON:**
```
📄 Registering JsonVehicleRepository
📄 JSON Repository initialized. File path: C:\project\prq_cars.json
📖 Reading all vehicles from JSON file...
✓ Successfully read 5 vehicles from JSON
🔍 Filtering vehicles by year range: 2020-2023 (JSON)
✓ Found 3 vehicles in year range 2020-2023
```

### **Using Database:**
```
🗄️ Registering DatabaseVehicleRepository
🗄️ Database Repository initialized
📖 Reading all vehicles from database...
✓ Successfully read 5 vehicles from database
🔍 Filtering vehicles by year range: 2020-2023 (Database)
✓ Found 3 vehicles in year range 2020-2023
```

---

## 💾 Data Flow Comparison

### JSON File Flow:
```
appsettings.json (UseDatabase: false)
         ↓
Program.cs registers JsonVehicleRepository
         ↓
VehiclesController receives JsonVehicleRepository
         ↓
JsonVehicleRepository reads/writes prq_cars.json
         ↓
Results displayed in view
```

### Database Flow:
```
appsettings.json (UseDatabase: true)
         ↓
Program.cs registers DatabaseVehicleRepository
         ↓
VehiclesController receives DatabaseVehicleRepository
         ↓
DatabaseVehicleRepository connects to MySQL
         ↓
SQL queries executed on PRQ_Cars table
         ↓
Results displayed in view
```

---

## 🎯 Usage Examples

### Search Vehicles (Both Sources)
1. Go to http://localhost:7001/Vehicles
2. Enter: Min Year = 2020, Max Year = 2023
3. Click "Search"
4. **Results come from either JSON or Database** depending on `appsettings.json`

### Create Vehicle (Both Sources)
1. Click "Add Vehicle" button
2. Fill in form:
   - Color: Blue
   - Year: 2024
   - Make: Toyota Corolla
   - Type: Sedan
3. Click "Create Vehicle"
4. **Vehicle saved to either JSON or Database** depending on `appsettings.json`

### View Current Data Source
1. Go to http://localhost:7001/Vehicles/SystemInfo
2. See badge showing: **Database** or **JSON File**
3. See current repository type
4. See instructions for switching

---

## 📊 Screenshot Guide

Here's what you should see for each data source:

### **JSON Mode Badge:**
```
📄 Data Source: JSON File
```
- Shows at top of search results
- Blue/cyan color
- File icon

### **Database Mode Badge:**
```
🗄️ Data Source: Database (MySQL)
```
- Shows at top of search results
- Green color
- Database icon

---

## ✅ Testing Checklist

- [ ] Application starts without errors
- [ ] JSON mode works (search, create, edit, delete)
- [ ] Database mode works (search, create, edit, delete)
- [ ] Can switch between modes by editing appsettings.json
- [ ] System Info page shows correct data source
- [ ] Logging shows correct repository type
- [ ] Search filters work correctly
- [ ] CRUD operations work in both modes
- [ ] UI displays data source indicator
- [ ] No errors in application output

---

## 🔧 Troubleshooting

### **Issue: "No vehicles found" in Database Mode**
- Ensure database exists and has data
- Check connection string in appsettings.json
- Verify MySQL is running
- Check that table name is `PRQ_Cars`

### **Issue: "File not found" in JSON Mode**
- Ensure `prq_cars.json` file exists in project root
- Check file path in appsettings.json
- Verify file has valid JSON data

### **Issue: Application won't start**
- Check `appsettings.json` syntax
- Ensure .NET 8.0 is installed: `dotnet --version`
- Clear build: `dotnet clean && dotnet restore`

### **Issue: Data source not switching**
- Restart application after changing appsettings.json
- Check console log to verify which repository loaded
- Ensure UseDatabase value is valid (true/false)

---

## 🎓 Learning Outcomes

This application demonstrates:
- ✅ **Dependency Injection** - Using DI for multiple implementations
- ✅ **Repository Pattern** - Abstracting data access
- ✅ **Configuration Management** - Using appsettings.json
- ✅ **MVC Architecture** - Proper separation of concerns
- ✅ **Multiple Data Sources** - Supporting both JSON and Database
- ✅ **Async/Await** - Asynchronous operations
- ✅ **Logging** - Application logging with ILogger
- ✅ **Entity Framework Alternative** - Manual SQL with MySQL

---

## 📝 Configuration Examples

### Switch to JSON (Development)
```json
{
  "DataSource": {
    "UseDatabase": false,
    "JsonFilePath": "../prq_cars.json"
  }
}
```

### Switch to Database (Production)
```json
{
  "DataSource": {
    "UseDatabase": true,
    "ConnectionString": "Server=prod-server;Database=car_park;User Id=admin;Password=secure;"
  }
}
```

---

## 🚀 Next Steps

1. **Try Both Modes**
   - Run with JSON first
   - Switch to Database and run again
   - Notice the same functionality with different storage

2. **Monitor Logging**
   - Watch console to see repository type
   - View which data source is being queried

3. **Customize**
   - Modify views to suit your needs
   - Add more vehicle properties
   - Extend functionality

4. **Deploy**
   - Use Database mode for production
   - Configure connection string
   - Set appropriate permissions

---

## 📞 Key Files to Remember

| File | Purpose |
|------|---------|
| `appsettings.json` | 🔑 **Configuration - Where you switch data source** |
| `Program.cs` | Dependency injection setup |
| `VehiclesController.cs` | Controller logic (works with both sources) |
| `IVehicleRepository.cs` | Interface (same for both implementations) |
| `JsonVehicleRepository.cs` | JSON file implementation |
| `DatabaseVehicleRepository.cs` | MySQL implementation |

---

## 🎯 Summary

This ASP.NET Core MVC application shows a **professional approach to working with multiple data sources**:

1. **Single Interface** (`IVehicleRepository`) - Same for both
2. **Two Implementations** - JSON and Database
3. **Configuration-Driven** - Switch via appsettings.json
4. **Dependency Injection** - Automatic registration
5. **Same Controller** - Works with either source
6. **Logging** - Shows which repository is in use

**The beauty of this design**: The Controller doesn't care if it's using JSON or Database - the repository abstraction handles it!

---

**Ready to see it in action! 🚀**

1. Take screenshots with JSON mode (see JSON data source badge)
2. Switch to Database mode
3. Take screenshots with Database mode (see Database data source badge)
4. Show the System Info page displaying configuration
5. Demonstrate switching and everything working seamlessly!

---

*Version 1.0.0 | ASP.NET Core MVC | 2024*
