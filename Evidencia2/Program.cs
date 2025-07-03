using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Evidencia2
{
    internal class Program
    {
        private static Pantalla pantalla = new Pantalla();
        private static string nombreArchivo = "CuentasBancarias.json";
        static void Main()
        {
            List<CuentaBancaria> cuentas = new List<CuentaBancaria>();
            ManejoDeArchivos manejoDeArchivos = new ManejoDeArchivos();

            if (manejoDeArchivos.ValidarArchivo(nombreArchivo))
            {
                // Cargar datos desde el archivo
                cuentas = manejoDeArchivos.CargarDatosBancariosDesdeArchivo(nombreArchivo);
                Console.WriteLine("Datos bancarios cargados desde el archivo.");
            }
            else
            {
                cuentas.Add(new CuentaBancaria
                {
                    NumeroTarjeta = "1234",
                    Pin = "1234",
                    Saldo = 1000.00m,
                    CVC = "123",
                    FechaVencimiento = new DateTime(2025, 12, 31)
                });
                cuentas.Add(new CuentaBancaria
                {
                    NumeroTarjeta = "5678",
                    Pin = "5678",
                    Saldo = 500.00m,
                    CVC = "456",
                    FechaVencimiento = new DateTime(2023, 6, 30) // Tarjeta vencida
                });
                cuentas.Add(new CuentaBancaria
                {
                    NumeroTarjeta = "9101",
                    Pin = "9101",
                    Saldo = 2000.00m,
                    CVC = "789",
                    FechaVencimiento = new DateTime(2024, 11, 30)
                });

                manejoDeArchivos.GuardarDatosBancariosEnArchivo(cuentas, nombreArchivo);
            }

            string numeroTarjeta;
            string pin;

            Console.WriteLine("Ingrese su número de tarjeta:");
            numeroTarjeta = Console.ReadLine();
            Console.WriteLine("Ingrese su PIN:");
            pin = Console.ReadLine();

            if (ValidarCuenta(numeroTarjeta, pin, cuentas))
            {
                bool continuar = true;
                while (!(continuar == false))
                {
                    pantalla.MostrarMenu();
                    int opcion = 0;
                    opcion = Convert.ToInt32(Console.ReadLine());

                    switch (opcion)
                    { 
                        case 1:
                            Console.WriteLine("Ingrese el monto a retirar:");
                            decimal montoRetiro = Convert.ToDecimal(Console.ReadLine());
                            RetirarDinero(numeroTarjeta, montoRetiro, cuentas);
                            manejoDeArchivos.GuardarDatosBancariosEnArchivo(cuentas, nombreArchivo);
                            break;
                         case 2:
                            Console.WriteLine("Ingrese el número de tarjeta de destino:");
                            string numeroTarjetaDestino = Console.ReadLine();
                            Console.WriteLine("Ingrese el monto a transferir:");
                            decimal montoTransferencia = Convert.ToDecimal(Console.ReadLine());
                            TransferirFondos(numeroTarjeta, numeroTarjetaDestino, montoTransferencia, cuentas);
                            manejoDeArchivos.GuardarDatosBancariosEnArchivo(cuentas, nombreArchivo);
                            break;
                        case 3:
                            Console.WriteLine("Ingrese el monto a depositar:");
                            decimal monto = Convert.ToDecimal(Console.ReadLine());
                            DepositarFondos(numeroTarjeta, monto, cuentas);
                            manejoDeArchivos.GuardarDatosBancariosEnArchivo(cuentas, nombreArchivo);
                            break;
                        case 4:
                            ConsultarSaldo(numeroTarjeta, pin, cuentas);
                            manejoDeArchivos.GuardarDatosBancariosEnArchivo(cuentas, nombreArchivo);
                            break;
                        case 5:
                            continuar = false;
                            manejoDeArchivos.GuardarDatosBancariosEnArchivo(cuentas, nombreArchivo);
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("Opción no válida. Por favor, intente de nuevo.");
                            Console.ReadKey();
                            Console.WriteLine("Presione cualquier tecla para cotinuar...");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Acceso denegado. Por favor, inténtelo de nuevo.");
                return; // Salimos del programa si la validación falla
            }
        }
        public static void TransferirFondos(string numeroTarjetaOrigen, string numeroTarjetaDestino, decimal monto, List<CuentaBancaria> cuentas)
        {
            var cuentaOrigen = cuentas.FirstOrDefault(c => c.NumeroTarjeta == numeroTarjetaOrigen);
            var cuentaDestino = cuentas.FirstOrDefault(c => c.NumeroTarjeta == numeroTarjetaDestino);

            if (cuentaOrigen != null && cuentaDestino != null)
            {
                if (cuentaOrigen.Saldo >= monto)
                {
                    cuentaOrigen.Saldo -= monto;
                    cuentaDestino.Saldo += monto;
                    
                    Console.WriteLine($"Transferencia exitosa. Nuevo saldo de origen: {cuentaOrigen.Saldo:C}");
                }
                else
                {
                    Console.WriteLine("Fondos insuficientes para realizar la transferencia.");
                }
            }
            else
            {
                Console.WriteLine("Número de tarjeta incorrecto.");
            }
            Console.ReadKey();
            Console.WriteLine("Presione cualquier tecla para cotinuar...");
        }
        private static void RetirarDinero(string numeroTarjeta, decimal monto, List<CuentaBancaria> cuentas)
        {
            var cuenta = cuentas.FirstOrDefault(c => c.NumeroTarjeta == numeroTarjeta);
            if (cuenta != null)
            {
                if (cuenta.Saldo >= monto)
                {
                    cuenta.Saldo = cuenta.Saldo - monto;
                    Console.WriteLine($"Retiro exitoso. Nuevo saldo: {cuenta.Saldo:C}");
                }
                else
                {
                    Console.WriteLine("Fondos insuficientes para realizar el retiro.");
                }
            }
            else
            {
                Console.WriteLine("Número de tarjeta incorrecto.");
            }
            Console.ReadKey();
            Console.WriteLine("Presione cualquier tecla para cotinuar...");
        }
        private static bool ValidarCuenta(string numeroTarjeta, string pin, List<CuentaBancaria> cuentas)
        {
            var cuenta = cuentas.FirstOrDefault(c => c.NumeroTarjeta == numeroTarjeta && c.Pin == pin);
            if (cuenta == null)
            {
                Console.WriteLine("Número de tarjeta o PIN incorrectos.");
                return false;
            }
            if (cuenta.FechaVencimiento < DateTime.Now)
            {
                Console.WriteLine("Su plástico ha vencido, pase a ventanilla para obtener uno nuevo.");
                return false;
            }
            return true;
        }
        public static void ConsultarSaldo(string numeroTarjeta, string pin, List<CuentaBancaria> cuentas)
        {
            var cuenta = cuentas.FirstOrDefault(c => c.NumeroTarjeta == numeroTarjeta && c.Pin == pin);
            if (cuenta != null)
            {
                Console.WriteLine($"Su saldo es: {cuenta.Saldo:C}");
            }
            else
            {
                Console.WriteLine("Número de tarjeta o PIN incorrectos.");
            }
            Console.ReadKey();
            Console.WriteLine("Presione cualquier tecla para cotinuar...");
        }
        public static void DepositarFondos(string numeroTarjeta, decimal monto, List<CuentaBancaria> cuentas)
        {
            var cuenta = cuentas.FirstOrDefault(c => c.NumeroTarjeta == numeroTarjeta);
            if (cuenta != null)
            {
                cuenta.Saldo = cuenta.Saldo + monto;
                Console.WriteLine($"Depósito exitoso. Nuevo saldo: {cuenta.Saldo:C}");
            }
            else
            {
                Console.WriteLine("Número de tarjeta incorrecto.");
            }
            Console.ReadKey();
            Console.WriteLine("Presione cualquier tecla para cotinuar...");
        }
        public class CuentaBancaria
        {
            public string NumeroTarjeta { get; set; }
            public string Pin { get; set; }
            public decimal Saldo { get; set; }
            public string CVC { get; set; }
            public DateTime FechaVencimiento { get; set; }
        }
        public class ManejoDeArchivos
        {
            public string RutaBase { get; }
            public ManejoDeArchivos()
            {
                RutaBase = AppDomain.CurrentDomain.BaseDirectory;
            }
            public bool ValidarArchivo(string nombreArchivo)
            {
                string rutaArchivo = Path.Combine(RutaBase, nombreArchivo);
                return File.Exists(rutaArchivo);
            }
            public void GuardarDatosBancariosEnArchivo(List<CuentaBancaria> cuentas, string nombreArchivo)
            {
                string rutaArchivo = Path.Combine(RutaBase, nombreArchivo);
                using (StreamWriter writer = new StreamWriter(rutaArchivo))
                {
                    var cuentasSerializadas = JsonSerializer.Serialize(cuentas, new JsonSerializerOptions { WriteIndented = true });
                    writer.Write(cuentasSerializadas);
                }
            }
            public List<CuentaBancaria> CargarDatosBancariosDesdeArchivo(string nombreArchivo)
            {
                string rutaArchivo = Path.Combine(RutaBase, nombreArchivo);
                using (StreamReader reader = new StreamReader(rutaArchivo))
                {
                    string contenido = reader.ReadToEnd();
                    return JsonSerializer.Deserialize<List<CuentaBancaria>>(contenido) ?? new List<CuentaBancaria>();
                }
            }
        }
    }
}
