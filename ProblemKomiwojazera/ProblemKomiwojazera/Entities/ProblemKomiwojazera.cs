using ProblemKomiwojazera.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ProblemKomiwojazera
{
    public class ProblemKomiwojazera
    {
        double _pdpKrzyzowania, _pdpMutacji;
        bool _isRunning = false;
        Random _random;
        int[,] _macierzOdleglosci; // [wiersze, kolumny]
        int _iloscMiast;
        List<Osobnik> _osobnicy;
        List<Osobnik> _wybraniDoKrzyzowania;
        long _sumaWartosciFuncPrzystosowania;
        Osobnik _najlepszyOsobnik;

        public ProblemKomiwojazera(string sciezkaDoPlikuOdleglosci, int iloscOsobnikow, double szansaKrzyzowania, double szansaMutacji, Random random)
        {
            _pdpKrzyzowania = szansaKrzyzowania;
            _pdpMutacji = szansaMutacji;
            _random = random;
            WczytajMacierzOdleglosci(sciezkaDoPlikuOdleglosci);
            _osobnicy = new List<Osobnik>(iloscOsobnikow);
        }

        public void InicjalizujOsobnikow()
        {
            for (int i = 0; i < _osobnicy.Capacity; i++)
            {
                Osobnik o = new Osobnik(_iloscMiast, _random);
                o.Inicjalizuj();
                _osobnicy.Add(o);
            }
        }

        public Osobnik Start(int iloscIteracji)
        {
            for (int i = 0; i < iloscIteracji; i++)
            {
                this.OcenaOsobnikow();
                this.Selekcja();
                this.Krzyzowanie(_pdpKrzyzowania);
                this.Mutacja(_pdpMutacji);
            }

            this.OcenaOsobnikow();
            return _najlepszyOsobnik;
        }

        public Osobnik Start(TimeSpan okresCzasu)
        {
            Timer t = new Timer();
            t.Interval = okresCzasu.TotalMilliseconds;
            t.Elapsed += (s, e) => { _isRunning = false; };
            _isRunning = true;
            t.Start();

            while (_isRunning)
            {
                this.OcenaOsobnikow();
                this.Selekcja();
                this.Krzyzowanie(_pdpKrzyzowania);
                this.Mutacja(_pdpMutacji);
            }

            return _najlepszyOsobnik;
        }

        private void WczytajMacierzOdleglosci(string sciezkaDoPlikuOdleglosci)
        {
            using (StreamReader sr = File.OpenText(sciezkaDoPlikuOdleglosci))
            {
                if (int.TryParse(sr.ReadLine(), out _iloscMiast))
                {
                    _macierzOdleglosci = new int[_iloscMiast, _iloscMiast];

                    string line; int i = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int j = 0;
                        var odleglosci = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var waga in odleglosci)
                        {
                            int w = Convert.ToInt32(waga);
                            _macierzOdleglosci[i, j] = w;
                            _macierzOdleglosci[j, i] = w;
                            j++;
                        }
                        i++;
                    }
                }
            }
        }

        private void Selekcja()
        {
            _osobnicy.Sort((os1, os2) =>
            {
                if (os1.SumaWagOdleglosci < os2.SumaWagOdleglosci)
                    return 1;
                else if (os1.SumaWagOdleglosci > os2.SumaWagOdleglosci)
                    return -1;
                else
                    return 0;
            });

            _wybraniDoKrzyzowania = new List<Osobnik>();
            for (int i = 0; i < _osobnicy.Count; i++)
            {
                long los = LongRandom(0, _sumaWartosciFuncPrzystosowania);
                long suma = 0;
                for (int j = 0; j < _osobnicy.Count; j++)
                {
                    if ((suma += _osobnicy[j].SumaWagOdleglosci) >= los)
                    {
                        _wybraniDoKrzyzowania.Add(_osobnicy[j]);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Krzyżuje osobników
        /// </summary>
        /// <param name="prawdopodobienstwo">Prawdopodobieństwo skrzyżowania w procentach (max dokładność 0.01)</param>
        private void Krzyzowanie(double prawdopodobienstwo)
        {
            List<Osobnik> nowaPopulacja = new List<Osobnik>();
            int i;
            for (i = 0; i < _wybraniDoKrzyzowania.Count; i+=2)
            {
                if (_random.Next(0, 100) <= prawdopodobienstwo * 100)
                {
                    int a = _random.Next(0, _iloscMiast);
                    int b= _random.Next(0, _iloscMiast);
                    nowaPopulacja.Add(_wybraniDoKrzyzowania[i].Krzyzuj(_wybraniDoKrzyzowania[i + 1], a, b));
                    nowaPopulacja.Add(_wybraniDoKrzyzowania[i + 1].Krzyzuj(_wybraniDoKrzyzowania[i], a, b));
                }
                else
                {
                    nowaPopulacja.Add(_wybraniDoKrzyzowania[i]);
                    nowaPopulacja.Add(_wybraniDoKrzyzowania[i + 1]);
                }
            }

            // zastąpienie dotychczasowej populacji nową
            _osobnicy = nowaPopulacja;
        }

        /// <summary>
        /// Mutuje osobników
        /// </summary>
        /// <param name="prawdopodobienstwo">Prawdopodobieństwo wystąpienia mutacji (dokładność do 0.01%)</param>
        private void Mutacja(double prawdopodobienstwo)
        {
            for (int i = 0; i < _osobnicy.Count; i++)
            {
                _osobnicy[i].Mutuj(prawdopodobienstwo);
            }
        }

        private void OcenaOsobnikow()
        {
            _sumaWartosciFuncPrzystosowania = 0;
            if (_najlepszyOsobnik == null)
                _najlepszyOsobnik = _osobnicy[0];

            for (int i = 0; i < _osobnicy.Count; i++)
            {
                long przystosowanieOsobnika = _osobnicy[i].ObliczFitnessFunc(_macierzOdleglosci);
                if (przystosowanieOsobnika < _najlepszyOsobnik.SumaWagOdleglosci)
                    _najlepszyOsobnik = _osobnicy[i];
                _sumaWartosciFuncPrzystosowania += przystosowanieOsobnika;
            }

            for (int i = 0; i < _osobnicy.Count; i++)
            {
                _osobnicy[i].SumaWagOdleglosci = _sumaWartosciFuncPrzystosowania + 1 - _osobnicy[i].SumaWagOdleglosci;
            }
        }

        private long LongRandom(long min, long max)
        {
            byte[] buf = new byte[8];
            _random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
    }
}
