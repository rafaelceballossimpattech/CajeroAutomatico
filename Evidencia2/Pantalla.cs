namespace Evidencia2
{
    public class Pantalla
    {
        public void MostrarMenu()
        {
            Console.Clear();
            Console.WriteLine("Bienvenido al sistema bancario. Seleccione una opción:");
            Console.WriteLine("1. Retiro de dinero");
            Console.WriteLine("2. Transferencia de fondos");
            Console.WriteLine("3. Depósito de fondos");
            Console.WriteLine("4. Consultar saldo");
            Console.WriteLine("5. Salir");
        }
        public int Cuadratica(int x)
        {
            return (int) System.Math.Pow(x, 2) - (4 * x) + 3;
        }
    }
}
