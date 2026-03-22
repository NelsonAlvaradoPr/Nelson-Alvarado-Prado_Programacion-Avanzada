using System;
using System.Collections.Generic;

namespace CalculadorTarifaEnvio
{
    /// <summary>
    /// Clase encargada de calcular tarifas de envío entre zonas geográficas
    /// </summary>
    public class TarifaEnvioCalculadora
    {
        /// <summary>
        /// Calcula el costo de envío entre dos zonas geográficas
        /// </summary>
        /// <param name="cantidadKilogramos">Cantidad de kilogramos a enviar</param>
        /// <param name="zonaOrigen">Código de la zona de origen (ej: "SJO")</param>
        /// <param name="zonaDestino">Código de la zona de destino (ej: "MIA")</param>
        /// <param name="tarifasBase">Diccionario con tarifas base por ruta. 
        /// Las claves deben estar en formato "ORIGEN-DESTINO" (ej: "SJO-MIA") 
        /// y los valores son la tarifa base por kilogramo en unidad monetaria</param>
        /// <returns>El costo total del envío</returns>
        /// <exception cref="ArgumentException">Se lanza si faltan parámetros requeridos</exception>
        /// <exception cref="KeyNotFoundException">Se lanza si la ruta no existe en las tarifas</exception>
        public static decimal CalcularTarifaEnvio(
            decimal cantidadKilogramos,
            string zonaOrigen,
            string zonaDestino,
            Dictionary<string, decimal> tarifasBase)
        {
            // Validar parámetros
            if (cantidadKilogramos <= 0)
                throw new ArgumentException("La cantidad de kilogramos debe ser mayor a 0", nameof(cantidadKilogramos));

            if (string.IsNullOrWhiteSpace(zonaOrigen))
                throw new ArgumentException("La zona de origen no puede estar vacía", nameof(zonaOrigen));

            if (string.IsNullOrWhiteSpace(zonaDestino))
                throw new ArgumentException("La zona de destino no puede estar vacía", nameof(zonaDestino));

            if (tarifasBase == null || tarifasBase.Count == 0)
                throw new ArgumentException("Las tarifas base no pueden estar vacías", nameof(tarifasBase));

            // Normalizar códigos de zona (convertir a mayúsculas)
            string rutaKey = $"{zonaOrigen.ToUpper()}-{zonaDestino.ToUpper()}";

            // Buscar la tarifa base para la ruta especificada
            if (!tarifasBase.TryGetValue(rutaKey, out decimal tarifaBase))
            {
                throw new KeyNotFoundException(
                    $"No se encontró tarifa para la ruta '{rutaKey}'. Rutas disponibles: {string.Join(", ", tarifasBase.Keys)}");
            }

            // Calcular el costo total: cantidad * tarifa base
            decimal costoTotal = cantidadKilogramos * tarifaBase;

            return costoTotal;
        }
    }
}
