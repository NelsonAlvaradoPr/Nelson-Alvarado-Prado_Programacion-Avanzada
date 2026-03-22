using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculadorTarifaEnvio
{
    /// <summary>
    /// Clase encargada de calcular tarifas de envío entre zonas geográficas
    /// </summary>
    public class TarifaEnvioCalculadora
    {
        /// <summary>
        /// Registro de cálculos realizados para auditoría
        /// </summary>
        public List<string> RegistroCalculos { get; } = new List<string>();

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
        public decimal CalcularTarifaEnvio(
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

            // Registrar en el log
            RegistrarCalculo("DIRECTO", zonaOrigen, zonaDestino, cantidadKilogramos, costoTotal);

            return costoTotal;
        }

        /// <summary>
        /// Calcula el costo de envío con transbordo (ruta intermedia)
        /// </summary>
        /// <param name="cantidadKilogramos">Cantidad de kilogramos a enviar</param>
        /// <param name="zonaOrigen">Código de la zona de origen</param>
        /// <param name="zonaIntermedia">Código de la zona intermedia</param>
        /// <param name="zonaDestino">Código de la zona de destino</param>
        /// <param name="tarifasBase">Diccionario con tarifas base por ruta</param>
        /// <returns>El costo total del envío con transbordo (suma de segmentos)</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si alguna ruta no existe en las tarifas</exception>
        public decimal CalcularTarifaConTransbordo(
            decimal cantidadKilogramos,
            string zonaOrigen,
            string zonaIntermedia,
            string zonaDestino,
            Dictionary<string, decimal> tarifasBase)
        {
            // Validar parámetros básicos
            if (cantidadKilogramos <= 0)
                throw new ArgumentException("La cantidad de kilogramos debe ser mayor a 0", nameof(cantidadKilogramos));

            if (tarifasBase == null || tarifasBase.Count == 0)
                throw new ArgumentException("Las tarifas base no pueden estar vacías", nameof(tarifasBase));

            // Calcular primer segmento: origen → intermedia
            decimal costo1 = CalcularTarifaEnvio(cantidadKilogramos, zonaOrigen, zonaIntermedia, tarifasBase);

            // Calcular segundo segmento: intermedia → destino
            decimal costo2 = CalcularTarifaEnvio(cantidadKilogramos, zonaIntermedia, zonaDestino, tarifasBase);

            // Costo total es la suma de ambos segmentos
            decimal costoTotal = costo1 + costo2;

            // Registrar en el log
            RegistrarCalculo("TRANSBORDO", $"{zonaOrigen}-{zonaIntermedia}-{zonaDestino}", "", cantidadKilogramos, costoTotal);

            return costoTotal;
        }

        /// <summary>
        /// Calcula el costo inverso de una ruta con surcharge del 10%
        /// Por ejemplo, si SJO->MIA cuesta $100, MIA->SJO con surcharge sería $110
        /// </summary>
        /// <param name="cantidadKilogramos">Cantidad de kilogramos a enviar</param>
        /// <param name="zonaOrigen">Código de la zona de origen</param>
        /// <param name="zonaDestino">Código de la zona de destino</param>
        /// <param name="tarifasBase">Diccionario con tarifas base por ruta</param>
        /// <returns>El costo total con surcharge del 10%</returns>
        public decimal CalcularTarifaInversaConSurcharge(
            decimal cantidadKilogramos,
            string zonaOrigen,
            string zonaDestino,
            Dictionary<string, decimal> tarifasBase)
        {
            // Calcular costo normal
            decimal costoBase = CalcularTarifaEnvio(cantidadKilogramos, zonaOrigen, zonaDestino, tarifasBase);

            // Aplicar surcharge del 10%
            decimal surcharge = costoBase * 0.10m;
            decimal costoConSurcharge = costoBase + surcharge;

            // Registrar en el log
            RegistrarCalculo("INVERSA_SURCHARGE", zonaDestino, zonaOrigen, cantidadKilogramos, costoConSurcharge);

            return costoConSurcharge;
        }

        /// <summary>
        /// Calcula el costo cumulativo con múltiples transbordo
        /// </summary>
        /// <param name="cantidadKilogramos">Cantidad de kilogramos a enviar</param>
        /// <param name="rutaCompleta">Lista de códigos de zonas en orden (origen -> intermedia1 -> intermedia2 -> ... -> destino)</param>
        /// <param name="tarifasBase">Diccionario con tarifas base por ruta</param>
        /// <returns>El costo total acumulado</returns>
        public decimal CalcularTarifaCumulativa(
            decimal cantidadKilogramos,
            List<string> rutaCompleta,
            Dictionary<string, decimal> tarifasBase)
        {
            if (rutaCompleta == null || rutaCompleta.Count < 2)
                throw new ArgumentException("La ruta debe tener al menos 2 ciudades (origen y destino)", nameof(rutaCompleta));

            if (cantidadKilogramos <= 0)
                throw new ArgumentException("La cantidad de kilogramos debe ser mayor a 0", nameof(cantidadKilogramos));

            decimal costoTotal = 0m;
            var segmentosConsumidos = new List<string>();

            // Calcular costo para cada segmento
            for (int i = 0; i < rutaCompleta.Count - 1; i++)
            {
                string origen = rutaCompleta[i];
                string destino = rutaCompleta[i + 1];

                decimal costoSegmento = CalcularTarifaEnvio(cantidadKilogramos, origen, destino, tarifasBase);
                costoTotal += costoSegmento;
                segmentosConsumidos.Add($"{origen}-{destino}");
            }

            // Registrar en el log
            RegistrarCalculo("CUMULATIVA", string.Join(" -> ", rutaCompleta), "", cantidadKilogramos, costoTotal);

            return costoTotal;
        }

        /// <summary>
        /// Registra un cálculo en el log con formato de fecha y hora
        /// </summary>
        private void RegistrarCalculo(string tipo, string origen, string destino, decimal cantidad, decimal costo)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string registro = $"[{timestamp}] Tipo: {tipo} | Origen: {origen} | Destino: {destino} | Cantidad: {cantidad}kg | Costo: ${costo:F2}";
            RegistroCalculos.Add(registro);
        }

        /// <summary>
        /// Obtiene todos los registros de cálculos realizados
        /// </summary>
        public IReadOnlyList<string> ObtenerRegistros() => RegistroCalculos.AsReadOnly();

        /// <summary>
        /// Limpia el registro de cálculos
        /// </summary>
        public void LimpiarRegistros() => RegistroCalculos.Clear();
    }
}
