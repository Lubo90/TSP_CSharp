using ProblemKomiwojazera.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemKomiwojazera
{
    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random();
            Console.WriteLine("Podaj nazwę pliku (bez rozszerzenia):");
            string fileName = Console.ReadLine();

            Console.WriteLine("Czas pracy (mm:ss) (lub ilość iteracji):");
            string czasPracy = Console.ReadLine();

            try
            {
                object ret = GetCzasPracy(czasPracy);

                ProblemKomiwojazera pk = new ProblemKomiwojazera(string.Format("D:\\{0}.txt", fileName), 20, 0.8, 0.014, r);
                pk.InicjalizujOsobnikow();

                Osobnik najlepszy = null;
                if (ret is TimeSpan)
                    najlepszy = pk.Start((TimeSpan)ret);
                else if (ret is int)
                    najlepszy = pk.Start((int)ret);

                //Osobnik najlepszy = pk.Start(100);
                Console.WriteLine(najlepszy.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Podano niepoprawne dane!");
            }

            Console.ReadKey();
        }

        static object GetCzasPracy(string input)
        {
            if (input.Contains(":"))
            {
                string[] dane = input.Split(new char[] { ':' });
                return new TimeSpan(0, int.Parse(dane[0]), int.Parse(dane[1]));
            }
            return int.Parse(input);
        }
    }
}
