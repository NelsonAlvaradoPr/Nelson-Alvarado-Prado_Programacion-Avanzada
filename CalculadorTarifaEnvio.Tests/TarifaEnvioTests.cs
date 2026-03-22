using System;
using System.Collections.Generic;
using System.Linq;
using CalculadorTarifaEnvio;
using Xunit;

namespace CalculadorTarifaEnvio.Tests
{
    /// <summary>
    /// Suite de pruebas unitarias para TarifaEnvioCalculadora
    /// Pruebas backward-compatible que garantizan estabilidad del sistema
    /// </summary>
    public class TarifaEnvioTests : IDisposable
    {
        private readonly TarifaEnvioCalculadora _calculadora;
        private readonly Dictionary<string, decimal> _tarifasBase;

        public TarifaEnvioTests()
        {
            _calculadora = new TarifaEnvioCalculadora();
            
            // Definir tarifas base para pruebas
            _tarifasBase = new Dictionary<string, decimal>
            {
                { "SJO-MIA", 2.50m },
                { "SJO-NYC", 3.00m },
                { "SJO-LAX", 1.50m },
                { "MIA-SJO", 2.50m },
                { "MIA-LAX", 3.50m },
                { "NYC-SJO", 3.00m },
                { "LAX-SJO", 1.50m },
                { "LAX-MIA", 3.50m }
            };
        }

        public void Dispose()
        {
            _calculadora.LimpiarRegistros();
        }

        #region Test Direct Shipments with Varying Weights

        [Fact]
        [Trait("Category", "Direct Shipments")]
        public void CalcularTarifaEnvio_WithSmallWeight_ShouldCalculateCorrectly()
        {
            // Arrange
            decimal peso = 5.0m;
            decimal expected = 12.50m; // 5.0 * 2.50

            // Act
            decimal resultado = _calculadora.CalcularTarifaEnvio(peso, "SJO", "MIA", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
            Assert.Single(_calculadora.ObtenerRegistros());
        }

        [Fact]
        [Trait("Category", "Direct Shipments")]
        public void CalcularTarifaEnvio_WithMediumWeight_ShouldCalculateCorrectly()
        {
            // Arrange
            decimal peso = 15.5m;
            decimal expected = 38.75m; // 15.5 * 2.50

            // Act
            decimal resultado = _calculadora.CalcularTarifaEnvio(peso, "SJO", "MIA", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Direct Shipments")]
        public void CalcularTarifaEnvio_WithLargeWeight_ShouldCalculateCorrectly()
        {
            // Arrange
            decimal peso = 100.0m;
            decimal expected = 300.00m; // 100.0 * 3.00

            // Act
            decimal resultado = _calculadora.CalcularTarifaEnvio(peso, "SJO", "NYC", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Theory]
        [InlineData(1.0, 2.50)]      // Envío mínimo
        [InlineData(10.5, 26.25)]    // Con decimales
        [InlineData(50.0, 125.00)]   // Peso medio
        [InlineData(250.75, 626.875)] // Peso alto con decimales
        [Trait("Category", "Direct Shipments")]
        public void CalcularTarifaEnvio_WithVariousDifferentWeights_ShouldCalculateCorrectly(
            decimal peso, decimal expected)
        {
            // Act
            decimal resultado = _calculadora.CalcularTarifaEnvio(peso, "SJO", "MIA", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        #endregion

        #region Test Non-existent City Code Exceptions

        [Fact]
        [Trait("Category", "Exception Handling")]
        public void CalcularTarifaEnvio_WithNonExistentRoute_ShouldThrowKeyNotFoundException()
        {
            // Act & Assert
            var exception = Assert.Throws<KeyNotFoundException>(() =>
                _calculadora.CalcularTarifaEnvio(10.0m, "SJO", "INVALID", _tarifasBase));

            Assert.Contains("No se encontró tarifa", exception.Message);
            Assert.Contains("SJO-INVALID", exception.Message);
        }

        [Fact]
        [Trait("Category", "Exception Handling")]
        public void CalcularTarifaEnvio_WithNullOriginZone_ShouldThrowArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaEnvio(10.0m, null!, "MIA", _tarifasBase));

            Assert.Contains("no puede estar vacía", exception.Message);
        }

        [Fact]
        [Trait("Category", "Exception Handling")]
        public void CalcularTarifaEnvio_WithEmptyDestinationZone_ShouldThrowArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaEnvio(10.0m, "SJO", "", _tarifasBase));

            Assert.Contains("no puede estar vacía", exception.Message);
        }

        [Fact]
        [Trait("Category", "Exception Handling")]
        public void CalcularTarifaEnvio_WithNegativeWeight_ShouldThrowArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaEnvio(-5.0m, "SJO", "MIA", _tarifasBase));

            Assert.Contains("debe ser mayor a 0", exception.Message);
        }

        [Fact]
        [Trait("Category", "Exception Handling")]
        public void CalcularTarifaEnvio_WithZeroWeight_ShouldThrowArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaEnvio(0m, "SJO", "MIA", _tarifasBase));

            Assert.Contains("debe ser mayor a 0", exception.Message);
        }

        [Fact]
        [Trait("Category", "Exception Handling")]
        public void CalcularTarifaEnvio_WithEmptyTarifas_ShouldThrowArgumentException()
        {
            // Arrange
            var tarifasVacias = new Dictionary<string, decimal>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaEnvio(10.0m, "SJO", "MIA", tarifasVacias!));

            Assert.Contains("no pueden estar vacías", exception.Message);
        }

        [Fact]
        [Trait("Category", "Exception Handling")]
        public void CalcularTarifaEnvio_WithNullTarifas_ShouldThrowArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaEnvio(10.0m, "SJO", "MIA", null!));

            Assert.Contains("no pueden estar vacías", exception.Message);
        }

        #endregion

        #region Test Transshipment (Multi-leg Routing)

        [Fact]
        [Trait("Category", "Transshipment")]
        public void CalcularTarifaConTransbordo_WithValidRoute_ShouldSumSegments()
        {
            // Arrange
            decimal peso = 10.0m;
            // SJO->MIA: 10 * 2.50 = 25.00
            // MIA->LAX: 10 * 3.50 = 35.00
            // Total: 60.00
            decimal expected = 60.00m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaConTransbordo(
                peso, "SJO", "MIA", "LAX", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
            Assert.Equal(3, _calculadora.ObtenerRegistros().Count()); // DIRECTO + DIRECTO + TRANSBORDO
        }

        [Fact]
        [Trait("Category", "Transshipment")]
        public void CalcularTarifaConTransbordo_WithDifferentWeights_ShouldCalculateCorrectly()
        {
            // Arrange
            decimal peso = 25.5m;
            // SJO->NYC: 25.5 * 3.00 = 76.50
            // NYC->SJO: 25.5 * 3.00 = 76.50
            // Total: 153.00
            decimal expected = 153.00m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaConTransbordo(
                peso, "SJO", "NYC", "SJO", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Transshipment")]
        public void CalcularTarifaConTransbordo_WithInvalidIntermediateRoute_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                _calculadora.CalcularTarifaConTransbordo(
                    10.0m, "SJO", "INVALID", "MIA", _tarifasBase));
        }

        [Fact]
        [Trait("Category", "Transshipment")]
        public void CalcularTarifaConTransbordo_WithInvalidFinalRoute_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                _calculadora.CalcularTarifaConTransbordo(
                    10.0m, "SJO", "MIA", "INVALID", _tarifasBase));
        }

        #endregion

        #region Test Output Log Format (Date and Time)

        [Fact]
        [Trait("Category", "Logging")]
        public void RegistroCalculos_ShouldHaveCorrectDateTimeFormat()
        {
            // Arrange
            var beforeTime = DateTime.Now.AddSeconds(-1); // Dar tiempo de margen
            
            // Act
            _calculadora.CalcularTarifaEnvio(10.0m, "SJO", "MIA", _tarifasBase);
            
            var afterTime = DateTime.Now.AddSeconds(1); // Dar tiempo de margen
            var registros = _calculadora.ObtenerRegistros();

            // Assert
            Assert.Single(registros);
            var registro = registros[0];

            // Verificar que contiene el formato correcto: [yyyy-MM-dd HH:mm:ss.fff]
            Assert.Matches(@"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]", registro);
            
            // Verificar que el timestamp está dentro del rango esperado (con margen)
            var timestampStr = registro.Substring(1, 23); // Extraer timestamp
            var parsedTime = DateTime.ParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss.fff", null);
            Assert.InRange(parsedTime, beforeTime, afterTime);
        }

        [Fact]
        [Trait("Category", "Logging")]
        public void RegistroCalculos_ShouldContainAllRequiredInfo()
        {
            // Act
            _calculadora.CalcularTarifaEnvio(15.5m, "SJO", "MIA", _tarifasBase);
            var registros = _calculadora.ObtenerRegistros();

            // Assert
            Assert.Single(registros);
            var registro = registros[0];

            Assert.Contains("Tipo: DIRECTO", registro);
            Assert.Contains("Origen: SJO", registro);
            Assert.Contains("Destino: MIA", registro);
            Assert.Contains("Cantidad: 15.5kg", registro);
            Assert.Contains("Costo: $38.75", registro);
        }

        [Fact]
        [Trait("Category", "Logging")]
        public void RegistroCalculos_MultipleOperations_ShouldMaintainChronologicalOrder()
        {
            // Act
            _calculadora.CalcularTarifaEnvio(10.0m, "SJO", "MIA", _tarifasBase);
            System.Threading.Thread.Sleep(10); // Pequeña pausa para garantizar timestamps diferentes
            _calculadora.CalcularTarifaEnvio(20.0m, "SJO", "NYC", _tarifasBase);
            System.Threading.Thread.Sleep(10);
            _calculadora.CalcularTarifaEnvio(5.0m, "SJO", "LAX", _tarifasBase);

            var registros = _calculadora.ObtenerRegistros();

            // Assert
            Assert.Equal(3, registros.Count);

            // Verificar que los timestamps están en orden
            var timestamp1 = DateTime.ParseExact(registros[0].Substring(1, 23), "yyyy-MM-dd HH:mm:ss.fff", null);
            var timestamp2 = DateTime.ParseExact(registros[1].Substring(1, 23), "yyyy-MM-dd HH:mm:ss.fff", null);
            var timestamp3 = DateTime.ParseExact(registros[2].Substring(1, 23), "yyyy-MM-dd HH:mm:ss.fff", null);

            Assert.True(timestamp1 <= timestamp2);
            Assert.True(timestamp2 <= timestamp3);
        }

        #endregion

        #region Test Reverse Calculation with 10% Surcharge

        [Fact]
        [Trait("Category", "Reverse Calculation")]
        public void CalcularTarifaInversaConSurcharge_ShouldAdd10PercentFee()
        {
            // Arrange
            decimal peso = 10.0m;
            // SJO->MIA: 10 * 2.50 = 25.00
            // Con 10% surcharge: 25.00 * 1.10 = 27.50
            decimal expected = 27.50m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaInversaConSurcharge(
                peso, "SJO", "MIA", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Theory]
        [InlineData(5.0, 8.25)]      // 5 * 1.50 = 7.50; 7.50 * 1.10 = 8.25 (SJO-LAX)
        [InlineData(20.0, 33.00)]    // 20 * 1.50 = 30.00; 30.00 * 1.10 = 33.00 (SJO-LAX)
        [InlineData(100.0, 165.00)]  // 100 * 1.50 = 150.00; 150.00 * 1.10 = 165.00 (SJO-LAX)
        [Trait("Category", "Reverse Calculation")]
        public void CalcularTarifaInversaConSurcharge_WithVariousWeights_ShouldCalculateCorrectly(
            decimal peso, decimal expected)
        {
            // Arrange
            string origen = "SJO";
            string destino = "LAX";

            // Act - Usamos la ruta SJO-LAX que tienen tarifa 1.50
            decimal resultado = _calculadora.CalcularTarifaInversaConSurcharge(
                peso, origen, destino, _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Reverse Calculation")]
        public void CalcularTarifaInversaConSurcharge_WithDecimalResult_ShouldMaintainPrecision()
        {
            // Arrange
            decimal peso = 7.5m;
            // SJO->MIA: 7.5 * 2.50 = 18.75
            // Con 10% surcharge: 18.75 * 1.10 = 20.625
            decimal expected = 20.625m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaInversaConSurcharge(
                peso, "SJO", "MIA", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Reverse Calculation")]
        public void CalcularTarifaInversaConSurcharge_WithNonExistentRoute_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                _calculadora.CalcularTarifaInversaConSurcharge(
                    10.0m, "SJO", "INVALID", _tarifasBase));
        }

        #endregion

        #region Test Cumulative Calculation with Transshipment Cities

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_WithTwoCities_ShouldCalculateDirectRoute()
        {
            // Arrange
            var ruta = new List<string> { "SJO", "MIA" };
            decimal peso = 10.0m;
            // SJO->MIA: 10 * 2.50 = 25.00
            decimal expected = 25.00m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaCumulativa(peso, ruta, _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_WithThreeCities_ShouldSumAllSegments()
        {
            // Arrange
            var ruta = new List<string> { "SJO", "MIA", "LAX" };
            decimal peso = 10.0m;
            // SJO->MIA: 10 * 2.50 = 25.00
            // MIA->LAX: 10 * 3.50 = 35.00
            // Total: 60.00
            decimal expected = 60.00m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaCumulativa(peso, ruta, _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_WithFourCities_ShouldSumAllSegmentsCumulatively()
        {
            // Arrange
            var ruta = new List<string> { "SJO", "MIA", "LAX", "SJO" };
            decimal peso = 5.0m;
            // SJO->MIA: 5 * 2.50 = 12.50
            // MIA->LAX: 5 * 3.50 = 17.50
            // LAX->SJO: 5 * 1.50 = 7.50
            // Total: 37.50
            decimal expected = 37.50m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaCumulativa(peso, ruta, _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_WithCaseSensitiveRoutes_ShouldNormalizeToUppercase()
        {
            // Arrange
            var ruta = new List<string> { "sjo", "mia", "lax" };
            decimal peso = 10.0m;
            decimal expected = 60.00m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaCumulativa(peso, ruta, _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_WithSingleCity_ShouldThrowException()
        {
            // Arrange
            var ruta = new List<string> { "SJO" };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaCumulativa(10.0m, ruta, _tarifasBase));

            Assert.Contains("al menos 2 ciudades", exception.Message);
        }

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_WithEmptyRoute_ShouldThrowException()
        {
            // Arrange
            var ruta = new List<string>();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaCumulativa(10.0m, ruta, _tarifasBase));

            Assert.Contains("al menos 2 ciudades", exception.Message);
        }

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_WithNullRoute_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _calculadora.CalcularTarifaCumulativa(10.0m, null!, _tarifasBase));

            Assert.Contains("al menos 2 ciudades", exception.Message);
        }

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_WithInvalidIntermediateRoute_ShouldThrowException()
        {
            // Arrange
            var ruta = new List<string> { "SJO", "INVALID", "MIA" };

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() =>
                _calculadora.CalcularTarifaCumulativa(10.0m, ruta, _tarifasBase));
        }

        [Fact]
        [Trait("Category", "Cumulative Calculation")]
        public void CalcularTarifaCumulativa_ShouldLogCorrectly()
        {
            // Arrange
            var ruta = new List<string> { "SJO", "MIA", "LAX" };
            decimal peso = 10.0m;

            // Act
            _calculadora.CalcularTarifaCumulativa(peso, ruta, _tarifasBase);
            var registros = _calculadora.ObtenerRegistros();

            // Assert
            // Debe haber 3 registros: 2 para cada segmento DIRECTO + 1 para CUMULATIVA
            Assert.Equal(3, registros.Count);
            Assert.Contains("CUMULATIVA", registros[2]);
            Assert.Contains("SJO -> MIA -> LAX", registros[2]);
        }

        #endregion

        #region Integration and Backward Compatibility Tests

        [Fact]
        [Trait("Category", "Integration")]
        public void TarifaEnvioCalculadora_ShouldMaintainBackwardCompatibility()
        {
            // Prueba que la instancia puede ser usada como antes
            // Arrange
            decimal peso = 10.0m;
            decimal expected = 25.00m;

            // Act
            decimal resultado = _calculadora.CalcularTarifaEnvio(peso, "SJO", "MIA", _tarifasBase);

            // Assert
            Assert.Equal(expected, resultado);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void LimpiarRegistros_ShouldClearAllLogs()
        {
            // Arrange
            _calculadora.CalcularTarifaEnvio(10.0m, "SJO", "MIA", _tarifasBase);
            _calculadora.CalcularTarifaEnvio(20.0m, "SJO", "NYC", _tarifasBase);

            // Act
            _calculadora.LimpiarRegistros();

            // Assert
            Assert.Empty(_calculadora.ObtenerRegistros());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ObtenerRegistros_ShouldReturnReadOnlyList()
        {
            // Arrange
            _calculadora.CalcularTarifaEnvio(10.0m, "SJO", "MIA", _tarifasBase);
            var registros = _calculadora.ObtenerRegistros();

            // Act & Assert
            Assert.Single(registros);
            // Intentar modificar debe lanzar excepción
            Assert.Throws<NotSupportedException>(() => 
                ((IList<string>)registros).Add("test"));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void SystemStability_MultipleOperations_ShouldMaintainConsistency()
        {
            // Arrange & Act
            var operaciones = new List<(decimal peso, string origen, string destino, decimal expected)>
            {
                (5.0m, "SJO", "MIA", 12.50m),
                (10.0m, "SJO", "NYC", 30.00m),
                (20.0m, "SJO", "LAX", 30.00m),
                (15.5m, "MIA", "SJO", 38.75m)
            };

            foreach (var op in operaciones)
            {
                var resultado = _calculadora.CalcularTarifaEnvio(op.peso, op.origen, op.destino, _tarifasBase);
                Assert.Equal(op.expected, resultado);
            }

            // Assert: Verificar que todos los registros se crearon correctamente
            var registros = _calculadora.ObtenerRegistros();
            Assert.Equal(operaciones.Count, registros.Count);
            Assert.All(registros, r => Assert.Matches(@"\[\d{4}-\d{2}-\d{2}", r));
        }

        #endregion
    }
}
