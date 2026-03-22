using System;
using System.Collections.Generic;
using CalculadorTarifaEnvio;

// Definir diccionario de tarifas base por ruta
var tarifas = new Dictionary<string, decimal>
{
    { "SJO-MIA", 2.50m },      // San Jose a Miami: $2.50 por kg
    { "SJO-NYC", 3.00m },      // San Jose a Nueva York: $3.00 por kg
    { "SJO-LAX", 1.50m },      // San Jose a Los Angeles: $1.50 por kg
    { "MIA-SJO", 2.50m },      // Miami a San Jose: $2.50 por kg
    { "MIA-LAX", 3.50m },      // Miami a Los Angeles: $3.50 por kg
    { "NYC-SJO", 3.00m },      // Nueva York a San Jose: $3.00 por kg
    { "LAX-SJO", 1.50m },      // Los Angeles a San Jose: $1.50 por kg
    { "LAX-MIA", 3.50m }       // Los Angeles a Miami: $3.50 por kg
};

var calculadora = new TarifaEnvioCalculadora();

Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("    CALCULADORA DE TARIFAS DE ENVÍO - DEMOSTRACIÓN COMPLETA");
Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

try
{
    // ========== EJEMPLO 1: Envío Directo con Pesos Variados ==========
    Console.WriteLine("1️⃣  ENVÍOS DIRECTOS CON PESOS VARIADOS");
    Console.WriteLine("─────────────────────────────────────────\n");

    var enviosDirect = new List<(decimal peso, string origen, string destino)>
    {
        (5.0m, "SJO", "MIA"),
        (15.5m, "SJO", "MIA"),
        (100.0m, "SJO", "NYC"),
        (25.75m, "LAX", "SJO")
    };

    foreach (var (peso, origen, destino) in enviosDirect)
    {
        decimal costo = calculadora.CalcularTarifaEnvio(peso, origen, destino, tarifas);
        Console.WriteLine($"  {origen} → {destino}: {peso} kg × ${tarifas[$"{origen}-{destino}"]:F2}/kg = ${costo:F2}");
    }

    Console.WriteLine();

    // ========== EJEMPLO 2: Envíos con Transbordo ==========
    Console.WriteLine("2️⃣  ENVÍOS CON TRANSBORDO (RUTAS INTERMEDIAS)");
    Console.WriteLine("─────────────────────────────────────────\n");

    // SJO → MIA → LAX
    decimal costoTransbordo1 = calculadora.CalcularTarifaConTransbordo(
        10.0m, "SJO", "MIA", "LAX", tarifas);
    Console.WriteLine($"  SJO → MIA → LAX (10 kg):");
    Console.WriteLine($"    Segmento 1 (SJO-MIA): 10 × $2.50 = $25.00");
    Console.WriteLine($"    Segmento 2 (MIA-LAX): 10 × $3.50 = $35.00");
    Console.WriteLine($"    Costo Total: ${costoTransbordo1:F2}\n");

    // SJO → NYC → SJO
    decimal costoTransbordo2 = calculadora.CalcularTarifaConTransbordo(
        25.5m, "SJO", "NYC", "SJO", tarifas);
    Console.WriteLine($"  SJO → NYC → SJO (25.5 kg):");
    Console.WriteLine($"    Segmento 1 (SJO-NYC): 25.5 × $3.00 = $76.50");
    Console.WriteLine($"    Segmento 2 (NYC-SJO): 25.5 × $3.00 = $76.50");
    Console.WriteLine($"    Costo Total: ${costoTransbordo2:F2}\n");

    // ========== EJEMPLO 3: Cálculo Inverso con Surcharge 10% ==========
    Console.WriteLine("3️⃣  CÁLCULO INVERSO CON SURCHARGE (10%)");
    Console.WriteLine("─────────────────────────────────────────\n");

    decimal costoInverso1 = calculadora.CalcularTarifaInversaConSurcharge(
        10.0m, "SJO", "MIA", tarifas);
    Console.WriteLine($"  SJO → MIA → SJO (10 kg) con 10% surcharge:");
    Console.WriteLine($"    Costo base (SJO-MIA): 10 × $2.50 = $25.00");
    Console.WriteLine($"    Surcharge (10%): $25.00 × 0.10 = $2.50");
    Console.WriteLine($"    Costo Total con Surcharge: ${costoInverso1:F2}\n");

    decimal costoInverso2 = calculadora.CalcularTarifaInversaConSurcharge(
        100.0m, "SJO", "NYC", tarifas);
    Console.WriteLine($"  SJO → NYC (100 kg) con 10% surcharge:");
    Console.WriteLine($"    Costo base (SJO-NYC): 100 × $3.00 = $300.00");
    Console.WriteLine($"    Surcharge (10%): $300.00 × 0.10 = $30.00");
    Console.WriteLine($"    Costo Total con Surcharge: ${costoInverso2:F2}\n");

    // ========== EJEMPLO 4: Cálculo Cumulativo con Múltiples Ciudades ==========
    Console.WriteLine("4️⃣  CÁLCULO CUMULATIVO CON MÚLTIPLES CIUDADES");
    Console.WriteLine("─────────────────────────────────────────\n");

    var ruta1 = new List<string> { "SJO", "MIA", "LAX" };
    decimal costoCumulativo1 = calculadora.CalcularTarifaCumulativa(10.0m, ruta1, tarifas);
    Console.WriteLine($"  Ruta: {string.Join(" → ", ruta1)} (10 kg)");
    Console.WriteLine($"    SJO-MIA: 10 × $2.50 = $25.00");
    Console.WriteLine($"    MIA-LAX: 10 × $3.50 = $35.00");
    Console.WriteLine($"    Costo Total: ${costoCumulativo1:F2}\n");

    var ruta2 = new List<string> { "SJO", "MIA", "LAX", "SJO" };
    decimal costoCumulativo2 = calculadora.CalcularTarifaCumulativa(5.0m, ruta2, tarifas);
    Console.WriteLine($"  Ruta: {string.Join(" → ", ruta2)} (5 kg)");
    Console.WriteLine($"    SJO-MIA: 5 × $2.50 = $12.50");
    Console.WriteLine($"    MIA-LAX: 5 × $3.50 = $17.50");
    Console.WriteLine($"    LAX-SJO: 5 × $1.50 = $7.50");
    Console.WriteLine($"    Costo Total: ${costoCumulativo2:F2}\n");

    // ========== EJEMPLO 5: Manejo de Excepciones ==========
    Console.WriteLine("5️⃣  MANEJO DE EXCEPCIONES");
    Console.WriteLine("─────────────────────────────────────────\n");

    try
    {
        calculadora.CalcularTarifaEnvio(10.0m, "SJO", "INVALID", tarifas);
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"  ❌ Ruta no válida: {ex.Message}\n");
    }

    try
    {
        calculadora.CalcularTarifaEnvio(-5.0m, "SJO", "MIA", tarifas);
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"  ❌ Peso inválido: {ex.Message}\n");
    }

    // ========== EJEMPLO 6: Registro de Cálculos (Auditoría) ==========
    Console.WriteLine("6️⃣  REGISTRO DE CÁLCULOS (AUDITORÍA)");
    Console.WriteLine("─────────────────────────────────────────\n");

    Console.WriteLine("  Últimos registros de auditoría:\n");
    var registros = calculadora.ObtenerRegistros();
    foreach (var registro in registros.TakeLast(5))
    {
        Console.WriteLine($"  {registro}");
    }

    Console.WriteLine($"\n  Total de operaciones registradas: {registros.Count}\n");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"❌ Error de argumentos: {ex.Message}");
}
catch (KeyNotFoundException ex)
{
    Console.WriteLine($"❌ Error de ruta: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error inesperado: {ex.Message}");
}

Console.WriteLine("═══════════════════════════════════════════════════════════════");
Console.WriteLine("             Fin de la demostración");
Console.WriteLine("═══════════════════════════════════════════════════════════════");
