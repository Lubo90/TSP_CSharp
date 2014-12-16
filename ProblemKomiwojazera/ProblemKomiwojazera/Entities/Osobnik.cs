using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProblemKomiwojazera.Entities
{
    public class Osobnik
    {
        Random _random;
        int[] _geny;
        int _iloscGenow;
        public int SumaWagOdleglosci;

        public Osobnik(int iloscGenow, Random random)
        {
            _random = random;
            _geny = new int[iloscGenow];
            _iloscGenow = iloscGenow;
        }

        public void Inicjalizuj()
        {
            List<int> miasta = new List<int>();
            for (int i = 0; i < _geny.Length; i++)
                miasta.Add(i);

            for (int i = 0; i < _geny.Length; i++)
            {
                var wylosowanyId = _random.Next(0, miasta.Count);
                _geny[i] = miasta[wylosowanyId];
                miasta.RemoveAt(wylosowanyId);
            }
        }

        /// <summary>
        /// Metoda krzyżuje dwa osobniki (OX)
        /// </summary>
        /// <param name="drugiRodzic">Drugi rodzic, z którego pobrane zostaną obczne wartości</param>
        /// <param name="pktPrzecieciaA">Punkt przecięcia A</param>
        /// <param name="pktPrzecieciaB">Punkt przecięcia B</param>
        /// <returns></returns>
        public Osobnik Krzyzuj(Osobnik drugiRodzic, int pktPrzecieciaA, int pktPrzecieciaB)
        {
            int pocz, kon;
            if (pktPrzecieciaA > pktPrzecieciaB)
            {
                pocz = pktPrzecieciaB;
                kon = pktPrzecieciaA;
            }
            else
            {
                pocz = pktPrzecieciaA;
                kon = pktPrzecieciaB;
            }

            // skopiowanie środka punktów przecięcia z pierwszego rodzica
            Osobnik nowy = new Osobnik(this._iloscGenow, _random);
            for (int i = pocz; i < kon; i++)
                nowy._geny[i] = this._geny[i];

            int indexDziecka = kon;

            // końcówka rodzica
            for (int i = kon; i < this._iloscGenow; i++)
            {
                bool exists = false;
                for (int j = pocz; j < kon; j++)
                    if (nowy._geny[j] == drugiRodzic._geny[i])
                    {
                        exists = true;
                        break;
                    }
                if (!exists)
                    nowy._geny[indexDziecka++] = drugiRodzic._geny[i];
                if (indexDziecka == _iloscGenow)
                    indexDziecka = 0;
            }

            //a teraz od początku do kon - 1
            for (int i = 0; i < kon; i++)
            {
                bool exists = false;
                for (int j = pocz; j < kon; j++)
                    if (nowy._geny[j] == drugiRodzic._geny[i])
                    {
                        exists = true;
                        break;
                    }
                if (!exists)
                    nowy._geny[indexDziecka++] = drugiRodzic._geny[i];
                //if (indexDziecka == pocz) // jeśli sterowanie osiągnęło punkt tablicy, w którym były skopiowane geny z pierwszego rodzica, to wyjdź
                //    break;
                if (indexDziecka == _iloscGenow)
                    indexDziecka = 0;
            }

            //int pozostaloElementow = kon - pocz +1;
            //int j=0;
            //for (int i = 0; i < pozostaloElementow; i++)
            //{

            //    if (i + kon < _iloscGenow)
            //        nowy._geny[kon + i] = drugiRodzic._geny[kon + i];
            //    else
            //    {
            //        nowy._geny[j] = drugiRodzic._geny[j];
            //        j++;
            //    }
            //}

            return nowy;
        }

        /// <summary>
        /// Mutuje osobnika
        /// </summary>
        /// <param name="prawdopodobienstwo">Procent szansy na zmutowanie (dokładność do 0.01%)</param>
        public void Mutuj(double prawdopodobienstwo)
        {
            int szansa = (int)(prawdopodobienstwo*10000);
            for (int i = 0; i < _geny.Length; i++)
            {
                int los = _random.Next(0, 10000);
                if (los <= szansa) // mutuj
                {
                    int docelowaPozycja = _random.Next(0, _iloscGenow - 1);
                    int tmp = this._geny[docelowaPozycja];
                    this._geny[docelowaPozycja] = this._geny[i];
                    this._geny[i] = tmp;
                }
            }
        }

        public int ObliczFitnessFunc(int[,] macierzOdleglosci)
        {
            int i;
            for (i = 0; i < _geny.Length - 1; i++)
                SumaWagOdleglosci += macierzOdleglosci[_geny[i], _geny[i + 1]];

            //dodanie pierwszego i ostatniego elementu
            SumaWagOdleglosci += macierzOdleglosci[_geny[0], _geny[i]];

            return SumaWagOdleglosci;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Wynik: {0}{1}", SumaWagOdleglosci, Environment.NewLine));
            for (int i = 0; i < _geny.Length; i++)
            {
                sb.AppendLine(string.Format("Gen {0}: {1}", i + 1, _geny[i]));
            }
            return sb.ToString();
        }
    }
}
