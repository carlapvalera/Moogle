using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine
{
    public class Operadores
    {
        public HashSet<string> TieneExistirPalabra = new HashSet<string>();
        public HashSet<string> NopuedeExistirPalabra = new HashSet<string>();
        public HashSet<Tuple<string,string>> ParejaPalabra = new HashSet<Tuple<string,string>>();
        public Dictionary<string,double> ImportanciaPalabra = new Dictionary<string,double>();

        public Operadores(string cad)//constructor de la clase, trabajo de los operadores
        {
            List<Tuple<string, int>> vect = Lectura.ObtenerPalabrasyPosicionesString(cad);

            foreach (int pos in ObtenerOpPosicion(cad, '^'))//operador "tiene que aparecer"
            {
                int R = EncontraralaDerecha(vect, pos);

                if (R != -1)
                {
                    if (!TieneExistirPalabra.Contains(vect[R].Item1))
                    {
                        TieneExistirPalabra.Add(vect[R].Item1);
                    }
                }
            }

            foreach (int pos in ObtenerOpPosicion(cad, '!'))
            //operador "no debe aparecer"
            {
                int R = EncontraralaDerecha(vect, pos);

                if (R != -1)
                { 
                    if (!NopuedeExistirPalabra.Contains(vect[R].Item1))
                    {
                        NopuedeExistirPalabra.Add(vect[R].Item1);
                    }
                }
            }

            foreach (var palabra in vect)
            {
                if (!ImportanciaPalabra.ContainsKey(palabra.Item1))
                {
                    ImportanciaPalabra.Add(palabra.Item1, 1);
                }
            }

            foreach (int pos in ObtenerOpPosicion(cad, '*'))
            //operador de "importancia"
            {
                int R = EncontraralaDerecha(vect, pos);

                if (R != -1)
                {
                    ImportanciaPalabra[vect[R].Item1] *= 2;// multiplica por 2 la cant de veces q este aparece ante una palabra
                }
            }

            foreach (int pos in ObtenerOpPosicion(cad, '~'))
                /*operador de "cercania"(algo que acotar este operador no diferencia entre la pareja de
                cercania mutables, un ejemplo, casa~alma = alma~casa)*/
            {
                int L = EncontraralaIzquierda(vect, pos);
                int R = EncontraralaDerecha(vect, pos);

                if (L != -1 && R != -1)
                {
                    string s1 = vect[L].Item1;
                    string s2 = vect[R].Item1;

                    if (s1 == s2)
                    {
                        continue;
                    }

                    if (CompararPalabras(s2, s1))
                    {
                        (s1, s2) = (s2, s1);
                    }

                    if (!ParejaPalabra.Contains(new Tuple<string, string>(s1, s2)))
                    {
                        ParejaPalabra.Add(new Tuple<string, string>(s1, s2));
                    }
                }
            }
        }
        public bool CompararPalabras(string s1, string s2)
        //compara palabras para el operador de cercania
        {
            for(int i = 0 ; i < Math.Min(s1.Length, s2.Length) ; i++)
            {
                if(s1[i] < s2[i])return true;
                if(s1[i] > s2[i])return false;
            }

            if(s1.Length < s2.Length)return true;

            return false;
        }

        public List<int> ObtenerOpPosicion(string cad, char c)
        //obtener la posicion de los operadores
        {
            List<int> Pos = new List<int>();
            for(int i = 0 ; i < cad.Length ; i++)
            {
                if(cad[i] == c)
                {
                    Pos.Add(i);
                }
            }
            return Pos;
        }

        public int EncontraralaDerecha(List<Tuple<string,int>> vect, int pos)
        //buscar a la derecha del operador para encontrar la palabra, si existe la palabra devuelve la primera posicion, si no -1
        {
            for(int i = 0 ; i < vect.Count ; i++)
            {
                if(pos < vect[i].Item2)
                {
                    return i;
                }
            }
            return -1;
        }

        public int EncontraralaIzquierda(List<Tuple<string,int>> vect, int pos)
        /*buscar a la izquierda para encontar la palabra necesario para el algoritmo de cercania,
        si existe la palabra devuelve la primera posicion, si no -1*/
        {
            for (int i = vect.Count-1 ; i >= 0 ; i--)
            {
                if(pos > vect[i].Item2)
                {
                    return i;
                }
            }
            return -1;
        }

       
    }
}