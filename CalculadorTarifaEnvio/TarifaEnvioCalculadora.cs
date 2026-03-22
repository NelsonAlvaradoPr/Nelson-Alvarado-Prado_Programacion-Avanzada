using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculadorTarifaEnvio
{
    /// <summary>
    /// Clase encargada de calcular tarifas de envío entre zonas geográficas
    /// con soporte para rutas inteligentes y direcciones alternativas
    /// </summary>
    public class TarifaEnvioCalculadora
    {
        /// <summary>
        /// Registro de cálculos realizados para auditoría
        /// </summary>
        public List<string> RegistroCalculos { get; } = new List<string>();

        #region Método Principal - CalcularTarifaEnvioAvanzado

        /// <summary>
        /// Calcula el costo de envío con routeo inteligente (directo, inverso o transbordo)
        /// </summary>
        /// <param name="cantidadKilogramos">Cantidad de kilogramos a enviar</param>
        /// <param name="zonaOrigen">Código de la zona de origen</param>
        /// <param name="zonaDestino">Código de la zona de destino</param>
        /// <param name="tarifasBase">Diccionario con tarifas base por ruta</param>
        /// <param name="operationLog">Salida: Registro detallado de la operación</param>
        /// <returns>El costo total del envío redondeado a 2 decimales</returns>
        /// <exception cref="ArgumentException">Si los parámetros de entrada no son válidos</exception>
        /// <exception cref="ZonaDesconocidaException">Si las zonas no existen en la red de tarifas</exception>
        public decimal CalcularTarifaEnvioAvanzado(
            decimal cantidadKilogramos,
            string zonaOrigen,
            string zonaDestino,
            Dictionary<string, decimal> tarifasBase,
            out string operationLog)
        {
            operationLog = "";

            // Validar parámetros básicos
            if (cantidadKilogramos <= 0)
                throw new ArgumentException("La cantidad de kilogramos debe ser mayor a 0");

            if (string.IsNullOrWhiteSpace(zonaOrigen) || string.IsNullOrWhiteSpace(zonaDestino))
                throw new ArgumentException("Las zonas no pueden estar vacías");

            if (tarifasBase == null || tarifasBase.Count == 0)
                throw new ArgumentException("Las tarifas base no pueden estar vacías");

            // Normalizar códigos
            string origen = zonaOrigen.ToUpper();
            string destino = zonaDestino.ToUpper();

            // Validar que las zonas existan en la red
            ValidarZonasExisten(origen, destino, tarifasBase);

            decimal costoFinal = 0m;
            string tipoRuta = "";

            // Intentar 3 estrategias de enrutamiento en orden de preferencia
            // 1. Ruta directa
            if (BuscarRutaDirecta(origen, destino, cantidadKilogramos, tarifasBase, out costoFinal, out string logDirecto))
            {
                tipoRuta = "DIRECTA";
                operationLog = logDirecto;
            }
            // 2. Ruta inversa con surcharge
            else if (BuscarRutaInversa(origen, destino, cantidadKilogramos, tarifasBase, out costoFinal, out string logInversa))
            {
                tipoRuta = "INVERSA_CON_SURCHARGE";
                operationLog = logInversa;
            }
            // 3. Ruta con transbordo (transferencia)
            else if (BuscarRutaTransbordo(origen, destino, cantidadKilogramos, tarifasBase, out costoFinal, out string logTransbordo))
            {
                tipoRuta = "TRANSBORDO";
                operationLog = logTransbordo;
            }
            else
            {
                // No se encontró ninguna ruta viable
                operationLog = GenerarMensajeErrorSinRuta(origen, destino, cantidadKilogramos);
                throw new KeyNotFoundException($"No se encontró ruta viable (directa, inversa o transbordo) para {origen} → {destino}");
            }

            // Redondear a 2 decimales
            costoFinal = Math.Round(costoFinal, 2);

            // Registrar en auditoría
            RegistrarCalculo(tipoRuta, origen, destino, cantidadKilogramos, costoFinal);

            return costoFinal;
        }

        #endregion

        #region Métodos Privados - Estrategias de Búsqueda de Rutas

        /// <summary>
        /// Valida que las zonas de origen y destino existan en la red de tarifas
        /// </summary>
        private void ValidarZonasExisten(string origen, string destino, Dictionary<string, decimal> tarifasBase)
        {
            var zonasEnRed = new HashSet<string>();
            foreach (var ruta in tarifasBase.Keys)
            {
                var partes = ruta.Split('-');
                if (partes.Length == 2)
                {
                    zonasEnRed.Add(partes[0]);
                    zonasEnRed.Add(partes[1]);
                }
            }

            if (!zonasEnRed.Contains(origen))
                throw new ZonaDesconocidaException(origen, $"La zona de origen '{origen}' no existe en la red de distribución.");

            if (!zonasEnRed.Contains(destino))
                throw new ZonaDesconocidaException(destino, $"La zona de destino '{destino}' no existe en la red de distribución.");
        }

        /// <summary>
        /// Busca y calcula una ruta directa
        /// </summary>
        private bool BuscarRutaDirecta(
            string origen, string destino, decimal cantidad,
            Dictionary<string, decimal> tarifasBase,
            out decimal costo, out string log)
        {
            costo = 0m;
            log = "";

            string rutaKey = $"{origen}-{destino}";
            if (!tarifasBase.TryGetValue(rutaKey, out decimal tarifa))
                return false;

            costo = cantidad * tarifa;
            log = GenerarMensajeRutaDirecta(origen, destino, cantidad, tarifa, costo);
            return true;
        }

        /// <summary>
        /// Busca una ruta inversa y aplica surcharge del 10%
        /// </summary>
        private bool BuscarRutaInversa(
            string origen, string destino, decimal cantidad,
            Dictionary<string, decimal> tarifasBase,
            out decimal costo, out string log)
        {
            costo = 0m;
            log = "";

            string rutaInversa = $"{destino}-{origen}";
            if (!tarifasBase.TryGetValue(rutaInversa, out decimal tarifaInversa))
                return false;

            decimal costoBase = cantidad * tarifaInversa;
            decimal surcharge = costoBase * 0.10m;
            costo = costoBase + surcharge;

            log = GenerarMensajeRutaInversa(origen, destino, cantidad, tarifaInversa, costoBase, surcharge, costo);
            return true;
        }

        /// <summary>
        /// Busca una ruta con transbordo (transferencia en ciudad intermedia)
        /// </summary>
        private bool BuscarRutaTransbordo(
            string origen, string destino, decimal cantidad,
            Dictionary<string, decimal> tarifasBase,
            out decimal costo, out string log)
        {
            costo = 0m;
            log = "";

            // Buscar ciudades intermedias que conecten origen → destino
            var ciudadIntermedia = BuscarCiudadIntermedia(origen, destino, tarifasBase);
            if (ciudadIntermedia == null)
                return false;

            string rutaOrigen = $"{origen}-{ciudadIntermedia}";
            string rutaDestino = $"{ciudadIntermedia}-{destino}";

            if (!tarifasBase.TryGetValue(rutaOrigen, out decimal tarifaOrigen))
                return false;
            if (!tarifasBase.TryGetValue(rutaDestino, out decimal tarifaDestino))
                return false;

            decimal costo1 = cantidad * tarifaOrigen;
            decimal costo2 = cantidad * tarifaDestino;
            costo = costo1 + costo2;

            log = GenerarMensajeRutaTransbordo(origen, ciudadIntermedia, destino, 
                cantidad, tarifaOrigen, tarifaDestino, costo1, costo2, costo);
            return true;
        }

        /// <summary>
        /// Busca una ciudad intermedia que conecte origen con destino
        /// Utiliza BFS (Breadth-First Search) para encontrar el primer camino viable
        /// </summary>
        private string? BuscarCiudadIntermedia(string origen, string destino, Dictionary<string, decimal> tarifasBase)
        {
            // Construir grafo de zonas disponibles
            var grafo = ConstruirGrafo(tarifasBase);

            // BFS desde origen para encontrar caminos de longitud 2 al destino
            var cola = new Queue<(string zona, int distancia)>();
            var visitadas = new HashSet<string> { origen };
            cola.Enqueue((origen, 0));

            while (cola.Count > 0)
            {
                var (zonaActual, distancia) = cola.Dequeue();

                // Si encontramos una ruta de 2 saltos al destino, retornar la ciudad intermedia
                if (distancia == 1 && grafo.ContainsKey(zonaActual) && grafo[zonaActual].Contains(destino))
                    return zonaActual;

                // Si no hemos llegado a distancia máxima, continuar expandiendo
                if (distancia < 1 && grafo.ContainsKey(zonaActual))
                {
                    foreach (var siguienteZona in grafo[zonaActual])
                    {
                        if (!visitadas.Contains(siguienteZona))
                        {
                            visitadas.Add(siguienteZona);
                            cola.Enqueue((siguienteZona, distancia + 1));
                        }
                    }
                }
            }

            return null; // No se encontró ciudad intermedia viable
        }

        /// <summary>
        /// Construye un grafo de zonas basado en las rutas disponibles
        /// </summary>
        private Dictionary<string, HashSet<string>> ConstruirGrafo(Dictionary<string, decimal> tarifasBase)
        {
            var grafo = new Dictionary<string, HashSet<string>>();

            foreach (var ruta in tarifasBase.Keys)
            {
                var partes = ruta.Split('-');
                if (partes.Length == 2)
                {
                    string origen = partes[0];
                    string destino = partes[1];

                    if (!grafo.ContainsKey(origen))
                        grafo[origen] = new HashSet<string>();

                    grafo[origen].Add(destino);
                }
            }

            return grafo;
        }

        #endregion

        #region Métodos Privados - Generación de Mensajes

        /// <summary>
        /// Genera el mensaje de log para una ruta directa
        /// </summary>
        private string GenerarMensajeRutaDirecta(string origen, string destino, decimal cantidad, decimal tarifa, decimal costo)
        {
            var fecha = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            return $"On {fecha}, a shipment of {cantidad} kg was processed from {origen} to {destino}. Total cost calculated: {costo:F2}.";
        }

        /// <summary>
        /// Genera el mensaje de log para una ruta inversa con surcharge
        /// </summary>
        private string GenerarMensajeRutaInversa(string origen, string destino, decimal cantidad, 
            decimal tarifa, decimal costoBase, decimal surcharge, decimal costoTotal)
        {
            var fecha = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var sb = new StringBuilder();
            sb.AppendLine($"On {fecha}, a shipment of {cantidad} kg was processed from {origen} to {destino}. Total cost calculated: {costoTotal:F2}.");
            sb.Append($"Note: A direct route was NOT found. However, a reverse route was found with a surcharge of 10%, ");
            sb.Append($"so the calculated cost is {costoBase:F2} + {surcharge:F2} (10%) = {costoTotal:F2}.");
            return sb.ToString();
        }

        /// <summary>
        /// Genera el mensaje de log para una ruta con transbordo
        /// </summary>
        private string GenerarMensajeRutaTransbordo(string origen, string intermedia, string destino, 
            decimal cantidad, decimal tarifa1, decimal tarifa2, decimal costo1, decimal costo2, decimal costoTotal)
        {
            var fecha = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            var sb = new StringBuilder();
            sb.AppendLine($"On {fecha}, a shipment of {cantidad} kg was processed from {origen} to {destino}. Total cost calculated: {costoTotal:F2}.");
            sb.Append($"Note: A direct route was NOT found. However, a route with a transfer was found going from {origen} to {intermedia} and then to {destino}, ");
            sb.Append($"so the calculated cost is {costo1:F2} + {costo2:F2} = {costoTotal:F2}.");
            return sb.ToString();
        }

        /// <summary>
        /// Genera el mensaje de error cuando no se encuentra ninguna ruta
        /// </summary>
        private string GenerarMensajeErrorSinRuta(string origen, string destino, decimal cantidad)
        {
            var fecha = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            return $"On {fecha}, a shipment of {cantidad} kg was NOT processed from {origen} to {destino}. " +
                   "A direct route was not found, and no alternative routes (reverse or transfer) are available.";
        }

        #endregion

        #region Métodos Antiguos - Mantener Compatibilidad

        /// <summary>
        /// Calcula el costo de envío entre dos zonas geográficas (método original)
        /// </summary>
        public decimal CalcularTarifaEnvio(
            decimal cantidadKilogramos,
            string zonaOrigen,
            string zonaDestino,
            Dictionary<string, decimal> tarifasBase)
        {
            if (cantidadKilogramos <= 0)
                throw new ArgumentException("La cantidad de kilogramos debe ser mayor a 0", nameof(cantidadKilogramos));

            if (string.IsNullOrWhiteSpace(zonaOrigen))
                throw new ArgumentException("La zona de origen no puede estar vacía", nameof(zonaOrigen));

            if (string.IsNullOrWhiteSpace(zonaDestino))
                throw new ArgumentException("La zona de destino no puede estar vacía", nameof(zonaDestino));

            if (tarifasBase == null || tarifasBase.Count == 0)
                throw new ArgumentException("Las tarifas base no pueden estar vacías", nameof(tarifasBase));

            string rutaKey = $"{zonaOrigen.ToUpper()}-{zonaDestino.ToUpper()}";

            if (!tarifasBase.TryGetValue(rutaKey, out decimal tarifaBase))
            {
                throw new KeyNotFoundException(
                    $"No se encontró tarifa para la ruta '{rutaKey}'. Rutas disponibles: {string.Join(", ", tarifasBase.Keys)}");
            }

            decimal costoTotal = cantidadKilogramos * tarifaBase;
            RegistrarCalculo("DIRECTO", zonaOrigen, zonaDestino, cantidadKilogramos, costoTotal);
            return costoTotal;
        }

        /// <summary>
        /// Calcula el costo de envío con transbordo (ruta intermedia)
        /// </summary>
        public decimal CalcularTarifaConTransbordo(
            decimal cantidadKilogramos,
            string zonaOrigen,
            string zonaIntermedia,
            string zonaDestino,
            Dictionary<string, decimal> tarifasBase)
        {
            if (cantidadKilogramos <= 0)
                throw new ArgumentException("La cantidad de kilogramos debe ser mayor a 0", nameof(cantidadKilogramos));

            if (tarifasBase == null || tarifasBase.Count == 0)
                throw new ArgumentException("Las tarifas base no pueden estar vacías", nameof(tarifasBase));

            decimal costo1 = CalcularTarifaEnvio(cantidadKilogramos, zonaOrigen, zonaIntermedia, tarifasBase);
            decimal costo2 = CalcularTarifaEnvio(cantidadKilogramos, zonaIntermedia, zonaDestino, tarifasBase);
            decimal costoTotal = costo1 + costo2;

            RegistrarCalculo("TRANSBORDO", $"{zonaOrigen}-{zonaIntermedia}-{zonaDestino}", "", cantidadKilogramos, costoTotal);
            return costoTotal;
        }

        /// <summary>
        /// Calcula el costo inverso con surcharge del 10%
        /// </summary>
        public decimal CalcularTarifaInversaConSurcharge(
            decimal cantidadKilogramos,
            string zonaOrigen,
            string zonaDestino,
            Dictionary<string, decimal> tarifasBase)
        {
            decimal costoBase = CalcularTarifaEnvio(cantidadKilogramos, zonaOrigen, zonaDestino, tarifasBase);
            decimal surcharge = costoBase * 0.10m;
            decimal costoConSurcharge = costoBase + surcharge;

            RegistrarCalculo("INVERSA_SURCHARGE", zonaDestino, zonaOrigen, cantidadKilogramos, costoConSurcharge);
            return costoConSurcharge;
        }

        /// <summary>
        /// Calcula el costo cumulativo con múltiples transbordo
        /// </summary>
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

            for (int i = 0; i < rutaCompleta.Count - 1; i++)
            {
                string origen = rutaCompleta[i];
                string destino = rutaCompleta[i + 1];
                decimal costoSegmento = CalcularTarifaEnvio(cantidadKilogramos, origen, destino, tarifasBase);
                costoTotal += costoSegmento;
            }

            RegistrarCalculo("CUMULATIVA", string.Join(" -> ", rutaCompleta), "", cantidadKilogramos, costoTotal);
            return costoTotal;
        }

        #endregion

        #region Métodos Privados - Logging

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

        #endregion
    }
}
