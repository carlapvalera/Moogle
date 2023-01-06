using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine
{
    class Diccionario
    {
        HashSet<string> Palabras = new HashSet<string>();
        int LimitePalabras = 5;// enn el caso que se quiera mostrar mas de una sugerencia(5) pero actualmente el programa solo muestra 1.

        double DistanciaLevenshtein(string s1, string s2)//(le adjunté el código aparte como m dijo)
        //calcula la cantidad  de minima cambios que se deben hacer para convertir una palabra en otra
        {
            int n = s1.Length;
            int m = s2.Length;
            int[,] dp = new int[n+1,m+1];

            for(int i = 0 ; i <= n ; i++)
            {
                dp[i,0] = i;
            }

            for(int j = 0 ; j <= m ; j++)
            {
                dp[0,j] = j;
            }
            
            for(int i = 1 ; i <= n ; i++)
            {
                for(int j = 1 ; j <= m ; j++)
                {
                    dp[i,j] = Math.Max(n,m);
                    
                    int delta = 0;
                    if(s1[i-1] != s2[j-1])delta = 1;

                    dp[i,j] = Math.Min(dp[i,j], dp[i-1,j] + 1);
                    dp[i,j] = Math.Min(dp[i,j], dp[i,j-1] + 1);
                    dp[i,j] = Math.Min(dp[i,j], dp[i-1,j-1] + delta);
                }
            }
            
            return 1.0 - (double)dp[n,m]/(double)Math.Max(n,m);
        }

        public void Insertar(string palabra)
        {
            if(Palabras.Contains(palabra))return;
            Palabras.Add(palabra);
        }

        public List<Tuple<string,double>> EncontrarPalabra(string word)//encontrar la mejor sugerencia posible, es decir la(las) palabra(palabras) con menos distancia entre la query
        {
            List<Tuple<string,double>> mejor = new List<Tuple<string,double>>();

            mejor.Add(new Tuple<string,double>("",0));

            foreach(var s in Palabras)
            {
                double sc = DistanciaLevenshtein(word, s);

                int pos = 0;

                for(int i = 0 ; i < mejor.Count ; i++)
                {
                    if(sc < mejor[i].Item2)
                    {
                        pos++;
                    }
                    else break;
                }

                mejor.Insert(pos, new Tuple<string,double>(s,sc));

                if(mejor.Count > LimitePalabras)//quedarme con las 5 mejores
                {
                    {
                        mejor.RemoveAt(mejor.Count - 1);
                    }
                }
            }

            return mejor;
        }
    }
}