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
    { "NYC-SJO", 3.00m },      // Nueva York a San Jose: $3.00 por kg
    { "LAX-SJO", 1.50m }       // Los Angeles a San Jose: $1.50 por kg
};

Console.WriteLine("=== Calculadora de Tarifas de Envío ===\n");

try
{
    // Ejemplo 1: Envío de 15.5 kg de SJO a MIA
    decimal cantidad1 = 15.5m;
    string origen1 = "SJO";
    string destino1 = "MIA";
    
    decimal costo1 = TarifaEnvioCalculadora.CalcularTarifaEnvio(cantidad1, origen1, destino1, tarifas);
    Console.WriteLine($"Ejemplo 1:");
    Console.WriteLine($"  Origen: {origen1} → Destino: {destino1}");
    Console.WriteLine($"  Cantidad: {cantidad1} kg");
    Console.WriteLine($"  Tarifa base: ${tarifas["SJO-MIA"]} por kg");
    Console.WriteLine($"  Costo total: ${costo1:F2}\n");

    // Ejemplo 2: Envío de 25 kg de SJO a NYC
    decimal cantidad2 = 25m;
    string origen2 = "SJO";
    string destino2 = "NYC";
    
    decimal costo2 = TarifaEnvioCalculadora.CalcularTarifaEnvio(cantidad2, origen2, destino2, tarifas);
    Console.WriteLine($"Ejemplo 2:");
    Console.WriteLine($"  Origen: {origen2} → Destino: {destino2}");
    Console.WriteLine($"  Cantidad: {cantidad2} kg");
    Console.WriteLine($"  Tarifa base: ${tarifas["SJO-NYC"]} por kg");
    Console.WriteLine($"  Costo total: ${costo2:F2}\n");

    // Ejemplo 3: Envío de 10 kg de SJO a LAX
    decimal cantidad3 = 10m;
    string origen3 = "SJO";
    string destino3 = "LAX";
    
    decimal costo3 = TarifaEnvioCalculadora.CalcularTarifaEnvio(cantidad3, origen3, destino3, tarifas);
    Console.WriteLine($"Ejemplo 3:");
    Console.WriteLine($"  Origen: {origen3} → Destino: {destino3}");
    Console.WriteLine($"  Cantidad: {cantidad3} kg");
    Console.WriteLine($"  Tarifa base: ${tarifas["SJO-LAX"]} por kg");
    Console.WriteLine($"  Costo total: ${costo3:F2}\n");

    // Ejemplo 4: Envío de 5.25 kg de MIA a SJO
    decimal cantidad4 = 5.25m;
    string origen4 = "MIA";
    string destino4 = "SJO";
    
    decimal costo4 = TarifaEnvioCalculadora.CalcularTarifaEnvio(cantidad4, origen4, destino4, tarifas);
    Console.WriteLine($"Ejemplo 4:");
    Console.WriteLine($"  Origen: {origen4} → Destino: {destino4}");
    Console.WriteLine($"  Cantidad: {cantidad4} kg");
    Console.WriteLine($"  Tarifa base: ${tarifas["MIA-SJO"]} por kg");
    Console.WriteLine($"  Costo total: ${costo4:F2}\n");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error de argumentos: {ex.Message}");
}
catch (KeyNotFoundException ex)
{
    Console.WriteLine($"Error de ruta: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error inesperado: {ex.Message}");
}

Console.WriteLine("\n=== Fin del programa ===");
