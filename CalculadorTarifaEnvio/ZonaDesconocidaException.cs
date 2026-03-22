using System;

namespace CalculadorTarifaEnvio
{
    /// <summary>
    /// Excepción que se lanza cuando una zona no existe en la red de tarifas
    /// </summary>
    public class ZonaDesconocidaException : Exception
    {
        /// <summary>
        /// La zona que no fue encontrada
        /// </summary>
        public string ZonaInvalida { get; }

        /// <summary>
        /// Constructor de la excepción
        /// </summary>
        /// <param name="zonaInvalida">La zona que no fue encontrada</param>
        /// <param name="message">Mensaje de error detallado</param>
        public ZonaDesconocidaException(string zonaInvalida, string message = null!)
            : base(message ?? $"La zona '{zonaInvalida}' no existe en la red de distribución.")
        {
            ZonaInvalida = zonaInvalida;
        }
    }
}
