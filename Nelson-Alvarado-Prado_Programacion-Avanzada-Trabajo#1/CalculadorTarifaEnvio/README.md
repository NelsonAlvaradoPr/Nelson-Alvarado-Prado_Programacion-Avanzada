# Calculadora de Tarifas de Envío

Un proyecto C# que implementa la función `CalcularTarifaEnvio` para calcular el costo de envíos entre zonas geográficas, con soporte para transbordo, cálculos inversos y auditoría.

## Descripción

La clase `TarifaEnvioCalculadora` proporciona múltiples métodos para calcular costos de envío:

### Métodos Disponibles

#### 1. **CalcularTarifaEnvio** (Envío Directo)
Calcula el costo directo entre dos zonas geográficas.

**Parámetros:**
- `cantidadKilogramos` (decimal): cantidad a enviar
- `zonaOrigen` (string): código de la zona de inicio (ej: "SJO")
- `zonaDestino` (string): código de la zona de fin (ej: "MIA")
- `tarifasBase` (Dictionary<string, decimal>): tarifas por kilogramo para cada ruta

**Fórmula:** `Costo Total = Cantidad (kg) × Tarifa Base ($/kg)`

```csharp
var calculadora = new TarifaEnvioCalculadora();
decimal costo = calculadora.CalcularTarifaEnvio(15.5m, "SJO", "MIA", tarifas);
// Resultado: 15.5 × 2.50 = $38.75
```

#### 2. **CalcularTarifaConTransbordo** (Envío con Ruta Intermedia)
Calcula el costo de un envío que pasa por una ciudad intermedia.

**Fórmula:** `Costo Total = Costo(Origen→Intermedia) + Costo(Intermedia→Destino)`

```csharp
decimal costo = calculadora.CalcularTarifaConTransbordo(
    10.0m, "SJO", "MIA", "LAX", tarifas);
// SJO→MIA: 10 × 2.50 = 25.00
// MIA→LAX: 10 × 3.50 = 35.00
// Total: 60.00
```

#### 3. **CalcularTarifaInversaConSurcharge** (Envío Inverso + 10%)
Calcula el costo aplicando un surcharge del 10% sobre la tarifa base.

**Fórmula:** `Costo Total = Costo Base × 1.10`

```csharp
decimal costo = calculadora.CalcularTarifaInversaConSurcharge(
    10.0m, "SJO", "MIA", tarifas);
// Costo base: 10 × 2.50 = 25.00
// Con surcharge 10%: 25.00 × 1.10 = $27.50
```

#### 4. **CalcularTarifaCumulativa** (Envío Multi-ciudad)
Calcula el costo total para una ruta que pasa por múltiples ciudades.

**Parámetros:**
- `cantidadKilogramos` (decimal): cantidad a enviar
- `rutaCompleta` (List<string>): lista ordenada de ciudades
- `tarifasBase` (Dictionary<string, decimal>): tarifas por ruta

**Fórmula:** `Costo Total = Σ(Costo de cada segmento)`

```csharp
var ruta = new List<string> { "SJO", "MIA", "LAX" };
decimal costo = calculadora.CalcularTarifaCumulativa(10.0m, ruta, tarifas);
// SJO→MIA: 10 × 2.50 = 25.00
// MIA→LAX: 10 × 3.50 = 35.00
// Total: 60.00
```

## Características

✅ **Validación de parámetros**: Verifica que todos los inputs sean válidos  
✅ **Normalización automática**: Convierte códigos de zona a mayúsculas  
✅ **Manejo de excepciones**: Proporciona mensajes de error claros  
✅ **Auditoria integrada**: Registra todos los cálculos con timestamp  
✅ **Múltiples rutas**: Soporta transbordo y rutas complejas  
✅ **Cálculos precisos**: Utiliza tipo `decimal` para exactitud monetaria  
✅ **Backward compatible**: Las nuevas funciones no rompen código existente  

## Compilación y Ejecución

### Requisitos
- .NET 8.0 SDK o superior

### Compilar y ejecutar

```bash
# Navegar al directorio del proyecto
cd CalculadorTarifaEnvio

# Ejecutar el programa de ejemplo
dotnet run

# Compilar sin ejecutar
dotnet build
```

## Suite de Pruebas Unitarias

El proyecto incluye una suite completa de pruebas unitarias en `CalculadorTarifaEnvio.Tests/` utilizando **xUnit**.

### Ejecutar las pruebas

```bash
cd CalculadorTarifaEnvio.Tests
dotnet test
```

### Categorías de Pruebas

#### 1. **Direct Shipments** (Envíos Directos)
- Pruebas con pesos variados (pequeño, medio, grande)
- Validación de precisión decimal
- Pruebas parametrizadas con múltiples valores

#### 2. **Exception Handling** (Manejo de Excepciones)
- Rutas no existentes → `KeyNotFoundException`
- Zonas nulas o vacías → `ArgumentException`
- Pesos negativos o cero → `ArgumentException`
- Tarifas vacías o nulas → `ArgumentException`

#### 3. **Transshipment** (Transbordo)
- Cálculo correcto de múltiples segmentos
- Suma acumulada de costos
- Excepciones para rutas inválidas (origen, intermedia o destino)

#### 4. **Logging** (Auditoría)
- Validación del formato de fecha/hora: `[yyyy-MM-dd HH:mm:ss.fff]`
- Verificación de información completa en registros
- Orden cronológico de operaciones

#### 5. **Reverse Calculation** (Cálculo Inverso)
- Aplicación correcta del 10% de surcharge
- Precisión de decimales
- Manejo de excepciones

#### 6. **Cumulative Calculation** (Cálculo Cumulativo)
- Rutas de 2, 3 y 4+ ciudades
- Normalización de códigos de zona
- Validación de excepciones para rutas inválidas
- Verificación de registros de auditoría

#### 7. **Integration** (Integración)
- Compatibilidad hacia atrás
- Limpieza de registros
- Operaciones múltiples consecutivas

### Ejemplo de Prueba

```csharp
[Fact]
public void CalcularTarifaEnvio_WithMediumWeight_ShouldCalculateCorrectly()
{
    // Arrange
    decimal peso = 15.5m;
    decimal expected = 38.75m;

    // Act
    decimal resultado = _calculadora.CalcularTarifaEnvio(
        peso, "SJO", "MIA", _tarifasBase);

    // Assert
    Assert.Equal(expected, resultado);
}
```

## Rutas Disponibles en el Ejemplo

| Origen | Destino | Tarifa ($/kg) |
|--------|---------|---------------|
| SJO    | MIA     | $2.50         |
| SJO    | NYC     | $3.00         |
| SJO    | LAX     | $1.50         |
| MIA    | SJO     | $2.50         |
| MIA    | LAX     | $3.50         |
| NYC    | SJO     | $3.00         |
| LAX    | SJO     | $1.50         |
| LAX    | MIA     | $3.50         |

## Excepciones

La función lanzará las siguientes excepciones ante entradas inválidas:

| Excepción | Condición |
|-----------|-----------|
| `ArgumentException` | Cantidad ≤ 0, zona vacía, o tarifas vacías |
| `KeyNotFoundException` | Ruta no existe en el diccionario de tarifas |

## Estructura del Proyecto

```
CalculadorTarifaEnvio/
├── CalculadorTarifaEnvio.csproj
├── TarifaEnvioCalculadora.cs      # Clase principal
├── Program.cs                      # Programa de ejemplo
├── README.md                       # Este archivo
│
CalculadorTarifaEnvio.Tests/
├── CalculadorTarifaEnvio.Tests.csproj
└── TarifaEnvioTests.cs            # Suite de pruebas (xUnit)
```

## Registros de Auditoría

Cada cálculo se registra automáticamente con:
- **Timestamp**: Fecha y hora exacta en formato `yyyy-MM-dd HH:mm:ss.fff`
- **Tipo de cálculo**: DIRECTO, TRANSBORDO, INVERSA_SURCHARGE, CUMULATIVA
- **Rutas**: Origen y destino
- **Cantidad**: Kilogramos
- **Costo**: Resultado final

**Ejemplo de registro:**
```
[2026-03-21 14:35:42.123] Tipo: DIRECTO | Origen: SJO | Destino: MIA | Cantidad: 15.5kg | Costo: $38.75
```

### Operaciones de Registro

```csharp
// Obtener todos los registros
var registros = calculadora.ObtenerRegistros();

// Limpiar registros
calculadora.LimpiarRegistros();
```

## Notas Importantes

- Los códigos de zona **no son sensibles a mayúsculas/minúsculas**
- Se utiliza el tipo `decimal` para cálculos monetarios (máxima precisión)
- Cada ruta es **unidireccional**: SJO→MIA ≠ MIA→SJO
- Las pruebas utilizan **xUnit** como framework de testing
- Todas las pruebas son **backward-compatible** con la versión anterior

## Contacto y Contribuciones

Para reportar bugs o sugerir mejoras, contacta al equipo de desarrollo.
