# Documentación de Pruebas Unitarias - CalculadorTarifaEnvio

## Descripción General

La suite de pruebas unitarias de `CalculadorTarifaEnvio` garantiza **estabilidad del sistema** después de cada cambio mediante pruebas exhaustivas y backward-compatible.

**Framework utilizado:** xUnit  
**Lenguaje:** C#  
**Target:** .NET 8.0

## Cómo Ejecutar las Pruebas

### Ejecución Básica

```bash
# Navegar al directorio de pruebas
cd CalculadorTarifaEnvio.Tests

# Ejecutar todas las pruebas
dotnet test

# Ejecutar con modo verbose
dotnet test -v detailed

# Ejecutar una prueba específica
dotnet test --filter "CalcularTarifaEnvio_WithSmallWeight_ShouldCalculateCorrectly"
```

### Ejecución Filtrada por Categoría

```bash
# Pruebas de envíos directos
dotnet test --filter "Category=Direct Shipments"

# Pruebas de manejo de excepciones
dotnet test --filter "Category=Exception Handling"

# Pruebas de transbordo
dotnet test --filter "Category=Transshipment"

# Pruebas de auditoría/logging
dotnet test --filter "Category=Logging"

# Pruebas de cálculo inverso
dotnet test --filter "Category=Reverse Calculation"

# Pruebas cumulativas
dotnet test --filter "Category=Cumulative Calculation"

# Pruebas de integración
dotnet test --filter "Category=Integration"
```

## Estructura de Pruebas

### 1. Envíos Directos (Direct Shipments) - **5 pruebas**

**Objetivo:** Validar que los cálculos directos son correctos con pesos variados.

```
✓ CalcularTarifaEnvio_WithSmallWeight_ShouldCalculateCorrectly
  - Peso pequeño: 5.0 kg SJO→MIA = $12.50

✓ CalcularTarifaEnvio_WithMediumWeight_ShouldCalculateCorrectly
  - Peso medio: 15.5 kg SJO→MIA = $38.75

✓ CalcularTarifaEnvio_WithLargeWeight_ShouldCalculateCorrectly
  - Peso grande: 100.0 kg SJO→NYC = $300.00

✓ CalcularTarifaEnvio_WithVariousDifferentWeights_ShouldCalculateCorrectly
  - Prueba parametrizada con: 1.0, 10.5, 50.0, 250.75 kg
  - 4 casos de prueba adicionales
```

### 2. Manejo de Excepciones (Exception Handling) - **7 pruebas**

**Objetivo:** Verificar que se lanzan excepciones apropiadas para inputs inválidos.

```
✓ CalcularTarifaEnvio_WithNonExistentRoute_ShouldThrowKeyNotFoundException
  - Ruta SJO→INVALID debe lanzar KeyNotFoundException

✓ CalcularTarifaEnvio_WithNullOriginZone_ShouldThrowArgumentException
  - Origen nulo debe lanzar ArgumentException

✓ CalcularTarifaEnvio_WithEmptyDestinationZone_ShouldThrowArgumentException
  - Destino vacío debe lanzar ArgumentException

✓ CalcularTarifaEnvio_WithNegativeWeight_ShouldThrowArgumentException
  - Peso negativo debe lanzar ArgumentException

✓ CalcularTarifaEnvio_WithZeroWeight_ShouldThrowArgumentException
  - Peso cero debe lanzar ArgumentException

✓ CalcularTarifaEnvio_WithEmptyTarifas_ShouldThrowArgumentException
  - Diccionario de tarifas vacío debe lanzar ArgumentException

✓ CalcularTarifaEnvio_WithNullTarifas_ShouldThrowArgumentException
  - Diccionario de tarifas nulo debe lanzar ArgumentException
```

### 3. Transbordo (Transshipment) - **4 pruebas**

**Objetivo:** Validar cálculos correctos con rutas intermedias.

```
✓ CalcularTarifaConTransbordo_WithValidRoute_ShouldSumSegments
  - SJO→MIA→LAX (10 kg): (10×2.50) + (10×3.50) = $60.00

✓ CalcularTarifaConTransbordo_WithDifferentWeights_ShouldCalculateCorrectly
  - SJO→NYC→SJO (25.5 kg): (25.5×3.00) + (25.5×3.00) = $153.00

✓ CalcularTarifaConTransbordo_WithInvalidIntermediateRoute_ShouldThrowException
  - Ruta intermedia inválida lanza KeyNotFoundException

✓ CalcularTarifaConTransbordo_WithInvalidFinalRoute_ShouldThrowException
  - Ruta final inválida lanza KeyNotFoundException
```

### 4. Auditoría/Logging (Logging) - **3 pruebas**

**Objetivo:** Verificar formato y contenido de registros con timestamp exacto.

```
✓ RegistroCalculos_ShouldHaveCorrectDateTimeFormat
  - Validar formato: [yyyy-MM-dd HH:mm:ss.fff]
  - Verificar timestamp está en rango correcto

✓ RegistroCalculos_ShouldContainAllRequiredInfo
  - Verificar: Tipo, Origen, Destino, Cantidad, Costo
  - Ejemplo: "[2026-03-21 14:35:42.123] Tipo: DIRECTO | ..."

✓ RegistroCalculos_MultipleOperations_ShouldMaintainChronologicalOrder
  - 3 operaciones consecutivas en orden cronológico
  - Timestamps ascendentes
```

### 5. Cálculo Inverso (Reverse Calculation) - **4 pruebas**

**Objetivo:** Validar aplicación correcta del 10% de surcharge.

```
✓ CalcularTarifaInversaConSurcharge_ShouldAdd10PercentFee
  - SJO→MIA (10 kg): (10×2.50)×1.10 = $27.50

✓ CalcularTarifaInversaConSurcharge_WithVariousWeights_ShouldCalculateCorrectly
  - Pruebas parametrizadas:
    - 5.0 kg → $13.75
    - 20.0 kg → $66.00
    - 100.0 kg → $165.00

✓ CalcularTarifaInversaConSurcharge_WithDecimalResult_ShouldMaintainPrecision
  - 7.5 kg SJO→MIA: 18.75 × 1.10 = $20.625

✓ CalcularTarifaInversaConSurcharge_WithNonExistentRoute_ShouldThrowException
  - Ruta inválida lanza excepción
```

### 6. Cálculo Cumulativo (Cumulative Calculation) - **8 pruebas**

**Objetivo:** Validar cálculos precisos para rutas multi-ciudad.

```
✓ CalcularTarifaCumulativa_WithTwoCities_ShouldCalculateDirectRoute
  - SJO→MIA (10 kg): 10×2.50 = $25.00

✓ CalcularTarifaCumulativa_WithThreeCities_ShouldSumAllSegments
  - SJO→MIA→LAX (10 kg): 25.00 + 35.00 = $60.00

✓ CalcularTarifaCumulativa_WithFourCities_ShouldSumAllSegmentsCumulatively
  - SJO→MIA→LAX→SJO (5 kg): 12.50 + 17.50 + 7.50 = $37.50

✓ CalcularTarifaCumulativa_WithCaseSensitiveRoutes_ShouldNormalizeToUppercase
  - Acepta "sjo", "mia", "lax"
  - Resultado igual: $60.00

✓ CalcularTarifaCumulativa_WithSingleCity_ShouldThrowException
  - Ruta con una sola ciudad lanza ArgumentException

✓ CalcularTarifaCumulativa_WithEmptyRoute_ShouldThrowException
  - Ruta vacía lanza ArgumentException

✓ CalcularTarifaCumulativa_WithNullRoute_ShouldThrowException
  - Ruta nula lanza ArgumentException

✓ CalcularTarifaCumulativa_WithInvalidIntermediateRoute_ShouldThrowException
  - Ciudad intermedia inválida lanza KeyNotFoundException

✓ CalcularTarifaCumulativa_ShouldLogCorrectly
  - Registra 3 operaciones (2 DIRECTO + 1 CUMULATIVA)
```

### 7. Integración (Integration) - **3 pruebas**

**Objetivo:** Validar estabilidad general del sistema.

```
✓ TarifaEnvioCalculadora_ShouldMaintainBackwardCompatibility
  - Código antiguo sigue funcionando sin cambios

✓ LimpiarRegistros_ShouldClearAllLogs
  - Limpia correctamente todos los registros

✓ ObtenerRegistros_ShouldReturnReadOnlyList
  - Lista retornada es de solo lectura

✓ SystemStability_MultipleOperations_ShouldMaintainConsistency
  - 4 operaciones consecutivas: todos los cálculos consistentes
  - Todos los registros creados correctamente
```

## Resumen de Cobertura

| Categoría | Pruebas | Estado |
|-----------|---------|--------|
| Envíos Directos | 5 | ✅ Completada |
| Excepciones | 7 | ✅ Completada |
| Transbordo | 4 | ✅ Completada |
| Auditoría | 3 | ✅ Completada |
| Inversa + Surcharge | 4 | ✅ Completada |
| Cumulativa | 8 | ✅ Completada |
| Integración | 3 | ✅ Completada |
| **TOTAL** | **34+** | ✅ **Completada** |

*Nota: La prueba de envíos directos usa parametrización adicional.*

## Patrones de Prueba

### Patrón AAA (Arrange-Act-Assert)

Todas las pruebas siguen el patrón AAA:

```csharp
[Fact]
public void CalcularTarifaEnvio_WithMediumWeight_ShouldCalculateCorrectly()
{
    // ARRANGE: Preparar datos
    decimal peso = 15.5m;
    decimal expected = 38.75m;

    // ACT: Ejecutar la función
    decimal resultado = _calculadora.CalcularTarifaEnvio(
        peso, "SJO", "MIA", _tarifasBase);

    // ASSERT: Verificar resultados
    Assert.Equal(expected, resultado);
}
```

### Pruebas Parametrizadas

Utilizan `[Theory]` con `[InlineData]`:

```csharp
[Theory]
[InlineData(1.0, 2.50)]
[InlineData(10.5, 26.25)]
[InlineData(50.0, 125.00)]
public void CalcularTarifaEnvio_WithVariousWeights(
    decimal peso, decimal expected)
{
    var resultado = _calculadora.CalcularTarifaEnvio(
        peso, "SJO", "MIA", _tarifasBase);
    Assert.Equal(expected, resultado);
}
```

### Pruebas de Excepciones

```csharp
[Fact]
public void CalcularTarifaEnvio_WithNegativeWeight_ShouldThrowArgumentException()
{
    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() =>
        _calculadora.CalcularTarifaEnvio(-5.0m, "SJO", "MIA", _tarifasBase));
    
    Assert.Contains("debe ser mayor a 0", exception.Message);
}
```

## Garantías de Estabilidad

Las pruebas garantizan que:

1. ✅ **Cálculos correctos** con precisión decimal
2. ✅ **Validación robusta** de inputs
3. ✅ **Excepciones apropiadas** para casos de error
4. ✅ **Registro de auditoría completo** con timestamps precisos
5. ✅ **Compatibilidad hacia atrás** (backward compatible)
6. ✅ **Integridad de datos** en operaciones múltiples
7. ✅ **Consistencia de estado** después de cada operación

## Información de Tarifas de Prueba

```csharp
private readonly Dictionary<string, decimal> _tarifasBase = new()
{
    { "SJO-MIA", 2.50m },   // San Jose → Miami
    { "SJO-NYC", 3.00m },   // San Jose → Nueva York
    { "SJO-LAX", 1.50m },   // San Jose → Los Angeles
    { "MIA-SJO", 2.50m },   // Miami → San Jose
    { "MIA-LAX", 3.50m },   // Miami → Los Angeles
    { "NYC-SJO", 3.00m },   // Nueva York → San Jose
    { "LAX-SJO", 1.50m },   // Los Angeles → San Jose
    { "LAX-MIA", 3.50m }    // Los Angeles → Miami
};
```

## Resolución de Problemas

### Las pruebas fallan con "KeyNotFoundException"

**Causa:** Ruta no existe en el diccionario  
**Solución:** Verificar que las tarifas incluyan la ruta en formato "ORIGEN-DESTINO" (mayúsculas)

### Las pruebas fallan con timestamp incorrecto

**Causa:** Timezone del sistema diferente  
**Solución:** Las pruebas usan `DateTime.Now`, verificar que el sistema tenga hora correcta

### Algunas pruebas fallan aleatoriamente

**Causa:** Pruebas de integración múltiple sin espera suficiente entre timestamps  
**Solución:** Las pruebas incluyen `Thread.Sleep(10)` entre operaciones

## Mejores Prácticas

1. **Ejecutar pruebas antes de hacer commit**
   ```bash
   dotnet test
   ```

2. **Verificar cobertura de categorías específicas**
   ```bash
   dotnet test --filter "Category=Transshipment"
   ```

3. **Mantener las pruebas independientes** - cada test puede ejecutarse sin depender de otros

4. **Usar nombres descriptivos** - método_escenario_resultadoEsperado

5. **Limpiar estado** - el destructor IDisposable limpia registros después de cada test

## Referencias

- **Documentación de xUnit:** https://xunit.net/
- **C# Decimal:** https://docs.microsoft.com/en-us/dotnet/api/system.decimal
- **Assert methods:** https://xunit.net/docs/getting-started/testing
