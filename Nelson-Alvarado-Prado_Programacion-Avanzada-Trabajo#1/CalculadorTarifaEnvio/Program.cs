using System;
using System.Collections.Generic;
using CalculadorTarifaEnvio;

// Definir diccionario de tarifas base por ruta
var tarifas = new Dictionary<string, decimal>
{
    { "SJO-MIA", 2.50m },      // San Jose → Miami
    { "SJO-NYC", 3.00m },      // San Jose → Nueva York
    { "SJO-LAX", 1.50m },      // San Jose → Los Angeles
    { "MIA-SJO", 2.50m },      // Miami → San Jose
    { "MIA-LAX", 3.50m },      // Miami → Los Angeles
    { "NYC-SJO", 3.00m },      // Nueva York → San Jose
    { "LAX-SJO", 1.50m },      // Los Angeles → San Jose
    { "LAX-MIA", 3.50m },      // Los Angeles → Miami
    { "TGU-MIA", 4.00m },      // Tegucigalpa → Miami
    { "MIA-MAD", 5.50m }       // Miami → Madrid
};

var calculadora = new TarifaEnvioCalculadora();

Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║         CALCULADORA AVANZADA DE TARIFAS DE ENVÍO - RED LOGÍSTICA             ║");
Console.WriteLine("║  Con soporte para rutas directas, inversas y transferencias automáticas       ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝\n");

try
{
    // ========== EJEMPLO 1: Ruta Directa ==========
    Console.WriteLine("📦 EJEMPLO 1: RUTA DIRECTA");
    Console.WriteLine("─────────────────────────────────────────────────────────────────\n");
    
    try
    {
        decimal costo1 = calculadora.CalcularTarifaEnvioAvanzado(
            15.5m, "SJO", "MIA", tarifas, out string log1);
        
        Console.WriteLine($"✓ Costo Final: ${costo1:F2}\n");
        Console.WriteLine("📋 Detalles de la Operación:");
        Console.WriteLine($"   {log1}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}\n");
    }

    // ========== EJEMPLO 2: Ruta Inversa con Surcharge ==========
    Console.WriteLine("📦 EJEMPLO 2: RUTA INVERSA CON SURCHARGE (10%)");
    Console.WriteLine("─────────────────────────────────────────────────────────────────\n");
    
    try
    {
        // No existe SJO → NYC, pero existe NYC → SJO
        decimal costo2 = calculadora.CalcularTarifaEnvioAvanzado(
            10.0m, "NYC", "LAX", tarifas, out string log2);
        
        Console.WriteLine($"✓ Costo Final: ${costo2:F2}\n");
        Console.WriteLine("📋 Detalles de la Operación:");
        Console.WriteLine($"   {log2}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}\n");
    }

    // ========== EJEMPLO 3: Ruta con Transbordo (Transferencia) ==========
    Console.WriteLine("📦 EJEMPLO 3: RUTA CON TRANSFERENCIA (TRANSBORDO)");
    Console.WriteLine("─────────────────────────────────────────────────────────────────\n");
    
    try
    {
        // No existe TGU → MAD directamente, pero existe vía TGU → MIA → MAD
        decimal costo3 = calculadora.CalcularTarifaEnvioAvanzado(
            20.0m, "TGU", "MAD", tarifas, out string log3);
        
        Console.WriteLine($"✓ Costo Final: ${costo3:F2}\n");
        Console.WriteLine("📋 Detalles de la Operación:");
        Console.WriteLine($"   {log3}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}\n");
    }

    // ========== EJEMPLO 4: Zona Desconocida ==========
    Console.WriteLine("⚠️  EJEMPLO 4: MANEJO DE ZONA DESCONOCIDA");
    Console.WriteLine("─────────────────────────────────────────────────────────────────\n");
    
    try
    {
        decimal costo4 = calculadora.CalcularTarifaEnvioAvanzado(
            10.0m, "SJO", "INVALID", tarifas, out string log4);
    }
    catch (ZonaDesconocidaException ex)
    {
        Console.WriteLine($"✓ Excepción Capturada Correctamente:");
        Console.WriteLine($"   Tipo: ZonaDesconocidaException");
        Console.WriteLine($"   Zona Inválida: {ex.ZonaInvalida}");
        Console.WriteLine($"   Mensaje: {ex.Message}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error inesperado: {ex.Message}\n");
    }

    // ========== EJEMPLO 5: Sin Ruta Disponible ==========
    Console.WriteLine("⚠️  EJEMPLO 5: MANEJO CUANDO NO HAY RUTA DISPONIBLE");
    Console.WriteLine("─────────────────────────────────────────────────────────────────\n");
    
    try
    {
        // Agregar una zona aislada sin conexiones
        var tarifasIncompletas = new Dictionary<string, decimal>(tarifas)
        {
            { "ORD-DEN", 2.00m }  // Chicago a Denver (ruta única)
        };
        
        decimal costo5 = calculadora.CalcularTarifaEnvioAvanzado(
            5.0m, "ORD", "MIA", tarifasIncompletas, out string log5);
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"✓ Excepción Capturada Correctamente:");
        Console.WriteLine($"   Tipo: KeyNotFoundException");
        Console.WriteLine($"   Mensaje: {ex.Message}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error inesperado: {ex.Message}\n");
    }

    // ========== EJEMPLO 6: Redondeo a 2 Decimales ==========
    Console.WriteLine("📦 EJEMPLO 6: REDONDEO A 2 DECIMALES");
    Console.WriteLine("─────────────────────────────────────────────────────────────────\n");
    
    try
    {
        // Usar un peso que genera decimales extra
        decimal costo6 = calculadora.CalcularTarifaEnvioAvanzado(
            7.33m, "SJO", "LAX", tarifas, out string log6);
        
        Console.WriteLine($"✓ Costo Final (redondeado): ${costo6:F2}");
        Console.WriteLine($"   Cálculo: 7.33 kg × $1.50/kg = $10.995 → ${costo6:F2}\n");
        Console.WriteLine("📋 Detalles de la Operación:");
        Console.WriteLine($"   {log6}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}\n");
    }

    // ========== AUDITORÍA: Mostrar Registros ==========
    Console.WriteLine("📊 AUDITORÍA: REGISTRO DE OPERACIONES");
    Console.WriteLine("─────────────────────────────────────────────────────────────────\n");
    
    var registros = calculadora.ObtenerRegistros();
    if (registros.Count > 0)
    {
        Console.WriteLine($"Total de operaciones registradas: {registros.Count}\n");
        foreach (var registro in registros.TakeLast(10))  // Mostrar últimas 10
        {
            Console.WriteLine($"   {registro}");
        }
    }
    else
    {
        Console.WriteLine("   No hay registros de operaciones.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error global: {ex.Message}");
}

Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                          Fin de la demostración                             ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝");

