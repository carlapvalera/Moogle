using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoogleEngine
{
    class Lectura
    {
        public static List<string> ObtenerDirectoriosArchivos(string dir)
        //se le pasa una direccion y te devuelve una lista con todos los archivos existentes en dicha direccion
        {
            List<string> vect = new List<string>(Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories));
           
            for(int i = 0 ; i < vect.Count ; i++)
            {
                vect[i] = vect[i].Substring(dir.Length);// quitarle la parte de la direccion
            }
           
            return vect;//todos los documentos existentes
        }

        public static string ListaPalabrasString(List<string> vect)
        //convertir una lista de palabras un string de todas las palabras
        {
            StringBuilder cad = new StringBuilder();

            for(int i = 0 ; i < vect.Count ; i++)
            {
                if(i > 0)cad.Append(" ");
                cad.Append(vect[i]);
            }

            return cad.ToString();
        }

        public static string ObtenerStringArchivo(string dir)
        //obtener todo el texto dentro de un documento
        {
            return File.ReadAllText(dir);
        }

        public static List<string> ObtenerPalabrasString(string cad)
        //separar las lista en una palabras
        {
            List<string> vect = new List<string>();

            StringBuilder palabra = new StringBuilder();

            for(int i = 0 ; i < cad.Length ; i++)
            {
                if(Char.IsLetterOrDigit(cad[i]))//pregunta si es digito o letra y en el caso que no lo sea guarda la palabra obtenida hasta el momento
                {
                    palabra.Append(cad[i]);
                }
                else
                {
                    if(palabra.Length > 0)
                    {
                        vect.Add(palabra.ToString().ToLower());
                        palabra.Clear();
                    }
                }
            }

            if(palabra.Length > 0)
            {
                vect.Add(palabra.ToString().ToLower());
                palabra.Clear();
            }

            return vect;
        }

        public static List<Tuple<string,int>> ObtenerPalabrasyPosicionesString(string cad)
        //obtener la posicion que ocupa cada palabra
        {
            List<Tuple<string,int>> vect = new List<Tuple<string,int>>();

            int last = -1;
            StringBuilder palabra = new StringBuilder();

            for(int i = 0 ; i < cad.Length ; i++)
            {
                if(Char.IsLetterOrDigit(cad[i]))
                {
                    if(palabra.Length == 0)last = i;
                    palabra.Append(cad[i]);
                }
                else
                {
                    if(palabra.Length > 0)
                    {
                        vect.Add(new Tuple<string,int>(palabra.ToString().ToLower(), last));
                        last = -1;
                        palabra.Clear();
                    }
                }
            }

            if(palabra.Length > 0)
            {
                vect.Add(new Tuple<string,int>(palabra.ToString().ToLower(), last));
                last = -1;
                palabra.Clear();
            }

            return vect;
        }

        public static string ArreglarQueryPalabras(string query, List<string> vect)
        /*arreglamos la query convirtiendola en la sugerencia(n es decir se encuentra la palabra mas semejante
        a cada palabra de la query y se remplaza, empezando dicho proceso del final hasta el inicio de modo
        talque cuando se llega a la posicion o ya la sugerencia esta terminada*/
        {
            List<Tuple<string,int>> arr = ObtenerPalabrasyPosicionesString(query);

            for(int i = arr.Count-1 ; i >= 0 ; i--)
            {
                query = query.Remove(arr[i].Item2, arr[i].Item1.Length);
                query = query.Insert(arr[i].Item2, vect[i]);
            }

            return query;
        }
    }
}