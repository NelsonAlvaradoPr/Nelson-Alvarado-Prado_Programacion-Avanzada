# Calculadora de Tarifas de Envío

Un proyecto C# que implementa la función `CalcularTarifaEnvio` para calcular el costo de envíos entre zonas geográficas.

## Descripción

La función `CalcularTarifaEnvio` determina el costo de envío entre dos zonas geográficas basándose en:
- **Cantidad en kilogramos** (decimal): cantidad a enviar
- **Zona de origen** (string): código de la zona de inicio (ej: "SJO")
- **Zona de destino** (string): código de la zona de fin (ej: "MIA")
- **Diccionario de tarifas base** (Dictionary<string, decimal>): tarifas por kilogramo para cada ruta

## Fórmula de Cálculo

```
Costo Total = Cantidad (kg) × Tarifa Base ($/kg)
```

## Ejemplo de Uso

```csharp
var tarifas = new Dictionary<string, decimal>
{
    { "SJO-MIA", 2.50m },  // San Jose a Miami: $2.50 por kg
    { "SJO-NYC", 3.00m }   // San Jose a Nueva York: $3.00 por kg
};

// Calcular envío de 15.5 kg de SJO a MIA
decimal costo = TarifaEnvioCalculadora.CalcularTarifaEnvio(15.5m, "SJO", "MIA", tarifas);
// Resultado: 15.5 * 2.50 = $38.75
```

## Características

- ✅ Validación de parámetros de entrada
- ✅ Normalización automática de códigos de zona (mayúsculas)
- ✅ Manejo de excepciones específicas
- ✅ Soporta múltiples rutas
- ✅ Cálculo preciso con tipo decimal

## Rutas Disponibles en el Ejemplo

| Origen | Destino | Tarifa ($/kg) |
|--------|---------|---------------|
| SJO    | MIA     | $2.50         |
| SJO    | NYC     | $3.00         |
| SJO    | LAX     | $1.50         |
| MIA    | SJO     | $2.50         |
| NYC    | SJO     | $3.00         |
| LAX    | SJO     | $1.50         |

## Compilación y Ejecución

### Requisitos
- .NET 8.0 SDK o superior

### Cómo ejecutar

```bash
# Navegar al directorio del proyecto
cd CalculadorTarifaEnvio

# Compilar y ejecutar
dotnet run

# O solo compilar
dotnet build
```

## Excepciones

La función lanzará las siguientes excepciones ante entradas inválidas:

- **ArgumentException**: Si cantidad ≤ 0, zona vacía, o tarifas vacías
- **KeyNotFoundException**: Si la ruta no existe en el diccionario de tarifas

## Estructura del Proyecto

```
CalculadorTarifaEnvio/
├── CalculadorTarifaEnvio.csproj   # Archivo de proyecto
├── TarifaEnvioCalculadora.cs       # Clase con la función principal
├── Program.cs                      # Programa de ejemplo
└── README.md                       # Este archivo
```

## Notas

- Los códigos de zona no son sensibles a mayúsculas/minúsculas
- Se usa el tipo `decimal` para cálculos monetarios (mayor precisión)
- Cada ruta es unidireccional (SJO→MIA es diferente de MIA→SJO)
